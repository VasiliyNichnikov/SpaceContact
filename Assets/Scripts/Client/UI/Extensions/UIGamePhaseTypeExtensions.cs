using System;
using Core.Game.Phases;

namespace Client.UI.Extensions
{
    public static class UIGamePhaseTypeExtensions
    {
        public static string ToName(this GamePhaseType gamePhaseType)
        {
            switch (gamePhaseType)
            {
                case GamePhaseType.Regroup:
                    return "Regroup";
                
                case GamePhaseType.Destiny:
                    return "Destiny";
                
                case GamePhaseType.Launch:
                    return "Launch";
                
                case GamePhaseType.Alliance:
                    return "Alliance";
                
                case GamePhaseType.None:
                case GamePhaseType.Initialization:
                default:
                    throw new ArgumentOutOfRangeException(nameof(gamePhaseType), gamePhaseType, null);
            }
        }
    }
}