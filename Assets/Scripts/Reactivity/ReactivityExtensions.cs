#nullable enable
using System;
using UnityEngine;

namespace Reactivity
{
    public static class ReactivityExtensions
    {
        private static readonly IEventsPool _pool = new EventsPool();
        
        public static void UpdateViewModelSimple<T>(this GameObject gameObject, ref T currentViewModel, T newViewModel) where T : class
        {
            GameObjectDispose(gameObject);
            currentViewModel = newViewModel;
        }

        /// <summary>
        /// На случай, когда ВМ является дочерней и родительская ВМ почистит все диспоузы
        /// </summary>
        public static void UpdateChildViewModel<T>(this GameObject gameObject, ref T currentViewModel, T newViewModel)
            where T : class
        {
            GameObjectDispose(gameObject);
            currentViewModel = newViewModel;
        }

        public static void UpdateViewModelDisposable<T>(this GameObject gameObject, ref T currentViewModel,
            T newViewModel) where T : class, IDisposable
        {
            // newViewModel.Dispose(); // По идеи сотрем все в GameObjectDispose, поэтому вызывать это не обязательно
            GameObjectDispose(gameObject);
            currentViewModel = newViewModel;
            AddDisposeToGameObject(gameObject, currentViewModel);
        }

        public static void Subscribe<T>(
            this GameObject gameObject, 
            IReactivityProperty<T> property,
            Action<T> onChangedValue) => SubscribeInternal(gameObject, property, onChangedValue, true);

        public static void SubscribeWithoutCall<T>(
            this GameObject gameObject,
            IReactivityProperty<T> property,
            Action<T> onChangedValue) => SubscribeInternal(gameObject, property, onChangedValue, false);
        
        public static void Subscribe(
            this GameObject gameObject,
            IEventProvider provider,
            Action onChangedValue) => SubscribeInternal(gameObject, provider, onChangedValue, true);
        
        public static void SubscribeWithoutCall(
            this GameObject gameObject,
            IEventProvider provider,
            Action onChangedValue) => SubscribeInternal(gameObject, provider, onChangedValue, false);
        
        private static void SubscribeInternal<T>(
            GameObject gameObject, 
            IReactivityProperty<T> property,
            Action<T> onChangedValue,
            bool callMethod)
        {
            var eventWrapper = _pool.Get();
            var disposable = eventWrapper.Subscribe(property, onChangedValue!, callMethod);
            AddDisposeToGameObject(gameObject, disposable);
        }

        private static void SubscribeInternal(
            GameObject gameObject, 
            IEventProvider provider, 
            Action onChangedValue, 
            bool callMethod)
        {
            var eventWrapper = _pool.Get();
            var disposable = eventWrapper.Subscribe(provider, onChangedValue, callMethod);
            AddDisposeToGameObject(gameObject, disposable);
        }
        
        private static void GameObjectDispose(GameObject gameObject)
        {
            var container = gameObject.GetComponent<DisposeContainer>();
            
            if (container != null)
            {
                container.Clear();
            }
        }

        private static void AddDisposeToGameObject(GameObject gameObject, IDisposable element)
        {
            var container = gameObject.GetComponent<DisposeContainer>();
            
            if (container == null)
            {
                container = gameObject.AddComponent<DisposeContainer>();
            }

            container.AddElement(element);
        }
    }
}