using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StartupCreativeAgency.Domain.Abstractions.Repositories
{
    /// <summary>
    /// Репозиторий (хранилище) для хранения текстовых и графических файлов.
    /// </summary>
    public interface IFileRepository
    {
        /// <summary>
        /// Путь каталога для хранения файлов данных (текстовых).
        /// </summary>
        string DataDirectory { get; set; }
        
        /// <summary>
        /// Путь каталога для хранения графических файлов.
        /// </summary>
        string ImagesDirectory { get; set; }
        
        /// <summary>
        /// Извлекает из хранилища файл по имени файла.
        /// </summary>
        /// <param name="fileName">Полное имя файла, включая расширение.</param>
        /// <returns>Файл в виде объекта <see cref="FileInfo"/>.</returns>
        FileInfo GetFileByFileName(string fileName);
        
        /// <summary>
        /// Асинхронно сохраняет данные изображения в файл в хранилище файлов.
        /// </summary>
        /// <param name="fileName">Полное имя файла для сохранения, включая расширение.</param>
        /// <param name="imageStream">Потоковые данные изображения в виде объекта <see cref="Stream"/>. 
        /// Поток должен быть открыт.</param>
        /// <returns>Относительный путь к файлу сохранённого изображения.</returns>
        Task<string> SaveImageAsync(string fileName, Stream imageStream);
        
        /// <summary>
        /// Асинхронно сохраняет текстовые данные в файл в хранилище файлов.
        /// </summary>
        /// <param name="fileName">Полное имя файла для сохранения, включая расширение.</param>
        /// <param name="textData">Текстовые данные для сохранения.</param>
        /// <param name="encoding">Кодировка текстового файла. Если параметр не задан, по умолчанию используется 
        /// кодировка <see cref="Encoding.UTF8"/>.</param>
        Task SaveTextDataAsync(string fileName, string textData, Encoding encoding = null);
    }
}
