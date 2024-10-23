using UnityEngine;

namespace UM.Runtime.UMUtility
{
    public static class ObjectExtensions
    {
        public static void Destroy(this Object obj, bool noUndo = false)
        {
            if(Application.isPlaying)
                Object.Destroy(obj);
            else
#if UNITY_EDITOR
                if(noUndo)
                    Object.DestroyImmediate(obj);
                else
                    UnityEditor.Undo.DestroyObjectImmediate(obj);
#else
                    Object.DestroyImmediate(obj);
#endif
        }
    }
}