using RollPunk.Client;
using RollPunk.Client.Forms;

namespace RollPunk.Scripts.Client.Forms
{
    internal class ConsoleController : IFormController<Console>
    {
        public Console View { get; set; }
        Form IFormControllerBase.View
        {
            get => View;
            set => View = (Console)value;
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
