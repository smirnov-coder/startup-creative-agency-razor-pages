using System;
using System.Runtime.Serialization;

namespace StartupCreativeAgency.Domain.Abstractions.Exceptions
{
    /// <summary>
    /// Исключение, выбрасываемое в случае, если сущность с заданным значением идентификатора уже существует.
    /// </summary>
    [Serializable]
    public class DuplicateEntityException : DomainServiceException
    {
        public DuplicateEntityException() { }

        public DuplicateEntityException(string message) : base(message) { }

        public DuplicateEntityException(string message, Exception innerException) : base(message, innerException) { }

        protected DuplicateEntityException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
