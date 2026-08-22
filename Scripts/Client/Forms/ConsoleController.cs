using PunkCommandSystem;
using RollPunk.Client;
using RollPunk.Client.Forms;

namespace RollPunk.Scripts.Client.Forms
{
    internal class ConsoleController : IFormPresenter<Console>
    {
        private readonly ClientConsole _console;
        private readonly CommandManager _commandManager;
        private Console _view;

        public ConsoleController(ClientConsole console, CommandManager commandManager = null)
        {
            _console = console;
            _commandManager = commandManager;
        }

        public void Attach(Console form)
        {
            _view = form;
            _view.Initialize(_console, _commandManager);
        }
    }
}
