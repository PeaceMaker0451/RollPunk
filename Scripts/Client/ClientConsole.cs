using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollPunk.Client
{
    public class ClientConsole
    {
        private StringBuilder _consoleBuffer = new();

        public event Action<string> ConsoleUpdated;

        public string ConsoleBuffer => _consoleBuffer.ToString();

        public void ConsoleLog(string log, bool addTime = true)
        {
            string formattedText = null;
            
            if (addTime)
                formattedText = $"\n[b]{DateTime.Now.ToLongTimeString()}[/b] - {log} ";
            else
                formattedText = $"\n{log}";

            _consoleBuffer.Append(formattedText);
            ConsoleUpdated?.Invoke(formattedText);
        }
    }
}
