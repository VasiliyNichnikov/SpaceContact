#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using Core.Game.Actions;
using Core.Game.Dto;
using Core.Game.Dto.Snapshots;
using Core.Game.Events;
using Core.Game.Server;
using Core.Game;
using Network.Infrastructure;
using Network.Requests;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEditor;
using Logger = Logs.Logger;

namespace Editor
{
    public static class ServerOperationChecks
    {
        private const string MenuPath = "Tools/Space Contact/Check Server Operations";
        private const ulong HostPlayerId = 0;
        private const ulong RemotePlayerId = 1;
        private const ulong UnknownPlayerId = 99;
        private const long InitialRevision = 10;
        private const int ExpectedBlockedCallbacks = 9;

        [MenuItem(MenuPath)]
        private static void Run()
        {
            try
            {
                CheckCommitAndReplay();
                CheckMultipleEventsAndCallbacks();
                CheckSenderScopeAndRejection();
                CheckEmptyAndHiddenOperations();
                CheckProjectionOwnership();
                CheckFailures();
                CheckNetworkHandler();
                Logger.Log("Server operation checks passed.");
            }
            catch (Exception exception)
            {
                Logger.Error($"Server operation checks failed: {exception}");
            }
        }

        [MenuItem(MenuPath, true)]
        private static bool CanRun()
        {
            return !EditorApplication.isPlayingOrWillChangePlaymode;
        }

        private static void CheckCommitAndReplay()
        {
            var context = CreateContext();
            var executor = new GameActionExecutor(context, new GameEventBatchFactory());
            var action = new TogglePlayerReadyGameAction();
            var serializer = new NewtonsoftSerializer();
            var wire = serializer.Serialize(action);
            var notifications = 0;
            context.StateChanged += () => notifications++;

            var first = executor.Execute(action, GameActor.Player(HostPlayerId));
            Require(first.IsSuccess && first.HasChanges && !first.IsDuplicate, "The first action was not accepted.");
            Require(context.Revision == InitialRevision + 1 && notifications == 1, "The action did not commit exactly once.");
            Require(first.BatchesByRecipient.Count == 2 &&
                first.BatchesByRecipient.Values.All(batch => batch.Revision == first.Revision), "Recipient revisions differ.");

            executor.Execute(new TogglePlayerReadyGameAction(), GameActor.Player(HostPlayerId));
            var replay = executor.Execute(serializer.Deserialize<TogglePlayerReadyGameAction>(wire), GameActor.Player(HostPlayerId));
            Require(replay.IsDuplicate && replay.Revision == first.Revision, "A replay lost its original revision.");
            Require(!context.Players.Get(HostPlayerId).IsReady && context.Revision == InitialRevision + 2 && notifications == 2,
                "A replay mutated state or notified observers.");
            var originalEvent = (PlayerReadyToggledGameEvent)replay.BatchesByRecipient[HostPlayerId].Events.Single();
            Require(originalEvent.IsReady, "A replay was rebuilt from newer state.");

            var missingId = JObject.Parse(Encoding.UTF8.GetString(wire));
            missingId.Property(nameof(BaseGameAction.Id))?.Remove();
            ExpectException<JsonSerializationException>(() =>
                serializer.Deserialize<TogglePlayerReadyGameAction>(Encoding.UTF8.GetBytes(missingId.ToString())));
        }

