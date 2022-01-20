using System;
using Plugins.UMDynamicEnum.Runtime;
using UMLogger.Plugins.UMLogger.Interfaces;

namespace UMLogger.Plugins.UMLogger.Core
{
    public class FileLogger : IUMInternalLogger
    {
        [EnumProvider("LoggerType")]
        public static EnumGenInfo GetGenInfo()
        {
            return new EnumGenInfo("FileLogger",1);
        }
        
        public void Log(object message, LogType logType, LogLevel logLevel = LogLevel.Debug, params object[] formatParams)
        {
            throw new NotImplementedException();
        }

        public void LogInfo(object message, LogLevel logLevel = LogLevel.Info, params object[] formatParams)
        {
            throw new NotImplementedException();
        }

        public void LogWarning(object message, LogLevel logLevel = LogLevel.Warn, params object[] formatParams)
        {
            throw new NotImplementedException();
        }

        public void LogError(object message, LogLevel logLevel = LogLevel.Error, params object[] formatParams)
        {
            throw new NotImplementedException();
        }

        public void LogException(Exception exception, LogLevel logLevel = LogLevel.Error, params object[] formatParams)
        {
            throw new NotImplementedException();
        }

        public void Assert(bool value, object error, LogLevel logLevel, params object[] formatParams)
        {
            throw new NotImplementedException();
        }

        public void UpdateContext(object context)
        {
            throw new NotImplementedException();
        }
    }
}