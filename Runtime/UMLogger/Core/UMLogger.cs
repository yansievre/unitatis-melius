using System;
using UM.Runtime.UMLogger.Interfaces;

namespace UM.Runtime.UMLogger.Core
{
    
    internal class UMLogger : IUMLogger
    {
        private Type _context;
        private IUMInternalLogger[] _internalLoggers;
        private LogLevel _allowedLogLevel;

        public UMLogger(Type context, IUMInternalLogger[] internalLoggers, LogLevel allowedLogLevel)
        {
            this._context = context;
            _internalLoggers = internalLoggers;
            _allowedLogLevel = allowedLogLevel;
        }

        public bool IsLogLevelAllowed(LogLevel logLevel)
        {
            return _allowedLogLevel.HasFlag(logLevel);
        }

        public void Log(object message, LogType logType, LogLevel logLevel = LogLevel.Debug, params object[] formatParams)
        {
            if (!IsLogLevelAllowed(logLevel)) return;
            foreach (var logger in _internalLoggers)
            {
                logger.UpdateContext(_context);
                logger.Log(message, logType, logLevel, formatParams);
            }
        }

        public void LogInfo(object message, LogLevel logLevel = LogLevel.Info, params object[] formatParams)
        {
            if (!IsLogLevelAllowed(logLevel)) return;
            foreach (var logger in _internalLoggers)
            {
                logger.UpdateContext(_context);
                logger.LogInfo(message, logLevel, formatParams);
            }
        }

        public void LogWarning(object message, LogLevel logLevel = LogLevel.Warn, params object[] formatParams)
        {
            if (!IsLogLevelAllowed(logLevel)) return;
            foreach (var logger in _internalLoggers)
            {
                logger.UpdateContext(_context);
                logger.LogWarning(message, logLevel, formatParams);
            }
        }

        public void LogError(object message, LogLevel logLevel = LogLevel.Error, params object[] formatParams)
        {
            if (!IsLogLevelAllowed(logLevel)) return;
            foreach (var logger in _internalLoggers)
            {
                logger.UpdateContext(_context);
                logger.LogError(message, logLevel, formatParams);
            }
        }

        public void LogException(Exception exception, LogLevel logLevel = LogLevel.Error, params object[] formatParams)
        {
            if (!IsLogLevelAllowed(logLevel)) return;
            foreach (var logger in _internalLoggers)
            {
                logger.UpdateContext(_context);
                logger.LogException(exception, logLevel, formatParams);
            }
        }

        public void Assert(bool value, object error, LogLevel logLevel, params object[] formatParams)
        {
            if (!IsLogLevelAllowed(logLevel)) return;
            foreach (var logger in _internalLoggers)
            {
                logger.UpdateContext(_context);
                logger.Assert(value, error, logLevel, formatParams);
            }
        }

        public LogLevel AllowedLogLevel => _allowedLogLevel;
    }
}
