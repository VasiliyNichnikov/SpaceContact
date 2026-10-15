#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using Core.Game.Dto;
using Core.Game.Dto.Snapshots;
using Core.Game.Validation;
using Core.Game.Visibility;
using Core.Game;
using Network.Infrastructure;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEditor;
using Logger = Logs.Logger;

namespace Editor
{
    public static class SnapshotValidationChecks
    {
        private const string MenuPath = "Tools/Space Contact/Check Snapshot Validation";
        private const ulong HostPlayerId = 0;
        private const ulong RemotePlayerId = 1;
        private const ulong UnknownPlayerId = 99;
        private const int HostPlanetId = 0;
        private const int RemotePlanetId = 1;
        private const int HostShipId = 0;
        private const int RemoteShipId = 1;
        private const long InitialRevision = 10;

        [MenuItem(MenuPath)]
        private static void Run()
        {
            try
            {
                CheckValidSnapshots();
                CheckInvalidValuesAndReferences();
                CheckWireRequiredness();
                CheckRolesAndAtomicLoading();
                CheckExtensibilityAndCycles();
                Logger.Log("Snapshot validation checks passed.");
            }
            catch (Exception exception)
            {
                Logger.Error($"Snapshot validation checks failed: {exception}");
            }
        }

        [MenuItem(MenuPath, true)]
        private static bool CanRun()
        {
            return !EditorApplication.isPlayingOrWillChangePlaymode;
        }

        private static void CheckValidSnapshots()
        {
            var serializer = new NewtonsoftSerializer();
            var server = CreateContext(GameContextRole.Server, HostPlayerId);
            server.LoadSnapshot(InitialRevision, CreateSnapshot());
            var original = serializer.Serialize(server.ToSnapshot());
            foreach (var recipient in new[] { HostPlayerId, RemotePlayerId })
            {
                var projected = new GameStateProjector(server).CreateSnapshot(recipient);
                var client = CreateContext(GameContextRole.Client, recipient);
                client.LoadSnapshot(InitialRevision, serializer.Deserialize<GameSnapshot>(serializer.Serialize(projected)));
                Require(client.Cards.GetHand(recipient).IsKnown, "The recipient's hand became hidden.");
                Require(!client.Cards.IsSpaceDeckKnown, "The client received a closed deck.");
            }

            // A ship may be on another player's planet; ownership and location are different relationships.
            var crossOwner = CreateSnapshot();
            crossOwner.Galaxy.Planets[HostPlayerId][0].ShipIds.Clear();
            crossOwner.Galaxy.Planets[RemotePlayerId][0].ShipIds.Add(HostShipId);
            Validate(crossOwner);
            Require(original.SequenceEqual(serializer.Serialize(server.ToSnapshot())), "Validation changed server state.");
        }

