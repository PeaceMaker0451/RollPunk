using RollPunk.Client;
using RollPunk.Client.Forms;
using RollPunk.UI.Forms;

namespace RollPunk.Scripts.Client.Forms
{
    internal class ConsoleController : IFormController<Console>
    {
        private readonly ClientConsole _console;
        private Console _view;

        public Console View => _view;
        public string FormPath => "res://Scenes/FormsScenes/Console.tscn";
        public IFormHandle FormHandle { get; private set; }

        public ConsoleController(ClientConsole console)
        {
            _console = console;
        }

        public void Initialize()
        {
            // Логика инициализации консоли уже в самой форме Console
            // Контроллер может добавить дополнительную логику если нужно
        }

        public void SetView(Console view)
        {
            _view = view;
        }

        public void SetFormHandle(IFormHandle handle)
        {
            FormHandle = handle;
        }
    }
}
