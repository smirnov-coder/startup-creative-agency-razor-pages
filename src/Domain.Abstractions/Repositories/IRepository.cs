using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Domain.Abstractions.Repositories
{
    /// <summary>
    /// Обобщённый интерфейс репозитория (хранилища сущностей).
    /// </summary>
    /// <typeparam name="TEntity">Тип данных хранимых в репозитории сущностей.</typeparam>
    /// <typeparam name="TKey">Тип данных идентификатора хранимых сущностей.</typeparam>
    public interface IRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        /// <summary>
        /// Асинхронно ивлекает из репозитория все сущности типа <typeparamref name="TEntity"/>.
        /// </summary>
        /// <returns>Коллекция сущностей в виде объекта, реализующего интерфейс <see cref="IList{TEntity}"/>.</returns>
        Task<IList<TEntity>> ListAsync();
        
        /// <summary>
        /// Асинхронно извлекает из репозитория все сущности типа <typeparamref name="TEntity"/>, удовлетворяющие условиям, 
        /// описанным в спецификации.
        /// </summary>
        /// <param name="specification">Объект спецификации, реализующий интерфейс <see cref="ISpecification{TEntity}"/>.</param>
        /// <returns>Коллекция сущностей в виде объекта, реализующего интерфейс <see cref="IList{TEntity}"/>.</returns>
        Task<IList<TEntity>> ListAsync(ISpecification<TEntity> specification);
        
        /// <summary>
        /// Асинхронно извлекает из репозитория экземпляр сущности типа <typeparamref name="TEntity"/> 
        /// с заданным значением идентификатора типа <typeparamref name="TKey"/>.
        /// </summary>
        /// <param name="entityId">Значение идентификатора сущности типа <typeparamref name="TKey"/>.</param>
        /// <returns>Объект сущности типа <typeparamref name="TEntity"/> или null, если сущность не найдена.</returns>
        Task<TEntity> GetByIdAsync(TKey entityId);
        
        /// <summary>
        /// Асинхронно извлекает из репозитория экземпляр сущности типа <typeparamref name="TEntity"/>, 
        /// удовлетворяющий условиям, описанным в спецификации.
        /// </summary>
        /// <param name="specification">Объект спецификации, реализуюший интерфейс <see cref="ISpecification{TEntity}"/>.</param>
        /// <returns>Объект сущности типа <typeparamref name="TEntity"/> или null, если сущность не найдена.</returns>
        Task<TEntity> GetBySpecificationAsync(ISpecification<TEntity> specification);
        
        /// <summary>
        /// Асинхронно сохраняет сущность типа <typeparamref name="TEntity"/> в репозитории.
        /// </summary>
        /// <param name="entity">Данные сущности типа <typeparamref name="TEntity"/> для сохранения.</param>
        /// <returns>Объект сохранённой сущности типа <typeparamref name="TEntity"/>.</returns>
        Task<TEntity> AddAsync(TEntity entity);
        
        /// <summary>
        /// Асинхронно обновляет данные сущности типа <typeparamref name="TEntity"/> в репозитории.
        /// </summary>
        /// <param name="entity">Обновлённые данные сущности типа <typeparamref name="TEntity"/>.</param>
        Task UpdateAsync(TEntity entity);
        
        /// <summary>
        /// Асинхронно удаляет из репозитория сущность типа <typeparamref name="TEntity"/> 
        /// с заданным значением идентификатора типа <typeparamref name="TKey"/>.
        /// </summary>
        /// <param name="entity">Значение идентификатора типа <typeparamref name="TKey"/> удаляемой сущности 
        /// типа <typeparamref name="TEntity"/>.</param>
        Task DeleteAsync(TEntity entity);
    }
}
