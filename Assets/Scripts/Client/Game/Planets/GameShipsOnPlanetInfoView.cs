using System.Collections.Generic;
using Client.Configs.Game;
using Client.Game.Planets.ViewModels;
using Reactivity;
using UnityEngine;
using VContainer;

namespace Client.Game.Planets
{
    public class GameShipsOnPlanetInfoView : MonoBehaviour, IGameShipsOnPlanetInfoVisitor
    {
        [SerializeField] 
        private RectTransform _rectTransform = null!;
        
        [SerializeField]
        private RectTransform _itemsContainerRectTransform = null!;
        
        private GameShipsOnPlanetInfoViewModel _viewModel = null!;
        private GameUIShipsOnPlanetItemsRegistrySO _uiShipsOnPlanetItemsRegistrySO = null!;
        private Queue<GameObject> _createdItems = new();
        
        [Inject]
        private void Constructor(GameUIShipsOnPlanetItemsRegistrySO uiShipsOnPlanetItemsRegistrySO)
        {
            _uiShipsOnPlanetItemsRegistrySO = uiShipsOnPlanetItemsRegistrySO;
        }
        
        public void Init(GameShipsOnPlanetInfoViewModel viewModel)
        {
            gameObject.UpdateViewModelSimple(ref _viewModel, viewModel);
            gameObject.Subscribe(_viewModel.InfoViewModels, RefreshItems);
        }
        
        public RectTransform RectTransform => 
            _rectTransform;
        
        public void Show() => 
            gameObject.SetActive(true);
        
        public void Hide() => 
            gameObject.SetActive(false);

        private void RefreshItems(IReadOnlyCollection<IGameShipsOnPlanetInfoItemViewModel> viewModels)
        {
            while (_createdItems.Count > 0)
            {
                var item = _createdItems.Dequeue();
                Destroy(item);
            }
            
            foreach (var viewModel in viewModels)
            {
                viewModel.Apply(this);
            }
        }

        void IGameShipsOnPlanetInfoVisitor.Visit(GameShipsInfoViewModel viewModel)
        {
            var prefab = _uiShipsOnPlanetItemsRegistrySO.ShipsInfoOnPlanetItemView;
            var createdObject = Instantiate(prefab, _itemsContainerRectTransform, false);
            createdObject.Init(viewModel);
            _createdItems.Enqueue(createdObject.gameObject);
        }
    }
}