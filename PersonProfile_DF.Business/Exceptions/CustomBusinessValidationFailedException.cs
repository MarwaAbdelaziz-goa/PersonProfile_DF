using System;

namespace PersonProfile_DF.Business.Exceptions
{	
    [Serializable]
    public class CustomBusinessValidationFailedException : Exception
    {
        public CustomBusinessValidationFailedException()
        {
        }

        public CustomBusinessValidationFailedException(string message) : base(message)
        {
        }

        public CustomBusinessValidationFailedException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

