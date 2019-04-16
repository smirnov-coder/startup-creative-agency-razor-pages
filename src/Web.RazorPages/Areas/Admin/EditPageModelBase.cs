using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Infrastructure;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin
{
    /// <summary>
    /// Абстрактный базовый класс страницы обновления данных сущности типа <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TModel">Тип данных модели представления, используемой страницей.</typeparam>
    /// <typeparam name="TEntity">Тип данных обновляемой сущности.</typeparam>
    /// <typeparam name="TKey">Тип данных идентификатора обновляемой сущности.</typeparam>
    public abstract class EditPageModelBase<TModel, TEntity, TKey> : PageModel
        where TModel : class
        where TEntity : BaseEntity<TKey>
    {
        private readonly IUserService _userService;
        
        /// <summary>
        /// Адрес страницы, которую необходимо отобразить после успешного выполнения операции.
        /// </summary>
        public string RedirectUrl { get; protected set; }
        
        /// <summary>
        /// Модель представления, используемая страницей.
        /// </summary>
        [BindProperty]
        public TModel Model { get; set; }

        public EditPageModelBase(IUserService userService) => _userService = userService;

        public async Task<IActionResult> OnGetAsync(TKey id)
        {
            var entity = await PerformGetSingleAsync(id);
            if (entity == null)
                return NotFound();
            Model = CreateModelFromEntity(entity);
            return Page();
        }

        /// <summary>
        /// Асинхронно выполняет операцию извлечения сущности типа <typeparamref name="TEntity"/> из коллекции сущностей доменной 
        /// модели по заданному значению идентификатора типа <typeparamref name="TKey"/>. Абстрактный метод. Должен быть переопределён 
        /// в каждом производном классе.
        /// </summary>
        /// <param name="entityId">Значение идентификатора типа <typeparamref name="TKey"/> извлекаемой сущности 
        /// типа <typeparamref name="TEntity"/>.</param>
        /// <returns>Объект сущности типа <typeparamref name="TEntity"/> или null, если сущность не найдена.</returns>
        protected abstract Task<TEntity> PerformGetSingleAsync(TKey entityId);

        /// <summary>
        /// Создаёт модель представления типа <typeparamref name="TModel"/> на основании данных сущности типа 
        /// <typeparamref name="TEntity"/>. 
        /// Абстрактный метод. Должен быть переопределён в каждом производном классе.
        /// </summary>
        /// <param name="entity">Данные сущности типа <typeparamref name="TEntity"/>.</param>
        /// <returns>Объект созданной модели представления типа <typeparamref name="TModel"/>.</returns>
        protected abstract TModel CreateModelFromEntity(TEntity entity);

        public virtual async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();
            var user = await _userService.GetUserAsync(User?.Identity?.Name);
            var entity = await CreateEntityFromModelAsync(Model, user);
            await PerformUpdateAsync(entity);
            this.SetDetails(OperationStatus.Success, $"The entity of type '{typeof(TEntity)}' with key value '{entity.Id}' for " +
                $"'{nameof(BaseEntity<TKey>.Id)}' updated successfully.");
            return RedirectToPage(RedirectUrl);
        }

        /// <summary>
        /// Асинхронно создаёт новый экземпляр сущности типа <typeparamref name="TEntity"/> на основании данных модели представления 
        /// типа <typeparamref name="TModel"/>. Абстрактный метод. Должен быть переопределён в каждом производном классе.
        /// </summary>
        /// <param name="model">Данные модели представления типа <typeparamref name="TModel"/>.</param>
        /// <param name="creator">Пользователь доменной модели, создающий новую сущность типа <typeparamref name="TEntity"/>.</param>
        /// <returns>Объект созданной сущности типа <typeparamref name="TEntity"/>.</returns>
        protected abstract Task<TEntity> CreateEntityFromModelAsync(TModel model, DomainUser creator);

        /// <summary>
        /// Асинхронно выполняет операцию обновления данных сущности в коллекции сущностей типа <typeparamref name="TEntity"/> 
        /// доменной модели. Абстрактный метод. Должен быть переопределён в каждом производном классе.
        /// </summary>
        /// <param name="entity">Обновлённые данные сущности типа <typeparamref name="TEntity"/>.</param>
        protected abstract Task PerformUpdateAsync(TEntity entity);
    }
}
