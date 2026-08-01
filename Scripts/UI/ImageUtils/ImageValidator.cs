using Godot;
using System;
using System.IO;

namespace RollPunk.UI.ImageUtils
{
    /// <summary>
    /// Валидатор изображений для проверки файлов перед загрузкой
    /// </summary>
    public class ImageValidator
    {
        private readonly long _maxFileSizeBytes;
        private readonly int _maxDimension;
        
        /// <summary>
        /// Создает новый экземпляр валидатора изображений
        /// </summary>
        /// <param name="maxFileSizeBytes">Максимальный размер файла в байтах</param>
        /// <param name="maxDimension">Максимальное разрешение по любой стороне</param>
        public ImageValidator(long maxFileSizeBytes, int maxDimension)
        {
            _maxFileSizeBytes = maxFileSizeBytes;
            _maxDimension = maxDimension;
        }
        
        /// <summary>
        /// Проверяет, можно ли безопасно загрузить файл изображения
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <param name="errorMessage">Сообщение об ошибке, если валидация не прошла</param>
        /// <returns>true если файл можно загружать</returns>
        public bool ValidateImageFile(string filePath, out string errorMessage)
        {
            errorMessage = string.Empty;
            
            try
            {
                // Проверка существования файла
                if (!File.Exists(filePath))
                {
                    errorMessage = "Файл не найден";
                    return false;
                }
                
                // Проверка размера файла
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > _maxFileSizeBytes)
                {
                    errorMessage = $"Файл слишком большой. Максимальный размер: {_maxFileSizeBytes / (1024 * 1024)}MB";
                    return false;
                }
                
                // Проверка что это изображение и получение размеров
                var image = new Image();
                var loadResult = image.Load(filePath);
                
                if (loadResult != Error.Ok)
                {
                    errorMessage = "Не удалось загрузить файл как изображение";
                    return false;
                }
                
                // Проверка разрешения
                if (image.GetWidth() > _maxDimension || image.GetHeight() > _maxDimension)
                {
                    errorMessage = $"Изображение слишком большое. Максимальное разрешение: {_maxDimension}x{_maxDimension}";
                    return false;
                }
                
                // Проверка что изображение не пустое
                if (image.GetWidth() <= 0 || image.GetHeight() <= 0)
                {
                    errorMessage = "Изображение имеет некорректные размеры";
                    return false;
                }
                
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Ошибка при проверке файла: {ex.Message}";
                return false;
            }
        }
        
        /// <summary>
        /// Проверяет загруженное изображение Godot
        /// </summary>
        /// <param name="image">Изображение для проверки</param>
        /// <param name="errorMessage">Сообщение об ошибке</param>
        /// <returns>true если изображение валидно</returns>
        public bool ValidateImage(Image image, out string errorMessage)
        {
            errorMessage = string.Empty;
            
            if (image == null)
            {
                errorMessage = "Изображение равно null";
                return false;
            }
            
            if (image.GetWidth() <= 0 || image.GetHeight() <= 0)
            {
                errorMessage = "Изображение имеет некорректные размеры";
                return false;
            }
            
            if (image.GetWidth() > _maxDimension || image.GetHeight() > _maxDimension)
            {
                errorMessage = $"Изображение слишком большое. Максимальное разрешение: {_maxDimension}x{_maxDimension}";
                return false;
            }
            
            return true;
        }
    }
}
