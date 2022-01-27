using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;
using Utility;

namespace Plugins.UMUtility
{
    [Serializable]
    public abstract class SerializableUMDictionary<TKey, TValue> : UMDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [NonSerialized,OdinSerialize, HideInInspector]
        private List<TKey> keyData = new List<TKey>();
	
        [NonSerialized,OdinSerialize, HideInInspector]
        private List<TValue> valueData = new List<TValue>();

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        { 
            this.Clear();
            for (int i = 0; i < this.keyData.Count && i < this.valueData.Count; i++)
            {
                this[this.keyData[i]] = this.valueData[i];
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            this.keyData.Clear();
            this.valueData.Clear();

            foreach (var item in this)
            {
                this.keyData.Add(item.Key);
                this.valueData.Add(item.Value);
            }
        }
    }
}