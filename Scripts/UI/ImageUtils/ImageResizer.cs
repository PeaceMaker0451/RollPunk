using Godot;
using System;

namespace RollPunk.UI.ImageUtils
{
    /// <summary>
    /// Утилита для изменения размера изображений
    /// </summary>
    public class ImageResizer
    {
        private readonly int _maxDimension;
        private readonly Image.Interpolation _interpolation;
        
        /// <summary>
        /// Создает новый экземпляр резайзера изображений
        /// </summary>
        /// <param name="maxDimension">Максимальный размер по большей стороне</param>
        /// <param name="interpolation">Метод интерполяции при изменении размера</param>
        public ImageResizer(int maxDimension, Image.Interpolation interpolation = Image.Interpolation.Lanczos)
        {
            _maxDimension = maxDimension;
            _interpolation = interpolation;
        }
        
        /// <summary>
        /// Изменяет размер изображения, сохраняя пропорции.
        /// Большая сторона становится MaxDimension, меньшая масштабируется пропорционально.
        /// </summary>
        /// <param name="sourceImage">Исходное изображение</param>
        /// <returns>Изображение с измененным размером</returns>
        public Image ResizeImage(Image sourceImage)
        {
            if (sourceImage == null)
                throw new ArgumentNullException(nameof(sourceImage));
                
            int originalWidth = sourceImage.GetWidth();
            int originalHeight = sourceImage.GetHeight();
            
            // Если изображение уже меньше или равно ограничению, возвращаем копию
            if (originalWidth <= _maxDimension && originalHeight <= _maxDimension)
            {
                var copy = new Image();
                copy.CopyFrom(sourceImage);
                return copy;
            }
            
            // Вычисляем новые размеры, сохраняя пропорции
            var (newWidth, newHeight) = CalculateNewDimensions(originalWidth, originalHeight);
            
            // Создаем новое изображение с измененным размером
            var resizedImage = new Image();
            resizedImage.CopyFrom(sourceImage);
            resizedImage.Resize(newWidth, newHeight, _interpolation);
            
            return resizedImage;
        }
        
        /// <summary>
        /// Вычисляет новые размеры изображения, сохраняя пропорции
        /// </summary>
        /// <param name="originalWidth">Исходная ширина</param>
        /// <param name="originalHeight">Исходная высота</param>
        /// <returns>Новые размеры (ширина, высота)</returns>
        private (int width, int height) CalculateNewDimensions(int originalWidth, int originalHeight)
        {
            // Определяем какая сторона больше
            if (originalWidth >= originalHeight)
            {
                // Ширина больше или равна высоте
                int newWidth = _maxDimension;
                int newHeight = (int)Math.Round((double)originalHeight * _maxDimension / originalWidth);
                return (newWidth, newHeight);
            }
            else
            {
                // Высота больше ширины
                int newHeight = _maxDimension;
                int newWidth = (int)Math.Round((double)originalWidth * _maxDimension / originalHeight);
                return (newWidth, newHeight);
            }
        }
        
        /// <summary>
        /// Получает максимальное разрешение для изображений
        /// </summary>
        public int GetMaxDimension() => _maxDimension;
    }
}
