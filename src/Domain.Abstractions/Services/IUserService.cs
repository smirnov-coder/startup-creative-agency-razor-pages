using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Domain.Abstractions.Services
{
    /// <summary>
    /// Сервис пользователей доменной модели.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Асинхронно возвращает коллекцию всех пользователей доменной модели.
        /// </summary>
        Task<IList<DomainUser>> GetUsersAsync();
        
        /// <summary>
        /// Асинхронно возвращает коллекцию пользователей доменной модели, помеченных для отображения в пользовательском интерфейсе
        /// в качестве членов команды компании.
        /// </summary>
        Task<IList<DomainUser>> GetDisplayedTeamMembersAsync();
        
        /// <summary>
        /// Асинхронно извлекает из коллекции пользователя доменной модели с заданным идентификационным именем.
        /// </summary>
        /// <returns>Объект пользователя доменной модели или null, если пользователь не найден.</returns>
        /// <param name="userName">Идентификационное имя пользователя доменной модели.</param>
        Task<DomainUser> GetUserAsync(string userName);
        
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
        Task<DomainUser> CreateUserAsync(string userName, string password, string email, string roleName, DomainUser createdBy = null);
        
        /// <summary>
        /// Асинхронно обновляет основную личную информацию пользователя доменной модели.
        /// </summary>
        /// <param name="userName">Идентификационное имя пользователя доменной модели, чьи данные подлежат обновлению.</param>
        /// <param name="firstName">Новое имя пользователя. Может быть пустым.</param>
        /// <param name="lastName">Новая фамилия пользователя. Может быть пустым.</param>
        /// <param name="jobPosition">Новая должность, занимаемая пользователем в компании. Может быть пустым.</param>
        /// <param name="photoFilePath">Новый абсолютный или относительный путь к фотографии пользователя. Может быть пустым.</param>
        /// <returns>Объект с обновлёнными данными пользователя доменной модели.</returns>
        Task<DomainUser> UpdateUserPersonalInfoAsync(string userName, string firstName, string lastName, 
            string jobPosition, string photoFilePath);
        
        /// <summary>
        /// Асинхронно добавляет новые или обновляет существующие ссылки на страницы пользователя доменной модели в социальных сетях.
        /// </summary>
        /// <param name="userName">Идентификационное имя пользователя доменной модели, чьи данные подлежат обновлению.</param>
        /// <param name="socialLinks">Одна или несколько ссылок на страницы пользователя доменной модели в социальных сетях.</param>
        /// <returns>Объект с обновлёнными данными пользователя доменной модели.</returns>
        Task<DomainUser> UpdateUserSocialLinksAsync(string userName, params SocialLink[] socialLinks);
        
        /// <summary>
        /// Асинхронно обновляет статус отображения пользователя доменной модели в пользовательском интерфейсе в качестве 
        /// члена команды компании.
        /// </summary>
        /// <param name="userName">Идентификационное имя пользователя доменной модели, чьи данные подлежат обновлению.</param>
        /// <param name="shouldBeDisplayed">Новый статус отображения. true, если пользователь должен отображаться в пользовательском 
        /// интерфейсе в качестве члена команды компании; иначе false.</param>
        /// <returns>Объект с обновлёнными данными пользователя доменной модели.</returns>
        Task<DomainUser> UpdateUserDisplayStatusAsync(string userName, bool shouldBeDisplayed);
        
        /// <summary>
        /// Асинхронно удаляет пользователя доменной модели с заданным идентификационным именем.
        /// </summary>
        /// <param name="userName">Идентификационное имя удаляемого пользователя доменной модели.</param>
        Task DeleteUserAsync(string userName);
    }
}
