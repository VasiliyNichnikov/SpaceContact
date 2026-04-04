using System;
using System.Collections.Generic;
using UnityEngine;
using Logger = Logs.Logger;
using Object = UnityEngine.Object;

namespace Client.UI.Utils
{
    public static class UIUtils
    {
        private const float Half = 0.5f;
        
        /// <summary>
        /// Создает необходимое количество элементов и инициализирует
        /// Считаем, что все элементы, которые находятся в родителе являются одним типом 
        /// </summary>
        public static void CreateRequiredNumberOfItems<TPrefab, TViewModel>(Transform parent, TPrefab prefab, IList<TViewModel> viewModels, Action<TPrefab, TViewModel> initialization) where TPrefab: MonoBehaviour
        {
            CreateRequiredNumberOfItems(parent, prefab, viewModels.Count, (index, item) =>
            {
                var viewModel = viewModels[index];
                initialization.Invoke(item, viewModel);
            });
        }
        
        public static Vector2 GetPositionOfObjectFromSceneInUI(
            Camera camera, 
            RectTransform canvasRectTransform, 
            Vector3 positionObjectOnScene)
        {
            var sizeDelta = canvasRectTransform.sizeDelta;
            var viewportPosition = camera.WorldToViewportPoint(positionObjectOnScene);
            var viewportRelative = new Vector2(viewportPosition.x, viewportPosition.y) - new Vector2(Half, Half);
            var screenPosition = new Vector2(viewportRelative.x * sizeDelta.x, viewportRelative.y * sizeDelta.y);

            return screenPosition;
        }
        
        public static void CreateRequiredNumberOfItems<TMono>(Transform parent, TMono prefab, int numberOfItems, Action<int, TMono> initializationItem) where TMono: MonoBehaviour
        {
            if (numberOfItems == 0)
            {
                Logger.Error($"{nameof(UIUtils)}.{nameof(CreateRequiredNumberOfItems)}: value of the items to create must be greater than 0.");
                
                return;
            }
            
            for (var i = parent.childCount - 1; i >= numberOfItems; i--)
            {
                Object.Destroy(parent.GetChild(i).gameObject);
            }

            while (parent.childCount < numberOfItems)
            {
                Object.Instantiate(prefab, parent, false);
            }

            for (var i = 0; i < numberOfItems; i++)
            {
                var item = parent.GetChild(i).GetComponent<TMono>();
                initializationItem(i, item);
            }
        }
    }
}