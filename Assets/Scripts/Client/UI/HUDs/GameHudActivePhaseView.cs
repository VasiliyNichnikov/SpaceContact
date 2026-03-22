using Client.UI.Extensions;
using Core.Game.Phases;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI.HUDs
{
    public class GameHudActivePhaseView : MonoBehaviour
    {
        [SerializeField]
        private Text _phaseNameText = null!;

        public void ShowPhase(GamePhaseType phase)
        {
            _phaseNameText.SetText(phase.ToName());
        }
    }
}