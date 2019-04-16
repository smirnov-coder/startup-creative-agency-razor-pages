using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StartupCreativeAgency.Domain.Entities
{
    // Рекомендации по сериализации объектов:
    // https://docs.microsoft.com/ru-ru/dotnet/standard/serialization/serialization-guidelines

    /// <summary>
    /// Представляет собой базовую сущность доменной модели.
    /// </summary>
    /// <typeparam name="T">Тип данных идентификатора сущности.</typeparam>
    [Serializable]
    public class BaseEntity<T> : ISerializable
    {
        /// <summary>
        /// Идентификатор сущности. 
        /// </summary>
        public T Id { get; protected set; }
        
        /// <summary>
        /// Дата и время создания сущности.
        /// </summary>
        public DateTime? CreatedOn { get; private set; }

        protected BaseEntity() => CreatedOn = DateTime.Now;

        /// <summary>
        /// Создаёт новый экземпляр объекта базовой сущности доменной модели с заданным идентификатором.
        /// </summary>
        public BaseEntity(T entityId) : this() => Id = entityId;

        #region Serialization
        protected BaseEntity(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            Id = (T)info.GetValue(nameof(Id), typeof(T));
            CreatedOn = (DateTime?)info.GetValue(nameof(CreatedOn), typeof(DateTime?));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            info.AddValue(nameof(Id), Id);
            info.AddValue(nameof(CreatedOn), CreatedOn);
        }
        #endregion
    }

    /// <summary>
    /// Представляет собой сущность доменной модели, содержащую информацию о пользователе, создавшем сущность.
    /// </summary>
    /// <typeparam name="T">Тип данных идентификатора сущностию</typeparam>
    [Serializable]
    public class CreatorEntity<T> : BaseEntity<T>, ISerializable
    {
        // Для работы Lazy Loading навигационные свойства должны быть виртуальными.

        /// <summary>
        /// Пользователь, создавший данный объект сущности.
        /// </summary>
        public virtual DomainUser CreatedBy { get; private set; }

        protected CreatorEntity() : base() { }

        /// <summary>
        /// Создаёт новый экземпляр сущности доменной модели с заданным значением идентификатора и информацией 
        /// о пользователе, создавшем сущность.
        /// </summary>
        /// <param name="entityId">Идентификатор сущности типа <typeparamref name="T"/>.</param>
        /// <param name="createdBy">Пользователь, создавший данный объект сущности.</param>
        public CreatorEntity(T entityId, DomainUser createdBy) : base(entityId) => CreatedBy = createdBy;

        #region Serialization
        protected CreatorEntity(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            CreatedBy = (DomainUser)info.GetValue(nameof(CreatedBy), typeof(DomainUser));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(CreatedBy), CreatedBy);
        }
        #endregion
    }

    /// <summary>
    /// Представляет собой сущность доменной модели, содержащую информацию о последнем изменении данных сущности.
    /// </summary>
    /// <typeparam name="T">Тип данных идентификатора сущности.</typeparam>
    [Serializable]
    public class UpdatableEntity<T> : CreatorEntity<T>, ISerializable
    {
        /// <summary>
        /// Дата и время последнего изменения данных сущности.
        /// </summary>
        public DateTime? LastUpdatedOn { get; set; }
        
        /// <summary>
        /// Пользователь, совершивший последние изменения данных сущности.
        /// </summary>
        public virtual DomainUser LastUpdatedBy { get; set; }

        protected UpdatableEntity() : base() { }

        /// <summary>
        /// Создаёт новый экземпляр сущности доменной модели с заданным значением идентификатора и информацией 
        /// о пользователе, создавшем сущность.
        /// </summary>
        /// <param name="entityId">Идентификатор сущности типа <typeparamref name="T"/>.</param>
        /// <param name="createdBy">Пользователь, создавший данный объект сущности.</param>
        public UpdatableEntity(T entityId, DomainUser createdBy) : base(entityId, createdBy)
        {
            LastUpdatedBy = CreatedBy;
            LastUpdatedOn = CreatedOn;
        }

        #region Serialization
        protected UpdatableEntity(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            LastUpdatedOn = (DateTime?)info.GetValue(nameof(LastUpdatedOn), typeof(DateTime?));
            LastUpdatedBy = (DomainUser)info.GetValue(nameof(LastUpdatedBy), typeof(DomainUser));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(LastUpdatedOn), LastUpdatedOn);
            info.AddValue(nameof(LastUpdatedBy), LastUpdatedBy);
        }
        #endregion
    }
}