        private static void CheckInvalidValuesAndReferences()
        {
            Reject(snapshot => snapshot.GamePlayers.Players.Clear(), ".Players");
            Reject(snapshot => snapshot.GamePlayers.Players.Add(snapshot.GamePlayers.Players[0]), ".Players");
            Reject(snapshot =>
            {
                var player = snapshot.GamePlayers.Players[1];
                player.Order = snapshot.GamePlayers.Players[0].Order;
                snapshot.GamePlayers.Players[1] = player;
            }, ".Players");
            Reject(snapshot => snapshot.Cards.HandsByPlayerId.Remove(RemotePlayerId), ".HandsByPlayerId");
            Reject(snapshot => snapshot.Galaxy.Ships.Add(UnknownPlayerId, new List<SpaceShipSnapshot>()), ".Ships");
            Reject(snapshot => snapshot.Galaxy.Planets[HostPlayerId][0].ShipIds.Add(int.MaxValue), ".ShipIds");
            Reject(snapshot => snapshot.Galaxy.Planets[RemotePlayerId][0].ShipIds.Add(HostShipId), ".ShipIds");
            Reject(snapshot => snapshot.Galaxy.Ships[RemotePlayerId].Add(new SpaceShipSnapshot { ShipId = HostShipId }), ".ShipId");
            Reject(snapshot => snapshot.Galaxy.Planets[RemotePlayerId].Add(snapshot.Galaxy.Planets[HostPlayerId][0]), ".PlanetId");
            Reject(snapshot => snapshot.Cards.SpaceDiscard.Add(new SpaceCardSnapshot()), ".SpaceDiscard");
            Reject(snapshot => snapshot.Cards.SpaceDiscard.Add(new SpaceCardSnapshot { IsConversation = true, DamageCount = 1 }), ".SpaceDiscard");
            Reject(snapshot => snapshot.Cards.SpaceDiscard.Add(SpaceCardSnapshot.DamageStateCard(-1)), ".DamageCount");
            Reject(snapshot => snapshot.Cards.ActiveDestinyCard = new DestinyCardSnapshot(), ".ActiveDestinyCard");
            Reject(snapshot => snapshot.Cards.ActiveDestinyCard = DestinyCardSnapshot.ColorCard(UnknownPlayerId), ".SelectedPlayerId");
            Reject(snapshot => snapshot.Encounter.PlanetIdToAttack = HostPlanetId, ".PlanetIdToAttack");
            Reject(snapshot => snapshot.Encounter.AggressorPlayerId = UnknownPlayerId, ".AggressorPlayerId");
            Reject(snapshot =>
            {
                snapshot.Encounter.AggressorPlayerId = HostPlayerId;
                snapshot.Encounter.DefenderPlayerId = RemotePlayerId;
                snapshot.Encounter.AggressorUnionPlayerIds.Add(HostPlayerId);
                snapshot.Encounter.DefenderUnionPlayerIds.Add(HostPlayerId);
            }, ".DefenderUnionPlayerIds");
            Reject(snapshot => snapshot.Phase.CurrentPhase = (GamePhaseType)int.MaxValue, ".CurrentPhase");
            Reject(snapshot => snapshot.Phase.EndTimeInSeconds = double.NaN, ".EndTimeInSeconds");
            Reject(snapshot => snapshot.Phase.ReadyTransitionTimeInSeconds = double.PositiveInfinity, ".ReadyTransitionTimeInSeconds");
            Reject(snapshot => snapshot.Phase.Revision = InitialRevision + 1, ".Revision");
            ExpectFailure(() => StateValidator.Validate(CreateSnapshot(), GameContextRole.Server, HostPlayerId, -1), ".Revision");
        }

        private static void CheckWireRequiredness()
        {
            var serializer = new NewtonsoftSerializer();
            var validJson = Encoding.UTF8.GetString(serializer.Serialize(CreateSnapshot()));
            foreach (var path in new[] { "Cards", "Cards.SpaceDiscard", "GamePlayers.Players[0].Order", "Cards.HandsByPlayerId.0.Count" })
            {
                var wire = JObject.Parse(validJson);
                var token = wire.SelectToken(path) ?? throw new InvalidOperationException("Missing check fixture field.");
                token.Parent?.Remove();
                ExpectJsonFailure(() => serializer.Deserialize<GameSnapshot>(Encoding.UTF8.GetBytes(wire.ToString())));
            }

            var explicitNull = JObject.Parse(validJson);
            explicitNull["Cards"] = JValue.CreateNull();
            ExpectJsonFailure(() => serializer.Deserialize<GameSnapshot>(Encoding.UTF8.GetBytes(explicitNull.ToString())));

            var inconsistent = JObject.Parse(validJson);
            var hand = (JObject)(inconsistent.SelectToken("Cards.HandsByPlayerId.0")
                ?? throw new InvalidOperationException("Missing hand fixture."));
            hand["Count"] = int.MaxValue;
            var snapshot = serializer.Deserialize<GameSnapshot>(Encoding.UTF8.GetBytes(inconsistent.ToString()));
            ExpectFailure(() => Validate(snapshot), ".Count");

            var missingDeck = JObject.Parse(validJson);
            ((JObject)(missingDeck["Cards"] ?? throw new InvalidOperationException())).Property("SpaceDeck")?.Remove();
            var withoutDeck = serializer.Deserialize<GameSnapshot>(Encoding.UTF8.GetBytes(missingDeck.ToString()));
            ExpectFailure(() => Validate(withoutDeck), ".SpaceDeck");

            var nullHand = JObject.Parse(validJson);
            var hands = (JObject)(nullHand.SelectToken("Cards.HandsByPlayerId") ?? throw new InvalidOperationException());
            hands[HostPlayerId.ToString()] = JValue.CreateNull();
            var withoutHand = serializer.Deserialize<GameSnapshot>(Encoding.UTF8.GetBytes(nullHand.ToString()));
            ExpectFailure(() => Validate(withoutHand), ".HandsByPlayerId");

            var response = new GameSnapshotResponseDto { Snapshot = CreateSnapshot(), Revision = InitialRevision };
            var envelope = JObject.Parse(Encoding.UTF8.GetString(serializer.Serialize(response)));
            envelope.Property("Revision")?.Remove();
            ExpectJsonFailure(() => serializer.Deserialize<GameSnapshotResponseDto>(Encoding.UTF8.GetBytes(envelope.ToString())));
        }

