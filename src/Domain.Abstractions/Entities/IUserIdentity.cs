using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StartupCreativeAgency.Domain.Entities
{
    /// <summary>
    /// Определяет контракт данных идентичности пользователя.
    /// </summary>
    public interface IUserIdentity
    {
        /// <summary>
        /// Идентификационное имя пользователя, с помощью которого пользователь осуществляет вход в систему.
        /// </summary>
        string UserName { get; }
        
        /// <summary>
        /// Адрес электронной почты пользователя. Используется для подтверждения регистрации и сброса пароля.
        /// </summary>
        string Email { get; }
    }
}