        private static void CheckMultipleEventsAndCallbacks()
        {
            var context = CreateContext();
            var executor = new GameActionExecutor(context, new GameEventBatchFactory());
            var secondExecutor = new GameActionExecutor(context, new GameEventBatchFactory());
            var events = new List<IGameEvent>
            {
                new PlayerReadyToggledGameEvent(HostPlayerId, true),
                new PlayerReadyToggledGameEvent(RemotePlayerId, true)
            };
            var snapshot = context.ToSnapshot();
            var intermediateRevisions = new List<long>();
            var blockedCallbacks = 0;
            var completedNotifications = 0;
            var completedStateWasConsistent = false;
            context.Players.OnReadinessPlayerChanged += _ =>
            {
                intermediateRevisions.Add(context.Revision);
                blockedCallbacks += Throws<InvalidOperationException>(() =>
                    secondExecutor.Execute(new TogglePlayerReadyGameAction(), GameActor.Player(HostPlayerId))) ? 1 : 0;
                blockedCallbacks += Throws<InvalidOperationException>(() => context.LoadSnapshot(InitialRevision, snapshot)) ? 1 : 0;
                blockedCallbacks += Throws<InvalidOperationException>(() => context.CommitRevision(InitialRevision + 1)) ? 1 : 0;
                blockedCallbacks += Throws<InvalidOperationException>(() => context.ToSnapshot()) ? 1 : 0;
                events.Clear();
            };
            context.StateChanged += () =>
            {
                completedNotifications++;
                completedStateWasConsistent = context.Revision == InitialRevision + 1 &&
                    context.ToSnapshot().GamePlayers.Players.All(player => player.IsReady);
                blockedCallbacks += Throws<InvalidOperationException>(() =>
                    executor.Execute(new TogglePlayerReadyGameAction(), GameActor.Player(HostPlayerId))) ? 1 : 0;
            };

            var result = executor.Execute(new TestAction(Guid.NewGuid(), (_, _) => GameActionResult.Success(events)), GameActor.System());
            Require(result.IsSuccess && intermediateRevisions.SequenceEqual(new[] { InitialRevision, InitialRevision }),
                "The event sequence changed during execution or revision advanced between events.");
            Require(blockedCallbacks == ExpectedBlockedCallbacks && completedNotifications == 1 && completedStateWasConsistent,
                "An observer saw an incomplete commit or started a nested operation.");
            Require(result.BatchesByRecipient.Values.All(batch => batch.Events.Length == 2), "A multi-event batch lost an event.");
        }

        private static void CheckSenderScopeAndRejection()
        {
            var context = CreateContext();
            var executor = new GameActionExecutor(context, new GameEventBatchFactory());
            var executions = 0;
            var sharedId = Guid.NewGuid();
            var action = new TestAction(sharedId, (_, actor) =>
            {
                executions++;
                return GameActionResult.Success(new PlayerReadyToggledGameEvent(actor.PlayerId ?? HostPlayerId, true));
            });
            Require(!executor.Execute(action, GameActor.Player(UnknownPlayerId)).IsSuccess && executions == 0,
                "An unknown sender executed an action.");
            Require(!executor.Execute(new TestAction(Guid.Empty, (_, _) => throw new InvalidOperationException()),
                GameActor.Player(HostPlayerId)).IsSuccess, "An empty action ID was accepted.");
            executor.Execute(action, GameActor.Player(HostPlayerId));
            executor.Execute(action, GameActor.Player(RemotePlayerId));
            executor.Execute(action, GameActor.System());
            Require(executions == 3 && context.Revision == InitialRevision + 3, "Sender or system action IDs collided.");

            var rejected = executor.Execute(new TestAction(Guid.NewGuid(), (_, _) =>
                GameActionResult.Failed("Rejected by the action.", new[] { new PlayerReadyToggledGameEvent(HostPlayerId, false) })),
                GameActor.Player(HostPlayerId));
            Require(!rejected.IsSuccess && rejected.BatchesByRecipient.Count == 0 && context.Players.Get(HostPlayerId).IsReady &&
                context.Revision == InitialRevision + 3 && !context.IsOperationFaulted, "A rejection applied events or faulted the session.");
        }

        private static void CheckEmptyAndHiddenOperations()
        {
            var context = CreateContext();
            var executor = new GameActionExecutor(context, new GameEventBatchFactory());
            var notifications = 0;
            context.StateChanged += () => notifications++;
            var empty = new TestAction(Guid.NewGuid(), (_, _) => GameActionResult.Success(Array.Empty<IGameEvent>()));
            var unchanged = executor.Execute(empty, GameActor.System());
            Require(unchanged.IsSuccess && !unchanged.HasChanges && context.Revision == InitialRevision && notifications == 0,
                "An empty operation advanced state.");
            Require(executor.Execute(empty, GameActor.System()).IsDuplicate, "An accepted empty operation was not cached.");

            // A synthetic server event exercises transport semantics with no recipient events.
            var hidden = new TestEvent(state => state.Players.SetReadyState(HostPlayerId, true),
                (_, _) => Array.Empty<IGameEvent>());
            var changed = executor.Execute(new TestAction(Guid.NewGuid(), (_, _) => GameActionResult.Success(hidden)), GameActor.System());
            Require(changed.HasChanges && context.Revision == InitialRevision + 1 && notifications == 1 &&
                changed.BatchesByRecipient.Values.All(batch => batch.Revision == changed.Revision && batch.Events.Length == 0),
                "A hidden operation failed to produce empty batches with its new revision.");
        }

