using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using StartupCreativeAgency.Domain.Abstractions;
using StartupCreativeAgency.Domain.Abstractions.Exceptions;
using StartupCreativeAgency.Domain.Abstractions.Repositories;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Domain.Logic.Specifications;

namespace StartupCreativeAgency.Infrastructure
{
    /// <summary>
    /// Представляет собой хранилище пользователей доменной модели.
    /// Класс делегирует часть функций обобщённому репозиторию сущностей типа <see cref="DomainUser"/>, а также использует 
    /// <see cref="UserManager{UserIdentity}"/> для хранения идентичностей пользователей.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private IRepository<DomainUser, int> _repository;
        private UserManager<UserIdentity> _userManager;

        /// <summary>
        /// Создаёт новый экземпляр репозитория пользователей доменной модели.
        /// </summary>
        /// <param name="repository">Обобщённый репозиторий сущностей типа <see cref="DomainUser"/>.</param>
        /// <param name="userManager">Репозиторий идентичностей пользователей <see cref="UserIdentity"/> в виде 
        /// объекта <see cref="UserManager{UserIdentity}"/>.</param>
        public UserRepository(IRepository<DomainUser, int> repository, UserManager<UserIdentity> userManager) 
        {
            _userManager = userManager;
            _repository = repository;
        }

        /// <summary>
        /// Асинхронно ивлекает из репозитория всеx пользователей.
        /// </summary>
        /// <returns>Коллекция пользователей в виде объекта, реализующего интерфейс <see cref="IList{DomainUser}"/>.</returns>
        /// <exception cref="DataAccessException"/>
        public async Task<IList<DomainUser>> ListAsync() => await ListAsync(new BaseSpecification<DomainUser>(x => true));

        /// <summary>
        /// Асинхронно извлекает из репозитория всеx пользователей, удовлетворяющих условиям, описанным в спецификации.
        /// </summary>
        /// <param name="specification">Объект спецификации, реулизующий интерфейс <see cref="ISpecification{DomainUser}"/>.</param>
        /// <returns>Коллекция пользователей в виде объекта, реализующего интерфейс <see cref="IList{DomainUser}"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DataAccessException"/> 
        public async Task<IList<DomainUser>> ListAsync(ISpecification<DomainUser> specification)
        {
            return await _repository.ListAsync(specification);
        }

        /// <summary>
        /// Асинхронно извлекает из репозитория пользователя по заданному идентификационному имени пользователя.
        /// </summary>
        /// <param name="userName">Идентификационное имя пользователя.</param>
        /// <returns>Объект пользователя доменной модели или null, если пользователь не найден.</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="DataAccessException"/> 
        public async Task<DomainUser> GetAsync(string userName)
        {
            EnsureStringArgumentNotNullOrEmpty(userName, nameof(userName));
            return await GetAsync(new BaseSpecification<DomainUser>(x => x.Identity.UserName == userName));
        }

        /// <summary>
        /// Асинхронно извлекает из репозитория пользователя, удовлетворяющего условиям, описанным в спецификации.
        /// </summary>
        /// <param name="specification">Объект спецификации, реулизующий интерфейс <see cref="ISpecification{DomainUser}"/>.</param>
        /// <returns>Объект пользователя доменной модели или null, если пользователь не найден.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DataAccessException"/>
        public async Task<DomainUser> GetAsync(ISpecification<DomainUser> specification)
        {
            return await _repository.GetBySpecificationAsync(specification);
        }

        /// <summary>
        /// Асинхронно создаёт нового пользователя и сохраняет его в репозитории пользователей.
        /// </summary>
        /// <param name="userName">Идентификационное имя пользователя.</param>
        /// <param name="password">Пароль пользователя для входа в систему.</param>
        /// <param name="email">Адрес электронной почты пользователя. Используется для подтверждения регистрации и 
        /// сброса пароля.</param>
        /// <param name="roleName">Роль пользователя в системе. Используется механизмом авторизации для доступа к 
        /// ресурсам приложения.</param>
        /// <param name="createdBy">Существующий пользователь, зарегистрировавший нового пользователя. Может быть пустым.</param>
        /// <returns>Объект созданного пользователя доменной модели.</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="DataAccessException"/>
        public async Task<DomainUser> CreateAsync(string userName, string password, string email, string roleName, DomainUser createdBy)
        {
            EnsureStringArgumentNotNullOrEmpty(userName, nameof(userName));
            EnsureStringArgumentNotNullOrEmpty(password, nameof(password));
            EnsureStringArgumentNotNullOrEmpty(email, nameof(email));
            EnsureStringArgumentNotNullOrEmpty(roleName, nameof(roleName));
            try
            {
                IdentityResult result = await _userManager.CreateAsync(new UserIdentity(userName, email), password);
                if (!result.Succeeded)
                {
                    throw new DataAccessException($"An error occurred while creating identity for user '{userName}'. " +
                        $"Inspect '{nameof(DataAccessException.Errors)}' property for details.", GetIdentityErrors(result));
                }
                var userIdentity = await _userManager.FindByNameAsync(userName);
                result = await _userManager.AddToRoleAsync(userIdentity, roleName);
                if (!result.Succeeded)
                {
                    throw new DataAccessException($"An error occurred while adding user '{userName}' to role '{roleName}'. " +
                        $"Inspect '{nameof(DataAccessException.Errors)}' property for details.", GetIdentityErrors(result));
                }
                var user = new DomainUser(userIdentity, createdBy);
                return await _repository.AddAsync(user);
            }
            catch (Exception ex) when (!(ex is DataAccessException))
            {
                throw new DataAccessException(GetExceptionMessage("create entity"), ex);
            }
        }

        /// <summary>
        /// Асинхронно обновляет профайл пользователя доменной модели, содержащий личную информацию: имя, фамилию, должность и т.д.
        /// </summary>
        /// <param name="user">Обновлённые данные пользователя доменной модели.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DataAccessException"/>
        public async Task UpdateProfileAsync(DomainUser user) => await _repository.UpdateAsync(user);

        /// <summary>
        /// Асинхронно удаляет пользователя доменной модели.
        /// </summary>
        /// <param name="user">Удаляемый пользователь доменной модели.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DataAccessException"/>
        public async Task DeleteAsync(DomainUser user)
        {
            EnsureUserNotNull(user);
            try
            {
                // DbContext настроен на каскадное удаление сущностей UserIdentity->DomainUser->UserProfile->SocialLink.
                var result = await _userManager.DeleteAsync(user.Identity as UserIdentity);
                if (!result.Succeeded)
                {
                    throw new DataAccessException($"An error occurred while deleting identity for user '{user.Identity.UserName}'. " +
                        $"Inspect '{nameof(DataAccessException.Errors)}' property for details.", GetIdentityErrors(result));
                }
            }
            catch (Exception ex) when (!(ex is DataAccessException))
            {
                throw new DataAccessException(GetExceptionMessage("delete entity"), ex);
            }
        }

        private void EnsureStringArgumentNotNullOrEmpty(string argument, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(argument))
                throw new ArgumentException("Argument value cannot be null or empty string.", argumentName);
        }

        private void EnsureUserNotNull(DomainUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
        }

        private string GetExceptionMessage(string action)
        {
            return $"Unable to {action} of type '{typeof(DomainUser)}'. See inner exception for details.";
        }

        private IEnumerable<string> GetIdentityErrors(IdentityResult identityResult)
        {
            return identityResult.Errors.Select(x => x.Description);
        }
    }
}
