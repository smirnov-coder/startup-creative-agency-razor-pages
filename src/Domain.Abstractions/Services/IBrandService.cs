using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Domain.Abstractions.Services
{
    /// <summary>
    /// Сервис для работы с корпоративными клиентами компании (брендами).
    /// </summary>
    public interface IBrandService
    {
        /// <summary>
        /// Асинхронно возвращает коллекцию всех брендов.
        /// </summary>
        Task<IList<Brand>> GetBrandsAsync();
        
        /// <summary>
        /// Асинхронно извлекает из коллекции бренд с заданным значением идентификатора.
        /// </summary>
        /// <returns>Объект бренда или null, если бренд не найден.</returns>
        /// <param name="brandId">Значение идентификатора бренда.</param>
        Task<Brand> GetBrandAsync(int brandId);
        
        /// <summary>
        /// Асинхронно добавляет новый бренд в коллекцию брендов.
        /// </summary>
        /// <param name="brand">Добавляемый в коллекцию бренд.</param>
        /// <returns>Объект добавленного в коллекцию бренда.</returns>
        Task<Brand> AddBrandAsync(Brand brand);
        
        /// <summary>
        /// Асинхронно обновляет данные бренда.
        /// </summary>
        /// <param name="brand">Обновлённые данные бренда.</param>
        Task UpdateBrandAsync(Brand brand);
        
        /// <summary>
        /// Асинхронно удаляет из коллекции бренд с заданным значением идентификатора.
        /// </summary>
        /// <param name="brandId">Значение идентификатора удаляемого бренда.</param>
        Task DeleteBrandAsync(int brandId);
    }
}
