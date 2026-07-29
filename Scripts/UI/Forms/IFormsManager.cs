using Godot;
using RollPunk.Modding.APIs;
using RollPunk.UI.Forms;
using System;
using System.Threading.Tasks;

namespace RollPunk.Client.Forms
{
    public interface IFormsManager : IAPIHandler
    {
        // Основные операции
        T GetForm<T>(IFormHandle handle) where T : Form;
        
        // Работа с программно созданными формами
        IFormHandle ShowForm(Form form, FormDisplayMode mode = FormDisplayMode.NewWindow, int priority = 0);
        IFormHandle ShowForm(string path, FormDisplayMode mode = FormDisplayMode.NewWindow, int priority = 0);
        
        // Перемещение между контейнерами
        void MoveToNewWindow(IFormHandle handle);
        void MoveToMainTab(IFormHandle handle, int priority = 0);
        
        // Управление
        void CloseForm(IFormHandle handle);

        // Работа с контроллерами
        IFormHandle ShowController<T>(T controller, FormDisplayMode mode = FormDisplayMode.MainTab, int priority = 0)
            where T : IFormController;

        T GetController<T>(IFormHandle handle) where T : class, IFormController;
        
        // Диалоги
        IDialogFactory Dialogs { get; }
    }

    public enum FormDisplayMode
    {
        MainTab,
        NewWindow
    }

    public interface IFormHandle
    {
        string Id { get; }
        bool IsValid { get; }
        FormLocation Location { get; }
    }

    public enum FormLocation
    {
        NewWindow,
        MainTab,
        Closed
    }

    public interface IDialogFactory
    {
        Task<DialogResult<string>> ShowStringInput(string title, string message, string placeholder = "", bool allowCancel = true, Vector2? minSize = null, string okButtonText = "Ок", string cancelButtonText = "Отмена");
        Task<DialogResult<int?>> ShowIntInput(string title, string message, int? defaultValue = null, int? minValue = null, int? maxValue = null, int step = 1, bool allowCancel = true, Vector2? minSize = null, string okButtonText = "Ок", string cancelButtonText = "Отмена");
        Task ShowInformation(string title, string message, Vector2? minSize = null, bool allowCancel = true, string okButtonText = "Продолжить");
        Task<DialogResult<bool>> ShowConfirmation(string title, string message, bool allowCancel = true, Vector2? minSize = null, string yesButtonText = "Да", string noButtonText = "Нет");
    }
}
