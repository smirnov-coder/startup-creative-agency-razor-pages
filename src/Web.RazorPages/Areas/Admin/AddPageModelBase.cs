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
    /// Абстрактный базовый класс страницы добавления новой сущности типа <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TModel">Тип данных модели представления, используемой страницей.</typeparam>
    /// <typeparam name="TEntity">Тип данных добавляемой сущности.</typeparam>
    /// <typeparam name="TKey">Тип данных идентификатора добавляемой сущности.</typeparam>
    public abstract class AddPageModelBase<TModel, TEntity, TKey> : PageModel
        where TModel : class, new()
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
        [BindProperty] public TModel Model { get; set; }

        public AddPageModelBase(IUserService userService) => _userService = userService;

        public void OnGet() => Model = new TModel();

        public virtual async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();
            var user = await _userService.GetUserAsync(User?.Identity?.Name);
            var entity = await CreateEntityFromModelAsync(Model, user);
            var result = await PerformAddAsync(entity);
            this.SetDetails(OperationStatus.Success, $"The entity of type '{typeof(TEntity)}' with key value '{result.Id}' for " +
                $"'{nameof(BaseEntity<TKey>.Id)}' saved successfully.");
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
        /// Асинхронно выполняет операцию добавления сущности в коллекцию сущностей типа <typeparamref name="TEntity"/> 
        /// доменной модели. Абстрактный метод. Должен быть переопределён в каждом производном классе.
        /// </summary>
        /// <param name="entity">Добавляемая в коллекцию сущность типа <typeparamref name="TEntity"/>.</param>
        /// <returns>Объект добавленной сущности типа <typeparamref name="TEntity"/>.</returns>
        protected abstract Task<TEntity> PerformAddAsync(TEntity entity);
    }
}
