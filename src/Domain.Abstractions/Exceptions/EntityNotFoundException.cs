using System;
using System.Runtime.Serialization;

namespace StartupCreativeAgency.Domain.Abstractions.Exceptions
{
    /// <summary>
    /// Исключение, выбрасываемое в случае, если не удалось найти сущность с заданным значением идентификатора.
    /// </summary>
    [Serializable]
    public class EntityNotFoundException : DomainServiceException
    {
        public EntityNotFoundException() { }

        public EntityNotFoundException(string message) : base(message) { }

        public EntityNotFoundException(string message, Exception innerException) : base(message, innerException) { }

        protected EntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
