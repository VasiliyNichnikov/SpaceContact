#nullable enable
using System;
using JetBrains.Annotations;

namespace Reactivity
{
    public class ReactivityPropertyEmpty<T> : IReactivityProperty<T>
    {
        public static IReactivityProperty<T> Instance => _instance ??= new ReactivityPropertyEmpty<T>();

        private static ReactivityPropertyEmpty<T>? _instance;
#pragma warning disable CS0067
        public event Action? OnChangedValue;
#pragma warning restore CS0067
        public T? Value => default(T);


        public void Dispose()
        {
            // nothing
        }
    }
}