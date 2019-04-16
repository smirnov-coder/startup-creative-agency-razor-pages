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
    /// Отзыв индивидуального клиента о компании.
    /// </summary>
    [Serializable]
    public class Testimonial : UpdatableEntity<int>, ISerializable
    {
        /// <summary>
        /// Автор отзыва.
        /// </summary>
        public string Author { get; set; }
        
        /// <summary>
        /// Название компании автора отзыва.
        /// </summary>
        public string Company { get; set; }
        
        /// <summary>
        /// Текст отзыва.
        /// </summary>
        public string Text { get; set; }

        protected Testimonial() : base() { /* EntityFramework & LazyLoading */ }

        /// <summary>
        /// Создаёт новый экземпляр сущности <see cref="Testimonial"/> с заданным значением идентификатора и 
        /// информацией о пользователе, создавшем объект.
        /// </summary>
        /// <param name="id">Идентификатор экземпляра сущности.</param>
        /// <param name="createdBy">Пользователь, создавший экземпляр сущности.</param>
        public Testimonial(int id, DomainUser createdBy) : base(id, createdBy) { }

        /// <summary>
        /// Создаёт новый экземпляр сущности <see cref="Testimonial"/> со значением идентификатора по умолчанию и 
        /// информацией о пользователе, создавшем объект.
        /// </summary>
        /// <param name="createdBy">Пользователь, создавший экземпляр сущности.</param>
        public Testimonial(DomainUser createdBy) : this(default(int), createdBy) { }

        #region Serialization
        protected Testimonial(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            Author = info.GetString(nameof(Author));
            Company = info.GetString(nameof(Company));
            Text = info.GetString(nameof(Text));
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Author), Author);
            info.AddValue(nameof(Company), Company);
            info.AddValue(nameof(Text), Text);
        }
        #endregion
    }
}
