using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Plugins.UMRefresh
{
    public static class UMRefreshHandler
    {
        public delegate void UMRefreshListener(bool scriptsRefreshed);
        private static List<UMRefreshListener> _preRefreshListeners = new List<UMRefreshListener>();
        private static List<UMRefreshListener> _postRefreshListeners = new List<UMRefreshListener>();
        
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
        }
        
        [Shortcut("Refresh",null,KeyCode.R,ShortcutModifiers.Action)]
        private static void Refresh()
        {
            if (_preRefreshListeners == null) _preRefreshListeners = new List<UMRefreshListener>();
            if (_postRefreshListeners == null) _postRefreshListeners = new List<UMRefreshListener>();
            foreach (var listener in _preRefreshListeners)
            {
                listener(false);
            }
            AssetDatabase.Refresh();
            foreach (var listener in _postRefreshListeners)
            {
                listener(false);
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
            foreach (var listener in _postRefreshListeners)
            {
                listener(true);
            }
        }
        
        public static void RegisterPreRefreshListener(UMRefreshListener callback)
        {
            if (_preRefreshListeners == null) _preRefreshListeners = new List<UMRefreshListener>();
            if (_preRefreshListeners.Contains(callback)) return;
            _preRefreshListeners.Add(callback);
        }
        
        public static void RegisterPostRefreshListener(UMRefreshListener callback)
        {
            if (_postRefreshListeners == null) _postRefreshListeners = new List<UMRefreshListener>();
            if (_postRefreshListeners.Contains(callback)) return;
            _postRefreshListeners.Add(callback);
        }
    }
}
