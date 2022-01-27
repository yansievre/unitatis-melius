using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.Utilities;

namespace Plugins.UMUtility
{
    public static class ReflectionFinder
    {
        
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
                        if (type.IsCastableTo(parentType))
                        {
                            list.Add(type);   
                        }
                    }

            return list.ToArray();
        }
    }
}
