#if ZENJECT
using System.Linq;
using Plugins.UMDebug;
using Plugins.UMDynamicEnum.Runtime;
using UMLogger.Plugins.UMLogger.Core;
using UnityEngine;
using Zenject;

[assembly:EnumGeneration("LoggerType","Assets/Plugins/unitatis-melius/UMLogger/Core",true)]
namespace UMLogger.Plugins.UMLogger
{
    
    [CreateAssetMenu(fileName = "UMLoggerInstaller", menuName = "Installers/UMLoggerInstaller")]
    internal class UMLoggerInstaller : ScriptableObjectInstaller<UMLoggerInstaller>
    {
        public LoggerType loggerTypes;
        public LogLevel logLevel;
        public override void InstallBindings()
        {
            if (loggerTypes.HasFlag(LoggerType.DebugLogger))
            {
                Container.BindInterfacesTo<DebugLogger>().AsSingle().NonLazy();
            }
            if (loggerTypes.HasFlag(LoggerType.FileLogger))
            {
                Container.BindInterfacesTo<FileLogger>().AsSingle().NonLazy();
            }

 
            Container.BindInterfacesTo<Core.UMLogger>().FromMethod(ctx =>
            {
                var overrideLogLevel = (OverrideLogLevelAttribute)ctx.ObjectType.GetCustomAttributes(typeof(OverrideLogLevelAttribute), true).FirstOrDefault();
                var targetLogLevel = logLevel;
                if (overrideLogLevel != null)
                {
                    targetLogLevel = logLevel & overrideLogLevel.LogLevel;
                }
                return ctx.Container.Instantiate<Core.UMLogger>(new object[]{ctx.ObjectInstance,targetLogLevel});
            });
        }
    }
}
#endif