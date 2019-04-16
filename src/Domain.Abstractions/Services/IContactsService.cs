using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Entities;


namespace StartupCreativeAgency.Domain.Abstractions.Services
{
    /// <summary>
    /// Сервис для работы с контактными данными компании.
    /// </summary>
    public interface IContactsService
    {
        /// <summary>
        /// Асинхронно возвращает коллекцию контактных данных компании.
        /// </summary>
        Task<IList<ContactInfo>> GetContactsAsync();
        
        /// <summary>
        /// Асинхронно сохраняет контактные данные компании.
        /// </summary>
        /// <param name="contacts">Коллекция контактных данных компании.</param>
        Task SaveContactsAsync(IEnumerable<ContactInfo> contacts);
        
        /// <summary>
        /// Асинхронно возвращает коллекцию ссылок на страницы компании в социальных сетях.
        /// </summary>
        Task<IDictionary<string, string>> GetSocialLinksAsync();
        
        /// <summary>
        /// Асинхронно сохраняет ссылки на страницы компании в социальных сетях.
        /// </summary>
        /// <param name="socialLinks">Коллекция ссылок на страницы компании в социальных сетях.</param>
        Task SaveSocialLinksAsync(IDictionary<string, string> socialLinks);
    }
}
