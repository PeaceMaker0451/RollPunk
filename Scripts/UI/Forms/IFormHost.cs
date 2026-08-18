using RollPunk.Client.Forms;
using RollPunk.UI.Frames;

namespace RollPunk.UI.Forms
{
    /// <summary>
    /// Внутренний контракт между Form и FormsManager.
    /// Позволяет форме делегировать операции над собой как над окном
    /// обратно в менеджер, не зная о нём напрямую.
    ///
    /// Реализуется FormsManager'ом. Устанавливается на форму при её показе.
    /// </summary>
    internal interface IFormHost
    {
        void RequestClose(Form form);
        void RequestMoveToNewWindow(Form form);
        void RequestMoveToMainTab(Form form, int priority);
        Frame GetContainingFrame(Form form);
    }
}
