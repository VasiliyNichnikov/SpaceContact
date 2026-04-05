using Client.UI.HUDs.ViewModels;
using Reactivity;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI.HUDs
{
    public class GamePlayerProfileView : MonoBehaviour
    {
        [SerializeField] 
        private Image _backgroundImage = null!;
        
        [SerializeField]
        private Image _defaultFrameImage = null!;
        
        [SerializeField]
        private GameObject _defenderFrameGameObject = null!;
        
        [SerializeField]
        private GameObject _attackerFrameGameObject = null!;

        private GamePlayerProfileViewModel _viewModel = null!;
        
        public void Init(GamePlayerProfileViewModel viewModel)
        {
            gameObject.UpdateChildViewModel(ref _viewModel, viewModel);
            gameObject.Subscribe(_viewModel.IsDefaultFrameVisible, _defaultFrameImage.gameObject.SetActive);
            gameObject.Subscribe(_viewModel.IsDefenderFrameVisible, _defenderFrameGameObject.SetActive);
            gameObject.Subscribe(_viewModel.IsAttackerFrameVisible, _attackerFrameGameObject.SetActive);
            _defaultFrameImage.color = _viewModel.PlayerColor;
        }
    }
}