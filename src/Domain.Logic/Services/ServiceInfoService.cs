using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Abstractions.Repositories;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Domain.Abstractions.Exceptions;

namespace StartupCreativeAgency.Domain.Logic.Services
{
    /// <summary>
    /// Сервис услуг, предоставляемых компанией.
    /// </summary>
    public class ServiceInfoService : ServiceBase<ServiceInfo, int>, IServiceInfoService
    {
        /// <summary>
        /// Создаёт новый экземпляр сервиса услуг компании.
        /// </summary>
        /// <param name="repository">Репозиторий для хранения описаний услуг компании.</param>
        public ServiceInfoService(IRepository<ServiceInfo, int> repository) : base(repository) { }

        /// <summary>
        /// Асинхронно возвращает коллекцию описаний всех услуг компании.
        /// </summary>
        /// <exception cref="DomainServiceException"/>
        public async Task<IList<ServiceInfo>> GetServiceInfosAsync() => await base.ListAsync();

        /// <summary>
        /// Асинхронно извлекает из коллекции услуг описание услуги с заданным значением идентификатора.
        /// </summary>
        /// <param name="serviceInfoId">Значение идентификатора услуги компании.</param>
        /// <returns>Объект описания услуги компании или null, если услуга не найдена.</returns>
        /// <exception cref="DomainServiceException"/>
        public async Task<ServiceInfo> GetServiceInfoAsync(int serviceInfoId) => await base.GetByIdAsync(serviceInfoId);

        /// <summary>
        /// Асинхронно добавлет в коллекцию услуг компании описание новой услуги.
        /// </summary>
        /// <param name="serviceInfo">Добавляемое описание услуги компании.</param>
        /// <returns>Объект добавленной услуги компании.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DuplicateEntityException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task<ServiceInfo> AddServiceInfoAsync(ServiceInfo serviceInfo) => await base.AddAsync(serviceInfo);

        /// <summary>
        /// Асинхронно обновляет описание услуги компании.
        /// </summary>
        /// <param name="serviceInfo">Обновлённые данные услуги компании.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="EntityNotFoundException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task UpdateServiceInfoAsync(ServiceInfo serviceInfo) => await base.UpdateAsync(serviceInfo);

        /// <summary>
        /// Асинхронно удаляет из коллекции услуг услугу с заданным значением идентификатора.
        /// </summary>
        /// <param name="serviceInfoId">Значение идентификатора удаляемой услуги компании.</param>
        /// <exception cref="EntityNotFoundException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task DeleteServiceInfoAsync(int serviceInfoId) => await base.DeleteAsync(serviceInfoId);

        protected override ServiceInfo UpdateExistingEntity(ServiceInfo existingEntity, ServiceInfo newData)
        {
            existingEntity.IconClass = newData.IconClass;
            existingEntity.Caption = newData.Caption;
            existingEntity.Description = newData.Description;
            existingEntity.LastUpdatedBy = newData.LastUpdatedBy;
            existingEntity.LastUpdatedOn = newData.LastUpdatedOn;
            return existingEntity;
        }
    }
}
