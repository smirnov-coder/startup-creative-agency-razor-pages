using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Abstractions.Repositories;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Abstractions.Exceptions;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Domain.Logic.Services
{
    /// <summary>
    /// Сервис сообщений от пользователей.
    /// </summary>
    public class MessageService : ServiceBase<Message, int>, IMessageService
    {
        private IRepository<Message, int> _repository;

        /// <summary>
        /// Создаёт новый экземпляр сервиса сообщения от пользователей.
        /// </summary>
        /// <param name="repository">Репозиторий для хранения сообщений от пользователей.</param>
        public MessageService(IRepository<Message, int> repository) : base(repository) => _repository = repository;

        /// <summary>
        /// Асинхронно возвращает коллекцию всех сообщений.
        /// </summary>
        /// <exception cref="DomainServiceException"/>
        public async Task<IList<Message>> GetMessagesAsync() => await base.ListAsync();

        /// <summary>
        /// Асихронно извлекает из коллекции сообщение с заданным значением идентификатора.
        /// </summary>
        /// <param name="messageId">Значение идентификатора сообщения.</param>
        /// <returns>Объект сообщения от пользователя или null, если сообщение не найдено.</returns>
        /// <exception cref="DomainServiceException"/>
        public async Task<Message> GetMessageAsync(int messageId) => await base.GetByIdAsync(messageId);

        /// <summary>
        /// Асинхронно добавляет в коллекцию новое сообщение.
        /// </summary>
        /// <param name="message">Добавляемое новое сообщение.</param>
        /// <returns>Объект добавленного сообщения.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="EntityNotFoundException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task<Message> SaveMessageAsync(Message message) => await base.AddAsync(message);

        /// <summary>
        /// Асинхронно удаляет из коллекции сообщение с заданным значением идентификатора.
        /// </summary>
        /// <param name="messageId">Значение идентификатора удаляемого сообщения.</param>
        /// <exception cref="EntityNotFoundException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task DeleteMessageAsync(int messageId) => await base.DeleteAsync(messageId);

        /// <summary>
        /// Асинхронно обновляет статус прочтения сообщения.
        /// </summary>
        /// <param name="messageId">Значение идентификатора обновляемого сообщения.</param>
        /// <param name="isRead">Новый статус прочтения сообщения. true, если сообщение прочтено; иначе false.</param>
        /// <exception cref="EntityNotFoundException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task UpdateMessageReadStatusAsync(int messageId, bool isRead)
        {
            var message = await GetByIdAsync(messageId);
            EnsureEntityExists(message, messageId);
            message.IsRead = isRead;
            await UpdateAsync(message);
        }

        protected override Message UpdateExistingEntity(Message existingEntity, Message newData)
        {
            existingEntity.Name = newData.Name;
            existingEntity.Company = newData.Company;
            existingEntity.Email = newData.Email;
            existingEntity.Subject = newData.Subject;
            existingEntity.Text = newData.Text;
            existingEntity.IPAddress = newData.IPAddress;
            existingEntity.IsRead = newData.IsRead;
            return existingEntity;
        }
    }
}
