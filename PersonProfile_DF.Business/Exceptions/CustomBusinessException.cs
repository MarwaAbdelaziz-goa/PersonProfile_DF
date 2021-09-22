using System;

namespace PersonProfile_DF.Business.Exceptions
{	
    [Serializable]
    public class CustomBusinessException : Exception
    {
        public CustomBusinessException()
        {
        }

        public CustomBusinessException(string message) : base(message)
        {
        }

        public CustomBusinessException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

