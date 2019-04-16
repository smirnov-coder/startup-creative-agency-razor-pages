using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StartupCreativeAgency.Domain.Entities
{
    /// <summary>
    /// Пользователь доменной модели. Инкапсулирует в себе идентичность пользователя, используюмую для аутентификации,
    /// и личную информацию.
    /// </summary>
    [Serializable]
    public class DomainUser : UpdatableEntity<int>, ISerializable
    {
        // Поле хранит конкретный тип используемой реализации интерфейса IUserIdentity.
        // Необходимо для сериализации/десериализации.
        private Type _identityType;

        /// <summary>
        /// Идентичность пользователя.
        /// </summary>
        public virtual IUserIdentity Identity { get; private set; }
        
        /// <summary>
        /// Профайл пользователя с личной информацией.
        /// </summary>
        public virtual UserProfile Profile { get; private set; }
        
        /// <summary>
        /// Пользователь, совершивший последние изменения в данных сущности. Т.к. только пользователь может изменить
        /// свои данные и во избежание цикличных ссылок при сериализации/десериализации, свойство всегда возвращает null.
        /// </summary>
        public override DomainUser LastUpdatedBy
        {
            get => null;
            set { }
        }

        protected DomainUser() : base() { /* Для EntityFramework и LazyLoading */ }

        /// TODO: Дополнить комментарии.

        public DomainUser(int id, IUserIdentity identity, UserProfile profile, DomainUser createdBy) : base(id, createdBy)
        {
            Identity = identity;
            Profile = profile ?? new UserProfile();
            if (Profile.SocialLinks.Count == 0)
            {
                Profile.AddSocialLinks(new SocialLink[]
                {
                    new SocialLink("Facebook", string.Empty),
                    new SocialLink("Twitter", string.Empty),
                    new SocialLink("GooglePlus", string.Empty),
                    new SocialLink("Linkedin", string.Empty)
                });
            }
            Profile.UserInfoUpdated += (sender, e) => LastUpdatedOn = DateTime.Now;
        }

        public DomainUser(int id, IUserIdentity identity, UserProfile profile)
            : this(id, identity, profile, null) { }

        public DomainUser(IUserIdentity identity, UserProfile profile, DomainUser createdBy)
            : this(default(int), identity, profile, createdBy) { }

        public DomainUser(IUserIdentity identity, UserProfile profile)
            : this(default(int), identity, profile, null) { }

        public DomainUser(IUserIdentity identity, DomainUser createdBy)
            : this(default(int), identity, null, createdBy) { }

        public DomainUser(IUserIdentity identity)
            : this(default(int), identity, null, null) { }

        #region Serialization
        protected DomainUser(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            _identityType = (Type)info.GetValue(nameof(_identityType), typeof(Type));
            Identity = (IUserIdentity)info.GetValue(nameof(Identity), _identityType);
            Profile = (UserProfile)info.GetValue(nameof(Profile), typeof(UserProfile));
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // При работе с сущностями могут использоваться прокси-объекты (в данном случае EntityFrameworkCore.Proxies).
            // Поэтому для получения информации о реальном типе замещаемого объекта надо обратиться к базовому классу,
            // используемому прокси-объектом, т.е. к свойству BaseType.
            info.AddValue(nameof(_identityType), Identity.GetType().BaseType);
            info.AddValue(nameof(Identity), Identity, Identity.GetType().BaseType);
            info.AddValue(nameof(Profile), Profile);
        }
        #endregion
    }
}
