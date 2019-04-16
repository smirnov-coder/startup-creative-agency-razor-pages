using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Abstractions.Repositories;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Domain.Abstractions.Exceptions;

namespace StartupCreativeAgency.Domain.Logic.Services
{
    /// <summary>
    /// Сервис отзывов индивидуальных клиентов компании.
    /// </summary>
    public class TestimonialService : ServiceBase<Testimonial, int>, ITestimonialService
    {
        /// <summary>
        /// Создаёт новый экземпляр сервиса отзывов индивидуальных клиентов компании.
        /// </summary>
        /// <param name="repository"></param>
        public TestimonialService(IRepository<Testimonial, int> repository) : base(repository) { }

        /// <summary>
        /// Асинхронно возвращает коллекцию всех отзывов.
        /// </summary>
        /// <exception cref="DomainServiceException"/>
        public async Task<IList<Testimonial>> GetTestimonialsAsync() => await base.ListAsync();

        /// <summary>
        /// Асинхронно извлекает из коллекции отзывов отзыв с заданным значением идентификатора.
        /// </summary>
        /// <param name="testimonialId">Значение идентификатора отзыва.</param>
        /// <returns>Объект отзыва или null, если отзыв не найден.</returns>
        /// <exception cref="DomainServiceException"/>
        public async Task<Testimonial> GetTestimonialAsync(int testimonialId) => await base.GetByIdAsync(testimonialId);

        /// <summary>
        /// Асинхронно добавляет в коллекцию отзывов новый отзыв.
        /// </summary>
        /// <param name="testimonial">Добавляемый в коллекцию отзыв.</param>
        /// <returns>Объект добавленного отзыва.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DuplicateEntityException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task<Testimonial> AddTestimonialAsync(Testimonial testimonial) => await base.AddAsync(testimonial);

        /// <summary>
        /// Асинхронно обновляет данные отзыва.
        /// </summary>
        /// <param name="testimonial">Обновлённые данные отзыва.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="EntityNotFoundException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task UpdateTestimonialAsync(Testimonial testimonial) => await base.UpdateAsync(testimonial);

        /// <summary>
        /// Асинхронно удаляет из коллекции отзывов отзыв с заданным значением идентификатора.
        /// </summary>
        /// <param name="testimonialId">Значение идентификатора удаляемого отзыва.</param>
        /// <exception cref="EntityNotFoundException"/>
        /// <exception cref="DomainServiceException"/>
        public async Task DeleteTestimonialAsync(int testimonialId) => await base.DeleteAsync(testimonialId);

        protected override Testimonial UpdateExistingEntity(Testimonial existingEntity, Testimonial newData)
        {
            existingEntity.Author = newData.Author;
            existingEntity.Company = newData.Company;
            existingEntity.Text = newData.Text;
            existingEntity.LastUpdatedBy = newData.LastUpdatedBy;
            existingEntity.LastUpdatedOn = newData.LastUpdatedOn;
            return existingEntity;
        }
    }
}
