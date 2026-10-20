using Client.Attributes;
using Client.UI.HUDs.ViewModels;
using Client.UI.Utils;
using Reactivity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Client.UI.HUDs
{
    public sealed class GameHudTimerPhaseView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _valueText = null!;

        [SerializeField]
        private GameObject _markGameObject = null!;

        [SerializeField]
        private Button _readyButton = null!;

        private IGameHudTimerPhaseViewModel? _viewModel;

        [CalledFromUnity]
        public void OnReadyButtonClick() =>
            _viewModel?.OnReadyButtonClickHandler();

        [Inject]
        private void Constructor(IGameHudTimerPhaseViewModel viewModel)
        {
            gameObject.UpdateViewModelDisposable(ref _viewModel!, viewModel);
            viewModel.Init();
            gameObject.Subscribe(viewModel.RemainingTimeInSeconds, SetValueText);
            gameObject.Subscribe(viewModel.IsReadyToNextPhase, _markGameObject.SetActive);
            gameObject.Subscribe(viewModel.CanToggleReady, SetReadyButtonInteractable);
        }

        private void Update() =>
            _viewModel?.Update();

        private void SetValueText(int value)
        {
            _valueText.SetText(UIUtils.SecondsToTimeFormat(value));
        }

        private void SetReadyButtonInteractable(bool canToggleReady)
        {
            _readyButton.interactable = canToggleReady;
        }
    }
}
