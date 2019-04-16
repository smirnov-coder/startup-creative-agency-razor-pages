using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Abstractions.Exceptions;
using StartupCreativeAgency.Domain.Abstractions.Repositories;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Domain.Logic.Services
{
    /// <summary>
    /// Сервис примеров выполненных работ компании.
    /// </summary>
    public class WorkExampleService : ServiceBase<WorkExample, int>, IWorkExampleService
    {
        /// <summary>
        /// Создаёт новый экземпляр сервиса примеров выполненных работ.
        /// </summary>
        /// <param name="repository">Репозиторий для хранения примеров работ.</param>
        public WorkExampleService(IRepository<WorkExample, int> repository) : base(repository) { }

        /// <summary>
        /// Асинхронно возвращает коллекцию всех примеров работ компании.
        /// </summary>
        /// <exception cref="DomainServiceException"/>
        public async Task<IList<WorkExample>> GetWorkExamplesAsync() => await base.ListAsync();

        /// <summary>
        /// Асинхронно извлекает из коллекции примеров работ работу с заданным значением идентификатора.
        /// </summary>
        /// <returns>Объект примера работы или null, если пример работы не найден.</returns>
        /// <param name="workExampleId">Значение идентификатора примера работы.</param>
        /// <exception cref="DomainServiceException"/>
        public async Task<WorkExample> GetWorkExampleAsync(int workExampleId) => await base.GetByIdAsync(workExampleId);

        /// <summary>
        /// Асинхронно добавляет в коллекцию примеров работ новую работу.
        /// </summary>
        /// <param name="workExample">Добавляемый в коллекцию пример работы.</param>
        /// <returns>Объект добавленного примера работы.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DuplicateEntityException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task<WorkExample> AddWorkExampleAsync(WorkExample workExample) => await base.AddAsync(workExample);

        /// <summary>
        /// Асинхронно обновляет данные примера работы.
        /// </summary>
        /// <param name="workExample">Обновлённые данные примера работы.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="EntityNotFoundException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task UpdateWorkExampleAsync(WorkExample workExample) => await base.UpdateAsync(workExample);

        /// <summary>
        /// Асинхронно удаляет из коллекции примеров работ работу с заданным значением идентификатора.
        /// </summary>
        /// <param name="workExampleId">Значение идентификатора удаляемого примера работы.</param>
        /// <exception cref="EntityNotFoundException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task DeleteWorkExampleAsync(int workExampleId) => await base.DeleteAsync(workExampleId);

        protected override WorkExample UpdateExistingEntity(WorkExample existingEntity, WorkExample newData)
        {
            existingEntity.ImagePath = newData.ImagePath;
            existingEntity.Name = newData.Name;
            existingEntity.Category = newData.Category;
            existingEntity.Description = newData.Description;
            existingEntity.LastUpdatedBy = newData.LastUpdatedBy;
            existingEntity.LastUpdatedOn = newData.LastUpdatedOn;
            return existingEntity;
        }
    }
}
