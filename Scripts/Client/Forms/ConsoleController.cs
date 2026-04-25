using RollPunk.Client;
using RollPunk.Client.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollPunk.Scripts.Client.Forms
{
    internal class ConsoleController
    {
        private UIController _ui;
        private ClientConsole _console;
        public ConsoleController(UIController ui, ClientConsole console)
        {
            _ui = ui;
            _console = console;
        }

        public void CreateConsole()
        {
            _ui.LoadFormAsNewFrame("res://Scenes/FormsScenes/Console.tscn", out var form);
            Console console = (Console)form;

            if (_ui.TryGetFrame(console, out var frame))
                frame.SetCloseButtonVisible(false);

        }
    }
}
