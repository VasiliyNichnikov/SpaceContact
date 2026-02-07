using System;
using System.Collections.Generic;
using Client.UI.Dialogs;
using Client.UI.Dialogs.Lobby;
using Configs.UI;
using Logs;
using VContainer;
using VContainer.Unity;

namespace Client.Factory
{
    public class DialogsFactory
    {
        private readonly Dictionary<Type, BaseDialog> _dependencies;
        
        private readonly IObjectResolver _resolver;
        private readonly SceneStorage _sceneStorage;
        
        public DialogsFactory(
            IObjectResolver resolver, 
            DialogsRegistrySO dialogsRegistry,
            SceneStorage sceneStorage)
        {
            _resolver = resolver;
            _sceneStorage = sceneStorage;
            _dependencies = CreateDependencies(dialogsRegistry);
        }
        
        public T? CreateDialog<T>() where T: BaseDialog
        {
            if (!_dependencies.ContainsKey(typeof(T)))
            {
                Logger.Error($"DialogsFactory.CreateDialog: dialog with type {typeof(T)} not found.");

                return null;
            }
            
            var dialogPrefab = _dependencies[typeof(T)];
            var createdDialog = _resolver.Instantiate(dialogPrefab, _sceneStorage.GuiHolder, false);
            
            return createdDialog as T;
        }

        private static Dictionary<Type, BaseDialog> CreateDependencies(DialogsRegistrySO dialogsRegistry)
        {
            return new Dictionary<Type, BaseDialog>
            {
                { typeof(LobbyDialog), dialogsRegistry.LobbyDialog }
            };
        }
    }
}