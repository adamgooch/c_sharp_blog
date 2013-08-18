using System;
using System.Runtime.Serialization;

namespace Application
{
    [Serializable]
    public class InvalidUserIdException : Exception
    {
        public InvalidUserIdException()
        {
        }

        public InvalidUserIdException( string message )
            : base( message )
        {
        }

        public InvalidUserIdException( string message, Exception inner )
            : base( message, inner )
        {
        }

        protected InvalidUserIdException(
            SerializationInfo info,
            StreamingContext context )
            : base( info, context )
        {
        }
    }

    [Serializable]
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException()
        {
        }

        public InvalidTokenException( string message )
            : base( message )
        {
        }

        public InvalidTokenException( string message, Exception inner )
            : base( message, inner )
        {
        }

        protected InvalidTokenException(
            SerializationInfo info,
            StreamingContext context )
            : base( info, context )
        {
        }
    }
}
