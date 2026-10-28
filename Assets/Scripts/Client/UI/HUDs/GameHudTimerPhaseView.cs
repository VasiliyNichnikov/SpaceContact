using Client.Attributes;
using Client.UI.HUDs.ViewModels;
using Client.UI.Utils;
using TMPro;
using Reactivity;
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

        private IGameHudTimerPhaseViewModel _viewModel = null!;

        [CalledFromUnity]
        public void OnReadyButtonClick() =>
            _viewModel.OnReadyButtonClickHandler();

        [Inject]
        private void Constructor(IGameHudTimerPhaseViewModel viewModel)
        {
            gameObject.UpdateChildViewModel(ref _viewModel, viewModel);
            gameObject.Subscribe(viewModel.RemainingTimeInSeconds, SetValueText);
            gameObject.Subscribe(viewModel.IsReadyToNextPhase, _markGameObject.SetActive);
            gameObject.Subscribe(viewModel.CanToggleReady, SetReadyButtonInteractable);
        }

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
