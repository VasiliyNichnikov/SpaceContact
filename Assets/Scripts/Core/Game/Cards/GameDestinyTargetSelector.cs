using Core.Game.Dto.States.Cards;
using Core.Game.Encounter;
using Logs;

namespace Core.Game.Cards
{
    public sealed class GameDestinyTargetSelector
    {
        private readonly IGameClientEncounterManager _encounterManager;
        
        public GameDestinyTargetSelector(IGameClientEncounterManager encounterManager)
        {
            _encounterManager = encounterManager;
        }
        
        /// <summary>
        /// Если равен Null, значит игроку самому надо выбрать защитника
        /// Или мы поймали ошибку
        /// </summary>
        public ulong? GetTarget(DestinyCardStateData card)
        {
            if (card.IsJoker)
            {
                return null;
            }
            
            if (card.IsColorCard)
            {
                if (_encounterManager.AggressorPlayer == null)
                {
                    Logger.Error($"{nameof(GameDestinyTargetSelector)}.{nameof(GetTarget)}: aggressorPlayer is null.");
                    
                    return null;
                }

                if (_encounterManager.AggressorPlayer.PlayerId == card.SelectedPlayerId)
                {
                    return null;
                }
                
                return card.SelectedPlayerId;
            }

            if (card.IsSpecificCard)
            {
                Logger.Warning("GameDestinyTargetSelector.GetTarget: todo, need implementation.");
                return null;
            }
            
            Logger.Error($"{nameof(GameDestinyTargetSelector)}.{nameof(GetTarget)}: card type is not supported.");
            return null;
        }
    }
}