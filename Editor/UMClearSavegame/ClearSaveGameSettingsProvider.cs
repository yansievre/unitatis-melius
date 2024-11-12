using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UM.Editor.UMClearSavegame
{
    class ClearSaveGameSettingsProvider : SettingsProvider
    {
        private SerializedObject m_CustomSettings;

        class Styles
        {
            public static GUIContent PathOption = new GUIContent("Path");
            public static GUIContent CustomPath = new GUIContent("Custom Path");
            public static GUIContent FileOption = new GUIContent("File Pattern");
            public static GUIContent CustomPattern = new GUIContent("Custom Pattern");
        }

        const string k_MyCustomSettingsPath = "Assets/Editor/ClearSaveGameSettings.asset";
        public ClearSaveGameSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope) {}

        public static bool IsSettingsAvailable()
        {
            return File.Exists(k_MyCustomSettingsPath);
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            // This function is called when the user clicks on the MyCustom element in the Settings window.
            m_CustomSettings = ClearSaveGameSettings.GetSerializedSettings();
        }

        public override void OnGUI(string searchContext)
        {
            var pathOption = m_CustomSettings.FindProperty("_pathOption");
            EditorGUILayout.PropertyField(pathOption, Styles.PathOption);
            if (pathOption.enumValueIndex == 0)
            {
                EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("_customPath"), Styles.CustomPath);
            }
            var fileOption = m_CustomSettings.FindProperty("_fileOptions");
            EditorGUILayout.PropertyField(fileOption, Styles.FileOption);
            if (fileOption.enumValueIndex == 0)
            {
                EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("_customPattern"), Styles.CustomPattern);
            }
            m_CustomSettings.ApplyModifiedPropertiesWithoutUndo();
        }

        // Register the SettingsProvider
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            if (!IsSettingsAvailable())
                ClearSaveGameSettings.GetOrCreateSettings();
            var provider = new ClearSaveGameSettingsProvider("unitatis-melius/Erase Savegames Settings", SettingsScope.Project);

            // Automatically extract all keywords from the Styles.
            provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
            return provider;
        }
    }
}