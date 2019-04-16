using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Domain.Abstractions.Repositories
{
    /// <summary>
    /// Представляет собой хранилище пользователей доменной модели.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Асинхронно ивлекает из репозитория всеx пользователей.
        /// </summary>
        /// <returns>Коллекция пользователей в виде объекта, реализующего интерфейс <see cref="IList{DomainUser}"/>.</returns>
        Task<IList<DomainUser>> ListAsync();
        
        /// <summary>
        /// Асинхронно извлекает из репозитория всеx пользователей, удовлетворяющих условиям, описанным в спецификации.
        /// </summary>
        /// <param name="specification">Объект спецификации, реулизующий интерфейс <see cref="ISpecification{DomainUser}"/>.</param>
        /// <returns>Коллекция пользователей в виде объекта, реализующего интерфейс <see cref="IList{DomainUser}"/>.</returns>
        Task<IList<DomainUser>> ListAsync(ISpecification<DomainUser> specification);
        
        /// <summary>
        /// Асинхронно извлекает из репозитория пользователя по заданному идентификационному имени пользователя.
        /// </summary>
        /// <param name="userName">Идентификационное имя пользователя.</param>
        /// <returns>Объект пользователя доменной модели или null, если пользователь не найден.</returns>
        Task<DomainUser> GetAsync(string userName);
        
        /// <summary>
        /// Асинхронно извлекает из репозитория пользователя, удовлетворяющего условиям, описанным в спецификации.
        /// </summary>
        /// <param name="specification">Объект спецификации, реулизующий интерфейс <see cref="ISpecification{DomainUser}"/>.</param>
        /// <returns>Объект пользователя доменной модели или null, если пользователь не найден.</returns>
        Task<DomainUser> GetAsync(ISpecification<DomainUser> specification);
        
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
        Task<DomainUser> CreateAsync(string userName, string password, string email, string roleName, DomainUser createdBy);
        
        /// <summary>
        /// Асинхронно обновляет профайл пользователя доменной модели, содержащий личную информацию: имя, фамилию, должность и т.д.
        /// </summary>
        /// <param name="user">Обновлённые данные пользователя доменной модели.</param>
        Task UpdateProfileAsync(DomainUser user);
        
        /// <summary>
        /// Асинхронно удаляет пользователя доменной модели.
        /// </summary>
        /// <param name="user">Удаляемый пользователь доменной модели.</param>
        Task DeleteAsync(DomainUser user);
    }
}
