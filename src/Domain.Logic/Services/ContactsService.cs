using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StartupCreativeAgency.Domain.Abstractions.Exceptions;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Domain.Logic.Services
{
    /// <summary>
    /// Сервис для работы с контактными данными компании.
    /// </summary>
    public class ContactsService : IContactsService
    {
        private IFileService _fileService;

        /// <summary>
        /// Полное имя файла, включая расширение, для хранения контактных данных компании. 
        /// Значение по умолчанию <c>contacts.json</c>.
        /// </summary>
        public string ContactsFileName { get; set; }  = "contacts.json";
        
        /// <summary>
        /// Полное имя файла, включая расширение, для хранения ссылок на страницы компании в социальных сетях. 
        /// Значение по умолчанию <c>social-links.json</c>.
        /// </summary>
        public string SocialLinksFileName { get; set; } = "social-links.json";

        /// <summary>
        /// Создаёт новый экземпляр сервиса для работы с контактными данными компании.
        /// </summary>
        /// <param name="fileService">Сервис для работы с файлами.</param>
        public ContactsService(IFileService fileService) => _fileService = fileService;

        #region Contacts
        /// <summary>
        /// Асинхронно возвращает коллекцию контактных данных компании.
        /// </summary>
        /// <exception cref="DomainServiceException"/>
        public async Task<IList<ContactInfo>> GetContactsAsync()
        {
            return await PerformGetAsync<List<ContactInfo>>(DataName.Contacts);
        }

        /// <summary>
        /// Асинхронно сохраняет контактные данные компании.
        /// </summary>
        /// <param name="contacts">Коллекция контактных данных компании.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task SaveContactsAsync(IEnumerable<ContactInfo> contacts)
        {
            await PerformSaveAsync(contacts, DataName.Contacts, nameof(contacts));
        }
        #endregion

        #region Social Links
        /// <summary>
        /// Асинхронно возвращает коллекцию ссылок на страницы компании в социальных сетях.
        /// </summary>
        /// <exception cref="DomainServiceException"/>
        public async Task<IDictionary<string, string>> GetSocialLinksAsync()
        {
            return await PerformGetAsync<Dictionary<string, string>>(DataName.SocialLinks);
        }

        /// <summary>
        /// Асинхронно сохраняет ссылки на страницы компании в социальных сетях.
        /// </summary>
        /// <param name="socialLinks">Коллекция ссылок на страницы компании в социальных сетях.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task SaveSocialLinksAsync(IDictionary<string, string> socialLinks)
        {
            await PerformSaveAsync(socialLinks, DataName.SocialLinks, nameof(socialLinks));
        }
        #endregion

        private async Task PerformSaveAsync(object data, DataName dataName, string argumentName)
        {
            if (data == null)
                throw new ArgumentNullException(argumentName);
            string action = string.Empty;
            try
            {
                string fileName = string.Empty;
                switch (dataName)
                {
                    case DataName.Contacts:
                        fileName = ContactsFileName;
                        action = "save contacts";
                        break;

                    case DataName.SocialLinks:
                        fileName = SocialLinksFileName;
                        action = "save social links";
                        break;
                }
                await _fileService.SaveTextDataAsync(fileName, JsonConvert.SerializeObject(data));
            }
            catch (Exception ex)
            {
                throw new DomainServiceException(GetExceptionMessage(action), ex);
            }
        }

        private async Task<T> PerformGetAsync<T>(DataName dataName)
        {
            string action = string.Empty;
            try
            {
                string fileName = string.Empty;
                switch (dataName)
                {
                    case DataName.Contacts:
                        fileName = ContactsFileName;
                        action = "get contacts";
                        break;

                    case DataName.SocialLinks:
                        fileName = SocialLinksFileName;
                        action = "get social links";
                        break;
                }
                string json = await _fileService.GetFileContentAsStringAsync(fileName);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                throw new DomainServiceException(GetExceptionMessage(action), ex);
            }
        }

        private string GetExceptionMessage(string action)
        {
            return $"Unable to {action}. See inner exception for details.";
        }

        private enum DataName
        {
            Contacts, SocialLinks
        }
    }
}
