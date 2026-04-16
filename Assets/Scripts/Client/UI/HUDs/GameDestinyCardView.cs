using Client.Attributes;
using Client.UI.HUDs.ViewModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI.HUDs
{
    public class GameDestinyCardView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _descriptionText = null!;
        
        [SerializeField]
        private Image _background = null!;
        
        [SerializeField]
        private GameObject _skipDestinyButtonGameObject = null!;

        private IGameDestinyCardViewModel _viewModel = null!;
        
        public void Init(IGameDestinyCardViewModel viewModel)
        {
            _viewModel = viewModel;
            _background.color = viewModel.BackgroundColor;
            _descriptionText.SetText(viewModel.Description);
            _skipDestinyButtonGameObject.SetActive(viewModel.IsSkipButtonVisible);
        }

        public void Show() => 
            gameObject.SetActive(true);
        
        public void Hide() =>
            gameObject.SetActive(false);
        
        [CalledFromUnity]
        public void OnSkipButtonClick() => 
            _viewModel.OnSkipButtonClickHandler();
    }
}