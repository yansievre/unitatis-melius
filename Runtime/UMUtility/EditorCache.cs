using System;
using System.Collections.Generic;

namespace UM.Runtime.unitatis_melius.Runtime.UMUtility
{
    /// <summary>
    /// Usable in runtime and editor to cache temporary data, will return defualt values in runtime.
    /// </summary>
    public static class EditorCache
    {
#if UNITY_EDITOR
        private static Dictionary<string, object> _cache = new Dictionary<string, object>();
#endif

        public static T Get<T>(string key)
        {
#if UNITY_EDITOR
            return _cache.TryGetValue(key, out var value) ? (T)(Convert.ChangeType(value, typeof(T))) : Activator.CreateInstance<T>();
#endif
            return default;
        }

        public static void Save(string key, object obj)
        {
#if UNITY_EDITOR
            _cache[key] = obj;
#endif
        }
    }
}