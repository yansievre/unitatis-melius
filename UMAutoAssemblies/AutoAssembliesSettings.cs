using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.UMEditorUtility;
using Plugins.UMUtility;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditorInternal;
using UnityEngine;

namespace Plugins.UMAutoAssemblies
{
    [Serializable]
    struct ReadonlyString
    {
        [HideLabel,Sirenix.OdinInspector.ReadOnly,SerializeField]
        public string value;

        public static implicit operator string(ReadonlyString st) => st.value;
        public static implicit operator ReadonlyString(string st) => new ReadonlyString(){value = st};
    }
    [Serializable]
    internal class AutoAssembliesSettings : ScriptableObject
    {
        public const string SettingsAssetPath =
            "Assets/Plugins/unitatis-melius/UMAutoAssemblies/AutoAssembliesSettings.asset";

        [SerializeField,ListDrawerSettings(CustomAddFunction = "AddIgnoredAssembly"),ReadOnly,InlineProperty,OnValueChanged("Update",true)]
        internal List<ReadonlyString> ignoredAssemblies;
        
        [SerializeField,ListDrawerSettings(CustomAddFunction = "AddInjectedAssembly"),ReadOnly,InlineProperty,OnValueChanged("Update",true)]
        internal List<ReadonlyString> injectedAssemblies;
        
        [SerializeField]
        internal List<FolderReference> injectionTargets;

        private ReadonlyString AddIgnoredAssembly()
        {
            var def = GetAsmDef();
            if (ignoredAssemblies.Contains(def.name)|| def.name == "UMAutoAssemblies.Editor") return null;
            return def.name;
        }
        
        private string AddInjectedAssembly()
        {
            var def = GetAsmDef();
            if (def.name == "UMAutoAssemblies.Editor") return null;
            return def.name;
        }

        private AssemblyDefinitionAsset GetAsmDef()
        {
            EditorGUIUtility.ShowObjectPicker<AssemblyDefinitionAsset>(null,false,"",0);
            return (AssemblyDefinitionAsset) EditorGUIUtility.GetObjectPickerObject();
        }

        private static Dictionary<string, Assembly> _cache;
        
        [Button]
        public void Update()
        {
            var allAssemblies =CompilationPipeline.GetAssemblies();
            foreach (var assembly in allAssemblies)
            {
                if (ignoredAssemblies.Contains(assembly.name)||assembly.name=="UMAutoAssemblies.Editor") continue;
                var path = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(assembly.name);
                if (injectionTargets.Any(x => path.IsSubPathOf(AssetDatabase.GUIDToAssetPath(x.guid))))
                {
                    Inject(assembly,injectedAssemblies);
                }
            }
        }

        public static void Inject(Assembly target, List<ReadonlyString> toInject)
        {
            if (_cache == null)
            {
                _cache = new Dictionary<string, Assembly>();
                var allAssemblies =CompilationPipeline.GetAssemblies();
                foreach (var assembly in allAssemblies)
                {
                    if (toInject.Contains(assembly.name))
                    {
                        _cache.Add(assembly.name,assembly);
                    }
                }
            }

            var assemblies = toInject.Select(x => _cache[x]).Where(x=>!target.assemblyReferences.Contains(x)).ToArray();
            target.assemblyReferences.AddRange(assemblies);
        }
    }
}