        private static void CheckRolesAndAtomicLoading()
        {
            var serializer = new NewtonsoftSerializer();
            var server = CreateContext(GameContextRole.Server, HostPlayerId);
            var notifications = 0;
            server.StateChanged += () => notifications++;
            var invalid = CreateSnapshot();
            invalid.Phase.EndTimeInSeconds = -1;
            ExpectFailure(() => server.LoadSnapshot(InitialRevision, invalid), ".EndTimeInSeconds");
            Require(!server.IsInitialized && server.Revision == 0 && notifications == 0, "Failed first load changed context.");
            server.LoadSnapshot(InitialRevision, CreateSnapshot());
            var before = serializer.Serialize(server.ToSnapshot());
            ExpectFailure(() => server.LoadSnapshot(InitialRevision + 1, invalid), ".EndTimeInSeconds");
            ExpectFailure(() => server.LoadSnapshot(InitialRevision - 1, CreateSnapshot()), ".Revision");
            Require(server.IsInitialized && server.Revision == InitialRevision && notifications == 1,
                "A failed load changed revision, readiness or notifications.");
            Require(before.SequenceEqual(serializer.Serialize(server.ToSnapshot())), "A failed load partially replaced state.");

            var projection = new GameStateProjector(server).CreateSnapshot(HostPlayerId);
            ExpectFailure(() => server.LoadSnapshot(InitialRevision, projection), ".SpaceDeck");
            var client = CreateContext(GameContextRole.Client, HostPlayerId);
            ExpectFailure(() => client.LoadSnapshot(InitialRevision, CreateSnapshot()), ".SpaceDeck");
            client.LoadSnapshot(InitialRevision, projection);
            var clientBefore = serializer.Serialize(client.ToSnapshot());
            var clientNotifications = 0;
            client.StateChanged += () => clientNotifications++;
            projection.Cards.HandsByPlayerId[HostPlayerId] = new PlayerHandSnapshot(0);
            ExpectFailure(() => client.LoadSnapshot(InitialRevision + 1, projection), ".Cards");
            Require(client.Revision == InitialRevision && clientNotifications == 0 &&
                clientBefore.SequenceEqual(serializer.Serialize(client.ToSnapshot())), "Rejected client state was installed.");

            projection = new GameStateProjector(server).CreateSnapshot(HostPlayerId);
            projection.Cards.HandsByPlayerId[RemotePlayerId] = new PlayerHandSnapshot(-1);
            ExpectFailure(() => client.LoadSnapshot(InitialRevision, projection), ".Count");
            projection.Cards.HandsByPlayerId[RemotePlayerId] = new PlayerHandSnapshot(new List<SpaceCardSnapshot>());
            ExpectFailure(() => client.LoadSnapshot(InitialRevision, projection), ".Cards");
            projection.Cards.HandsByPlayerId[RemotePlayerId] = new PlayerHandSnapshot(0);
            client.LoadSnapshot(InitialRevision, projection);
            Require(!client.Cards.GetHand(RemotePlayerId).IsKnown && client.Cards.GetHand(RemotePlayerId).Count == 0,
                "A valid empty hidden hand was rejected.");
        }

