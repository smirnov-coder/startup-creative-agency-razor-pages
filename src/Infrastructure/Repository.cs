using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StartupCreativeAgency.Domain.Abstractions;
using StartupCreativeAgency.Domain.Abstractions.Exceptions;
using StartupCreativeAgency.Domain.Abstractions.Repositories;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Domain.Logic.Specifications;

namespace StartupCreativeAgency.Infrastructure
{
    /// <summary>
    /// Обобщённый класс репозитория (хранилища сущностей).
    /// </summary>
    /// <typeparam name="TContext">Тип данных контекста базы данных.</typeparam>
    /// <typeparam name="TEntity">Тип данных хранимых в репозитории сущностей.</typeparam>
    /// <typeparam name="TKey">Тип данных идентификатора хранимых сущностей.</typeparam>
    public class Repository<TContext, TEntity, TKey> : IRepository<TEntity, TKey> 
        where TContext : DbContext
        where TEntity : BaseEntity<TKey>
    {
        private TContext _db;
        private DbSet<TEntity> _entitySet;

        /// <summary>
        /// Создаёт новый объект репозитория.
        /// </summary>
        /// <param name="db">Объект контекста базы данных. Должен быть производным от <see cref="DbContext"/>.</param>
        public Repository(TContext db)
        {
            _db = db;
            _entitySet = _db.Set<TEntity>();
        }

        /// <summary>
        /// Асинхронно ивлекает из репозитория все сущности типа <typeparamref name="TEntity"/>.
        /// </summary>
        /// <returns>Коллекция сущностей в виде объекта, реализующего интерфейс <see cref="IList{TEntity}"/>.</returns>
        /// <exception cref="DataAccessException"/>
        public virtual async Task<IList<TEntity>> ListAsync() => await ListAsync(new BaseSpecification<TEntity>(x => true));

        /// <summary>
        /// Асинхронно извлекает из репозитория все сущности типа <typeparamref name="TEntity"/>, удовлетворяющие условиям, 
        /// описанным в спецификации.
        /// </summary>
        /// <param name="specification">Объект спецификации, реализующий интерфейс <see cref="ISpecification{TEntity}"/>.</param>
        /// <returns>Коллекция сущностей в виде объекта, реализующего интерфейс <see cref="IList{TEntity}"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DataAccessException"/>
        public virtual async Task<IList<TEntity>> ListAsync(ISpecification<TEntity> specification)
        {
            EnsureSpecificationNotNull(specification);
            try
            {
                return await ApplySpecification(specification).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new DataAccessException(GetExceptionMessage("list entities"), ex);
            }
        }

        /// <summary>
        /// Асинхронно извлекает из репозитория экземпляр сущности типа <typeparamref name="TEntity"/> 
        /// с заданным значением идентификатора типа <typeparamref name="TKey"/>.
        /// </summary>
        /// <param name="entityId">Значение идентификатора сущности типа <typeparamref name="TKey"/>.</param>
        /// <returns>Объект сущности типа <typeparamref name="TEntity"/> или null, если сущность не найдена.</returns>
        /// <exception cref="DataAccessException"/>
        public virtual async Task<TEntity> GetByIdAsync(TKey entityId)
        {
            return await GetBySpecificationAsync(new BaseSpecification<TEntity>(x => x.Id.Equals(entityId)));
        }

        /// <summary>
        /// Асинхронно извлекает из репозитория экземпляр сущности типа <typeparamref name="TEntity"/>, 
        /// удовлетворяющий условиям, описанным в спецификации.
        /// </summary>
        /// <param name="specification">Объект спецификации, реализуюший интерфейс <see cref="ISpecification{TEntity}"/>.</param>
        /// <returns>Объект сущности типа <typeparamref name="TEntity"/> или null, если сущность не найдена.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DataAccessException"/>
        public virtual async Task<TEntity> GetBySpecificationAsync(ISpecification<TEntity> specification)
        {
            EnsureSpecificationNotNull(specification);
            try
            {
                return await ApplySpecification(specification).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new DataAccessException(GetExceptionMessage("get entity"), ex);
            }
        }

        /// <summary>
        /// Асинхронно сохраняет сущность типа <typeparamref name="TEntity"/> в репозитории.
        /// </summary>
        /// <param name="entity">Данные сущности типа <typeparamref name="TEntity"/> для сохранения.</param>
        /// <returns>Объект сохранённой сущности типа <typeparamref name="TEntity"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DataAccessException"/>
        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            EnsureEntityNotNull(entity);
            try
            {
                var result = _entitySet.Add(entity);
                await _db.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(GetExceptionMessage("add entity"), ex);
            }
        }

        /// <summary>
        /// Асинхронно обновляет данные сущности типа <typeparamref name="TEntity"/> в репозитории.
        /// </summary>
        /// <param name="entity">Обновлённые данные сущности типа <typeparamref name="TEntity"/>.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DataAccessException"/>
        public virtual async Task UpdateAsync(TEntity entity)
        {
            EnsureEntityNotNull(entity);
            try
            {
                _db.Entry(entity).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new DataAccessException(GetExceptionMessage("update entity"), ex);
            }
        }

        /// <summary>
        /// Асинхронно удаляет из репозитория сущность типа <typeparamref name="TEntity"/> 
        /// с заданным значением идентификатора типа <typeparamref name="TKey"/>.
        /// </summary>
        /// <param name="entity">Значение идентификатора типа <typeparamref name="TKey"/> удаляемой сущности 
        /// типа <typeparamref name="TEntity"/>.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DataAccessException"/>
        public virtual async Task DeleteAsync(TEntity entity)
        {
            EnsureEntityNotNull(entity);
            try
            {
                _entitySet.Remove(entity);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new DataAccessException(GetExceptionMessage("delete entity"), ex);
            }
        }

        private void EnsureSpecificationNotNull(ISpecification<TEntity> specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));
        }

        private void EnsureEntityNotNull(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
        }
        
        private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification)
        {
            return SpecificationEvaluator<TEntity, TKey>.GetQuery(_entitySet, specification);
        }

        private string GetExceptionMessage(string action)
        {
            return $"Unable to {action} of type '{typeof(TEntity)}'. See inner exception for details.";
        }
    }
}
