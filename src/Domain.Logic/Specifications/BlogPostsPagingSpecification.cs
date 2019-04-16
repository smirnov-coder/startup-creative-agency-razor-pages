using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Domain.Logic.Specifications
{
    /// <summary>
    /// Спецификация частичной коллекции блог постов, отсортированной по убыванию значений даты и времени создания блог постов.
    /// </summary>
    public class BlogPostsPagingSpecification : BaseSpecification<BlogPost>
    {
        /// <summary>
        /// Создаёт новый экземпляр спецификации с заданным значением пропуска выборки и количества извлекаемых блог постов.
        /// </summary>
        /// <param name="skip">Количество пропускаемых блог постов от начала результата выборки.</param>
        /// <param name="take">Количество извлекаемых из результата выборки блог постов, начиная с позиции, определяемой
        /// параметром <paramref name="skip"/>.</param>
        public BlogPostsPagingSpecification(int skip, int take) : base(x => true)
        {
            EnsureIntValueInRange(skip, nameof(skip));
            EnsureIntValueInRange(take, nameof(take));

            ApplyOrderByDescending(x => x.CreatedOn);
            ApplyPaging(skip, take);
        }

        private void EnsureIntValueInRange(int value, string paramName)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("Value cannot be less than 0.", paramName);
        }
    }
}
