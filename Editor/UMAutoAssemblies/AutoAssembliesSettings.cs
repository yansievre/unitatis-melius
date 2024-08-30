using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UM.Runtime.UMUtility;
using UnityEditorInternal;
using UnityEngine;

namespace UM.Editor.UMAutoAssemblies
{
    [Serializable]
    struct ReadonlyString
    {
        [HideLabel,Sirenix.OdinInspector.ReadOnly,SerializeField]
        public string value;

        public static implicit operator string(ReadonlyString st) => st.value;
        public static implicit operator ReadonlyString(string st) => new ReadonlyString(){value = st};
    }
    [Serializable]
    internal class AutoAssembliesSettings : ScriptableObject
    {
        private bool _updateRequired = false;
        public const string SettingsAssetPath =
            "Assets/Plugins/unitatis-melius/Editor/UMAutoAssemblies/AutoAssembliesSettings.asset";

        [AssetSelector(DrawDropdownForListElements = false,ExcludeExistingValuesInList = true,FlattenTreeView = true,IsUniqueList = true)]
        [SerializeField,InlineProperty,OnValueChanged("OnChange",true)]
        internal List<AssemblyDefinitionAsset> ignoredAssemblies;
        
        
        
        [AssetSelector(DrawDropdownForListElements = false,ExcludeExistingValuesInList = true,FlattenTreeView = true,IsUniqueList = true)]
        [SerializeField,InlineProperty,OnValueChanged("OnChange",true)]
        internal List<AssemblyDefinitionAsset> injectedAssemblies;
        
        [SerializeField]
        internal List<FolderReference> injectionTargets;

        private void OnChange()
        {
            _updateRequired = true;
        }
        
        [Button][InfoBox("Update Required!",InfoMessageType.Error,nameof(_updateRequired))]
        public void Update()
        {
            AutoAssemblyInjector.UpdateAll();

            _updateRequired = false;
        }
    }

    
}