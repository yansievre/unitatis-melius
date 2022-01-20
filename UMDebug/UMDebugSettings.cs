using System;
using UnityEngine;

namespace Plugins.UMDebug
{
    internal class UMDebugException : Exception
    {
        public UMDebugException(string message) : base(message)
        {
        }
    }

    public static class UMDebugFinder
    {
        private static IUMDebugSettings _instance;
        public static IUMDebugSettings Instance
        {
            get
            {
                if (_instance != null) return _instance;
                var umDebug = Resources.Load<UMDebugSettings>("UMDebugSettings");
                if (umDebug == null)
                {
                    throw new UMDebugException("Couldn't find UMDebugSettings under resources.");   
                }
                umDebug.FixIfNecessary();
                _instance = umDebug;
                return _instance;
            }
        }
    }
    
    [CreateAssetMenu(fileName = "UMDebugSettings", menuName = "UMDebug/UMDebugSettings")]
    internal class UMDebugSettings : ScriptableObject, IUMDebugSettings
    {
        

        [SerializeField]
        private BuildMode targetBuildMode;


        public bool IsDevelopmentMode => BuildMode == BuildMode.Development;
        public bool IsDevReleaseMode  => BuildMode == BuildMode.DevelopmentRelease;
        public bool IsCustomerRelease  => BuildMode == BuildMode.CustomerRelease;
        public BuildMode BuildMode => targetBuildMode;

        internal void FixIfNecessary()
        {
            if (Application.isEditor) return;
            if (targetBuildMode != BuildMode.DevelopmentRelease)
            {
                targetBuildMode = Debug.isDebugBuild ? BuildMode.Development : BuildMode.CustomerRelease;
            }
        }
    }
}
