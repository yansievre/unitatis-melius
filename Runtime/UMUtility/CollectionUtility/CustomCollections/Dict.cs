﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using UM.Runtime.UMUtility.CollectionUtility.CustomCollections;
using UnityEngine;

#if ODIN_INSPECTOR
[assembly: RegisterFormatter(typeof(Dict<,>.DictFormatter<,>))]
#endif
namespace UM.Runtime.UMUtility.CollectionUtility.CustomCollections
{
    
    [Serializable]
    public class Dict<TKey,TValue> : Dictionary<TKey,TValue>, ISerializationCallbackReceiver
    {
#if ODIN_INSPECTOR
        public class DictFormatter<TKey, TValue> : MinimalBaseFormatter<Dict<TKey, TValue>>
        {
            private static readonly Serializer<List<TKey>> KeySerializer = Serializer.Get<List<TKey>>();
            private static readonly Serializer<List<TValue>> ValueSerializer = Serializer.Get<List<TValue>>();

            protected override void Read(ref Dict<TKey, TValue> value, IDataReader reader)
            {
                value._keyData = KeySerializer.ReadValue(reader);
                value._valueData = ValueSerializer.ReadValue(reader);
            }
	
            protected override void Write(ref Dict<TKey, TValue> value, IDataWriter writer)
            {
                KeySerializer.WriteValue(value._keyData, writer);
                ValueSerializer.WriteValue(value._valueData, writer);
            }
        }
#endif
        [SerializeField, HideInInspector]
        private List<TKey> _keyData;
	       
        [SerializeField, HideInInspector]
        private List<TValue> _valueData;
        
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (_keyData == null || _valueData == null)
                return;
            Clear();
            for (int i = 0; i < _keyData.Count && i < _valueData.Count; i++)
            {
                this[_keyData[i]] = _valueData[i];
            }
        }
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _keyData = new List<TKey>(Count);
            _valueData = new List<TValue>(Count);
        
            foreach (var item in this)
            {
                _keyData.Add(item.Key);
                _valueData.Add(item.Value);
            }
        }
        /// <summary><para>Adds an element with the specified key and value to the dictionary.</para></summary>
        /// <param name="key">The key of the element to add to the dictionary.</param>
        /// <param name="value"></param>
        public new void Add(TKey key, TValue value)
        {
            
            if (this.ContainsKey(key))
            {
                Debug.LogException(new Exception($"Attempted to add key '{key.ToString()} to dictionary but this key is already registered. Will continue"));
                return;
            }
            base.Add(key,value);
        }

        /// <summary><para>Removes the element with the specified key from the dictionary.</para></summary>
        /// <param name="key">The key of the element to be removed from the dictionary.</param>
        public new bool Remove(TKey key)
        {
            if (!this.ContainsKey(key)) return false;
            return base.Remove(key);
        }
        
        /// <summary><para>Gets or sets the value associated with the specified key.</para></summary>
        /// <param name="key">The key whose value is to be gotten or set.</param>
        public new TValue this[TKey key]
        {
            get
            {
                if (this.ContainsKey(key)) return base[key];
                Debug.LogException(new KeyNotFoundException($"Attempted to get value of key {key} but no such key exists. Will continue with default."));
                return default;
            }
            set
            {
                if (this.ContainsKey(key)) base[key] = value;
                else Add(key,value);
            }
        }

        public override string ToString()
        {
            var values = string.Join(",\n\t", this.Select(x => $"{x.Key.ToString()} : {x.Value.ToString()}"));
            string result = "{\n\t" + values + "\n}";
            return result;
        }
    }
    
    public static class SerializableDictionaryExtensions
    {
        public static Dict<TKey, TValue> Create<TKey, TValue>()
        {
            return new Dict<TKey, TValue>();
        }
        
        public static Dict<TKey, TValue> ToDict<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            var serializableDictionary = new Dict<TKey, TValue>();
            foreach (var (key, value) in dictionary)
            {
                serializableDictionary[key] = value;
            }

            return serializableDictionary;
        }
        
        //to serializable dictionary from enumerable
        public static Dict<TKey, TValue> ToDict<TKey, TValue>(this IEnumerable<TValue> enumerable, Func<TValue, TKey> keySelector)
        {
            var serializableDictionary = new Dict<TKey, TValue>();
            foreach (var value in enumerable)
            {
                serializableDictionary[keySelector(value)] = value;
            }

            return serializableDictionary;
        }
        
        //to serializable dictionary from enumerable
        public static Dict<TKey, TValue> ToDict<TKey, TValue, TObject>(this IEnumerable<TObject> enumerable, Func<TObject, TKey> keySelector, Func<TObject, TValue> valueSelector)
        {
            var serializableDictionary = new Dict<TKey, TValue>();
            foreach (var value in enumerable)
            {
                serializableDictionary[keySelector(value)] = valueSelector(value);
            }

            return serializableDictionary;
        }
    
    }
}
