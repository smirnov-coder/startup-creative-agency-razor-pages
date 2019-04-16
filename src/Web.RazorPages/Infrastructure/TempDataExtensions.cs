using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace StartupCreativeAgency.Web.RazorPages.Infrastructure
{
    public static class TempDataExtensions
    {
        /// <summary>
        /// Добавляет в коллекцию <see cref="ITempDataDictionary"/> новый объект типа <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Тип данных добавляемого объекта.</typeparam>
        /// <param name="key">Значение ключа, по которому будет доступен объект в коллекции <see cref="ITempDataDictionary"/>.</param>
        /// <param name="value">Объект типа <typeparamref name="T"/>, добавляемый в коллекцию <see cref="ITempDataDictionary"/>.</param>
        public static void Set<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// Извлекает из коллекции <see cref="ITempDataDictionary"/> объект типа <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Тип данных извлекаемого объекта.</typeparam>
        /// <param name="key">Значение ключа в коллекции <see cref="ITempDataDictionary"/>.</param>
        /// <returns>Объект типа <typeparamref name="T"/>.</returns>
        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            tempData.TryGetValue(key, out object obj);
            return obj == null ? null : JsonConvert.DeserializeObject<T>((string)obj);
        }
    }
}
