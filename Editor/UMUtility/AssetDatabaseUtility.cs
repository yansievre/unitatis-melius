using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace UM.Editor.UMUtility
{
    public static class AssetDatabaseUtility
    {
        public static IEnumerable<T> FindAssetsByType<T>() where T : UnityEngine.Object
        {
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));

            for( int i = 0; i < guids.Length; i++ )
            {
                string assetPath = AssetDatabase.GUIDToAssetPath( guids[i]);
                    T asset = AssetDatabase.LoadAssetAtPath<T>( assetPath );
                if( asset != null )
                {
                    yield return asset;
                }
            }
        }
        
        public static T FindAssetByType<T>() where T : UnityEngine.Object
        {
            return FindAssetsByType<T>().First();
        }
    }
}