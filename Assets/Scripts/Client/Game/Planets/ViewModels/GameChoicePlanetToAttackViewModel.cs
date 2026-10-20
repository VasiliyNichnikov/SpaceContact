using Core.Game;
using Core.Game.Actions;
using CoreConvertor;

namespace Client.Game.Planets.ViewModels
{
    public sealed class GameChoicePlanetToAttackViewModel : IGameShipsOnPlanetInfoItemViewModel
    {
        private readonly int _planetId;
        private readonly ulong _defenderPlayerId;
        private readonly GameContext _context;
        private readonly GameActionClient _actionClient;
        private readonly PendingGameActions _pendingActions;

        public GameChoicePlanetToAttackViewModel(int planetId, GamePlayerData defender, GameContext context,
            GameActionClient actionClient, PendingGameActions pendingActions)
        {
            _planetId = planetId;
            _defenderPlayerId = defender.Id;
            _context = context;
            _actionClient = actionClient;
            _pendingActions = pendingActions;
            DefenderColor = ColorConvertor.FromCoreColor(defender.Color);
        }

        public UnityEngine.Color DefenderColor { get; }

        public static bool CanSelect(GameContext context, PendingGameActions pendingActions, int planetId,
            ulong defenderPlayerId)
        {
            var localPlayer = context.LocalPlayerState;
            
            if (localPlayer is null || pendingActions.HasAny)
            {
                return false;
            }

            var validator = new SelectTargetPlanetGameAction(planetId, defenderPlayerId).GetValidator();
            return validator.Check(context, GameActor.Player(localPlayer.Id)).IsSuccess;
        }

        public void OnAttackPlanetButtonClickHandler()
        {
            if (CanSelect(_context, _pendingActions, _planetId, _defenderPlayerId))
            {
                _actionClient.Send(new SelectTargetPlanetGameAction(_planetId, _defenderPlayerId));
            }
        }

        public void Apply(IGameShipsOnPlanetInfoVisitor visitor) => visitor.Visit(this);
    }
}
