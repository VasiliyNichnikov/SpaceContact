#nullable enable
using System;

namespace Reactivity
{
    public class ReactivityProperty<T> : IReactivityProperty<T>
    {
        public T? Value
        {
            get => _value;
            set
            {
                if (Equals(_value, value))
                {
                    return;
                }

                _value = value;
                OnChangedValue?.Invoke();
            }
        }

        public event Action? OnChangedValue;

        private T? _value;

        public ReactivityProperty(T? value = default)
        {
            _value = value;
        }

        public void ForceRefresh()
        {
            OnChangedValue?.Invoke();
        }
        
        public void Dispose()
        {
            OnChangedValue = null;
        }
    }
}