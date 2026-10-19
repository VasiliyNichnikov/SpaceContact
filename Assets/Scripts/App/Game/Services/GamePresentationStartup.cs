using System;
using Client.Game.Field;
using Client.UI.Loaders;
using Core.Game;
using VContainer.Unity;

namespace App.Game.Services
{
    public sealed class GamePresentationStartup : IStartable, IDisposable
    {
        private readonly GameContext _context;
        private readonly GameFieldPlanetsViewProvider _fieldPlanetsViewProvider;
        private readonly IGameFieldViewManager _fieldViewManager;
        private readonly GameUILoader _uiLoader;
        private bool _isStarted;
        private bool _isLoaded;

        public GamePresentationStartup(
            GameContext context, 
            GameFieldPlanetsViewProvider fieldPlanetsViewProvider,
            IGameFieldViewManager fieldViewManager, 
            GameUILoader uiLoader)
        {
            _context = context;
            _fieldPlanetsViewProvider = fieldPlanetsViewProvider;
            _fieldViewManager = fieldViewManager;
            _uiLoader = uiLoader;
        }

        public void Start()
        {
            if (_isStarted)
            {
                return;
            }

            _isStarted = true;
            _context.StateChanged += TryLoad;
            TryLoad();
        }

        public void Dispose()
        {
            _context.StateChanged -= TryLoad;
        }

        private void TryLoad()
        {
            if (_isLoaded || _context.LocalPlayerState is null)
            {
                return;
            }

            // A resync must not instantiate a second field or HUD.
            _isLoaded = true;
            _context.StateChanged -= TryLoad;
            _fieldPlanetsViewProvider.CreateFieldPlanets();
            _fieldViewManager.Init();
            _uiLoader.Load();
        }
    }
}
