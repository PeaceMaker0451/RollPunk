using Godot;
using System;

namespace RollPunk.UI.ImageUtils
{
    /// <summary>
    /// Конвертер для работы с данными изображений
    /// </summary>
    public class ImageDataConverter
    {
        private readonly ImageValidator _validator;
        private readonly ImageResizer _resizer;
        
        /// <summary>
        /// Создает новый экземпляр конвертера изображений
        /// </summary>
        /// <param name="validator">Валидатор для проверки изображений</param>
        /// <param name="resizer">Резайзер для изменения размера изображений</param>
        public ImageDataConverter(ImageValidator validator, ImageResizer resizer)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _resizer = resizer ?? throw new ArgumentNullException(nameof(resizer));
        }
        
        /// <summary>
        /// Конвертирует изображение Godot в массив байтов PNG
        /// </summary>
        /// <param name="image">Изображение для конвертации</param>
        /// <returns>Массив байтов PNG или null при ошибке</returns>
        public byte[] ImageToBytes(Image image)
        {
            if (image == null)
                return null;
                
            try
            {
                return image.SavePngToBuffer();
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Ошибка при конвертации изображения в байты: {ex.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// Конвертирует массив байтов PNG в изображение Godot
        /// </summary>
        /// <param name="imageData">Данные изображения в формате PNG</param>
        /// <returns>Изображение Godot или null при ошибке</returns>
        public Image BytesToImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return null;
                
            try
            {
                var image = new Image();
                var loadResult = image.LoadPngFromBuffer(imageData);
                
                if (loadResult != Error.Ok)
                {
                    GD.PrintErr($"Ошибка при загрузке изображения из байтов: {loadResult}");
                    return null;
                }
                
                return image;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Ошибка при конвертации байтов в изображение: {ex.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// Конвертирует массив байтов PNG в текстуру Godot
        /// </summary>
        /// <param name="imageData">Данные изображения в формате PNG</param>
        /// <returns>Текстура или null при ошибке</returns>
        public ImageTexture BytesToTexture(byte[] imageData)
        {
            var image = BytesToImage(imageData);
            if (image == null)
                return null;
                
            try
            {
                var texture = ImageTexture.CreateFromImage(image);
                return texture;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Ошибка при создании текстуры из изображения: {ex.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// Загружает изображение с диска, изменяет размер и конвертирует в байты
        /// </summary>
        /// <param name="filePath">Путь к файлу изображения</param>
        /// <param name="errorMessage">Сообщение об ошибке</param>
        /// <returns>Массив байтов PNG или null при ошибке</returns>
        public byte[] LoadImageFileToBytes(string filePath, out string errorMessage)
        {
            errorMessage = string.Empty;
            
            // Валидация файла
            if (!_validator.ValidateImageFile(filePath, out errorMessage))
                return null;
            
            try
            {
                // Загружаем изображение
                var image = new Image();
                var loadResult = image.Load(filePath);
                
                if (loadResult != Error.Ok)
                {
                    errorMessage = $"Не удалось загрузить изображение: {loadResult}";
                    return null;
                }
                
                // Изменяем размер
                var resizedImage = _resizer.ResizeImage(image);
                
                // Конвертируем в байты
                var imageData = ImageToBytes(resizedImage);
                
                if (imageData == null)
                {
                    errorMessage = "Не удалось конвертировать изображение в PNG";
                    return null;
                }
                
                return imageData;
            }
            catch (Exception ex)
            {
                errorMessage = $"Ошибка при обработке изображения: {ex.Message}";
                return null;
            }
        }
    }
}
