using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StartupCreativeAgency.Domain.Entities
{
    /// <summary>
    /// Блог пост.
    /// </summary>
    [Serializable]
    public class BlogPost : UpdatableEntity<int>, ISerializable
    {
        /// <summary>
        /// Относительный или абсолютный путь к файлу главного изображения блог поста (обложки).
        /// </summary>
        public string ImagePath { get; set; }
        
        /// <summary>
        /// Заголовок блог поста.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Категория содержимого блог поста.
        /// </summary>
        public string Category { get; set; }
        
        /// <summary>
        /// Содержимое блог поста.
        /// </summary>
        public string Content { get; set; }

        protected BlogPost() : base() { /* EntityFramework & LazyLoading */ }

        /// <summary>
        /// Создаёт новый экземпляр сущности <see cref="BlogPost"/> с заданным значением идентификатора и 
        /// информацией о пользователе, создавшем объект.
        /// </summary>
        /// <param name="id">Идентификатор экземпляра сущности.</param>
        /// <param name="createdBy">Пользователь, создавший экземпляр сущности.</param>
        public BlogPost(int id, DomainUser createdBy) : base(id, createdBy) { }

        /// <summary>
        /// Создаёт новый экземпляр сущности <see cref="BlogPost"/> со значением идентификатора по умолчанию и 
        /// информацией о пользователе, создавшем объект.
        /// </summary>
        /// <param name="createdBy">Пользователь, создавший экземпляр сущности.</param>
        public BlogPost(DomainUser createdBy) : this(default(int), createdBy) { }

        #region Serialization
        protected BlogPost(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            ImagePath = info.GetString(nameof(ImagePath));
            Title = info.GetString(nameof(Title));
            Category = info.GetString(nameof(Category));
            Content = info.GetString(nameof(Content));
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ImagePath), ImagePath);
            info.AddValue(nameof(Title), Title);
            info.AddValue(nameof(Category), Category);
            info.AddValue(nameof(Content), Content);
        }
        #endregion
    }
}
