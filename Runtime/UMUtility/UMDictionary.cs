using System;
using System.Collections.Generic;
using UnityEngine;

namespace UM.Runtime.UMUtility
{
    public class UMDictionary<TKey,TValue> : Dictionary<TKey,TValue>
    {
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
    }
}