        private static void CheckProjectionOwnership()
        {
            var context = CreateContext();
            var executor = new GameActionExecutor(context, new GameEventBatchFactory());
            var cards = new List<SpaceCardSnapshot> { SpaceCardSnapshot.ConversationStateCard() };
            var hand = new PlayerHandSnapshot(cards);
            var action = new TestAction(Guid.NewGuid(), (_, _) => GameActionResult.Success(new SetPlayerHandGameEvent(HostPlayerId, hand)));
            var result = executor.Execute(action, GameActor.System());
            cards.Clear();
            var ownerEvent = (SetPlayerHandGameEvent)result.BatchesByRecipient[HostPlayerId].Events.Single();
            var otherEvent = (SetPlayerHandGameEvent)result.BatchesByRecipient[RemotePlayerId].Events.Single();
            Require(ownerEvent.Hand.Cards?.Count == 1 && otherEvent.Hand.Cards is null && otherEvent.Hand.Count == 1 &&
                context.Cards.GetHand(HostPlayerId).Count == 1, "Projected batches alias the action payload or leak a hand.");
        }

        private static void CheckFailures()
        {
            foreach (var failDuringProjection in new[] { false, true })
            {
                var context = CreateContext();
                var executor = new GameActionExecutor(context, new GameEventBatchFactory());
                var failure = new TestEvent(_ =>
                {
                    if (!failDuringProjection)
                    {
                        throw new InvalidOperationException("Expected event failure.");
                    }
                }, (_, _) => throw new InvalidOperationException("Expected projection failure."));
                var action = new TestAction(Guid.NewGuid(), (_, _) => GameActionResult.Success(new IGameEvent[]
                {
                    new PlayerReadyToggledGameEvent(HostPlayerId, true), failure
                }));
                var notifications = 0;
                context.StateChanged += () => notifications++;
                ExpectException<InvalidOperationException>(() => executor.Execute(action, GameActor.System()));
                Require(context.IsOperationFaulted && context.Revision == InitialRevision && notifications == 0,
                    "A failed operation was published as a committed revision.");
                ExpectException<InvalidOperationException>(() => executor.Execute(action, GameActor.System()));
                ExpectException<InvalidOperationException>(() => new GameStateProjector(context).CreateSnapshot(HostPlayerId));
            }

            var server = CreateContext();
            var successfulObserverCalled = false;
            server.StateChanged += () => throw new InvalidOperationException("Expected observer failure; commit must stay successful.");
            server.StateChanged += () => successfulObserverCalled = true;
            var serverExecutor = new GameActionExecutor(server, new GameEventBatchFactory());
            var accepted = serverExecutor.Execute(new TogglePlayerReadyGameAction(), GameActor.Player(HostPlayerId));
            Require(accepted.IsSuccess && successfulObserverCalled && !server.IsOperationFaulted,
                "An observer failure invalidated a successful commit or skipped other observers.");

            var exhausted = CreateContext(long.MaxValue);
            var limited = new GameActionExecutor(exhausted, new GameEventBatchFactory())
                .Execute(new TogglePlayerReadyGameAction(), GameActor.Player(HostPlayerId));
            Require(!limited.IsSuccess && !exhausted.Players.Get(HostPlayerId).IsReady && !exhausted.IsOperationFaulted,
                "Revision overflow changed state.");

            var dependencies = new TestDependencies();
            var notInitialized = new GameContext(dependencies, dependencies, GameContextRole.Server);
            ExpectException<InvalidOperationException>(() => new GameActionExecutor(notInitialized, new GameEventBatchFactory())
                .Execute(new TogglePlayerReadyGameAction(), GameActor.Player(HostPlayerId)));
            var client = new GameContext(dependencies, dependencies, GameContextRole.Client);
            client.LoadSnapshot(InitialRevision, new GameStateProjector(CreateContext()).CreateSnapshot(HostPlayerId));
            ExpectException<InvalidOperationException>(() => new GameActionExecutor(client, new GameEventBatchFactory())
                .Execute(new TogglePlayerReadyGameAction(), GameActor.Player(HostPlayerId)));
            ExpectException<InvalidOperationException>(() => client.CommitRevision(InitialRevision));
            ExpectException<InvalidOperationException>(() => client.CommitRevision(InitialRevision + 2));
        }

