using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Domain.Abstractions.Services
{
    /// <summary>
    /// Сервис сообщений от пользователей.
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Асинхронно возвращает коллекцию всех сообщений.
        /// </summary>
        Task<IList<Message>> GetMessagesAsync();
        
        /// <summary>
        /// Асихронно извлекает из коллекции сообщение с заданным значением идентификатора.
        /// </summary>
        /// <returns>Объект сообщения от пользователя или null, если сообщение не найдено.</returns>
        /// <param name="messageId">Значение идентификатора сообщения.</param>
        Task<Message> GetMessageAsync(int messageId);
        
        /// <summary>
        /// Асинхронно добавляет в коллекцию новое сообщение.
        /// </summary>
        /// <param name="message">Добавляемое новое сообщение.</param>
        /// <returns>Объект добавленного сообщения.</returns>
        Task<Message> SaveMessageAsync(Message message);
        
        /// <summary>
        /// Асинхронно удаляет из коллекции сообщение с заданным значением идентификатора.
        /// </summary>
        /// <param name="messageId">Значение идентификатора удаляемого сообщения.</param>
        Task DeleteMessageAsync(int messageId);
        
        /// <summary>
        /// Асинхронно обновляет статус прочтения сообщения.
        /// </summary>
        /// <param name="messageId">Значение идентификатора обновляемого сообщения.</param>
        /// <param name="isRead">Новый статус прочтения сообщения. true, если сообщение прочтено; иначе false.</param>
        Task UpdateMessageReadStatusAsync(int messageId, bool isRead);
    }
}
