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

        public GameChoicePlanetToAttackViewModel(
            int planetId, 
            GamePlayerData defender, 
            GameContext context,
            GameActionClient actionClient)
        {
            _planetId = planetId;
            _defenderPlayerId = defender.Id;
            _context = context;
            _actionClient = actionClient;
            DefenderColor = ColorConvertor.FromCoreColor(defender.Color);
        }

        public UnityEngine.Color DefenderColor { get; }

        public bool IsButtonVisible() => 
            _actionClient.ValidateAction(CreateSelectTargetPlanetGameAction());

        public void OnAttackPlanetButtonClickHandler()
        {
            var action = CreateSelectTargetPlanetGameAction();
            
            if (_actionClient.ValidateAction(action))
            {
                _actionClient.Send(action);
            }
        }

        public void Apply(IGameShipsOnPlanetInfoVisitor visitor) => visitor.Visit(this);

        private SelectTargetPlanetGameAction CreateSelectTargetPlanetGameAction()
        {
            return new SelectTargetPlanetGameAction(_planetId,
                _context.Phase.Revision,
                _context.Cards.DestinyDeckCount,
                _defenderPlayerId);
        }
    }
}
