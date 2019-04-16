using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Domain.Abstractions.Services
{
    /// <summary>
    /// Сервис отзывов индивидуальных клиентов компании.
    /// </summary>
    public interface ITestimonialService
    {
        /// <summary>
        /// Асинхронно возвращает коллекцию всех отзывов.
        /// </summary>
        Task<IList<Testimonial>> GetTestimonialsAsync();
        
        /// <summary>
        /// Асинхронно извлекает из коллекции отзывов отзыв с заданным значением идентификатора.
        /// </summary>
        /// <returns>Объект отзыва или null, если отзыв не найден.</returns>
        /// <param name="testimonialId">Значение идентификатора отзыва.</param>
        Task<Testimonial> GetTestimonialAsync(int testimonialId);
        
        /// <summary>
        /// Асинхронно добавляет в коллекцию отзывов новый отзыв.
        /// </summary>
        /// <param name="testimonial">Добавляемый в коллекцию отзыв.</param>
        /// <returns>Объект добавленного отзыва.</returns>
        Task<Testimonial> AddTestimonialAsync(Testimonial testimonial);
        
        /// <summary>
        /// Асинхронно обновляет данные отзыва.
        /// </summary>
        /// <param name="testimonial">Обновлённые данные отзыва.</param>
        Task UpdateTestimonialAsync(Testimonial testimonial);
        
        /// <summary>
        /// Асинхронно удаляет из коллекции отзывов отзыв с заданным значением идентификатора.
        /// </summary>
        /// <param name="testimonialId">Значение идентификатора удаляемого отзыва.</param>
        Task DeleteTestimonialAsync(int testimonialId);
    }
}
