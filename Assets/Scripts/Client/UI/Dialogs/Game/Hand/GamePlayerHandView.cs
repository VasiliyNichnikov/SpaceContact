using System.Collections.Generic;
using System.Linq;
using Client.Data.Game;
using Client.UI.Dialogs.Game.Hand.ViewModels;
using Client.UI.Utils;
using Reactivity;
using UnityEngine;
using VContainer;

namespace Client.UI.Dialogs.Game.Hand
{
    public class GamePlayerHandView : MonoBehaviour
    {
        private const float Half = 0.5f;
        
        [SerializeField]
        private RectTransform _spaceCardsContainer = null!;
        
        [SerializeField]
        private GamePlayerSpaceCardView _spaceCardViewPrefab = null!;

        private PlayerHandDisplayData _displayData;
        private IGamePlayerHandViewModel _viewModel = null!;

        [Inject]
        private void Constructor(PlayerHandDisplayData displayData)
        {
            _displayData = displayData;
        }

        public void Init(IGamePlayerHandViewModel viewModel)
        {
            gameObject.UpdateChildViewModel(ref _viewModel, viewModel);
            gameObject.Subscribe(_viewModel.CardsViewModels, RefreshCards);
        }
        
        private void RefreshCards(IReadOnlyCollection<IGamePlayerSpaceCardViewModel> viewModels)
        {
            var viewModelsAsList = viewModels.ToList();
            UIUtils.CreateRequiredNumberOfItems(
                _spaceCardsContainer,
                _spaceCardViewPrefab,
                viewModels.Count,
                (i, view) =>
                {
                    InitCard(i, viewModelsAsList.Count, view, viewModelsAsList[i]);
                });
        }

        private void InitCard(
            int index,
            int count,
            GamePlayerSpaceCardView view, 
            IGamePlayerSpaceCardViewModel viewModel)
        {
            var t = count > 1
                ? index / (float)(count - 1)
                : Half;

            var normalized = (t - Half) * 2f;
            var posX = normalized * (_displayData.ArchWidth / 2.0f);
            var posY = Mathf.Abs(normalized) * -_displayData.ArchHeight;
            var rotZ = normalized * -_displayData.MaxRotation;
            
            var position = new Vector3(posX, posY, 0.0f);
            var rotation = new Vector3(0.0f, 0.0f, rotZ);
            view.SetLocation(position, rotation, index);
            view.Init(viewModel);
        }
    }
}