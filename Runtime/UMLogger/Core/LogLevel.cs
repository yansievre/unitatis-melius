using System;

namespace UM.Runtime.UMLogger.Core
{
    [Flags]
    public enum LogLevel
    {
        Trace = 1,
        Debug = 2,
        Info = 4,
        Warn = 8,
        Error = 16,
        Fatal = 32,
    }
    
    public enum LogType
    {
        /// <summary>
        ///   <para>LogType used for Errors.</para>
        /// </summary>
        Error,
        /// <summary>
        ///   <para>LogType used for Asserts. (These could also indicate an error inside Unity itself.)</para>
        /// </summary>
        Assert,
        /// <summary>
        ///   <para>LogType used for Warnings.</para>
        /// </summary>
        Warning,
        /// <summary>
        ///   <para>LogType used for regular log messages.</para>
        /// </summary>
        Log,
        /// <summary>
        ///   <para>LogType used for Exceptions.</para>
        /// </summary>
        Exception,
    }
}