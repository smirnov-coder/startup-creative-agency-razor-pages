using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StartupCreativeAgency.Domain.Entities
{
    /// <summary>
    /// Содержит информацию о корпоративном клиенте компании: название и логотип.
    /// </summary>
    [Serializable]
    public class Brand : UpdatableEntity<int>, ISerializable
    {
        /// <summary>
        /// Название компании-клиента.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Абсолютный или относительный путь к файлу логотипа компании-клиента.
        /// </summary>
        public string ImagePath { get; set; }

        protected Brand() : base() { /* EntityFramework & LazyLoading */ }

        /// <summary>
        /// Создаёт новый экземпляр сущности <see cref="Brand"/> с заданным значением идентификатора и 
        /// информацией о пользователе, создавшем объект.
        /// </summary>
        /// <param name="id">Идентификатор экземпляра сущности.</param>
        /// <param name="createdBy">Пользователь, создавший экземпляр сущности.</param>
        public Brand(int id, DomainUser createdBy) : base(id, createdBy) { }

        /// <summary>
        /// Создаёт новый экземпляр сущности <see cref="Brand"/> со значением идентификатора по умолчанию и 
        /// информацией о пользователе, создавшем объект.
        /// </summary>
        /// <param name="createdBy">Пользователь, создавший экземпляр сущности.</param>
        public Brand(DomainUser createdBy) : this(default(int), createdBy) { }

        #region Serialization
        protected Brand(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            Name = info.GetString(nameof(Name));
            ImagePath = info.GetString(nameof(ImagePath));
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Name), Name);
            info.AddValue(nameof(ImagePath), ImagePath);
        }
        #endregion
    }
}
