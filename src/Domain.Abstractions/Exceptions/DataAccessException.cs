using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StartupCreativeAgency.Domain.Abstractions.Exceptions
{
    /// <summary>
    /// Исключение, выбрасываемое при возникновении ошибки выполнения операций доступа к данным. Должно использоваться 
    /// в качестве обёртки для исключений конкретных реализаций механизмов хранения и доступа к данным, например, СУБД SQLite.
    /// </summary>
    [Serializable]
    public class DataAccessException : Exception
    {
        /// <summary>
        /// Коллекция строковых описаний возникших ошибок. Например, ошибки ASP.NET Core Identity.
        /// </summary>
        public IEnumerable<string> Errors { get; set; }

        public DataAccessException() { }

        public DataAccessException(string message) : base(message) { }

        public DataAccessException(string message, IEnumerable<string> errors) : base(message) => Errors = errors;

        public DataAccessException(string message, Exception innerException) : base(message, innerException) { }

        protected DataAccessException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
