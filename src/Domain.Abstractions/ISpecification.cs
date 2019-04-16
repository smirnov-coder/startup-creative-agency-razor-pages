using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StartupCreativeAgency.Domain.Abstractions
{
    /// <summary>
    /// Представляет собой подробное описание условий выборки сущности(-ей) из набора сущностей.
    /// </summary>
    /// <typeparam name="T">Тип данных сущности.</typeparam>
    /// <remarks>Класс позаимствован из обучающего проекта с сайта https://docs.microsoft.com.</remarks>
    public interface ISpecification<T>
    {
        /// <summary>
        /// Лямбда-выражение, определяющее условие выборки.
        /// </summary>
        Expression<Func<T, bool>> Criteria { get; }
        
        /// <summary>
        /// Коллекция лямбда-выражений, определяющих включение в результат выборки зависимых сущностей.
        /// </summary>
        List<Expression<Func<T, object>>> Includes { get; }
        
        /// <summary>
        /// Коллекция имён свойств, определяющих зависимые сущности, которые необходимо включить в результат выборки.
        /// </summary>
        List<string> IncludeStrings { get; }
        
        /// <summary>
        /// Лямбда-выражение, определяющее условие сортировки результата выборки по возрастанию.
        /// </summary>
        Expression<Func<T, object>> OrderBy { get; }
        
        /// <summary>
        /// Лямбда-выражение, определяющее условие сортировки результата выборки по убыванию.
        /// </summary>
        Expression<Func<T, object>> OrderByDescending { get; }
        
        /// <summary>
        /// Количество сущностей, извлекаемых из результата выборки.
        /// </summary>
        int Take { get; }
        
        /// <summary>
        /// Количество пропускаемых сущностей от начала результата выборки.
        /// </summary>
        int Skip { get; }
        
        /// <summary>
        /// Индикатор, определяющий необходимость возврата только части результата выборки.
        /// </summary>
        bool IsPagingEnabled { get; }
    }
}
