using System.IO;
using UnityEditor;
using UnityEditorInternal;

namespace UM.Editor.UMAutoAssemblies
{
    public class OnAssetImportInjection : AssetPostprocessor
    {
        private static bool ignore = false;
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (ignore) return;
            foreach (var path in importedAssets)
            {
                if (Path.GetExtension(path) == ".asmdef" && AutoAssemblyInjector.AssetIsIncluded(path))
                {
                    var loaded = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(path);
                    AutoAssemblyInjector.Inject(loaded);
                }  
            }
            foreach (var path in movedAssets)
            {
                if (Path.GetExtension(path) == ".asmdef" && AutoAssemblyInjector.AssetIsIncluded(path))
                {
                    var loaded = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(path);
                    AutoAssemblyInjector.Inject(loaded);
                }  
            }

            ignore = true;
            AssetDatabase.Refresh();
            ignore = false;
        }
    }
}
