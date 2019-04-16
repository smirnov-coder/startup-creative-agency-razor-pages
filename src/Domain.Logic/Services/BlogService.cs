using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Abstractions.Repositories;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Domain.Logic.Specifications;
using StartupCreativeAgency.Domain.Abstractions.Exceptions;

namespace StartupCreativeAgency.Domain.Logic.Services
{
    /// <summary>
    /// Сервис ведения блога.
    /// </summary>
    public class BlogService : ServiceBase<BlogPost, int>, IBlogService
    {
        private int _fetchThreshold = 0;

        /// <summary>
        /// Максимальное значение количества извлекаемых блог постов за раз. Значение по умолчанию 0, что соответствует
        /// извлечению всех блог постов.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public int FetchThreshold
        {
            get { return _fetchThreshold; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be less than 0.");
                _fetchThreshold = value;
            }
        }

        /// <summary>
        /// Создаёт новый экземпляр сервиса ведения блога.
        /// </summary>
        /// <param name="repository">Репозиторий для хранения блог постов.</param>
        public BlogService(IRepository<BlogPost, int> repository) : base(repository) { }

        /// <summary>
        /// Асинхронно возвращает часть коллекции блог постов.
        /// </summary>
        /// <param name="skip">Количество пропускаемых блог постов от начала коллекции. Значение по умолчанию 0.</param>
        /// <param name="take">Количество извлекаемых блог постов из коллекции. При значении 0, будут ивзлечены все оставшиеся 
        /// блог посты, начиная с позиции, определяемой параметром <paramref name="skip"/>. Значение по умолчанию 0.</param>
        /// <returns>Коллекция блог постов, отсортированных по убыванию значений даты и времени создания блог постов, в виде объекта, 
        /// реализующего интерфейс <see cref="IList{BlogPost}"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task<IList<BlogPost>> GetBlogPostsAsync(int skip = 0, int take = 0)
        {
            if (take < 0)
                throw new ArgumentOutOfRangeException(nameof(take), take, "Number of fetching blog posts cannot be less than 0.");
            if (skip < 0)
                throw new ArgumentOutOfRangeException(nameof(skip), skip, "Number of skipped blog posts cannot be less than 0.");
            int fetchAmount;
            if (take == 0 && skip == 0)
                fetchAmount = FetchThreshold;
            else
                fetchAmount = take > FetchThreshold && FetchThreshold > 0 ? FetchThreshold : take;
            return await base.ListAsync(new BlogPostsPagingSpecification(skip, fetchAmount));
        }

        /// <summary>
        /// Асинхронно возвращает блог пост с заданным значением идентификатора.
        /// </summary>
        /// <param name="blogPostId">Значение идентификатора блог поста.</param>
        /// <returns>Объект блог поста или null, если блог пост не найден.</returns>
        /// <exception cref="DomainServiceException"/>
        public async Task<BlogPost> GetBlogPostAsync(int blogPostId) => await base.GetByIdAsync(blogPostId);

        /// <summary>
        /// Асинхронно добавляет новый пост в блог.
        /// </summary>
        /// <param name="blogPost">Добавляемый в коллекцию блог пост.</param>
        /// <returns>Объект добавленного в коллекцию блог поста.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DuplicateEntityException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task<BlogPost> AddBlogPostAsync(BlogPost blogPost) => await base.AddAsync(blogPost);

        /// <summary>
        /// Асинхронно обновляет данные блог поста.
        /// </summary>
        /// <param name="blogPost">Обновлённые данные блог поста.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="EntityNotFoundException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task UpdateBlogPostAsync(BlogPost blogPost) => await base.UpdateAsync(blogPost);

        /// <summary>
        /// Асинхронно удаляет из блога пост с заданным значением идентификатора.
        /// </summary>
        /// <param name="blogPostId">Значение идентификатора удаляемого блог поста.</param>
        /// <exception cref="EntityNotFoundException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task DeleteBlogPostAsync(int blogPostId) => await base.DeleteAsync(blogPostId);

        protected override BlogPost UpdateExistingEntity(BlogPost existingEntity, BlogPost newData)
        {
            existingEntity.Title = newData.Title;
            existingEntity.ImagePath = newData.ImagePath;
            existingEntity.Category = newData.Category;
            existingEntity.Content = newData.Content;
            existingEntity.LastUpdatedBy = newData.LastUpdatedBy;
            existingEntity.LastUpdatedOn = newData.LastUpdatedOn;
            return existingEntity;
        }
    }
}
