using RollPunk.UI.Forms;

namespace RollPunk.Client.Forms
{
    /// <summary>
    /// Контроллер (презентер) формы. Знает про домен и связывает его с формой.
    ///
    /// Контроллер получает готовую форму через Attach, после чего живёт вместе с ней
    /// и работает с её публичным API. Никаких SetView/SetFormHandle/Initialize по отдельности —
    /// вся инициализация выполняется в Attach.
    /// </summary>
    /// <typeparam name="TForm">Тип формы, которой управляет контроллер.</typeparam>
    public interface IFormPresenter<in TForm> where TForm : Form
    {
        /// <summary>
        /// Вызывается менеджером после создания и показа формы.
        /// Контроллер выполняет здесь всю инициализацию и подписки.
        /// </summary>
        void Attach(TForm form);
    }
}
