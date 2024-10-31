using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UM.Runtime.UMUtility.ScriptableSingleton;
using UnityEngine;

namespace UM.Runtime.UMField
{
    [CreateAssetMenu(menuName = "UM/Field Valid Types")]
    public class UMFieldValidTypes : SerializedScriptableSingleton<UMFieldValidTypes>
    {
        [SerializeField]
        private List<Type> _validTypes;

        private ImmutableHashSet<Type> _immutableValidTypes;
        public ImmutableHashSet<Type> ValidTypes
        {
            get { return _immutableValidTypes ??= new ImmutableHashSet<Type>(new HashSet<Type>(_validTypes)); }
        }
    }
}