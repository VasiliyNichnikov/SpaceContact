using Client.Attributes;
using Client.UI.HUDs.ViewModels;
using Client.UI.Utils;
using Reactivity;
using TMPro;
using UnityEngine;
using VContainer;

namespace Client.UI.HUDs
{
    public sealed class GameHudTimerPhaseView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _valueText = null!;
        
        [SerializeField]
        private GameObject _markGameObject = null!;

        private IGameHudTimerPhaseViewModel? _viewModel;
        
        [Inject]
        private void Constructor(IGameHudTimerPhaseViewModel viewModel)
        {
            gameObject.UpdateViewModelDisposable(ref _viewModel!, viewModel);
            gameObject.Subscribe(_viewModel.RemainingTimeInSeconds, SetValueText);
            gameObject.Subscribe(_viewModel.IsReadyToNextPhase, _markGameObject.SetActive);
        }
        
        [CalledFromUnity]
        public void OnReadyButtonClick() => 
            _viewModel?.OnReadyButtonClickHandler();

        private void Update() => 
            _viewModel?.Update();

        private void SetValueText(int value)
        {
            _valueText.SetText(UIUtils.SecondsToTimeFormat(value));
        }
    }
}