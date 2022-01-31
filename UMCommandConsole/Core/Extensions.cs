using System;
using System.Linq;
using System.Reflection;

namespace Plugins.UMCommandConsole.Core
{
    internal static class Extensions
    {
        public static bool HasAttributes(this MethodInfo methodInfo,params Type[] types)
        {
            var attributes = methodInfo.GetCustomAttributes();
            return types.All(seekedType => attributes.Any(existingType => existingType.GetType() == seekedType));
        }
        
        public static bool HasAttributes(this FieldInfo methodInfo,params Type[] types)
        {
            var attributes = methodInfo.GetCustomAttributes();
            return types.All(seekedType => attributes.Any(existingType => existingType.GetType() == seekedType));
        }
        
        public static bool HasAttributes(this PropertyInfo methodInfo,params Type[] types)
        {
            var attributes = methodInfo.GetCustomAttributes();
            return types.All(seekedType => attributes.Any(existingType => existingType.GetType() == seekedType));
        }
        
        public static bool HasAttribute<T>(this MethodInfo methodInfo)
        {
            return methodInfo.HasAttributes(typeof(T));
        }
        
        public static bool HasAttribute<T>(this FieldInfo methodInfo)
        {
            return methodInfo.HasAttributes(typeof(T));
        }
        public static bool HasAttribute<T>(this PropertyInfo methodInfo)
        {
            return methodInfo.HasAttributes(typeof(T));
        }
    }
}