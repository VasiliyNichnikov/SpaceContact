#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public sealed class GameRulesValidationContext
    {
        public const int MinimumPlayerCount = 2;

        private readonly List<string> _errors = new();

        public IReadOnlyList<string> Errors => _errors;

        public int MaximumPlayerCount { get; private set; } = MinimumPlayerCount;

        public void IncludePlayerCount(int playerCount)
        {
            MaximumPlayerCount = Math.Max(MaximumPlayerCount, playerCount);
        }

        public void AddError(string message)
        {
            _errors.Add(message);
        }

        public void ValidateAssets<T>(Action<T> validate) where T : ScriptableObject
        {
            var assetGuids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { "Assets" });

            if (assetGuids.Length == 0)
            {
                AddError($"Game rules: no {typeof(T).Name} asset found.");
            }

            foreach (var guid in assetGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                try
                {
                    var config = AssetDatabase.LoadAssetAtPath<T>(path);

                    if (config == null)
                    {
                        AddError($"{path}: cannot load config.");
                        continue;
                    }

                    validate(config);
                }
                catch (Exception exception)
                {
                    AddError($"{path}: {exception.Message}");
                }
            }
        }
    }
}
#endif
