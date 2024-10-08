using System;

namespace UM.Runtime.UMDataSystem
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