using RollPunk.Modding.APIs;
using RollPunk.UI.Forms;
using System;
using System.Threading.Tasks;

namespace RollPunk.Client.Forms
{
    public interface IFormsManager
    {
        // Основные операции
        IFormHandle ShowInNewWindow(string formPath);
        IFormHandle ShowInMainTab(string formPath, int priority = 0);
        T GetForm<T>(IFormHandle handle) where T : Form;
        
        // Перемещение между контейнерами
        void MoveToNewWindow(IFormHandle handle);
        void MoveToMainTab(IFormHandle handle, int priority = 0);
        
        // Управление
        void CloseForm(IFormHandle handle);
        
        // Диалоги
        IDialogFactory Dialogs { get; }
        
        // API для модинга
        API GetAPI();
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
        Task<string> ShowStringInput(string title, string placeholder = "");
        Task<int?> ShowIntInput(string title, int? defaultValue = null);
        Task ShowInformation(string title, string message);
        Task<bool> ShowConfirmation(string title, string message);
    }
}
