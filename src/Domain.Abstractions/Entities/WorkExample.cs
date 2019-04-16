using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StartupCreativeAgency.Domain.Entities
{
    /// <summary>
    /// Представляет собой пример выполненного проекта.
    /// </summary>
    [Serializable]
    public class WorkExample : UpdatableEntity<int>, ISerializable
    {
        /// <summary>
        /// Название проекта.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Категория проекта.
        /// </summary>
        public string Category { get; set; }
        
        /// <summary>
        /// Описание проекта.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Абсолютный или относительный путь к файлу иллюстрации проекта.
        /// </summary>
        public string ImagePath { get; set; }

        protected WorkExample() : base() { /* EntityFramework & LazyLoading */ }

        /// <summary>
        /// Создаёт новый экземпляр сущности <see cref="WorkExample"/> с заданным значением идентификатора и 
        /// информацией о пользователе, создавшем объект.
        /// </summary>
        /// <param name="id">Идентификатор экземпляра сущности.</param>
        /// <param name="createdBy">Пользователь, создавший экземпляр сущности.</param>
        public WorkExample(int id, DomainUser createdBy) : base(id, createdBy) { }

        /// <summary>
        /// Создаёт новый экземпляр сущности <see cref="WorkExample"/> со значением идентификатора по умолчанию и 
        /// информацией о пользователе, создавшем объект.
        /// </summary>
        /// <param name="createdBy">Пользователь, создавший экземпляр сущности.</param>
        public WorkExample(DomainUser createdBy) : base(default(int), createdBy) { }

        #region Serialization
        protected WorkExample(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            Name = info.GetString(nameof(Name));
            Category = info.GetString(nameof(Category));
            Description = info.GetString(nameof(Description));
            ImagePath = info.GetString(nameof(ImagePath));
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Name), Name);
            info.AddValue(nameof(Category), Category);
            info.AddValue(nameof(Description), Description);
            info.AddValue(nameof(ImagePath), ImagePath);
        }
        #endregion
    }
}