        private static void CheckNetworkHandler()
        {
            var context = CreateContext();
            var serializer = new NewtonsoftSerializer();
            var broadcaster = new TestBroadcaster();
            var handler = new ApplyGameActionNetworkRequestHandler(serializer,
                new GameActionExecutor(context, new GameEventBatchFactory()), broadcaster);
            var request = serializer.Serialize(new GameApplyActionRequestDto { Action = new TogglePlayerReadyGameAction() });
            handler.Handle(request, UnknownPlayerId);
            Require(context.Revision == InitialRevision && broadcaster.Recipients.Count == 0, "Unknown sender reached delivery.");
            var response = handler.Handle(request, HostPlayerId);
            var duplicate = handler.Handle(request, HostPlayerId);
            Require(response.SequenceEqual(duplicate) && broadcaster.Recipients.SequenceEqual(new[] { RemotePlayerId }) &&
                context.Revision == InitialRevision + 1, "A repeated request re-executed or rebroadcast its events.");

            broadcaster.ThrowOnSend = true;
            var next = serializer.Serialize(new GameApplyActionRequestDto { Action = new TogglePlayerReadyGameAction() });
            ExpectException<InvalidOperationException>(() => handler.Handle(next, HostPlayerId));
            broadcaster.ThrowOnSend = false;
            handler.Handle(next, HostPlayerId);
            Require(!context.Players.Get(HostPlayerId).IsReady && context.Revision == InitialRevision + 2 &&
                broadcaster.Recipients.Count == 1, "A delivery error caused the accepted action to execute twice.");
        }

        private static GameContext CreateContext(long revision = InitialRevision)
        {
            var dependencies = new TestDependencies();
            var context = new GameContext(dependencies, dependencies, GameContextRole.Server);
            var snapshot = new GameSnapshot
            {
                GamePlayers = new GamePlayersSnapshot(),
                Galaxy = new GalaxySnapshot(),
                Cards = new CardsSnapshot(),
                Encounter = new EncounterSnapshot(),
                Phase = new PhaseSnapshot()
            };
            foreach (var playerId in new[] { HostPlayerId, RemotePlayerId })
            {
                snapshot.GamePlayers.Players.Add(new GamePlayerSnapshot
                {
                    PlayerId = playerId, Name = $"Player {playerId}", Order = (int)playerId
                });
                snapshot.Galaxy.Planets.Add(playerId, new List<PlanetSnapshot>());
                snapshot.Galaxy.Ships.Add(playerId, new List<SpaceShipSnapshot>());
                snapshot.Cards.HandsByPlayerId.Add(playerId, new PlayerHandSnapshot(new List<SpaceCardSnapshot>()));
            }

            snapshot.Cards.SpaceDeck = new List<SpaceCardSnapshot>();
            snapshot.Cards.DestinyDeck = new List<DestinyCardSnapshot>();
            snapshot.Phase.Revision = revision;
            context.LoadSnapshot(revision, snapshot);
            return context;
        }

        private static bool Throws<TException>(Action action) where TException : Exception
        {
            try
            {
                action();
                return false;
            }
            catch (TException)
            {
                return true;
            }
        }

        private static void ExpectException<TException>(Action action) where TException : Exception
        {
            Require(Throws<TException>(action), $"Expected {typeof(TException).Name} was not thrown.");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }

        private sealed class TestAction : IGameAction
        {
            private readonly Func<GameContext, GameActor, GameActionResult> _execute;

            public TestAction(Guid id, Func<GameContext, GameActor, GameActionResult> execute)
            {
                Id = id;
                _execute = execute;
            }

            public Guid Id { get; }

            public GameActionResult Execute(GameContext context, GameActor actor) => _execute(context, actor);
        }

        private sealed class TestEvent : IGameEvent
        {
            private readonly Action<GameContext> _apply;
            private readonly Func<GameContext, ulong, IEnumerable<IGameEvent>> _project;

            public TestEvent(Action<GameContext> apply, Func<GameContext, ulong, IEnumerable<IGameEvent>> project)
            {
                _apply = apply;
                _project = project;
            }

            public void Apply(GameContext context) => _apply(context);

            public IEnumerable<IGameEvent> ToClientEvents(GameContext context, ulong recipientPlayerId) => _project(context, recipientPlayerId);
        }

        private sealed class TestBroadcaster : IServerEventBroadcaster
        {
            public List<ulong> Recipients { get; } = new();

            public bool ThrowOnSend { get; set; }

            public void SendEvent(ulong clientId, GameEventBatchData batch)
            {
                if (ThrowOnSend)
                {
                    throw new InvalidOperationException("Expected delivery failure.");
                }

                Recipients.Add(clientId);
            }
        }

        private sealed class TestDependencies : IGameDataProvider, IGameServerTime
        {
            public ulong LocalPlayerId => HostPlayerId;

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
