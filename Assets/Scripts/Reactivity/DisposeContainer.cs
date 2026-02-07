#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Reactivity
{
    public class DisposeContainer : MonoBehaviour
    {
        private readonly List<IDisposable> _elements = new List<IDisposable>();

        public void AddElement(IDisposable element)
        {
            if (_elements.Contains(element))
            {
                Debug.LogError("DisposeContainer.AddElement: element is already contains in list.");
                return;
            }
            
            _elements.Add(element);
        }

        public void Clear()
        {
            foreach (var element in _elements)
            {
                element.Dispose();
            }

            _elements.Clear();
        }

        private void OnDestroy()
        {
            Clear();
        }
    }
}