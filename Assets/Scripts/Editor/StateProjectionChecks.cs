#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using Core.Game.Dto.Snapshots;
using Core.Game.Events;
using Core.Game.Visibility;
using Core.Game;
using Logger = Logs.Logger;
using Network.Infrastructure;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEditor;

namespace Editor
{
    public static class StateProjectionChecks
    {
        private const ulong HostPlayerId = 0;
        private const ulong RemotePlayerId = 1;
        private const long InitialRevision = 10;
        private const int HostCardDamage = 77;
        private const int RemoteCardDamage = 88;
        private const string MenuPath = "Tools/Space Contact/Check State Projection";

        [MenuItem(MenuPath)]
        private static void Run()
        {
            try
            {
                CheckSnapshotsAndEvents();
                CheckSchemaAndParentVisibility();
                Logger.Log("State projection checks passed.");
            }
            catch (Exception exception)
            {
                Logger.Error($"State projection checks failed: {exception}");
            }
        }

        [MenuItem(MenuPath, true)]
        private static bool CanRun()
        {
            return !EditorApplication.isPlayingOrWillChangePlaymode;
        }

        private static void CheckSnapshotsAndEvents()
        {
            var serializer = new NewtonsoftSerializer();
            var server = CreateContext(GameContextRole.Server, HostPlayerId);
            server.LoadSnapshot(InitialRevision, CreateSnapshot());
            var original = serializer.Serialize(server.ToSnapshot());
            var projector = new GameStateProjector(server);
            var hostSnapshot = projector.CreateSnapshot(HostPlayerId);
            var remoteSnapshot = projector.CreateSnapshot(RemotePlayerId);
            CheckWire(serializer, hostSnapshot, HostPlayerId, RemotePlayerId);
            CheckWire(serializer, remoteSnapshot, RemotePlayerId, HostPlayerId);
            Require(original.SequenceEqual(serializer.Serialize(server.ToSnapshot())), "Projection changed server state.");

            var host = CreateContext(GameContextRole.Client, HostPlayerId);
            var remote = CreateContext(GameContextRole.Client, RemotePlayerId);
            host.LoadSnapshot(InitialRevision, serializer.Deserialize<GameSnapshot>(serializer.Serialize(hostSnapshot)));
            remote.LoadSnapshot(InitialRevision, serializer.Deserialize<GameSnapshot>(serializer.Serialize(remoteSnapshot)));
            Require(host.Cards.GetHand(HostPlayerId).IsKnown && !host.Cards.GetHand(RemotePlayerId).IsKnown,
                "Host hand visibility is incorrect.");
            Require(remote.Cards.GetHand(RemotePlayerId).IsKnown && !remote.Cards.GetHand(HostPlayerId).IsKnown,
                "Remote hand visibility is incorrect.");
            Require(host.Cards.GetHand(RemotePlayerId).Count == 1 && remote.Cards.GetHand(HostPlayerId).Count == 1,
                "Hidden hand counts did not survive network deserialization.");
            Require(!host.Cards.IsSpaceDeckKnown && !remote.Cards.IsDestinyDeckKnown, "A client knows a closed deck.");
            Require(hostSnapshot.Cards.ActiveDestinyCard is null, "A future Destiny card was revealed.");
            Require(hostSnapshot.Cards.SpaceDiscard.Count == 2, "Public discard cards were lost.");
            Require(hostSnapshot.Cards.SpaceDiscard[0].DamageCount == 1, "Discard order is not canonical.");

            var handEvent = new SetPlayerHandGameEvent(HostPlayerId, new PlayerHandSnapshot(new List<SpaceCardSnapshot>()));
            handEvent.Apply(server);
            var hostEvent = handEvent.ToClientEvents(server, HostPlayerId).Single();
            var remoteEvent = handEvent.ToClientEvents(server, RemotePlayerId).Single();
            serializer.Deserialize<IGameEvent>(serializer.Serialize<IGameEvent>(hostEvent)).Apply(host);
            var remoteBytes = serializer.Serialize<IGameEvent>(remoteEvent);
            Require(JObject.Parse(Encoding.UTF8.GetString(remoteBytes))["Hand"]?["Cards"] is null,
                "A hand event leaked foreign cards.");
            serializer.Deserialize<IGameEvent>(remoteBytes).Apply(remote);
            Require(host.Cards.GetHand(HostPlayerId).IsKnown && host.Cards.GetHand(HostPlayerId).Count == 0,
                "An empty known hand became hidden.");
            Require(!remote.Cards.GetHand(HostPlayerId).IsKnown && remote.Cards.GetHand(HostPlayerId).Count == 0,
                "Hidden hand count was not replaced.");
            Require(handEvent.Hand.Cards is not null, "Event projection modified the domain event.");

            var refillCards = new List<SpaceCardSnapshot>
            {
                SpaceCardSnapshot.DamageStateCard(HostCardDamage), SpaceCardSnapshot.ConversationStateCard()
            };
            var refill = new SetPlayerHandGameEvent(HostPlayerId, new PlayerHandSnapshot(refillCards));
            refill.Apply(server);
            var refillEvent = refill.ToClientEvents(server, RemotePlayerId).Single();
            serializer.Deserialize<IGameEvent>(serializer.Serialize<IGameEvent>(refillEvent)).Apply(remote);
            Require(!remote.Cards.GetHand(HostPlayerId).IsKnown && remote.Cards.GetHand(HostPlayerId).Count == refillCards.Count,
                "A nonzero hidden hand count did not survive event serialization.");

            server.Phase.SetPhase(GamePhaseType.Destiny, InitialRevision, null);
            server.Cards.SetActiveDestinyCard(DestinyCardData.Convert(DestinyCardSnapshot.ColorCard(RemotePlayerId)));
            Require(projector.CreateSnapshot(HostPlayerId).Cards.ActiveDestinyCard?.SelectedPlayerId == RemotePlayerId,
                "The revealed Destiny card is missing.");
            var hostCards = hostSnapshot.Cards.HandsByPlayerId[HostPlayerId].Cards;
            Require(hostCards is not null, "Owner cards are missing.");
            hostCards?.Clear();
            Require(remoteSnapshot.Cards.HandsByPlayerId[HostPlayerId].Count == 1,
                "Recipient snapshots share mutable state.");
        }

