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
    /// Сервис для работы с корпоративными клиентами компании (брендами).
    /// </summary>
    public class BrandService : ServiceBase<Brand, int>, IBrandService
    {
        /// <summary>
        /// Создаёт новый экземпляр сервиса брендов.
        /// </summary>
        /// <param name="repository">Репозиторий для хранения брендов.</param>
        public BrandService(IRepository<Brand, int> repository) : base(repository) { }

        /// <summary>
        /// Асинхронно возвращает коллекцию всех брендов.
        /// </summary>
        /// <exception cref="DomainServiceException"/>
        public async Task<IList<Brand>> GetBrandsAsync() => await base.ListAsync();

        /// <summary>
        /// Асинхронно извлекает из коллекции бренд с заданным значением идентификатора.
        /// </summary>
        /// <param name="brandId">Значение идентификатора бренда.</param>
        /// <returns>Объект бренда или null, если бренд не найден.</returns>
        /// <exception cref="DomainServiceException"/>
        public async Task<Brand> GetBrandAsync(int brandId) => await base.GetByIdAsync(brandId);

        /// <summary>
        /// Асинхронно добавляет новый бренд в коллекцию брендов.
        /// </summary>
        /// <param name="brand">Добавляемый в коллекцию бренд.</param>
        /// <returns>Объект добавленного в коллекцию бренда.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DuplicateEntityException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task<Brand> AddBrandAsync(Brand brand) => await base.AddAsync(brand);

        /// <summary>
        /// Асинхронно обновляет данные бренда.
        /// </summary>
        /// <param name="brand">Обновлённые данные бренда.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="EntityNotFoundException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task UpdateBrandAsync(Brand brand) => await base.UpdateAsync(brand);

        /// <summary>
        /// Асинхронно удаляет из коллекции бренд с заданным значением идентификатора.
        /// </summary>
        /// <param name="brandId">Значение идентификатора удаляемого бренда.</param>
        /// <exception cref="EntityNotFoundException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task DeleteBrandAsync(int brandId) => await base.DeleteAsync(brandId);

        protected override Brand UpdateExistingEntity(Brand existingEntity, Brand newData)
        {
            existingEntity.ImagePath = newData.ImagePath;
            existingEntity.Name = newData.Name;
            existingEntity.LastUpdatedBy = newData.LastUpdatedBy;
            existingEntity.LastUpdatedOn = newData.LastUpdatedOn;
            return existingEntity;
        }
    }
}
