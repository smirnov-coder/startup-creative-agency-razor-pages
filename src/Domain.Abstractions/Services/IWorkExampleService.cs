using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Domain.Abstractions.Services
{
    /// <summary>
    /// Сервис примеров выполненных работ компании.
    /// </summary>
    public interface IWorkExampleService
    {
        /// <summary>
        /// Асинхронно возвращает коллекцию всех примеров работ компании.
        /// </summary>
        Task<IList<WorkExample>> GetWorkExamplesAsync();
        
        /// <summary>
        /// Асинхронно извлекает из коллекции примеров работ работу с заданным значением идентификатора.
        /// </summary>
        /// <returns>Объект примера работы или null, если пример работы не найден.</returns>
        /// <param name="workExampleId">Значение идентификатора примера работы.</param>
        Task<WorkExample> GetWorkExampleAsync(int workExampleId);
        
        /// <summary>
        /// Асинхронно добавляет в коллекцию примеров работ новую работу.
        /// </summary>
        /// <param name="workExample">Добавляемый в коллекцию пример работы.</param>
        /// <returns>Объект добавленного примера работы.</returns>
        Task<WorkExample> AddWorkExampleAsync(WorkExample workExample);
        
        /// <summary>
        /// Асинхронно обновляет данные примера работы.
        /// </summary>
        /// <param name="workExample">Обновлённые данные примера работы.</param>
        Task UpdateWorkExampleAsync(WorkExample workExample);
        
        /// <summary>
        /// Асинхронно удаляет из коллекции примеров работ работу с заданным значением идентификатора.
        /// </summary>
        /// <param name="workExampleId">Значение идентификатора удаляемого примера работы.</param>
        Task DeleteWorkExampleAsync(int workExampleId);
    }
}
