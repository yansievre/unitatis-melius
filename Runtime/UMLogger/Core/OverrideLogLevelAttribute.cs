using System;

namespace UM.Runtime.UMLogger.Core
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = false,Inherited = true)]
    public class OverrideLogLevelAttribute : Attribute
    {
        public readonly LogLevel LogLevel;

        public OverrideLogLevelAttribute(LogLevel logLevel)
        {
            LogLevel = logLevel;
        }
    }
    
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = false,Inherited = true)]
    public class AllowAllLogsAttribute : OverrideLogLevelAttribute
    {

        public AllowAllLogsAttribute() : base(
            LogLevel.Debug &
            LogLevel.Error &
            LogLevel.Fatal &
            LogLevel.Info &
            LogLevel.Trace &
            LogLevel.Warn
            )
        {
        }
    }
}
