using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using StartupCreativeAgency.Domain.Abstractions.Exceptions;
using StartupCreativeAgency.Domain.Abstractions.Repositories;

namespace StartupCreativeAgency.Infrastructure
{
    /// <summary>
    /// Репозиторий (хранилище) для хранения текстовых и графических файлов.
    /// </summary>
    public class FileRepository : IFileRepository
    {
        /// <summary>
        /// Путь каталога для хранения файлов данных (текстовых). По умолчанию значение устанавливается в ./Data.
        /// </summary>
        public string DataDirectory { get; set; }
        
        /// <summary>
        /// Путь каталога для хранения графических файлов. По умолчанию значение устанавливается в ./wwwroot/images.
        /// </summary>
        public string ImagesDirectory { get; set; }

        /// <summary>
        /// Создаёт новый экземпляр класса хранилища файлов (репозитория).
        /// </summary>
        /// <param name="environment">
        /// Объект, содержащий информацию об окружении, в котором выполняется приложение. На основании данных объекта формируются 
        /// пути к каталогам для хранения текстовых и графических файлов.
        /// </param>
        public FileRepository(IHostingEnvironment environment)
        {
            DataDirectory = Path.Combine(environment.ContentRootPath, "Data");
            ImagesDirectory = Path.Combine(environment.WebRootPath, "images");
        }

        /// <summary>
        /// Извлекает из хранилища файл по имени файла.
        /// </summary>
        /// <param name="fileName">Полное имя файла, включая расширение.</param>
        /// <returns>Файл в виде объекта <see cref="FileInfo"/>.</returns>
        /// <exception cref = "ArgumentException" />
        /// <exception cref = "FileNotFoundException" />
        /// <exception cref = "DataAccessException" />
        public FileInfo GetFileByFileName(string fileName)
        {
            EnsureFileNameNotNullOrEmpty(fileName);
            try
            {
                string filePath = Path.Combine(DataDirectory, fileName);
                if (!IsFileExist(filePath))
                    throw new FileNotFoundException($"File '{fileName}' not found in directory '{DataDirectory}'.", fileName);
                return new FileInfo(filePath);
            }
            catch (Exception ex) when (!(ex is FileNotFoundException))
            {
                throw new DataAccessException($"Unable to get file '{fileName}'. See inner exception for details.", ex);
            }
        }

        /// <summary>
        /// Проверяет, существует ли файл по заданному пути.
        /// </summary>
        /// <param name="filePath">Полный путь к файлу.</param>
        /// <remarks>Данный метод является простой обёрткой над методом <see cref="File.Exists"/> для целей тестирования.</remarks>
        /// <returns>true, если файл существует; иначе false</returns>
        protected virtual bool IsFileExist(string filePath) => File.Exists(filePath);

        /// <summary>
        /// Асинхронно сохраняет данные изображения в файл в хранилище файлов.
        /// </summary>
        /// <param name="fileName">Полное имя файла для сохранения, включая расширение.</param>
        /// <param name="imageStream">Потоковые данные изображения в виде объекта <see cref="Stream"/>. 
        /// Поток должен быть открыт.</param>
        /// <returns>Относительный путь к файлу сохранённого изображения.</returns>
        /// <exception cref = "ArgumentException" />
        /// <exception cref = "ArgumentNullException" />
        /// <exception cref = "DataAccessException" />
        public async Task<string> SaveImageAsync(string fileName, Stream imageStream)
        {
            EnsureFileNameNotNullOrEmpty(fileName);
            if (imageStream == null)
                throw new ArgumentNullException(nameof(imageStream));
            try
            {
                string filePath = Path.Combine(ImagesDirectory, fileName);
                // Если принимаем в качестве входного параметра уже открытый stream, то обязательно
                // надо сбрасывать position на 0.
                imageStream.Position = 0;
                using (var file = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    await imageStream.CopyToAsync(file);
                }
                return $"~/images/{fileName}";
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"An error occurred while saving image data to file '{fileName}'. " +
                    $"See inner exception for details.", ex);
            }
        }

        /// <summary>
        /// Асинхронно сохраняет текстовые данные в файл в хранилище файлов.
        /// </summary>
        /// <param name="fileName">Полное имя файла для сохранения, включая расширение.</param>
        /// <param name="textData">Текстовые данные для сохранения.</param>
        /// <param name="encoding">Кодировка текстового файла. Если параметр не задан, по умолчанию используется кодировка 
        /// <see cref="Encoding.UTF8"/>.</param>
        /// <exception cref = "ArgumentException" />
        /// <exception cref = "DataAccessException" />
        public async Task SaveTextDataAsync(string fileName, string textData, Encoding encoding = null)
        {
            EnsureFileNameNotNullOrEmpty(fileName);
            encoding = encoding ?? Encoding.UTF8;
            try
            {
                string filePath = Path.Combine(DataDirectory, fileName);
                await File.WriteAllTextAsync(filePath, textData, encoding);
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"An error occurred while saving text data to file '{fileName}'. " +
                    $"See inner exception for details.", ex);
            }
        }

        private void EnsureFileNameNotNullOrEmpty(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name cannot be null or empty string.", nameof(fileName));
        }
    }
}
