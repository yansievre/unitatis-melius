#if ZENJECT
using System.Linq;
using UM.Runtime.UMDynamicEnum;
using UM.Runtime.UMLogger.Core;
using UM.Runtime.UMUtility;
using UnityEngine;
using Zenject;

[assembly:EnumGeneration("LoggerType","Assets/Plugins/unitatis-melius/Runtime/UMLogger/Core",true)]
namespace UM.Runtime.UMLogger
{
    
    [CreateAssetMenu(fileName = "UMLoggerInstaller", menuName = "Installers/UMLoggerInstaller")]
    internal class UMLoggerInstaller : ScriptableObjectInstaller<UMLoggerInstaller>
    {
        public LoggerType loggerTypes;
        public LogLevel logLevel;
        public override void InstallBindings()
        {
            if (loggerTypes.CheckBitFlag(1))
            {
                Container.BindInterfacesTo<FileLogger>().AsSingle().NonLazy();
            }
            if (loggerTypes.CheckBitFlag(2))
            {
                Container.BindInterfacesTo<DebugLogger>().AsSingle().NonLazy();
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