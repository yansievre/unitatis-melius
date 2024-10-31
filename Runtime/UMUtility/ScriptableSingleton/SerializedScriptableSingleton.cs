﻿using Sirenix.OdinInspector;
using UnityEngine;

namespace UM.Runtime.UMUtility.ScriptableSingleton
{
    public abstract class SerializedScriptableSingleton<TObject> : SerializedScriptableObject where TObject : SerializedScriptableObject
    {
        private static TObject _instance;

        public static TObject Instance
        {
            get
            {
                if (_instance == null) CreateOrLoadInstance();

                return _instance;
            }
        }

        private static void CreateOrLoadInstance()
        {
            var filePath = GetResourcePath();
            if (!string.IsNullOrEmpty(filePath))
                _instance = Resources.Load<TObject>(filePath);

#if UNITY_EDITOR
            if (_instance != null) return;
            _instance = CreateInstance<TObject>();
            UnityEditor.AssetDatabase.CreateAsset(_instance, $"Assets/Resources/{filePath}.asset");
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }

        private static string GetResourcePath()
        {
            var attributes = typeof(TObject).GetCustomAttributes(true);

            foreach (var attribute in attributes)
                if (attribute is AssetPathAttribute pathAttribute)
                    return pathAttribute.Path;
            Debug.LogError($"{typeof(TObject)} does not have {nameof(AssetPathAttribute)}.");

            return string.Empty;
        }

        private void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                if (_instance != null && _instance != this)
                {
                    Debug.LogError($"An instance of {typeof(TObject)} already exist.");
                    DestroyImmediate(this, true);
                }
#endif
        }
    }
}