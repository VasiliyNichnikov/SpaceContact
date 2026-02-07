using System;
using UI;
using UnityEngine;
using Logger = Logs.Logger;

namespace Client.UI.Dialogs
{
    public abstract class BaseDialog : MonoBehaviour
    {
        [SerializeField, Header("Can be null")] 
        private AnimationDialog? _animation;

        private DialogsManager _manager = null!;
        private bool _isStartedToClose = false;

        private event Action? OnCloseEvent;
        
        public void BaseInit(DialogsManager manager)
        {
            _manager = manager;
        }

        public void AddActionOnClose(Action action)
        {
            OnCloseEvent += action;
        }
        
        public virtual void Show(Action? onComplete = null)
        {
            _isStartedToClose = false;
            
            if (_animation != null && _animation.CanPlayOpenAnimation())
            {
                _animation.Open(onComplete);
            }
            else
            {
                onComplete?.Invoke();
            }
        }

        public void Hide()
        {
            if (_isStartedToClose)
            {
                Logger.Warning($"Dialog {GetType()} is already closed.");
                
                return;
            }

            _isStartedToClose = true;
            _manager.RemoveDialog(this, HideInternal);
        }

        protected virtual void HideInternal(Action onCompleted)
        {
            OnCloseEvent?.Invoke();
            
            if (_animation != null && _animation.CanPlayCloseAnimation())
            {
                _animation.Close(onCompleted);
            }
            else
            {
                onCompleted.Invoke();
            }
        }
    }
}