using System;
using Logs;

namespace Core.Game.Phases
{
    public sealed class GamePhaseTimeController
    {
        private readonly IGameServerTime _serverTime;
        private double _endTimeInSeconds;

        public GamePhaseTimeController(IGameServerTime serverTime)
        {
            _serverTime = serverTime;
        }
        
        public bool IsFinished => 
            _endTimeInSeconds <= _serverTime.ServerTimeInSeconds;
        
        public int RemainingTime
        {
            get
            {
                var remainingTime = _endTimeInSeconds - _serverTime.ServerTimeInSeconds;

                if (remainingTime <= 0)
                {
                    return 0;
                }
                
                return (int)Math.Round(remainingTime, MidpointRounding.AwayFromZero);
            }
        }
        
        public void SetEndTimeInSeconds(double value)
        {
            if (value < 0)
            {
                Logger.Error($"{nameof(GamePhaseTimeController)}.{nameof(SetEndTimeInSeconds)}: the value cannot be negative.");
                
                return;
            }
            
            _endTimeInSeconds = value;
        }
    }
}