using System;

namespace Plugins.UMDataSystem
{
    class DataSystemException : Exception
    {
        public DataSystemException()
        {
        }


        public DataSystemException(string message) : base(message)
        {
        }

        public DataSystemException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}