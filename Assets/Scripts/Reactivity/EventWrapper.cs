#nullable enable
using System;

namespace Reactivity
{
    internal class EventWrapper : IDisposable
    {
        private readonly EventsPool _pool;
        
        private IReactivityEvent? _event;
        private IEventProvider? _provider;
        
        public EventWrapper(EventsPool pool)
        {
            _pool = pool;
        }

        public IDisposable Subscribe<T>(IReactivityProperty<T> property, Action<T?> onChangedValue, bool callMethod)
        {
            _event = property;
            _event.OnChangedValue += () =>
            {
                onChangedValue.Invoke(property.Value);
            };

            if (callMethod)
            {
                onChangedValue.Invoke(property.Value);
            }
            
            return this;
        }

        public IDisposable Subscribe(IEventProvider provider, Action onChangedValue, bool callMethod)
        {
            _provider = provider;
            _provider.OnCalled += onChangedValue.Invoke;

            if (callMethod)
            {
                onChangedValue.Invoke();
            }

            return this;
        }

        public void Dispose()
        {
            _event?.Dispose();
            _event = null;
            _provider = null;
            
            _pool.Remove(this);
        }
    }
}