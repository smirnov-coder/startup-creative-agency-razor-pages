using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Web.RazorPages.Controllers.Api
{
    /// <summary>
    /// Абстрактный базовый класс API-контроллера. Предоставляет реализацию логики, общей для других API-контроллеров.
    /// </summary>
    /// <typeparam name="TEntity">Тип данных сущности, обслуживаемой контроллером.</typeparam>
    /// <typeparam name="TKey">Тип данных идентификатора сушности.</typeparam>
    [Route("api/[controller]")]
    public abstract class ApiControllerBase<TEntity, TKey> : ControllerBase where TEntity : BaseEntity<TKey>
    {
        //
        // GET api/[controller]/5
        //
        [HttpGet("{id}")]
        public async Task<ActionResult<TEntity>> GetByIdAsync(TKey id)
        {
            var entity = await PerformGetAsync(id);
            if (entity == null)
            {
                return NotFound(HttpUtility.HtmlEncode($"The entity of type '{typeof(TEntity)}' with key value '{id}' " +
                    $"for '{nameof(BaseEntity<TKey>.Id)}' not found."));
            }
            PrepareEntityForReturn(entity);
            return entity;
        }

        /// <summary>
        /// Выполняет операцию извлечения экземпляра сущности типа <typeparamref name="TEntity"/> по заданному значению 
        /// идентификатора типа <typeparamref name="TKey"/>. Абстрактный метод. Должен быть переопределён в каждом производном классе.
        /// </summary>
        /// <param name="id">Значение идентификатора типа <typeparamref name="TKey"/> сущности типа <typeparamref name="TEntity"/>.
        /// </param>
        /// <returns>Объект сущности типа <typeparamref name="TEntity"/>.</returns>
        protected abstract Task<TEntity> PerformGetAsync(TKey id);

        /// <summary>
        /// Выполняет дополнительную обработку сущности типа <typeparamref name="TEntity"/> перед возвратом вызывающей стороне. 
        /// Виртуальный метод. Может быть переопределён в производном классе. Базовая реализация не делает ничего.
        /// </summary>
        /// <param name="entity">Объект сущности типа <typeparamref name="TEntity"/>.</param>
        protected virtual void PrepareEntityForReturn(TEntity entity) { }
    }
}
