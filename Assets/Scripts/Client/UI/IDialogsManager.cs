#nullable enable

using System;
using Client.UI.Dialogs;

namespace Client.UI
{
    public interface IDialogsManager
    {
        event Action<BaseDialog>? OnDialogOpened;
        
        event Action<BaseDialog>? OnDialogClosed;
        
        T ShowDialog<T>() where T : BaseDialog;
    }
}