        private static void CheckExtensibilityAndCycles()
        {
            var data = new ExtraSnapshot { Value = -1 };
            ExpectFailure(() => StateValidator.Validate(data, GameContextRole.Server, HostPlayerId, InitialRevision), ".Value");
            data.Value = 0;
            StateValidator.Validate(data, GameContextRole.Server, HostPlayerId, InitialRevision);
            data.Next = data;
            ExpectFailure(() => StateValidator.Validate(data, GameContextRole.Server, HostPlayerId, InitialRevision), ".Next");
        }

        private static void Reject(Action<GameSnapshot> mutate, string expectedPath)
        {
            var snapshot = CreateSnapshot();
            mutate(snapshot);
            ExpectFailure(() => Validate(snapshot), expectedPath);
        }

        private static void Validate(GameSnapshot snapshot)
        {
            StateValidator.Validate(snapshot, GameContextRole.Server, HostPlayerId, InitialRevision);
        }

        private static void ExpectFailure(Action action, string expectedPath)
        {
            try
            {
                action();
            }
            catch (StateValidationException exception)
            {
                Require(exception.Path.Contains(expectedPath), "Validation reported an unexpected field path.");
                return;
            }

            throw new InvalidOperationException("Invalid state was accepted.");
        }

        private static void ExpectJsonFailure(Action action)
        {
            try
            {
                action();
            }
            catch (JsonSerializationException)
            {
                return;
            }

            throw new InvalidOperationException("A required JSON field was missing or null but was accepted.");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }

        private static GameContext CreateContext(GameContextRole role, ulong localPlayerId)
        {
            var dependencies = new TestDependencies(localPlayerId);
            return new GameContext(dependencies, dependencies, role);
        }

        private static GameSnapshot CreateSnapshot()
        {
            return new GameSnapshot
            {
                GamePlayers = new GamePlayersSnapshot
                {
                    Players = new List<GamePlayerSnapshot>
                    {
                        new() { PlayerId = HostPlayerId, Name = "Host", Order = 0 },
                        new() { PlayerId = RemotePlayerId, Name = "Remote", Order = 1 }
                    }
                },
                Cards = new CardsSnapshot
                {
                    SpaceDeck = new List<SpaceCardSnapshot>(),
                    DestinyDeck = new List<DestinyCardSnapshot>(),
                    HandsByPlayerId = new Dictionary<ulong, PlayerHandSnapshot>
                    {
                        [HostPlayerId] = new(new List<SpaceCardSnapshot>()),
                        [RemotePlayerId] = new(new List<SpaceCardSnapshot> { SpaceCardSnapshot.ConversationStateCard() })
                    }
                },
                Galaxy = new GalaxySnapshot
                {
                    Planets = new Dictionary<ulong, List<PlanetSnapshot>>
                    {
                        [HostPlayerId] = new() { new PlanetSnapshot { PlanetId = HostPlanetId, ShipIds = new List<int> { HostShipId } } },
                        [RemotePlayerId] = new() { new PlanetSnapshot { PlanetId = RemotePlanetId, ShipIds = new List<int> { RemoteShipId } } }
                    },
                    Ships = new Dictionary<ulong, List<SpaceShipSnapshot>>
                    {
                        [HostPlayerId] = new() { new SpaceShipSnapshot { ShipId = HostShipId } },
                        [RemotePlayerId] = new() { new SpaceShipSnapshot { ShipId = RemoteShipId } }
                    }
                },
                Encounter = new EncounterSnapshot(),
                Phase = new PhaseSnapshot { CurrentPhase = GamePhaseType.Initialization, Revision = InitialRevision }
            };
        }

        private sealed class ExtraSnapshot
        {
            [StateVisibility(StateAudience.Everyone)]
            [StateRequired]
            [StateRange(0)]
            public int Value;

            [StateVisibility(StateAudience.Everyone)]
            public ExtraSnapshot? Next;
        }

        private sealed class TestDependencies : IGameDataProvider, IGameServerTime
        {
            public TestDependencies(ulong localPlayerId)
            {
                LocalPlayerId = localPlayerId;
            }

            public ulong LocalPlayerId { get; }

            public double ServerTimeInSeconds => 0;

            public void Subscribe(IGameServerTime.Update action)
            {
            }

            public void Unsubscribe(IGameServerTime.Update action)
            {
            }
        }
    }
}
#endif
