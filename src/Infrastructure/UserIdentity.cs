using System;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Identity;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Infrastructure
{
    /// <summary>
    /// Представляет собой идентичность пользователя: часть сущности пользователя доменной модели (<see cref="DomainUser"/>), 
    /// ответственную за аутентификацию.
    /// </summary>
    [Serializable]
    public class UserIdentity : IdentityUser, IUserIdentity, ISerializable
    {
        /// <summary>
        /// Создаёт новый объект идентичности пользователя со значениями полей по умолчанию.
        /// </summary>
        public UserIdentity() { }

        /// <summary>
        /// Создаёт новый объект идентичности пользователя с заданными значениями имени пользователя и адреса электронной почты.
        /// </summary>
        /// <param name="userName">Имя пользователя.</param>
        /// <param name="email">Адрес электронной почты.</param>
        public UserIdentity(string userName, string email)
        {
            UserName = userName;
            Email = email;
        }

        #region Serialization
        protected UserIdentity(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            UserName = info.GetString(nameof(UserName));
            Email = info.GetString(nameof(Email));
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // В приложении используются только свойства UserName и Email (в планах), определённые
            // в интерфейсе IUserIdentity. Поэтому остальные свойства игнорируются.
            info.AddValue(nameof(UserName), UserName);
            info.AddValue(nameof(Email), Email);
        }
        #endregion
    }
}
