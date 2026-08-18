using Godot;
using RollPunk.Modding.APIs;
using RollPunk.UI.Forms;
using RollPunk.UI.Frames;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RollPunk.Client.Forms
{
    /// <summary>
    /// Единственная точка создания форм и роутинга их по местам показа.
    ///
    /// Типичный сценарий: forms.Open&lt;MyForm&gt;() — получил форму, работаешь с ней напрямую.
    /// Всё, что касается самой формы как окна (Close, MoveTo..., SetTitle), — на форме.
    /// Работа с фреймами напрямую (без форм) — через IFramesHost.
    /// </summary>
    public interface IFormsManager : IAPIHandler
    {
        /// <summary>
        /// Создаёт форму по её типу (сцена берётся из [FormScene]) и показывает.
        /// </summary>
        T Open<T>(FormDisplayMode mode = FormDisplayMode.NewWindow, int priority = 0)
            where T : Form;

        /// <summary>
        /// Показывает уже созданный инстанс формы. Полезно для форм, собираемых кодом,
        /// или для форм с нетривиальной инициализацией.
        /// </summary>
        T Open<T>(T form, FormDisplayMode mode = FormDisplayMode.NewWindow, int priority = 0)
            where T : Form;

        /// <summary>
        /// Показывает форму в заранее подготовленном фрейме.
        /// Используется в продвинутых сценариях, когда нужно кастомизировать фрейм до показа формы.
        /// </summary>
        T OpenIn<T>(Frame frame) where T : Form;

        /// <summary>
        /// Создаёт форму, привязывает к ней контроллер и показывает.
        /// </summary>
        TForm OpenWith<TForm>(IFormPresenter<TForm> presenter,
                              FormDisplayMode mode = FormDisplayMode.NewWindow,
                              int priority = 0)
            where TForm : Form;

        /// <summary>Все открытые формы.</summary>
        IEnumerable<Form> OpenForms { get; }

        /// <summary>Находит первую открытую форму заданного типа. Null, если такой нет.</summary>
        T FindOpen<T>() where T : Form;

        /// <summary>Диалоги (сообщения, ввод, подтверждения).</summary>
        IDialogFactory Dialogs { get; }
    }

    public enum FormDisplayMode
    {
        MainTab,
        NewWindow
    }

    public interface IDialogFactory
    {
        Task<DialogResult<string>> ShowStringInput(string title, string message, string placeholder = "", bool allowCancel = true, Vector2? minSize = null, string okButtonText = "Ок", string cancelButtonText = "Отмена");
        Task<DialogResult<int?>> ShowIntInput(string title, string message, int? defaultValue = null, int? minValue = null, int? maxValue = null, int step = 1, bool allowCancel = true, Vector2? minSize = null, string okButtonText = "Ок", string cancelButtonText = "Отмена");
        Task ShowInformation(string title, string message, Vector2? minSize = null, bool allowCancel = true, string okButtonText = "Продолжить");
        Task<DialogResult<bool>> ShowConfirmation(string title, string message, bool allowCancel = true, Vector2? minSize = null, string yesButtonText = "Да", string noButtonText = "Нет");
    }
}
