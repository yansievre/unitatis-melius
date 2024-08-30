using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UM.Runtime.UMDynamicEnum;
using UnityEditor;
using Assembly = System.Reflection.Assembly;

namespace UM.Editor.UMDynamicEnum
{
    internal class EnumGenerationInfo
    {
        public string EnumName;
        public string ParentDirectory;
        public bool IsBitFlag;
        public Dictionary<string, int> Values = new Dictionary<string, int>();
    }
    
    internal static class DynamicEnumGenerator
    {
        private static bool PauseGeneration = false;
        public static T GetAssemblyAttribute<T>(this System.Reflection.Assembly ass) where T :  Attribute
        {
            object[] attributes = ass.GetCustomAttributes(typeof(T), false);
            if (attributes == null || attributes.Length == 0)
                return null;
            return attributes.OfType<T>().SingleOrDefault();
        }
        
        public static T[] GetAssemblyAttributes<T>(this System.Reflection.Assembly ass) where T :  Attribute
        {
            object[] attributes = ass.GetCustomAttributes(typeof(T), false);
            if (attributes == null || attributes.Length == 0)
                return null;
            return attributes.OfType<T>().ToArray();
        }
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void AfterScriptReload()
        {
            if (PauseGeneration) return;
            if(EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += AfterScriptReload;
                return;
            }
 
            EditorApplication.delayCall += GenerateEnums;
        }
        
        public static void GenerateEnums()
        {
            PauseGeneration = true;
            Dictionary<string, EnumGenerationInfo> cache = new Dictionary<string, EnumGenerationInfo>();
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in allAssemblies)
            {
                var generationAttributes = assembly.GetAssemblyAttributes<EnumGenerationAttribute>();
                if(generationAttributes==null) continue;
                foreach (var generationData in generationAttributes)
                {
                    if(cache.ContainsKey(generationData.EnumName)) throw new UMDynamicEnumException("Cannot define an enum twice: "+generationData.EnumName);
                    var data = new EnumGenerationInfo()
                    {
                        EnumName = generationData.EnumName,
                        ParentDirectory = generationData.ParentDirectory,
                        IsBitFlag = generationData.IsBitFlag
                    };
                    cache.Add(data.EnumName,data);
                }
            }

            var definedInAssembly = typeof(EnumProviderAttribute).Assembly.GetName().Name;
            foreach (var assembly in allAssemblies)
            {
                if (assembly.GetReferencedAssemblies().All(a => a.Name != definedInAssembly)) continue;
                ProcessAssembly(assembly, cache);
            }

            foreach (var generationInfo in cache.Values)
            {
                string enumName = generationInfo.EnumName;
                string filePathAndName = Path.Combine(generationInfo.ParentDirectory,enumName + ".cs"); 
                using ( StreamWriter streamWriter = new StreamWriter( filePathAndName ) )
                {
                    if (generationInfo.IsBitFlag)
                    {
                        streamWriter.WriteLine("[System.Flags]");
                    }
                    streamWriter.WriteLine( "public enum " + enumName );
                    streamWriter.WriteLine( "{" );
                    foreach (var val in generationInfo.Values)
                    {
                        streamWriter.WriteLine( "\t" + val.Key+"="+val.Value + "," );
                    }
                    streamWriter.WriteLine( "}" );
                }
            }
            AssetDatabase.Refresh();
            PauseGeneration = false;
        }

        private static void ProcessAssembly(Assembly assembly, Dictionary<string, EnumGenerationInfo> cache)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsClass) continue;
                foreach (var methodInfo in type.GetMethods())
                {
                    string errorInfo = $"{type.Name}, {methodInfo.Name}";
                    var customAttribute = methodInfo.GetCustomAttributes<EnumProviderAttribute>().FirstOrDefault();
                    if (!ReferenceEquals(customAttribute, null))
                    {
                        if (!cache.ContainsKey(customAttribute.EnumName))
                            throw new UMDynamicEnumException($"Invalid enum name. {customAttribute.EnumName}, {errorInfo}");
                        ProcessMethodInfo(methodInfo,errorInfo, cache[customAttribute.EnumName]);
                    }
                }
            }
        }

        private static void ProcessMethodInfo(MethodInfo methodInfo,string errorInfo, EnumGenerationInfo generationInfo)
        {
            if (!methodInfo.IsStatic)
            {
                throw new UMDynamicEnumException($"This method must be static. {errorInfo}.");
            }

            if (methodInfo.GetParameters().Length != 0)
            {
                throw new UMDynamicEnumException($"This method shouldn't have any parameters. {errorInfo}.");
            }

            if (methodInfo.ReturnType == typeof(EnumGenInfo))
            {
                var result = (EnumGenInfo) methodInfo.Invoke(null, null);
                AttemptAddGenInfo(generationInfo, result, errorInfo);
            }
            else if (methodInfo.ReturnType == typeof(EnumGenInfo[]))
            {
                var result = (EnumGenInfo[]) methodInfo.Invoke(null, null);
                foreach (var genInfo in result)
                {
                    AttemptAddGenInfo(generationInfo, genInfo, errorInfo);
                }
            }
            else
            {
                throw new UMDynamicEnumException(
                    $"Invalid return value for method. {errorInfo}. Should be {nameof(EnumGenInfo)} or {nameof(EnumGenInfo)}[]");
            }
        }

        private static void AttemptAddGenInfo(EnumGenerationInfo generationInfo, EnumGenInfo result, string errorInfo)
        {
            if (generationInfo.Values.ContainsKey(result.ValueName))
                throw new UMDynamicEnumException($"Invalid value name, already exists. {result.ValueName}, {errorInfo}");
            if (generationInfo.Values.ContainsValue(result.Value))
                throw new UMDynamicEnumException($"Invalid value, already exists. {result.Value}, {errorInfo}");
            generationInfo.Values.Add(result.ValueName, result.Value);
        }

       
        
    }
}
