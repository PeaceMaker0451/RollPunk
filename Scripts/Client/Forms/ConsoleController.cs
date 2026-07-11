using RollPunk.Client.Forms;

namespace RollPunk.Scripts.Client.Forms
{
    internal class ConsoleController : IFormController
    {
        private readonly Console _view;
        private readonly ClientConsole _console;

        public ConsoleController(Console view, ClientConsole console)
        {
            _view = view;
            _console = console;
        }

        public void Initialize()
        {
            // Логика инициализации консоли уже в самой форме Console
            // Контроллер может добавить дополнительную логику если нужно
        }
    }
}
