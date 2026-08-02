using Godot;
using System;

namespace RollPunk.UI.ImageUtils
{
    /// <summary>
    /// Утилита для загрузки изображений с диска
    /// </summary>
    public class ImageLoader
    {
        private readonly ImageDataConverter _converter;

        public ImageLoader(ImageDataConverter converter)
        {
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        /// <summary>
        /// Загружает изображение с диска и конвертирует в byte[]
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <param name="errorMessage">Сообщение об ошибке</param>
        /// <returns>Данные изображения или null при ошибке</returns>
        public byte[] LoadImage(string filePath, out string errorMessage)
        {
            return _converter.LoadImageFileToBytes(filePath, out errorMessage);
        }

        /// <summary>
        /// Загружает изображение и конвертирует в текстуру
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <param name="errorMessage">Сообщение об ошибке</param>
        /// <returns>Текстура или null при ошибке</returns>
        public ImageTexture LoadImageAsTexture(string filePath, out string errorMessage)
        {
            var imageData = LoadImage(filePath, out errorMessage);
            if (imageData == null)
                return null;

            return _converter.BytesToTexture(imageData);
        }
    }
}
