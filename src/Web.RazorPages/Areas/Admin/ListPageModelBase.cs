using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Infrastructure;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin
{
    /// <summary>
    /// Абстрактный базовый класс страницы отображения коллекции сущностей типа <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">Тип данных сущности.</typeparam>
    /// <typeparam name="TKey">Тип данных идентификатора сущности.</typeparam>
    public abstract class ListPageModelBase<TEntity, TKey> : PageModel where TEntity : BaseEntity<TKey>
    {
        /// <summary>
        /// Адрес страницы, которую необходимо отобразить после выполнения операции.
        /// </summary>
        public string RedirectUrl { get; protected set; }
        
        /// <summary>
        /// Модель представления, используемая страницей.
        /// </summary>
        public IList<TEntity> Model { get; set; }

        public virtual async Task OnGetAsync() => Model = await PerformGetManyAsync();

        /// <summary>
        /// Асинхронно выполняет операцию извлечения коллекции сущностей типа <typeparamref name="TEntity"/> из доменной модели.
        /// Абстрактный метод Должен быть переопределён в каждом производном классе.
        /// </summary>
        protected abstract Task<IList<TEntity>> PerformGetManyAsync();

        public virtual async Task<IActionResult> OnPostDeleteAsync(TKey id)
        {
            if (ModelState.IsValid)
            {
                await PerformDeleteAsync(id);
                this.SetDetails(OperationStatus.Success, $"The entity of type '{typeof(TEntity)}' with key value '{id}' for " +
                    $"'{nameof(BaseEntity<TKey>.Id)}' deleted successfully.");
            }
            else
            {
                this.SetDetails(OperationStatus.Error, $"Unable to delete entity of type '{typeof(TEntity)}' with key value '{id}' " +
                    $"for '{nameof(BaseEntity<TKey>.Id)}'. Reload the page and try again.");
            }
            return RedirectToPage(RedirectUrl);
        }

        /// <summary>
        /// Асинхронно выполняет удаление сущности с заданным значением идентификатора типа <typeparamref name="TKey"/> 
        /// из коллекции сущностей типа <typeparamref name="TEntity"/> доменной модели.
        /// </summary>
        /// <param name="entityId">Значение идентификатора типа <typeparamref name="TKey"/> сущности типа 
        /// <typeparamref name="TEntity"/>.</param>
        protected abstract Task PerformDeleteAsync(TKey entityId);
    }
}
