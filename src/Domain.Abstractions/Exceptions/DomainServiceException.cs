using System;

namespace StartupCreativeAgency.Domain.Abstractions.Exceptions
{
    /// <summary>
    /// Общее исключение, выбрасываемое сервисами доменной модели. Должно использоваться в качестве обёртки
    /// для исключений, генерируемых логикой доменной модели в тех случаях, когда невозможно выбросить конкретное исключение
    /// из пространства имён <see cref="StartupCreativeAgency.Domain.Abstractions.Exceptions"/>.
    /// </summary>
    [Serializable]
    public class DomainServiceException : Exception
    {
        public DomainServiceException() { }

        public DomainServiceException(string message) : base(message) { }

        public DomainServiceException(string message, Exception inner) : base(message, inner) { }

        protected DomainServiceException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}