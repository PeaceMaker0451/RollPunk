using Godot;
using System;
using System.IO;

namespace RollPunk.UI.ImageUtils
{
    /// <summary>
    /// Валидатор изображений для проверки файлов перед загрузкой
    /// </summary>
    public static class ImageValidator
    {
        private const long MaxFileSizeBytes = 50 * 1024 * 1024; // 50MB максимальный размер файла
        private const int MaxDimension = 8192; // Максимальное разрешение по любой стороне
        
        /// <summary>
        /// Проверяет, можно ли безопасно загрузить файл изображения
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <param name="errorMessage">Сообщение об ошибке, если валидация не прошла</param>
        /// <returns>true если файл можно загружать</returns>
        public static bool ValidateImageFile(string filePath, out string errorMessage)
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
                if (fileInfo.Length > MaxFileSizeBytes)
                {
                    errorMessage = $"Файл слишком большой. Максимальный размер: {MaxFileSizeBytes / (1024 * 1024)}MB";
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
                if (image.GetWidth() > MaxDimension || image.GetHeight() > MaxDimension)
                {
                    errorMessage = $"Изображение слишком большое. Максимальное разрешение: {MaxDimension}x{MaxDimension}";
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
        public static bool ValidateImage(Image image, out string errorMessage)
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
            
            if (image.GetWidth() > MaxDimension || image.GetHeight() > MaxDimension)
            {
                errorMessage = $"Изображение слишком большое. Максимальное разрешение: {MaxDimension}x{MaxDimension}";
                return false;
            }
            
            return true;
        }
    }
}
