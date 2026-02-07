#nullable enable

using System;

namespace Reactivity
{
    public interface IEventProvider
    {
        event Action? OnCalled;
    }
}