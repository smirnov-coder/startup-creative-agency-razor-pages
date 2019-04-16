using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Domain.Abstractions.Services
{
    /// <summary>
    /// Сервис услуг, предоставляемых компанией.
    /// </summary>
    public interface IServiceInfoService
    {
        /// <summary>
        /// Асинхронно возвращает коллекцию описаний всех услуг компании.
        /// </summary>
        Task<IList<ServiceInfo>> GetServiceInfosAsync();
        
        /// <summary>
        /// Асинхронно извлекает из коллекции услуг описание услуги с заданным значением идентификатора.
        /// </summary>
        /// <returns>Объект описания услуги компании или null, если услуга не найдена.</returns>
        /// <param name="serviceInfoId">Значение идентификатора услуги компании.</param>
        Task<ServiceInfo> GetServiceInfoAsync(int serviceInfoId);
        
        /// <summary>
        /// Асинхронно добавлет в коллекцию услуг компании описание новой услуги.
        /// </summary>
        /// <param name="serviceInfo">Добавляемое описание услуги компании.</param>
        /// <returns>Объект добавленной услуги компании.</returns>
        Task<ServiceInfo> AddServiceInfoAsync(ServiceInfo serviceInfo);
        
        /// <summary>
        /// Асинхронно обновляет описание услуги компании.
        /// </summary>
        /// <param name="serviceInfo">Обновлённые данные услуги компании.</param>
        Task UpdateServiceInfoAsync(ServiceInfo serviceInfo);
        
        /// <summary>
        /// Асинхронно удаляет из коллекции услуг услугу с заданным значением идентификатора.
        /// </summary>
        /// <param name="serviceInfoId">Значение идентификатора удаляемой услуги компании.</param>
        Task DeleteServiceInfoAsync(int serviceInfoId);
    }
}
