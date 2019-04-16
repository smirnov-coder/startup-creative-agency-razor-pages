using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Domain.Abstractions.Services
{
    /// <summary>
    /// Сервис ведения корпоративного блога.
    /// </summary>
    public interface IBlogService
    {
        /// <summary>
        /// Максимальное значение количества извлекаемых блог постов за раз.
        /// </summary>
        int FetchThreshold { get; set; }
        
        /// <summary>
        /// Асинхронно возвращает часть коллекции блог постов.
        /// </summary>
        /// <param name="skip">Количество пропускаемых блог постов от начала коллекции. Значение по умолчанию 0.</param>
        /// <param name="take">Количество извлекаемых блог постов из коллекции. При значении 0, будут ивзлечены все оставшиеся 
        /// блог посты, начиная с позиции, определяемой параметром <paramref name="skip"/>. Значение по умолчанию 0.</param>
        /// <returns>Коллекция блог постов, в виде объекта, реализующего интерфейс <see cref="IList{BlogPost}"/>.</returns>
        Task<IList<BlogPost>> GetBlogPostsAsync(int skip = 0, int take = 0);
        
        /// <summary>
        /// Асинхронно возвращает блог пост с заданным значением идентификатора.
        /// </summary>
        /// <param name="blogPostId">Значение идентификатора блог поста.</param>
        /// <returns>Объект блог поста или null, если блог пост не найден.</returns>
        Task<BlogPost> GetBlogPostAsync(int blogPostId);
        
        /// <summary>
        /// Асинхронно добавляет новый пост в блог.
        /// </summary>
        /// <param name="blogPost">Добавляемый в коллекцию блог пост.</param>
        /// <returns>Объект добавленного в коллекцию блог поста.</returns>
        Task<BlogPost> AddBlogPostAsync(BlogPost blogPost);
        
        /// <summary>
        /// Асинхронно обновляет данные блог поста.
        /// </summary>
        /// <param name="blogPost">Обновлённые данные блог поста.</param>
        Task UpdateBlogPostAsync(BlogPost blogPost);
        
        /// <summary>
        /// Асинхронно удаляет из блога пост с заданным значением идентификатора.
        /// </summary>
        /// <param name="blogPostId">Значение идентификатора удаляемого блог поста.</param>
        Task DeleteBlogPostAsync(int blogPostId);
    }
}
