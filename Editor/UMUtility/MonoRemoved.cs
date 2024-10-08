using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UM.Editor.UMUtility
{
    public static class MonoRemoved
    {
        [MenuItem("CONTEXT/MonoBehaviour/Remove Component", priority = int.MinValue)]
        private static void ComponentContextMenuItem(MenuCommand menuCommand)
        {
            if (menuCommand.context == null)
            {
                foreach (var gameObject in Selection.gameObjects)
                {
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
                }
            }
            var mb = (MonoBehaviour) menuCommand.context;
            try
            {
                var go = mb.gameObject;
                mb.CallMethodOnTarget("OnDestroyEditor");
                go.CallMethodOnGameObject("OnMonoBehaviourRemoved");
            }catch(Exception e){}
            Object.DestroyImmediate(mb, true);
        }
    }
}