using System.Collections.Generic;
using System.Linq;
using Plugins.UMEditableAssembly;
using Plugins.UMEditorUtility;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Plugins.UMAutoAssemblies
{
    public static class AutoAssemblyInjector
    {

        private static AutoAssembliesSettings Settings => AutoAssembliesSettingsProvider.GetOrCreateSettings();

        private static List<string> _folderPaths;

        public static void RegenerateFolderPaths()
        {
            if (Settings == null)
            {
                _folderPaths = new List<string>();
                return;
            }
            _folderPaths = Settings.injectionTargets.Select(x => AssetDatabase.GUIDToAssetPath(x.guid)).ToList();
        }
        
        public static bool AssetIsIncluded(string path)
        {
            if (Settings == null) return false;
            if (_folderPaths == null)
                RegenerateFolderPaths();
            return _folderPaths.Any(path.IsSubPathOf);
        }
        public static void UpdateAll()
        {
            HashSet<string> ignored = new HashSet<string>(Settings.ignoredAssemblies.Select(x => x.name));
            RegenerateFolderPaths();
            var allDefinitionAssets = FindAssetsByType<TextAsset>(_folderPaths.ToArray()).OfType<AssemblyDefinitionAsset>().ToArray();
            foreach (var asset in allDefinitionAssets)
            {
                if(ignored.Contains(asset.name)) continue;
                var path = AssetDatabase.GetAssetPath(asset);
                if (AssetIsIncluded(path))
                {
                    Inject(asset, Settings.injectedAssemblies);
                }
            }
            AssetDatabase.Refresh();
        }
        public static List<T> FindAssetsByType<T>(string[] folders = null) where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}",typeof (T).ToString().Replace("UnityEngine.", "")),folders);
            for( int i = 0; i < guids.Length; i++ )
            {
                string assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );
                T asset = AssetDatabase.LoadAssetAtPath<T>( assetPath );
                if( asset != null )
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }

        public static void Inject(AssemblyDefinitionAsset target)
        {
            Inject(target, Settings.injectedAssemblies);
        }
        public static void Inject(AssemblyDefinitionAsset target, List<AssemblyDefinitionAsset> toInject)
        {
            var editable = new EditableAssembly(target);
            var res = false;
            foreach (var asm in toInject)
            {
                res = res || editable.AddAssembly(asm);
            }

            if (res)
            {
                editable.Save(false);
                EditorUtility.SetDirty(target);
            }
        }
    }
}
