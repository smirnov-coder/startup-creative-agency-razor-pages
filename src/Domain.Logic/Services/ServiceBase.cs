using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Abstractions;
using StartupCreativeAgency.Domain.Abstractions.Exceptions;
using StartupCreativeAgency.Domain.Abstractions.Repositories;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Domain.Logic.Specifications;

namespace StartupCreativeAgency.Domain.Logic.Services
{
    /// <summary>
    /// Абстрактный класс сервиса доменной модели. Предоставляет базовую реализацию основных операций с коллекциями сущностей.
    /// </summary>
    /// <typeparam name="TEntity">Тип данных сущности.</typeparam>
    /// <typeparam name="TKey">Тип данных идентификатора сущности.</typeparam>
    public abstract class ServiceBase<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        private IRepository<TEntity, TKey> _repository;

        public ServiceBase(IRepository<TEntity, TKey> repository) => _repository = repository;

        /// <summary>
        /// Асинхронно возвращает коллекцию всех сущностей типа <typeparamref name="TEntity"/>.
        /// </summary>
        /// <exception cref="DomainServiceException"/>
        protected virtual async Task<IList<TEntity>> ListAsync()
        {
            return await ListAsync(new BaseSpecification<TEntity>(x => true));
        }

        /// <summary>
        /// Асинхронно возвращает коллекцию сущностей типа <typeparamref name="TEntity"/>, удовлетворяющих условиям,
        /// определённым в спецификации.
        /// </summary>
        /// <param name="specification">Спецификация условий выборки.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DomainServiceException"/>
        protected virtual async Task<IList<TEntity>> ListAsync(ISpecification<TEntity> specification)
        {
            EnsureSpecificationNotNull(specification);
            try
            {
                return await _repository.ListAsync(specification);
            }
            catch (Exception ex)
            {
                throw new DomainServiceException(GetExceptionMessage("list entities"), ex);
            }
        }

        /// <summary>
        /// Асинхронно извлекает из коллекции сущностей типа <typeparamref name="TEntity"/> сущность с заданным значением
        /// идентификатора типа <typeparamref name="TKey"/>.
        /// </summary>
        /// <param name="entityId">Значение идентификатора типа <typeparamref name="TKey"/> сущности 
        /// типа <typeparamref name="TEntity"/>.</param>
        /// <returns>Объект сущности типа <typeparamref name="TEntity"/> или null, если сущность не найдена.</returns>
        /// <exception cref="DomainServiceException"/>
        protected virtual async Task<TEntity> GetByIdAsync(TKey entityId)
        {
            return await GetBySpecificationAsync(new BaseSpecification<TEntity>(x => x.Id.Equals(entityId)));
        }

        /// <summary>
        /// Асинхронно извлекает из коллекции сущностей типа <typeparamref name="TEntity"/> сущность, удовлетворяющую условиям,
        /// определённым в спецификации.
        /// </summary>
        /// <param name="specification">Спецификация условий выборки.</param>
        /// <returns>Объект сущности типа <typeparamref name="TEntity"/> или null, если сущность не найдена.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DomainServiceException"/>
        protected virtual async Task<TEntity> GetBySpecificationAsync(ISpecification<TEntity> specification)
        {
            EnsureSpecificationNotNull(specification);
            try
            {
                return await _repository.GetBySpecificationAsync(specification);
            }
            catch (Exception ex)
            {
                throw new DomainServiceException(GetExceptionMessage("get entity"), ex);
            }
        }

