using System;
using System.Linq;
using System.Reflection;
using Sirenix.Utilities;
using UM.Runtime.UMRefresh;
using UM.Runtime.UMUtility;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UM.Editor.UMRefresh
{
    public static class UMRefreshHandler
    {
        private static ReflectionFinder.AttributeUsage<Attributes>[] S_PreRefreshMethods;
        private static ReflectionFinder.AttributeUsage<Attributes>[] S_PostRefreshMethods;
        
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            RefreshExtensions.OnRefreshRequest -= RefreshObject;
            RefreshExtensions.OnRefreshRequest += RefreshObject;
            var methods = ReflectionFinder.FindAttributeUsagesOnMethods<Attributes>();
            S_PreRefreshMethods = methods.Where(x => x.attribute.IsPreRefresh).ToArray();
            S_PostRefreshMethods = methods.Where(x => !x.attribute.IsPreRefresh).ToArray();
        }
        
        [Shortcut("UnitatisMelius/Refresh",null,KeyCode.R,ShortcutModifiers.Action)]
        public static void Refresh()
        {
            CallPreRefresh(); 
            AssetDatabase.Refresh();
            CallPostRefresh();
        }

        private static void CallPreRefresh()
        {
            foreach (var usage in S_PreRefreshMethods)
            {
                foreach (var target in Object.FindObjectsByType(usage.targetClass, FindObjectsSortMode.None))
                {
                    usage.targetMethod.Invoke(target, Array.Empty<object>());
                }
            }
        }
        
        private static void CallPostRefresh()
        {
            foreach (var usage in S_PostRefreshMethods)
            {
                if(usage.targetMethod.IsStatic)
                {
                    usage.targetMethod.Invoke(null, Array.Empty<object>());
                    continue;
                }
                foreach (var target in Object.FindObjectsByType(usage.targetClass, FindObjectsSortMode.None))
                {
                    usage.targetMethod.Invoke(target, Array.Empty<object>());
                }
            }
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void ScriptRefresh()
        {
            if(EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += ScriptRefresh;
                return;
            }
 
            EditorApplication.delayCall += AfterScriptRefresh;
        }

        private static void AfterScriptRefresh()
        {
            CallPreRefresh();
            CallPostRefresh();
        }

        public static void RefreshObject(object obj)
        {
            var type = obj.GetType();
            S_PreRefreshMethods.Where(x => type.IsAssignableFrom(x.targetClass))
                .Concat(S_PostRefreshMethods.Where(x => x.targetClass == type))
                .ForEach(x => x.targetMethod.Invoke(obj, Array.Empty<object>()));
        }
    }
}
