#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace Reactivity
{
    public sealed class ReactivityListProperty<T> : IReactivityReadOnlyCollectionProperty<T>
    {
        private List<T>? _value;

        public ReactivityListProperty(List<T>? value = null)
        {
            _value = value;
        }
        
        public event Action? OnChangedValue;

        public IReadOnlyCollection<T>? Value
        {
            get => _value;
            set
            {
                if (Equals(_value, value))
                {
                    return;
                }

                _value = value?.ToList();
                OnChangedValue?.Invoke();
            }
        }

        public void Add(T value)
        {
            _value ??= new List<T>();
            _value.Add(value);
            OnChangedValue?.Invoke();
        }

        public void Remove(T value)
        {
            if (_value == null)
            {
                throw new NullReferenceException("List cannot be null.");
            }
            
            _value.Remove(value);
            OnChangedValue?.Invoke();
        }
        
        public void Dispose()
        {
            OnChangedValue = null;
        }
    }
}