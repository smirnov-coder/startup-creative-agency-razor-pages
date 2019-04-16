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
    /// Содержит информацию об услуге, предоставляемой компанией.
    /// </summary>
    [Serializable]
    public class ServiceInfo : UpdatableEntity<int>, ISerializable
    {
        /// <summary>
        /// Слоган услуги. Используется для отображения в пользовательском интерфейсе.
        /// </summary>
        public string Caption { get; set; }
        
        /// <summary>
        /// Описание услуги.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Класс иконки услуги. Используется для отображения в пользовательском интерфейсе.
        /// </summary>
        public string IconClass { get; set; }

        protected ServiceInfo() : base() { /* EntityFramework & LazyLoading */ }

        /// <summary>
        /// Создаёт новый экземпляр сущности <see cref="ServiceInfo"/> с заданным значением идентификатора и 
        /// информацией о пользователе, создавшем объект.
        /// </summary>
        /// <param name="id">Идентификатор экземпляра сущности.</param>
        /// <param name="createdBy">Пользователь, создавший экземпляр сущности.</param>
        public ServiceInfo(int id, DomainUser createdBy) : base(id, createdBy) { }

        /// <summary>
        /// Создаёт новый экземпляр сущности <see cref="ServiceInfo"/> со значением идентификатора по умолчанию и 
        /// информацией о пользователе, создавшем объект.
        /// </summary>
        /// <param name="createdBy">Пользователь, создавший экземпляр сущности.</param>
        public ServiceInfo(DomainUser createdBy) : this(default(int), createdBy) { }

        #region Serialization
        protected ServiceInfo(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            Caption = info.GetString(nameof(Caption));
            Description = info.GetString(nameof(Description));
            IconClass = info.GetString(nameof(IconClass));
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Caption), Caption);
            info.AddValue(nameof(Description), Description);
            info.AddValue(nameof(IconClass), IconClass);
        }
        #endregion
    }
}
