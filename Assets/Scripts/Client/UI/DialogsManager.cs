using System;
using System.Collections.Generic;
using System.Linq;
using Client.Factory;
using Client.UI.Dialogs;
using Logs;
using Object = UnityEngine.Object;

namespace Client.UI
{
    public class DialogsManager : IDialogsManager
    {
        private readonly DialogsFactory _factory;

        private readonly List<BaseDialog> _createdDialogs = new();

        public DialogsManager(DialogsFactory factory)
        {
            _factory = factory;
        }

        public event Action<BaseDialog>? OnDialogOpened;
        
        public event Action<BaseDialog>? OnDialogClosed;

        public T ShowDialog<T>() where T : BaseDialog
        {
            var dialog = ShowDialogInternal<T>();
            OnDialogOpened?.Invoke(dialog);

            return dialog;
        }

        public void CloseOpenedDialogs()
        {
            var dialogs = new List<BaseDialog>(_createdDialogs);

            while (dialogs.Count > 0)
            {
                var firstDialog = dialogs[0];
                dialogs.RemoveAt(0);
                firstDialog.Hide();
                OnDialogClosed?.Invoke(firstDialog);
            }
            
            _createdDialogs.Clear();
        }

        public void RemoveDialog(BaseDialog dialog, Action<Action> hideInternal)
        {
            if (!_createdDialogs.Contains(dialog))
            {
                Logger.Error("DialogsManager.RemoveDialog: dialog not found in list.");
                return;
            }

            // костыль, чтобы удалять диалог из списка сразу
            // в то же время если у диалога есть анимация - ждать.
            // И если во время анимации кто-то решит открыть диалог заново, он бы без проблем создался
            _createdDialogs.Remove(dialog);
            hideInternal.Invoke(() => Object.Destroy(dialog.gameObject));
            OnDialogClosed?.Invoke(dialog);
        }

        private T ShowDialogInternal<T>() where T : BaseDialog
        {
            var foundDialog = _createdDialogs.FirstOrDefault(dialog => dialog as T != null);

            if (foundDialog != null)
            {
                foundDialog.Show();
                
                return (T)foundDialog;
            }

            var createdDialog = _factory.CreateDialog<T>();
            createdDialog!.BaseInit(this);
            createdDialog.Show();
            _createdDialogs.Add(createdDialog);

            return createdDialog;
        }
    }
}