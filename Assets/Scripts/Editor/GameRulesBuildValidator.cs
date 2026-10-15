#if UNITY_EDITOR
using System.Collections.Generic;
using System;
using Logger = Logs.Logger;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using UnityEditor;

namespace Editor
{
    public sealed class GameRulesBuildValidator : IPreprocessBuildWithReport
    {
        private const string MenuPath = "Tools/Space Contact/Validate Game Rules";

        private static readonly IGameRulesValidation[] _validations =
        {
            new LobbySettingsValidation(),
            new CardsConfigValidation(),
            new PlanetsConfigValidation(),
            new StateVisibilitySchemaValidation()
        };

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var errors = ValidateAssets();

            if (errors.Count > 0)
            {
                throw new BuildFailedException(string.Join(Environment.NewLine, errors));
            }
        }

        [MenuItem(MenuPath)]
        private static void ValidateFromMenu()
        {
            var errors = ValidateAssets();

            if (errors.Count == 0)
            {
                Logger.Log("Game rule configs are valid.");
                return;
            }

            foreach (var error in errors)
            {
                Logger.Error(error);
            }
        }

        [MenuItem(MenuPath, true)]
        private static bool CanValidateFromMenu()
        {
            return !EditorApplication.isPlayingOrWillChangePlaymode;
        }

        private static IReadOnlyList<string> ValidateAssets()
        {
            var context = new GameRulesValidationContext();

            foreach (var validation in _validations)
            {
                try
                {
                    validation.Validate(context);
                }
                catch (Exception exception)
                {
                    context.AddError($"{validation.GetType().Name}: {exception.Message}");
                }
            }

            return context.Errors;
        }
    }
}
#endif