        /// <summary>
        /// Асинхронно добавляет в коллекцию сущностей типа <typeparamref name="TEntity"/> новую сущность.
        /// </summary>
        /// <param name="entity">Добавляемая в коллекцию типа <typeparamref name="TEntity"/> новая сущность.</param>
        /// <returns>Объект добавленной сущности типа <typeparamref name="TEntity"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DuplicateEntityException"/>
        /// <exception cref="DomainServiceException"/>
        protected virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            EnsureEntityNotNull(entity);
            try
            {
                var existing = await _repository.GetByIdAsync(entity.Id);
                EnsureNoDuplicateEntity(existing);
                return await _repository.AddAsync(entity);
            }
            catch (Exception ex) when (!(ex is DuplicateEntityException))
            {
                throw new DomainServiceException(GetExceptionMessage("add entity"), ex);
            }
        }

        /// <summary>
        /// Асинхронно обновляет данные сущности типа <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="entity">Обновлённые данные сущности типа <typeparamref name="TEntity"/>.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="EntityNotFoundException"/>
        /// <exception cref="DomainServiceException"/>
        protected virtual async Task UpdateAsync(TEntity entity)
        {
            EnsureEntityNotNull(entity);
            try
            {
                var existingEntity = await FindExistingEntity(entity.Id);
                var modifiedEntity = UpdateExistingEntity(existingEntity, entity);
                await _repository.UpdateAsync(modifiedEntity);
            }
            catch (Exception ex) when (!(ex is EntityNotFoundException))
            {
                throw new DomainServiceException(GetExceptionMessage("update entity"), ex);
            }
        }

        /// <summary>
        /// Асинхронно удалает из коллекции сущностей типа <typeparamref name="TEntity"/> сущность с заданным 
        /// значением идентификатора типа <typeparamref name="TKey"/>.
        /// </summary>
        /// <param name="entityId">Значение идентификатора типа <typeparamref name="TKey"/> удаляемой сущности 
        /// типа <typeparamref name="TEntity"/>.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="EntityNotFoundException"/>
        /// <exception cref="DomainServiceException"/>
        protected virtual async Task DeleteAsync(TKey entityId)
        {
            try
            {
                var existing = await _repository.GetByIdAsync(entityId);
                EnsureEntityExists(existing, entityId);
                await _repository.DeleteAsync(existing);
            }
            catch (Exception ex) when (!(ex is EntityNotFoundException))
            {
                throw new DomainServiceException(GetExceptionMessage("delete entity"), ex);
            }
        }

        /// <summary>
        /// Заменяет данные существующей сущности типа <typeparamref name="TEntity"/> новыми данными. Абстрактный метод. Должен быть
        /// переопределён в каждом производном классе.
        /// </summary>
        /// <param name="existingEntity">Существующая сущность типа <typeparamref name="TEntity"/>.</param>
        /// <param name="newData">Новые данные для обновления существующей сущности типа <typeparamref name="TEntity"/>.</param>
        /// <returns>Существующая сущность типа <typeparamref name="TEntity"/> с обновлёнными значениями данных.</returns>
        protected abstract TEntity UpdateExistingEntity(TEntity existingEntity, TEntity newData);

        /// <summary>
        /// Ищет в коллекции сущностей типа <typeparamref name="TEntity"/> сущность с заданным значением идентификатора 
        /// типа <typeparamref name="TKey"/>.
        /// </summary>
        /// <param name="entityId">Значение идентификатора типа <typeparamref name="TKey"/> искомой сущности 
        /// типа <typeparamref name="TEntity"/>.</param>
        /// <returns>Существующая сущность типа <typeparamref name="TEntity"/>.</returns>
        protected virtual async Task<TEntity> FindExistingEntity(TKey entityId)
        {
            var existing = await _repository.GetByIdAsync(entityId);
            EnsureEntityExists(existing, entityId);
            return existing;
        }

        private void EnsureEntityNotNull(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
        }

        private void EnsureNoDuplicateEntity(TEntity entity)
        {
            if (entity != null)
            {
                throw new DuplicateEntityException($"The entity of type '{typeof(TEntity)}' with key value '{entity.Id}' " +
                    $"for '{nameof(BaseEntity<TKey>.Id)}' is already exists. If you want to update it, use 'Update' method");
            }
        }

        protected void EnsureEntityExists(TEntity entity, TKey entityId, [CallerMemberName]string methodName = null)
        {
            if (entity == null)
            {
                if (!string.IsNullOrWhiteSpace(methodName) && methodName.ToUpper().Contains("FIND"))
                {
                    throw new EntityNotFoundException($"The entity of type '{typeof(TEntity)}' with key value '{entityId}' " +
                        $"for '{nameof(BaseEntity<TKey>.Id)}' that you trying to update doesn't exist. " +
                        $"To add new entity, use 'Add' method.");
                }
                else if (!string.IsNullOrWhiteSpace(methodName) && methodName.ToUpper().Contains("DELETE"))
                {
                    throw new EntityNotFoundException($"The entity type '{typeof(TEntity)}' with key value '{entityId}' " +
                        $"for '{nameof(BaseEntity<TKey>.Id)}' not found.");
                }
                else
                {
                    throw new InvalidOperationException("Some necessary parameter values are missed.");
                }
            }
        }

        private void EnsureSpecificationNotNull(ISpecification<TEntity> specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));
        }

        private string GetExceptionMessage(string action)
        {
            return $"Unable to {action} of type '{typeof(TEntity)}'. See inner exception for details.";
        }
    }
}
