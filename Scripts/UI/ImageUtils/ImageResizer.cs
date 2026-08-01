using Godot;
using System;

namespace RollPunk.UI.ImageUtils
{
    /// <summary>
    /// Утилита для изменения размера изображений
    /// </summary>
    public static class ImageResizer
    {
        private const int MaxDimension = 1024;
        
        /// <summary>
        /// Изменяет размер изображения, сохраняя пропорции.
        /// Большая сторона становится MaxDimension, меньшая масштабируется пропорционально.
        /// </summary>
        /// <param name="sourceImage">Исходное изображение</param>
        /// <returns>Изображение с измененным размером</returns>
        public static Image ResizeImage(Image sourceImage)
        {
            if (sourceImage == null)
                throw new ArgumentNullException(nameof(sourceImage));
                
            int originalWidth = sourceImage.GetWidth();
            int originalHeight = sourceImage.GetHeight();
            
            // Если изображение уже меньше или равно ограничению, возвращаем копию
            if (originalWidth <= MaxDimension && originalHeight <= MaxDimension)
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
            resizedImage.Resize(newWidth, newHeight, Image.Interpolation.Lanczos);
            
            return resizedImage;
        }
        
        /// <summary>
        /// Вычисляет новые размеры изображения, сохраняя пропорции
        /// </summary>
        /// <param name="originalWidth">Исходная ширина</param>
        /// <param name="originalHeight">Исходная высота</param>
        /// <returns>Новые размеры (ширина, высота)</returns>
        private static (int width, int height) CalculateNewDimensions(int originalWidth, int originalHeight)
        {
            // Определяем какая сторона больше
            if (originalWidth >= originalHeight)
            {
                // Ширина больше или равна высоте
                int newWidth = MaxDimension;
                int newHeight = (int)Math.Round((double)originalHeight * MaxDimension / originalWidth);
                return (newWidth, newHeight);
            }
            else
            {
                // Высота больше ширины
                int newHeight = MaxDimension;
                int newWidth = (int)Math.Round((double)originalWidth * MaxDimension / originalHeight);
                return (newWidth, newHeight);
            }
        }
        
        /// <summary>
        /// Получает максимальное разрешение для изображений
        /// </summary>
        public static int GetMaxDimension() => MaxDimension;
    }
}
