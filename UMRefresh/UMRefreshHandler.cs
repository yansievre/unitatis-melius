using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Plugins.UMRefresh
{
    public static class UMRefreshHandler
    {
        public delegate void UMRefreshListener();
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
                listener();
            }
            AssetDatabase.Refresh();
            foreach (var listener in _postRefreshListeners)
            {
                listener();
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
