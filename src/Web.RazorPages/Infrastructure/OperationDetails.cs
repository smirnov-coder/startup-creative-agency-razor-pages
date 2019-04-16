using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StartupCreativeAgency.Web.RazorPages.Infrastructure
{
    /// <summary>
    /// Информация о выполненной операции.
    /// </summary>
    public class OperationDetails
    {
        /// <summary>
        /// Статус операции.
        /// </summary>
        public OperationStatus Status { get; set; }
        
        /// <summary>
        /// Текст сообщения.
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Статус операции.
    /// </summary>
    public enum OperationStatus
    {
        /// <summary>
        /// Операция выполнена успешно.
        /// </summary>
        Success,
        
        /// <summary>
        /// Произошла ошибка выполнения операции.
        /// </summary>
        Error
    }
}
