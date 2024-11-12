using System;
using System.Collections.Generic;
using ObservableCollections;
using UnityEditor;
using UnityEngine;

namespace UM.Runtime.UMUtility.ReactiveUtility
{
    [Serializable]
    public class SerializableObservableList<T> : ObservableList<T>, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector]
        internal List<T> serializedList;

        public SerializableObservableList()
            : base()
        {
        }

        public SerializableObservableList(int capacity)
            : base(capacity)
        {
        }
        public SerializableObservableList(IEnumerable<T> collection)
            : base(collection)
        {
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            serializedList = new List<T>(this);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Clear();
            if(serializedList == null)
            {
                serializedList = new List<T>();
            }
            foreach (var item in serializedList)
            {
                Add(item);
            }
        }
    }
    
#if UNITY_EDITOR
    // internal class SerializableObservableListPropertyDrawer<TList, TElement> : Sirenix.OdinInspector.Editor.OdinValueDrawer<TList>
    //     where TList : SerializableObservableList<TElement> // Using IList lets the CustomListDrawer work for both List and arrays.
    // {
    //     protected override void DrawPropertyLayout(GUIContent label)
    //     {
    //         EditorGUI.BeginChangeCheck();
    //         ValueEntry.Property.FindChild(x => x.Name == "serializedList", false).Draw(label);
    //         if (EditorGUI.EndChangeCheck())
    //         {
    //             ValueEntry.SmartValue.
    //         }
    //     }
    // }
#endif
}