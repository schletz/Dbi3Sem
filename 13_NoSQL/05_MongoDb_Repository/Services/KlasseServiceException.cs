using System;
using System.Runtime.Serialization;

namespace MongoDbDemo.Services
{
    [Serializable]
    internal class KlasseServiceException : Exception
    {
        public KlasseServiceException()
        {
        }

        public KlasseServiceException(string message) : base(message)
        {
        }

        public KlasseServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected KlasseServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}