using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupCreativeAgency.Domain.Abstractions.Services
{
    /// <summary>
    /// Сервис для работы с текстовыми и графическими файлами.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Асинхронно возвращает содержимое файла в виде строки символов в кодировке <see cref="Encoding.UTF8"/>.
        /// </summary>
        /// <param name="fileName">Полное имя файла, включая расширение.</param>
        Task<string> GetFileContentAsStringAsync(string fileName);
        
        /// <summary>
        /// Асинхронно сохраняет данные изображения в файл.
        /// </summary>
        /// <param name="fileName">Полное имя файла для сохранения, включая расширение.</param>
        /// <param name="imageStream">Потоковые данные изображения в виде объекта <see cref="Stream"/>. 
        /// Поток должен быть открыт.</param>
        /// <returns>Относительный путь к файлу сохранённого изображения.</returns>
        Task<string> SaveImageAsync(string fileName, Stream imageStream);
        
        /// <summary>
        /// Асинхронно сохраняет текстовые данные в файл.
        /// </summary>
        /// <param name="fileName">Полное имя файла для сохранения, включая расширение.</param>
        /// <param name="textData">Текстовые данные для сохранения.</param>
        Task SaveTextDataAsync(string fileName, string textData);
        
        /// <summary>
        /// Генерирует уникальное имя файла с заданным расширением.
        /// </summary>
        /// <param name="extension">Расширение файла, включая символ точки.</param>
        /// <param name="prefix">Префикс имени файла. Опциональный параметр.</param>
        string GenerateUniqueFileName(string extension, string prefix = null);
    }
}
