using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace UM.Editor.UMClearSavegame
{
    class ClearSaveGameSettings : ScriptableObject
    {
        enum PathOptions
        {
            Custom,
            DataPath,
            PersistentDataPath,
        }
        enum FileOptions
        {
            Custom,
            AllJsonFiles,
        }
        
        public const string k_MyCustomSettingsPath = "Assets/Editor/MyCustomSettings.asset";

        [SerializeField]
        private PathOptions _pathOption = PathOptions.PersistentDataPath;
        [SerializeField, ShowIf(nameof(_pathOption), PathOptions.Custom)]
        private string _customPath;
        [SerializeField]
        private FileOptions _fileOptions = FileOptions.AllJsonFiles;
        [SerializeField, ShowIf(nameof(_fileOptions), FileOptions.Custom)]
        private string _customPattern = "*.json";
        
        [MenuItem("Tools/Erase Save Files %e")]
        private static void EraseSaveGames()
        {
            var settings = GetOrCreateSettings();
            var path = settings._pathOption == PathOptions.Custom ? settings._customPath : settings._pathOption == PathOptions.DataPath ? Application.dataPath : Application.persistentDataPath;
            var file = settings._fileOptions == FileOptions.Custom ? settings._customPattern : "*.json";
            var files = System.IO.Directory.GetFiles(path, file);
            if (files.Length == 0)
            {
                Debug.Log("No files found at " + path);
                return;
            }
            foreach (var f in files)
            {
                Debug.Log($"Deleting {f} at " + path);
                System.IO.File.Delete(f);
            }
        }

        internal static ClearSaveGameSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<ClearSaveGameSettings>(k_MyCustomSettingsPath);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<ClearSaveGameSettings>();
                AssetDatabase.CreateAsset(settings, k_MyCustomSettingsPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }
}