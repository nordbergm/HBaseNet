using System;

namespace HBaseNet
{
    [Serializable]
    public class HBaseException : Exception
    {
        public HBaseException(string message, Exception innerException = null) : base(message, innerException)
        {   
        }
    }
}
