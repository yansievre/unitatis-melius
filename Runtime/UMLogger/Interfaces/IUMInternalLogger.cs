using System;
using UM.Runtime.UMLogger.Core;
using LogType = UM.Runtime.UMLogger.Core.LogType;
using Object = System.Object;

namespace UM.Runtime.UMLogger.Interfaces
{
    /// <summary>
    /// Dont use this for anything
    /// </summary>
    public interface IUMLoggerMethods
    {
        void Log(object message, LogType logType,LogLevel logLevel = LogLevel.Debug, params object[] formatParams);
        void LogInfo(object message,LogLevel logLevel = LogLevel.Info, params object[] formatParams);
        void LogWarning(object message,LogLevel logLevel = LogLevel.Warn, params object[] formatParams);
        void LogError(object message,LogLevel logLevel = LogLevel.Error, params object[] formatParams);
        void LogException(Exception exception,LogLevel logLevel = LogLevel.Error, params object[] formatParams);
        void Assert(bool value, object error,LogLevel logLevel,params object[] formatParams);
    }
    /// <summary>
    /// Extend this to create your own loggers
    /// </summary>
    public interface IUMInternalLogger : IUMLoggerMethods
    {
        void UpdateContext(Object context);
        
    }
    /// <summary>
    /// This is the logger that will be used
    /// </summary>
    public interface IUMLogger : IUMLoggerMethods
    {
        bool IsLogLevelAllowed(LogLevel logLevel);
        LogLevel AllowedLogLevel { get; }
    }
}
