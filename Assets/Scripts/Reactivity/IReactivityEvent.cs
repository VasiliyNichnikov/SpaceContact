#nullable enable
using System;

namespace Reactivity
{
    public interface IReactivityEvent : IDisposable
    {
        event Action OnChangedValue;
    }
}