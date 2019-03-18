using System;

namespace CyLR.src.read
{
    class DiskReadException : Exception
    {
        public DiskReadException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}