using System;
using Client.UI.Extensions;
using Core.Game.Phases;
using UnityEngine;
using UnityEngine.UI;
using Logger = Logs.Logger;

namespace Client.UI.HUDs
{
    public class GameHudPhaseScaleView : MonoBehaviour
    {
        [Serializable]
        private struct PhaseViewItem
        {
            public Text PhaseName;
        }
        
        [SerializeField]
        private PhaseViewItem[] _phaseViewItems = null!;

        public void ShowScale(GamePhaseType[] phases)
        {
            if (phases.Length != _phaseViewItems.Length)
            {
                Logger.Error("GameHudPhasesView.ShowPhaseScale: the number of phases on the scale does not match the display.");
                
                return;
            }

            for (var i = 0; i < phases.Length; i++)
            {
                var phaseViewItem = _phaseViewItems[i];
                var phaseType  =  phases[i];
                phaseViewItem.PhaseName.SetText(phaseType.ToName());
            }
        }
    }
}