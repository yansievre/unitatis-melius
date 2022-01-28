using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace Plugins.UMAutoAssemblies
{
   


    class AutoAssembliesSettingsProvider : SettingsProvider
    {
        private static AutoAssembliesSettings ActiveSettings=null;
        private SerializedObject m_CustomSettings;
        private PropertyTree _propertyTree;
        
        public AutoAssembliesSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope)
        {
        }

        internal static AutoAssembliesSettings GetOrCreateSettings()
        {
            if (ActiveSettings != null) return ActiveSettings;
            /*
            var json = EditorPrefs.GetString(k_EditorPrefsKey, "");
            GGBuildVersioningSettings settings = null;
            try
            {
                settings = JsonUtility.FromJson<GGBuildVersioningSettings>(json);
            }
            catch (Exception e)
            {
                
            }*/
            
            ActiveSettings = AssetDatabase.LoadAssetAtPath<AutoAssembliesSettings>(AutoAssembliesSettings.SettingsAssetPath);
            if (ActiveSettings == null)
            {
                ActiveSettings = ScriptableObject.CreateInstance<AutoAssembliesSettings>();
                AssetDatabase.CreateAsset(ActiveSettings, AutoAssembliesSettings.SettingsAssetPath);
                AssetDatabase.SaveAssets();
            }

            return ActiveSettings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
  
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            // This function is called when the user clicks on the MyCustom element in the Settings window.
            m_CustomSettings = GetSerializedSettings();
            _propertyTree = PropertyTree.Create(m_CustomSettings);
        }

        public override void OnGUI(string searchContext)
        {
            _propertyTree.Draw(false);
            
            /*
            if( Event.current.commandName == "ObjectSelectorUpdated" && (EditorGUIUtility.GetObjectPickerControlID() ==  AutoAssembliesSettings.IgnoredAssemblyId ||EditorGUIUtility.GetObjectPickerControlID() ==  AutoAssembliesSettings.IgnoredAssemblyId) )
            {
                var obj = (AssemblyDefinitionAsset)EditorGUIUtility.GetObjectPickerObject();
                if (obj == null) return;
                var path = AutoAssembliesSettings.SettingsAssetPath;
                var settings = AssetDatabase.LoadAssetAtPath<AutoAssembliesSettings>(path);
                if (settings == null) return;
                if (EditorGUIUtility.GetObjectPickerControlID() == AutoAssembliesSettings.IgnoredAssemblyId)
                {
                    if (settings.ignoredAssemblies.Contains(obj.name) || obj.name == "UMAutoAssemblies.Editor") return;
                    settings.ignoredAssemblies.Add(obj.name);
                }else if (EditorGUIUtility.GetObjectPickerControlID() == AutoAssembliesSettings.InjectedAssemblyId)
                {
                    if (settings.injectedAssemblies.Contains(obj.name) || obj.name == "UMAutoAssemblies.Editor") return;
                    settings.injectedAssemblies.Add(obj.name);
                }
            }
            */
            m_CustomSettings.ApplyModifiedProperties();
        }

        // Register the SettingsProvider
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            var provider = new AutoAssembliesSettingsProvider("unitatis-melius/AutoAssemblies", SettingsScope.Project);

            return provider;

        }
    }
}