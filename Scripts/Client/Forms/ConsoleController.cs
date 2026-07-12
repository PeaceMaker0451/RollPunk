using RollPunk.Client;
using RollPunk.Client.Forms;
using RollPunk.UI.Forms;

namespace RollPunk.Scripts.Client.Forms
{
    internal class ConsoleController : IFormController<Console>
    {
        private Console _view;
        public Console View => _view;

        public void SetView(Console view)
        {
            _view = view;
        }

        public string FormPath => "res://Scenes/FormsScenes/Console.tscn";
        
        private readonly ClientConsole _console;

        public ConsoleController(ClientConsole console)
        {
            _console = console;
        }

        public void Initialize()
        {
            // Логика инициализации консоли уже в самой форме Console
            // Контроллер может добавить дополнительную логику если нужно
        }
    }
}
