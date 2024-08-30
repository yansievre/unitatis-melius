using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UM.Runtime.UMUtility
{
    public static class ReflectionFinder
    {
        public struct AttributeUsage<T>
        {
            public T attribute;
            public Type targetClass;
            public MethodInfo targetMethod;
        }
        
        /// <summary>
        /// Find all usages of a specific attribute in the current domain
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>   </returns>
        public static Tuple<T,Type>[] FindAttributeUsages<T>() where T : Attribute
        {
            List<Tuple<T, Type>> list = new List<Tuple<T, Type>>();
            string definedIn = typeof(T).Assembly.GetName().Name;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                // Note that we have to call GetName().Name.  Just GetName() will not work.  The following
                // if statement never ran when I tried to compare the results of GetName().
                if ((!assembly.GlobalAssemblyCache) && ((assembly.GetName().Name == definedIn) || assembly.GetReferencedAssemblies().Any(a => a.Name == definedIn)))
                    foreach (Type type in assembly.GetTypes())
                    {
                        var attributes = type.GetCustomAttributes(typeof(T), true);
                        if (attributes.Length > 0)
                        {
                            list.AddRange(attributes.Select(x=>Tuple.Create((T)x,type)));
                        } 
                    }

            return list.ToArray();
        }
        
        public static AttributeUsage<T>[] FindAttributeUsagesOnMethods<T>() where T : Attribute
        {
            List<AttributeUsage<T>> list = new List<AttributeUsage<T>>();
            string definedIn = typeof(T).Assembly.GetName().Name;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                // Note that we have to call GetName().Name.  Just GetName() will not work.  The following
                // if statement never ran when I tried to compare the results of GetName().
                if ((!assembly.GlobalAssemblyCache) && ((assembly.GetName().Name == definedIn) || assembly.GetReferencedAssemblies().Any(a => a.Name == definedIn)))
                    foreach (Type type in assembly.GetTypes())
                    {
                        MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

                        // Display the attributes for each of the members of MyClass1.
                        for(int i = 0; i < methodInfos.Length; i++)
                        {
                            var methodInfo = methodInfos[i];
                            IEnumerable<T> targetAttributes = methodInfo.GetCustomAttributes<T>();
                            list.AddRange(targetAttributes.Select(x => new AttributeUsage<T>
                            {
                                attribute = x,
                                targetClass = type,
                                targetMethod = methodInfo
                            }));
                        }
                    }

            return list.ToArray();
        }
        
        public static Type[] FindChildClasses<T>() where T : class
        {
            List<Type> list = new List<Type>();
            var parentType = typeof(T);
            string definedIn = typeof(T).Assembly.GetName().Name;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                // Note that we have to call GetName().Name.  Just GetName() will not work.  The following
                // if statement never ran when I tried to compare the results of GetName().
                if ((!assembly.GlobalAssemblyCache) && ((assembly.GetName().Name == definedIn) || assembly.GetReferencedAssemblies().Any(a => a.Name == definedIn)))
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (parentType.IsAssignableFrom(type))
                        {
                            list.Add(type);   
                        }
                    }

            return list.ToArray();
        }
    }
}
