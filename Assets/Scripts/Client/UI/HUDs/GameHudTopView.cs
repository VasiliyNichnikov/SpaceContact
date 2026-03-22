using System.Linq;
using Client.UI.HUDs.ViewModels;
using Reactivity;
using UnityEngine;

namespace Client.UI.HUDs
{
    public class GameHudTopView : MonoBehaviour
    {
        [SerializeField]
        private GameHudActivePhaseView _activePhaseView = null!;
        
        [SerializeField]
        private GameHudPhaseScaleView _phaseScaleView = null!;

        private IGameHudTopViewModel _viewModel = null!;
        
        public void Init(IGameHudTopViewModel viewModel)
        {
            gameObject.UpdateChildViewModel(ref _viewModel, viewModel);
            gameObject.SubscribeWithoutCall(_viewModel.ActivePhaseChangedEvent, ChangeActivePhase);
        }

        private void ChangeActivePhase()
        {
            _activePhaseView.ShowPhase(_viewModel.ActiveGamePhaseType);
            _phaseScaleView.ShowScale(_viewModel.PhaseScale.ToArray());
        }
    }
}