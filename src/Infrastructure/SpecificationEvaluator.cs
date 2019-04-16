using System.Linq;
using Microsoft.EntityFrameworkCore;
using StartupCreativeAgency.Domain.Abstractions;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Infrastructure
{
    /// <summary>
    /// Представляет собой исполнительный механизм (обработчик) спецификации.
    /// </summary>
    /// <typeparam name="TEntity">Тип данных сущности спецификации.</typeparam>
    /// <typeparam name="TKey">Тип данных идентификатора сущности спецификации.</typeparam>
    /// <remarks>Класс позаимствован из обучающего проекта с сайта https://docs.microsoft.com.</remarks>
    public class SpecificationEvaluator<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        /// <summary>
        /// Формирует объект запроса на основании спецификации для заданного набора сущностей.
        /// </summary>
        /// <param name="entitySet">Коллекция сущностей для обработки.</param>
        /// <param name="specification">Объект спецификации, содержащий условия выборки, сортировки, загрузки 
        /// зависимых сущностей и т.д.</param>
        /// <returns>Объект запроса в виде интерфейсного типа <see cref="IQueryable{TEntity}"/>.</returns>
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> entitySet, ISpecification<TEntity> specification)
        {
            var query = entitySet;

            // Обработка условия выборки.
            if (specification.Criteria != null)
                query = query.Where(specification.Criteria);

            // Включение в результирующий набор зависимых сущностей.
            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));
            query = specification.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

            // Сортировка результирующего набора.
            if (specification.OrderBy != null)
                query = query.OrderBy(specification.OrderBy);
            else if (specification.OrderByDescending != null)
                query = query.OrderByDescending(specification.OrderByDescending);
         
            // Формирование частичного результирующего набора.
            if (specification.IsPagingEnabled)
            {
                query = query.Skip(specification.Skip);
                if (specification.Take > 0)
                    query = query.Take(specification.Take);
            }

            return query;
        }
    }
}
