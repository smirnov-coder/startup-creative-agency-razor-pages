using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using StartupCreativeAgency.Domain.Abstractions;

namespace StartupCreativeAgency.Domain.Logic.Specifications
{
    /// <summary>
    /// Базовый класс спецификации.
    /// </summary>
    /// <typeparam name="T">Тип данных сущности.</typeparam>
    /// <remarks>Класс позаимствован из обучающего проекта с сайта https://docs.microsoft.com.</remarks>
    public class BaseSpecification<T> : ISpecification<T>
    {
        /// <summary>
        /// Лямбда-выражение, определяющее условие выборки.
        /// </summary>
        public Expression<Func<T, bool>> Criteria { get; private set; }
        
        /// <summary>
        /// Коллекция лямбда-выражений, определяющих включение в результат выборки зависимых сущностей.
        /// </summary>
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        
        /// <summary>
        /// Коллекция имён свойств, определяющих зависимые сущности, которые необходимо включить в результат выборки.
        /// </summary>
        public List<string> IncludeStrings { get; } = new List<string>();
        
        /// <summary>
        /// Лямбда-выражение, определяющее условие сортировки результата выборки по возрастанию.
        /// </summary>
        public Expression<Func<T, object>> OrderBy { get; private set; }
        
        /// <summary>
        /// Лямбда-выражение, определяющее условие сортировки результата выборки по убыванию.
        /// </summary>
        public Expression<Func<T, object>> OrderByDescending { get; private set; }
        
        /// <summary>
        /// Количество сущностей, извлекаемых из результата выборки.
        /// </summary>
        public int Take { get; private set; }
        
        /// <summary>
        /// Количество пропускаемых сущностей от начала результата выборки.
        /// </summary>
        public int Skip { get; private set; }
        
        /// <summary>
        /// Индикатор, показывающий необходимость возврата части резальтата выборки.
        /// </summary>
        public bool IsPagingEnabled { get; private set; } = false;
        
        /// <summary>
        /// Создаёт новый экземпляр спецификации с заданным условием выборки.
        /// </summary>
        /// <param name="criteria">Лямбда-выражение, определяющее условие выборки.</param>
        public BaseSpecification(Expression<Func<T, bool>> criteria) => Criteria = criteria;

        /// <summary>
        /// Добавляет в условие выборки включение зависимых сущностей с помощью лямбда-выражения.
        /// </summary>
        /// <param name="includeExpression">Лямбда-выражение, определяющее зависимые сущности, включаемые в результат выборки.</param>
        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression) => Includes.Add(includeExpression);

        /// <summary>
        /// Добавляет в условие выборки включение зависимых сущностей с помощью имени свойства.
        /// </summary>
        /// <param name="includeString">Имя свойства сущности, ссылающееся на зависимую сущность, которую необходимо включить в 
        /// результат выборки.</param>
        protected virtual void AddInclude(string includeString) => IncludeStrings.Add(includeString);

        /// <summary>
        /// Задаёт условия возврата только части результата выборки.
        /// </summary>
        /// <param name="skip">Количество пропускаемых сущностей от начала результата выборки.</param>
        /// <param name="take">Количество извлекаемых сущностей результата выборки с позиции, определяемой 
        /// параметром <paramref name="skip"/>.</param>
        protected virtual void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }

        /// <summary>
        /// Задаёт условие сортировки результата выборки по возрастанию.
        /// </summary>
        /// <param name="orderByExpression">Лямбда-выражение, определяющее условие сортировки результата выборки по возрастанию.</param>
        protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        /// <summary>
        /// Задаёт условие сортировки результата выборки по убыванию.
        /// </summary>
        /// <param name="orderByDescendingExpression">Лямбда-выражение, определяющее условие сортировки результата выборки 
        /// по убыванию.</param>
        protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }
    }
}
