#nullable enable

using System;

namespace Reactivity
{
    public class EmptyEventProvider : IEventProvider
    {
        public static IEventProvider Instance => _instance ??= new EmptyEventProvider();
        
        private static EmptyEventProvider? _instance;
        
        private EmptyEventProvider()
        {
            // nothing
        }
        
#pragma warning disable CS0067
        public event Action? OnCalled;
#pragma warning restore CS0067
    }
}