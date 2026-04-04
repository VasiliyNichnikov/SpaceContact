#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class AsmdefWithNullableCreator
    {
        [MenuItem("Assets/Create/C# Assembly Definition + Nullable", priority = 80)]
        private static void CreateAsmdefWithNullable()
        {
            string selectedPath = GetSelectedFolderPath();
            if (string.IsNullOrEmpty(selectedPath))
            {
                EditorUtility.DisplayDialog(
                    "Create Assembly Definition",
                    "Выбери папку в Project window.",
                    "OK");
                return;
            }

            string folderName = Path.GetFileName(selectedPath.TrimEnd('/'));
            string asmdefFileName = folderName + ".asmdef";
            string asmdefPath = Path.Combine(selectedPath, asmdefFileName).Replace("\\", "/");
            string rspPath = Path.Combine(selectedPath, "csc.rsp").Replace("\\", "/");

            if (File.Exists(asmdefPath))
            {
                EditorUtility.DisplayDialog(
                    "Create Assembly Definition",
                    $"Файл уже существует:\n{asmdefPath}",
                    "OK");
                return;
            }

            if (File.Exists(rspPath))
            {
                bool overwriteRsp = EditorUtility.DisplayDialog(
                    "csc.rsp already exists",
                    $"Файл уже существует:\n{rspPath}\n\nПерезаписать?",
                    "Yes",
                    "No");

                if (!overwriteRsp)
                    return;
            }

            var asmdefJson = new AssemblyDefinitionJson
            {
                name = folderName,
                rootNamespace = folderName,
                references = new string[0],
                includePlatforms = new string[0],
                excludePlatforms = new string[0],
                allowUnsafeCode = false,
                overrideReferences = false,
                precompiledReferences = new string[0],
                autoReferenced = true,
                defineConstraints = new string[0],
                versionDefines = new VersionDefine[0],
                noEngineReferences = false
            };

            string json = JsonUtility.ToJson(asmdefJson, true);
            File.WriteAllText(asmdefPath, json);
            File.WriteAllText(rspPath, "-nullable:enable\n");

            AssetDatabase.Refresh();

            Object asmdefAsset = AssetDatabase.LoadAssetAtPath<Object>(asmdefPath);
            if (asmdefAsset != null)
            {
                Selection.activeObject = asmdefAsset;
                EditorGUIUtility.PingObject(asmdefAsset);
            }

            Debug.Log($"Created:\n{asmdefPath}\n{rspPath}");
        }

        [MenuItem("Assets/Create/C# Assembly Definition + Nullable", true)]
        private static bool ValidateCreateAsmdefWithNullable()
        {
            string selectedPath = GetSelectedFolderPath();
            return !string.IsNullOrEmpty(selectedPath) && selectedPath.StartsWith("Assets");
        }

        private static string GetSelectedFolderPath()
        {
            Object selected = Selection.activeObject;
            if (selected == null)
                return null;

            string path = AssetDatabase.GetAssetPath(selected);

            if (string.IsNullOrEmpty(path))
                return null;

            if (AssetDatabase.IsValidFolder(path))
                return path;

            string directory = Path.GetDirectoryName(path);
            return string.IsNullOrEmpty(directory) ? null : directory.Replace("\\", "/");
        }

        [System.Serializable]
        private class AssemblyDefinitionJson
        {
            public string name;
            public string rootNamespace;
            public string[] references;
            public string[] includePlatforms;
            public string[] excludePlatforms;
            public bool allowUnsafeCode;
            public bool overrideReferences;
            public string[] precompiledReferences;
            public bool autoReferenced;
            public string[] defineConstraints;
            public VersionDefine[] versionDefines;
            public bool noEngineReferences;
        }

        [System.Serializable]
        private class VersionDefine
        {
            public string name;
            public string expression;
            public string define;
        }
    }
}
#endif