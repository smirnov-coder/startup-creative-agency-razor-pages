using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Abstractions.Exceptions;
using StartupCreativeAgency.Domain.Abstractions.Repositories;
using StartupCreativeAgency.Domain.Abstractions.Services;

namespace StartupCreativeAgency.Domain.Logic.Services
{
    /// <summary>
    /// Сервис для работы с текстовыми и графическими файлами.
    /// </summary>
    public class FileService : IFileService
    {
        private IFileRepository _repository;

        /// <summary>
        /// Создаёт новый экземпляр сервиса для работы с текстовыми и графическими файлами.
        /// </summary>
        /// <param name="repository">Репозиторий для хранения файлов.</param>
        public FileService(IFileRepository repository) => _repository = repository;

        /// <summary>
        /// Асинхронно возвращает содержимое файла в виде строки символов в кодировке <see cref="Encoding.UTF8"/>.
        /// </summary>
        /// <param name="fileName">Полное имя файла, включая расширение.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="DomainServiceException" />
        public async Task<string> GetFileContentAsStringAsync(string fileName)
        {
            EnsureFileNameNotNullOrEmpty(fileName);
            try
            {
                return await File.ReadAllTextAsync(_repository.GetFileByFileName(fileName).FullName, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new DomainServiceException($"Unable to read text data from file '{fileName}'. " +
                    $"See inner exception for details.", ex);
            }
        }

        /// <summary>
        /// Асинхронно сохраняет данные изображения в файл.
        /// </summary>
        /// <param name="fileName">Полное имя файла для сохранения, включая расширение.</param>
        /// <param name="imageStream">Потоковые данные изображения в виде объекта <see cref="Stream"/>. 
        /// Поток должен быть открыт.</param>
        /// <returns>Относительный путь к файлу сохранённого изображения.</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DomainServiceException" />
        public async Task<string> SaveImageAsync(string fileName, Stream imageStream)
        {
            EnsureFileNameNotNullOrEmpty(fileName);
            if (imageStream == null)
                throw new ArgumentNullException(nameof(imageStream));
            try
            {
                return await _repository.SaveImageAsync(fileName, imageStream);
            }
            catch (Exception ex)
            {
                throw new DomainServiceException($"Unable to save image '{fileName}'. " +
                    $"See inner exception for details.", ex);
            }
        }

        /// <summary>
        /// Асинхронно сохраняет текстовые данные в файл.
        /// </summary>
        /// <param name="fileName">Полное имя файла для сохранения, включая расширение.</param>
        /// <param name="textData">Текстовые данные для сохранения.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="DomainServiceException" />
        public async Task SaveTextDataAsync(string fileName, string textData)
        {
            EnsureFileNameNotNullOrEmpty(fileName);
            try
            {
                await _repository.SaveTextDataAsync(fileName, textData, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new DomainServiceException($"Unable to save text data to file '{fileName}'. " +
                    $"See inner exception for details.", ex);
            }
        }

        /// <summary>
        /// Генерирует уникальное имя файла с заданным расширением.
        /// </summary>
        /// <param name="extension">Расширение файла, включая символ точки.</param>
        /// <param name="prefix">Префикс имени файла. Опциональный параметр.</param>
        /// <exception cref="ArgumentException" />
        public string GenerateUniqueFileName(string extension, string prefix = null)
        {
            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentException("File extension cannot be null or empty string.", nameof(extension));
            return string.Format("{0}{1}{2}", prefix, Guid.NewGuid(), extension);
        }

        private void EnsureFileNameNotNullOrEmpty(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name cannot be null or empty string.", nameof(fileName));
        }
    }
}
