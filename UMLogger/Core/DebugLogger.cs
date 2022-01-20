﻿using System;
using Plugins.UMDynamicEnum.Runtime;
using UMLogger.Plugins.UMLogger.Interfaces;
using UnityEngine;
using Object = System.Object;

namespace UMLogger.Plugins.UMLogger.Core
{

    public class DebugLogger : IUMInternalLogger
    {
        private string Context { get; set; }


        public void UpdateContext(object context)
        {
            if (context is MonoBehaviour mono)
            {
                Context = $"[{mono.GetType().Name}:go-{mono.gameObject.name}]";
            }
            else
            {
                Context = $"[{context.GetType().Name}]";
            }
        }
        [EnumProvider("LoggerType")]
        public static EnumGenInfo GetGenInfo()
        {
            return new EnumGenInfo("DebugLogger",2);
        }

        private string GetColor(LogType logType)
        {
            Color color = logType switch
            {
                LogType.Error => Color.red,
                LogType.Warning => Color.yellow,
                LogType.Log => Color.white,
                LogType.Exception => Color.magenta,
                LogType.Assert =>  Color.blue,
                _ => throw new ArgumentOutOfRangeException(nameof(logType), logType, null)
            };

            return ColorUtility.ToHtmlStringRGB(color);
        }

        private string GenerateLogText(object message, object[] formatParams,LogType logType,LogLevel logLevel)
        {
            var msg = message.ToString();
            if (formatParams != null) msg = string.Format(msg, formatParams);
            var st = $"<color={GetColor(logType)}><b>{logLevel.ToString().Substring(0, 1)}:<i>{Context}</i></b> === {message}</color>";
            return st;
        }

       


       
        public void Log(object message, LogType logType,LogLevel logLevel = LogLevel.Debug, params object[] formatParams)
        {
            var logText = GenerateLogText(message, formatParams, logType, logLevel);
            Debug.Log(logText);
        }
        

        public void LogInfo(object message,LogLevel logLevel = LogLevel.Info, params object[] formatParams)
        {
            Log(message, LogType.Log, logLevel, formatParams);
        }

        public void Assert(bool value, object error,LogLevel logLevel,params object[] formatParams)
        {
            if (!value)
            {
                Log(error, LogType.Assert, logLevel, formatParams);
            }
        }

        public void LogWarning(object message,LogLevel logLevel = LogLevel.Warn, params object[] formatParams)
        {
            Log(message, LogType.Warning, logLevel, formatParams);
        }
        
        
        public void LogError(object message,LogLevel logLevel = LogLevel.Error, params object[] formatParams)
        {
            Log(message, LogType.Error, logLevel, formatParams);
        }
        
        public void LogException(Exception exception,LogLevel logLevel = LogLevel.Error, params object[] formatParams)
        {
            Log("An exception was thrown. Info below", LogType.Exception, logLevel, formatParams);
            Debug.LogException(exception);
        }

        
    }
}