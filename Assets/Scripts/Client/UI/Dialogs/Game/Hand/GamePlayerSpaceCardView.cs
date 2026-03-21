using Client.UI.Dialogs.Game.Hand.ViewModels;
using Client.UI.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI.Dialogs.Game.Hand
{
    public class GamePlayerSpaceCardView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _rectTransform = null!;
        
        [SerializeField]
        private Image _background = null!;
        
        [SerializeField]
        private Text _titleText = null!;
        
        [SerializeField]
        private Text _valueText = null!;
        
        public void Init(IGamePlayerSpaceCardViewModel viewModel)
        {
            _background.color = viewModel.BackgroundColor;
            _titleText.SetText(viewModel.Title);
            _valueText.SetText(viewModel.Value);
        }

        public void SetLocation(Vector3 position, Vector3 rotation, int siblingIndex)
        {
            _rectTransform.SetSiblingIndex(siblingIndex);
            _rectTransform.localPosition = position;
            _rectTransform.localEulerAngles = rotation;
        }
    }
}