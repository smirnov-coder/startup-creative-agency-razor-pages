using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace StartupCreativeAgency.Domain.Entities
{
    /// <summary>
    /// Профайл пользователя доменной модели. Содержит личную информацию пользователя.
    /// </summary>
    [Serializable]
    public class UserProfile : BaseEntity<int>, ISerializable
    {
        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string FirstName { get; private set; }
        
        /// <summary>
        /// Фамилия пользователя.
        /// </summary>
        public string LastName { get; private set; }
        
        /// <summary>
        /// Абсолютный или относительный путь к файлу фаотографии пользователя.
        /// </summary>
        public string PhotoFilePath { get; private set; }
        
        /// <summary>
        /// Занимаемая пользователем должность в компании.
        /// </summary>
        public string JobPosition { get; private set; }
        
        /// <summary>
        /// Показывает готовность профайла для отображения данных пользователя в пользовательском интерфейсе.
        /// Для отображения должна быть предоставлена следующая информация: имя, фамилия, должность, фотография,
        /// 4 не пустые ссылки на страницы с соц. сетях Facebook, Twitter, Google+, Linkedin.
        /// </summary>
        public bool IsReadyForDisplay { get; private set; }
        
        /// <summary>
        /// Показывает, отображаются ли данные пользователя в пользовательском интерфейсе (в качестве члена команды).
        /// </summary>
        public bool DisplayAsTeamMember { get; private set; }

        private List<SocialLink> _socialLinks = new List<SocialLink>();
        
        /// <summary>
        /// Коллекция ссылок на страницы пользователя в социальных сетях.
        /// </summary>
        public virtual IReadOnlyList<SocialLink> SocialLinks => _socialLinks.AsReadOnly();
        
        /// <summary>
        /// Событие, возникающее при изменении данных профайла пользователя.
        /// </summary>
        public event EventHandler<EventArgs> UserInfoUpdated;

        /// <summary>
        /// Создаёт новый экземпляр сущности <see cref="UserProfile"/> со значениями свойств и идентификатора по умолчанию.
        /// </summary>
        public UserProfile() : base() { }

        /// <summary>
        /// Создаёт новый экземпляр сущности <see cref="UserProfile"/> со значением идентификатора по умолчанию и
        /// заданными значениями свойств.
        /// </summary>
        /// <param name="firstName">Имя пользователя. Может быть пустой строкой.</param>
        /// <param name="lastName">Фамилия пользователя. Может быть пустой строкой.</param>
        /// <param name="jobPosition">Занимаемая пользователем должность в компании. Может быть пустой строкой.</param>
        /// <param name="photoFilePath">Абсолютный или относительный путь к файлу фотографии пользователя. Может быть пустой строкой.</param>
        /// <param name="socialLinks">Ноль, одна или несколько ссылок на страницы в социальных сетях.</param>
        public UserProfile(string firstName, string lastName, string jobPosition, string photoFilePath, 
            params SocialLink[] socialLinks) : this()
        {
            UpdatePersonalInfo(firstName, lastName, jobPosition, photoFilePath);
            AddSocialLinks(socialLinks);
        }

        #region Serialization
        protected UserProfile(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            FirstName = info.GetString(nameof(FirstName));
            LastName = info.GetString(nameof(LastName));
            PhotoFilePath = info.GetString(nameof(PhotoFilePath));
            JobPosition = info.GetString(nameof(JobPosition));
            IsReadyForDisplay = info.GetBoolean(nameof(IsReadyForDisplay));
            DisplayAsTeamMember = info.GetBoolean(nameof(DisplayAsTeamMember));
            _socialLinks = (List<SocialLink>)info.GetValue(nameof(SocialLinks), typeof(List<SocialLink>));
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(FirstName), FirstName);
            info.AddValue(nameof(LastName), LastName);
            info.AddValue(nameof(PhotoFilePath), PhotoFilePath);
            info.AddValue(nameof(JobPosition), JobPosition);
            info.AddValue(nameof(IsReadyForDisplay), IsReadyForDisplay);
            info.AddValue(nameof(DisplayAsTeamMember), DisplayAsTeamMember);
            info.AddValue(nameof(SocialLinks), _socialLinks);
        }
        #endregion

        protected virtual void RaiseUserInfoUpdated()
        {
            UserInfoUpdated?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Очищает коллекцию ссылок пользователя на страницы в социальных сетях.
        /// </summary>
        public void ClearSocialLinks()
        {
            _socialLinks.Clear();
            IsReadyForDisplay = false;
            DisplayAsTeamMember = false;
            RaiseUserInfoUpdated();
        }

        /// <summary>
        /// Обновляет ссылки на страницы пользователя в социальных сетях. Если информация о странице в социальной сети
        /// отсутствует в коллекции, то она добавляется в конец коллекции; если информация о странице в социальной сети 
        /// присутствует в коллекции, то обновляется значение ссылки на страницу.
        /// </summary>
        /// <param name="socialLinks">Одна или несколько ссылок на страницы в социальных сетях.</param>
        /// <returns>Количество новых добавленных записей в коллекцию ссылок или 0 в случае, если произошло обновление 
        /// существующих записей.</returns>
        public int AddSocialLinks(params SocialLink[] socialLinks)
        {
            if (socialLinks == null)
                throw new ArgumentNullException(nameof(socialLinks));
            int addedCount = 0;
            foreach (var socialLink in socialLinks)
            {
                var existing = SocialLinks.FirstOrDefault(x => x.NetworkName == socialLink.NetworkName);
                if (existing != null)
                {
                    existing.Url = socialLink.Url;
                }
                else
                {
                    _socialLinks.Add(new SocialLink(socialLink.NetworkName, socialLink.Url));
                    addedCount++;
                }
            }
            if (!IsReadyForDisplay)
                ValidateReadyForDisplay();
            RaiseUserInfoUpdated();
            return addedCount;
        }

        /// <summary>
        /// Изменяет индикатор, отвечающий за отображение данных пользователя в пользовательском интерфейсе.
        /// </summary>
        /// <param name="shouldBeDisplayed">Значение индикатора.</param>
        /// <exception cref="InvalidOperationException"/>
        public void ChangeDisplayStatus(bool shouldBeDisplayed)
        {
            if (shouldBeDisplayed)
            {
                if (IsReadyForDisplay)
                {
                    DisplayAsTeamMember = true;
                }
                else
                {
                    throw new InvalidOperationException($"Unable to mark user for display as team member. Not all " +
                        $"required data has beed provided. Please check values for next fields: '{nameof(FirstName)}', " +
                        $"'{nameof(LastName)}', '{nameof(JobPosition)}', '{nameof(PhotoFilePath)}', '{nameof(SocialLinks)}'.");
                }
            }
            else
            {
                DisplayAsTeamMember = false;
            }
            RaiseUserInfoUpdated();
        }

        /// <summary>
        /// Обновляет основную личную информацию пользователя.
        /// </summary>
        /// <param name="firstName">Имя пользователя. Может быть пустой строкой.</param>
        /// <param name="lastName">Фамилия пользователя. Может быть пустой строкой.</param>
        /// <param name="jobPosition">Занимаемая пользователем должность в компании. Может быть пустой строкой.</param>
        /// <param name="photoFilePath">Абсолютный или относительный путь к файлу фотографии пользователя. Может быть пустой строкой.</param>
        public void UpdatePersonalInfo(string firstName, string lastName, string jobPosition, string photoFilePath)
        {
            FirstName = firstName;
            LastName = lastName;
            JobPosition = jobPosition;
            PhotoFilePath = photoFilePath;
            ValidateReadyForDisplay();
            RaiseUserInfoUpdated();
        }

        // Проверяет готовность профайла для отображения данных пользователя в пользовательском интерфейсе.
        private void ValidateReadyForDisplay()
        {
            if (string.IsNullOrWhiteSpace(FirstName)
                || string.IsNullOrWhiteSpace(LastName)
                || string.IsNullOrWhiteSpace(JobPosition)
                || string.IsNullOrWhiteSpace(PhotoFilePath)
                || SocialLinks.Count == 0 
                || SocialLinks.Any(x => string.IsNullOrWhiteSpace(x.Url)))
            {
                IsReadyForDisplay = false;
                DisplayAsTeamMember = false;
            }
            else
            {
                IsReadyForDisplay = true;
            }
        }
    }
}
