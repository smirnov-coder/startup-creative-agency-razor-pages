using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StartupCreativeAgency.Domain.Entities
{
    /// <summary>
    /// Сообщение от пользователя.
    /// </summary>
    [Serializable]
    public class Message : BaseEntity<int>, ISerializable
    {
        /// <summary>
        /// Имя отправителя сообщения.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Адрес электронной почты отправителя для обратной связи.
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Тема сообщения.
        /// </summary>
        public string Subject { get; set; }
        
        /// <summary>
        /// Название компании отправителя сообщения.
        /// </summary>
        public string Company { get; set; }
        
        /// <summary>
        /// Текст сообщения.
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// IP-адрес, с которого было отправлено сообщение (на всякий случай, для целей безопасности,
        /// например, для блокировки при DDOS-атаке).
        /// </summary>
        public string IPAddress { get; set; }
        
        /// <summary>
        /// Показывает, является ли сообщение прочитанным.
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Создаёт новый экземпляр сущности <see cref="Message"/> со значением идентификатора по умолчанию.
        /// </summary>
        public Message() : base() { }

        #region Serialization
        protected Message(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            Name = info.GetString(nameof(Name));
            Email = info.GetString(nameof(Email));
            Subject = info.GetString(nameof(Subject));
            Company = info.GetString(nameof(Company));
            Text = info.GetString(nameof(Text));
            IPAddress = info.GetString(nameof(IPAddress));
            IsRead = info.GetBoolean(nameof(IsRead));
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Name), Name);
            info.AddValue(nameof(Email), Email);
            info.AddValue(nameof(Subject), Subject);
            info.AddValue(nameof(Company), Company);
            info.AddValue(nameof(Text), Text);
            info.AddValue(nameof(IPAddress), IPAddress);
            info.AddValue(nameof(IsRead), IsRead);
        }
        #endregion
    }
}
