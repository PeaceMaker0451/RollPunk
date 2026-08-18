using RollPunk.UI.Forms;
using RollPunk.UI.Frames;
using System.Collections.Generic;

namespace RollPunk.UI.Forms
{
    /// <summary>
    /// Публичный контракт для работы с фреймами напрямую.
    /// Используется, когда нужно окно (Frame) без привязки к Form —
    /// например, для показа пустого фрейма с последующей ручной настройкой,
    /// или для доступа к главному фрейму приложения со вкладками.
    ///
    /// Для типового сценария "показать форму" используй IFormsManager.
    /// </summary>
    public interface IFramesHost
    {
        /// <summary>Главный фрейм приложения со вкладками.</summary>
        TabedFrame MainFrame { get; }

        /// <summary>Все открытые в данный момент фреймы (главный + дочерние).</summary>
        IEnumerable<Frame> OpenFrames { get; }

        /// <summary>
        /// Открывает новый фрейм с формой в качестве содержимого.
        /// </summary>
        Frame OpenFrame(Form form, bool alwaysOnTop = false);

        /// <summary>
        /// Открывает новый пустой фрейм. Содержимое устанавливается позднее
        /// через Frame.SetForm(...).
        /// </summary>
        Frame OpenEmptyFrame(bool alwaysOnTop = false);

        /// <summary>Закрывает указанный фрейм программно.</summary>
        void CloseFrame(Frame frame);
    }
}
