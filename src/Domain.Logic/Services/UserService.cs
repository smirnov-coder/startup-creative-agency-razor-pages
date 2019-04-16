using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Abstractions;
using StartupCreativeAgency.Domain.Abstractions.Exceptions;
using StartupCreativeAgency.Domain.Abstractions.Repositories;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Domain.Logic.Specifications;

namespace StartupCreativeAgency.Domain.Logic.Services
{
    /// <summary>
    /// Сервис пользователей доменной модели.
    /// </summary>
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;

        /// <summary>
        /// Создаёт новый экземпляр сервиса пользователей доменной модели.
        /// </summary>
        /// <param name="userRepository">Репозиторий для хранения пользователей доменной модели.</param>
        public UserService(IUserRepository userRepository) => _userRepository = userRepository;

        /// <summary>
        /// Асинхронно возвращает коллекцию всех пользователей доменной модели.
        /// </summary>
        /// <exception cref="DomainServiceException"/>
        public async Task<IList<DomainUser>> GetUsersAsync()
        {
            return await PerformGetManyAsync(new BaseSpecification<DomainUser>(x => true));
        }

        /// <summary>
        /// Асинхронно возвращает коллекцию пользователей доменной модели, помеченных для отображения в пользовательском интерфейсе
        /// в качестве членов команды компании.
        /// </summary>
        /// <exception cref="DomainServiceException"/>
        public async Task<IList<DomainUser>> GetDisplayedTeamMembersAsync()
        {
            return await PerformGetManyAsync(new TeamMemberSpecification());
        }

        private async Task<IList<DomainUser>> PerformGetManyAsync(ISpecification<DomainUser> specification)
        {
            try
            {
                return await _userRepository.ListAsync(specification);
            }
            catch (Exception ex)
            {
                throw new DomainServiceException(GetExceptionMessage("get entities"), ex);
            }
        }

        /// <summary>
        /// Асинхронно извлекает из коллекции пользователя доменной модели с заданным идентификационным именем.
        /// </summary>
        /// <param name="userName">Идентификационное имя пользователя доменной модели.</param>
        /// <returns>Объект пользователя доменной модели или null, если пользователь не найден.</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task<DomainUser> GetUserAsync(string userName)
        {
            EnsureStringArgumentNotNullOrEmpty(userName, nameof(userName));
            try
            {
                return await _userRepository.GetAsync(userName);
            }
            catch (Exception ex)
            {
                throw new DomainServiceException(GetExceptionMessage("get entity"), ex);
            }
        }

