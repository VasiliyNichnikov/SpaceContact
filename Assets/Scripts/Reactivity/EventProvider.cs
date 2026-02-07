#nullable enable

using System;

namespace Reactivity
{
    public class EventProvider : IEventProvider, IDisposable
    {
        public event Action? OnCalled;

        public void Call()
        {
            OnCalled?.Invoke();
        }

        public void Dispose()
        {
            OnCalled = null;
        }
    }
}