        private static void CheckWire(NewtonsoftSerializer serializer, GameSnapshot snapshot, ulong owner, ulong other)
        {
            var wire = JObject.Parse(Encoding.UTF8.GetString(serializer.Serialize(snapshot)));
            var cards = wire["Cards"];
            Require(cards is not null, "Cards section is missing.");
            Require(cards?["SpaceDeck"] is null && cards?["DestinyDeck"] is null, "Closed decks appear in the wire payload.");
            var hands = cards?["HandsByPlayerId"];
            Require(hands?[owner.ToString()]?["Cards"] is JArray, "Owner card contents are missing.");
            Require(hands?[other.ToString()]?["Cards"] is null, "Foreign card contents appear in the wire payload.");
            Require((int?)hands?[other.ToString()]?["Count"] == 1, "Foreign hand count is missing.");
            Require((int?)hands?[owner.ToString()]?["Count"] == 1, "Owner hand count is missing.");
        }

        private static void CheckSchemaAndParentVisibility()
        {
            var source = new RestrictedContainer { Hidden = new PublicChild { Value = "secret" } };
            var projected = StateProjection.ToClient(source, HostPlayerId);
            Require(projected.Hidden is null && source.Hidden is not null,
                "A child bypassed its parent's visibility through a JSON property alias.");
            RequireThrows(() => StateProjection.ValidateSchema(typeof(UnmarkedSnapshot)), "Unmarked snapshot was accepted.");
            var secretEvent = new RestrictedPublicEvent();
            RequireThrows(() => StateProjection.EnsurePublic(secretEvent, RemotePlayerId),
                "An incomplete public event was accepted.");
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
                Galaxy = new GalaxySnapshot
                {
                    Planets = new Dictionary<ulong, List<PlanetSnapshot>>
                    {
                        [HostPlayerId] = new(),
                        [RemotePlayerId] = new()
                    },
                    Ships = new Dictionary<ulong, List<SpaceShipSnapshot>>
                    {
                        [HostPlayerId] = new(),
                        [RemotePlayerId] = new()
                    }
                },
                Encounter = new EncounterSnapshot(),
                Phase = new PhaseSnapshot { CurrentPhase = GamePhaseType.Initialization, Revision = InitialRevision },
                Cards = new CardsSnapshot
                {
                    SpaceDeck = new List<SpaceCardSnapshot> { SpaceCardSnapshot.ConversationStateCard() },
                    DestinyDeck = new List<DestinyCardSnapshot> { DestinyCardSnapshot.ColorCard(RemotePlayerId) },
                    SpaceDiscard = new List<SpaceCardSnapshot>
                    {
                        SpaceCardSnapshot.DamageStateCard(2), SpaceCardSnapshot.DamageStateCard(1)
                    },
                    HandsByPlayerId = new Dictionary<ulong, PlayerHandSnapshot>
                    {
                        [HostPlayerId] = new(new List<SpaceCardSnapshot> { SpaceCardSnapshot.DamageStateCard(HostCardDamage) }),
                        [RemotePlayerId] = new(new List<SpaceCardSnapshot> { SpaceCardSnapshot.DamageStateCard(RemoteCardDamage) })
                    }
                }
            };
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }

        private static void RequireThrows(Action action, string message)
        {
            try
            {
                action();
            }
            catch (InvalidOperationException)
            {
                return;
            }

            throw new InvalidOperationException(message);
        }

        [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
        private sealed class RestrictedContainer
        {
            [JsonProperty("hiddenChild")]
            [StateVisibility(StateAudience.ServerOnly)]
            public PublicChild? Hidden { get; set; }
        }

        private sealed class PublicChild
        {
            [StateVisibility(StateAudience.Everyone)]
            public string Value { get; set; } = string.Empty;
        }

        private sealed class UnmarkedSnapshot
        {
            public string Value { get; set; } = string.Empty;
        }

        private sealed class RestrictedPublicEvent : PublicGameEvent
        {
            [StateVisibility(StateAudience.Owner)]
            public string Secret { get; } = "secret";

            public override void Apply(GameContext context)
            {
            }
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