        /// <summary>
        /// Асинхронно создаёт нового пользователя доменной модели.
        /// </summary>
        /// <param name="userName">Идентификационное имя создаваемого пользователя доменной модели.</param>
        /// <param name="password">Пароль для аутентификации.</param>
        /// <param name="email">Адрес электронной почты пользователя.</param>
        /// <param name="roleName">Роль пользователя для целей авторизации.</param>
        /// <param name="createdBy">Пользователь доменной модели, который зарегистрировал данного пользователя. 
        /// Опциональный параметр.</param>
        /// <returns>Объект созданного пользователя доменной модели.</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="DuplicateEntityException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task<DomainUser> CreateUserAsync(string userName, string password, string email, string roleName, 
            DomainUser createdBy = null)
        {
            EnsureStringArgumentNotNullOrEmpty(userName, nameof(userName));
            EnsureStringArgumentNotNullOrEmpty(password, nameof(password));
            EnsureStringArgumentNotNullOrEmpty(email, nameof(email));
            EnsureStringArgumentNotNullOrEmpty(roleName, nameof(roleName));
            try
            {
                var user = await GetUserAsync(userName);
                if (user != null)
                {
                    throw new DuplicateEntityException($"The entity of type '{typeof(DomainUser)}' with value '{userName}' " +
                        $"for '{nameof(DomainUser.Identity.UserName)}' already exists.");
                }
                return await _userRepository.CreateAsync(userName, password, email, roleName, createdBy);
            }
            catch (Exception ex) when (!(ex is DuplicateEntityException))
            {
                throw new DomainServiceException(GetExceptionMessage("create entity"), ex);
            }
        }

        /// <summary>
        /// Асинхронно обновляет основную личную информацию пользователя доменной модели.
        /// </summary>
        /// <param name="userName">Идентификационное имя пользователя доменной модели, чьи данные подлежат обновлению.</param>
        /// <param name="firstName">Новое имя пользователя. Может быть пустым.</param>
        /// <param name="lastName">Новая фамилия пользователя. Может быть пустым.</param>
        /// <param name="jobPosition">Новая должность, занимаемая пользователем в компании. Может быть пустым.</param>
        /// <param name="photoFilePath">Новый абсолютный или относительный путь к фотографии пользователя. Может быть пустым.</param>
        /// <returns>Объект с обновлёнными данными пользователя доменной модели.</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="EntityNotFoundException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task<DomainUser> UpdateUserPersonalInfoAsync(string userName, string firstName, string lastName, 
            string jobPosition, string photoFilePath)
        {
            return await PerformUpdateAsync(userName, user => 
                user.Profile.UpdatePersonalInfo(firstName, lastName, jobPosition, photoFilePath));
        }

        /// <summary>
        /// Асинхронно добавляет новые или обновляет существующие ссылки на страницы пользователя доменной модели в социальных сетях.
        /// </summary>
        /// <param name="userName">Идентификационное имя пользователя доменной модели, чьи данные подлежат обновлению.</param>
        /// <param name="socialLinks">Одна или несколько ссылок на страницы пользователя доменной модели в социальных сетях.</param>
        /// <returns>Объект с обновлёнными данными пользователя доменной модели.</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="EntityNotFoundException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task<DomainUser> UpdateUserSocialLinksAsync(string userName, params SocialLink[] socialLinks)
        {
            return await PerformUpdateAsync(userName, user => user.Profile.AddSocialLinks(socialLinks));
        }

        /// <summary>
        /// Асинхронно обновляет статус отображения пользователя доменной модели в пользовательском интерфейсе в качестве 
        /// члена команды компании.
        /// </summary>
        /// <param name="userName">Идентификационное имя пользователя доменной модели, чьи данные подлежат обновлению.</param>
        /// <param name="shouldBeDisplayed">Новый статус отображения. true, если пользователь должен отображаться в пользовательском 
        /// интерфейсе в качестве члена команды компании; иначе false.</param>
        /// <returns>Объект с обновлёнными данными пользователя доменной модели.</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="EntityNotFoundException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task<DomainUser> UpdateUserDisplayStatusAsync(string userName, bool shouldBeDisplayed)
        {
            return await PerformUpdateAsync(userName, user => user.Profile.ChangeDisplayStatus(shouldBeDisplayed));
        }

        private async Task<DomainUser> PerformUpdateAsync(string userName, Action<DomainUser> userInteraction)
        {
            EnsureStringArgumentNotNullOrEmpty(userName, nameof(userName));
            try
            {
                var user = await GetUserAsync(userName);
                EnsureUserExists(user, userName);
                userInteraction.Invoke(user);
                await _userRepository.UpdateProfileAsync(user);
                return user;
            }
            catch (Exception ex) when (!(ex is EntityNotFoundException))
            {
                throw new DomainServiceException(GetExceptionMessage("update entity"), ex);
            }
        }

        /// <summary>
        /// Асинхронно удаляет пользователя доменной модели с заданным идентификационным именем.
        /// </summary>
        /// <param name="userName">Идентификационное имя удаляемого пользователя доменной модели.</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="EntityNotFoundException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task DeleteUserAsync(string userName)
        {
            EnsureStringArgumentNotNullOrEmpty(userName, nameof(userName));
            try
            {
                var user = await GetUserAsync(userName);
                EnsureUserExists(user, userName);
                await _userRepository.DeleteAsync(user);
            }
            catch (Exception ex) when (!(ex is EntityNotFoundException))
            {
                throw new DomainServiceException(GetExceptionMessage("delete entity"), ex);
            }
        }

        private void EnsureStringArgumentNotNullOrEmpty(string argument, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(argument))
                throw new ArgumentException("Argument value cannot be null or empty string.", argumentName);
        }

        private void EnsureUserExists(DomainUser user, string userName, [CallerMemberName]string methodName = null)
        {
            if (user == null)
            {
                if (!string.IsNullOrWhiteSpace(methodName) && methodName.ToUpper().Contains("UPDATE"))
                {
                    throw new EntityNotFoundException($"The entity of type '{typeof(DomainUser)}' with value '{userName}' " +
                        $"for '{nameof(DomainUser.Identity.UserName)}' that you trying to update doesn't exist.");
                }
                else if (!string.IsNullOrWhiteSpace(methodName) && (methodName.ToUpper().Contains("DELETE")))
                {
                    throw new EntityNotFoundException($"The entity of type '{typeof(DomainUser)}' with value '{userName}' " +
                        $"for '{nameof(DomainUser.Identity.UserName)}' not found.");
                }
                else
                {
                    throw new InvalidOperationException("Some necessary parameter values are missed.");
                }
            }
        }

        private string GetExceptionMessage(string action)
        {
            return $"Unable to {action} of type '{typeof(DomainUser)}'. See inner exception for details.";
        }
    }
}
