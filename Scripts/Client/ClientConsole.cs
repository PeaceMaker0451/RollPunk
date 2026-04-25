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
        public event Action<string> ConsoleUpdated;

        public StringBuilder ConsoleBuffer {  get; private set; } = new();

        public void ConsoleLog(string log, bool addTime = true)
        {
            string formattedText = null;
            
            if (addTime)
                formattedText = $"\n[b]{DateTime.Now.ToLongTimeString()}[/b] - {log} ";
            else
                formattedText = $"\n{log}";

            GD.Print(log);
            ConsoleBuffer.Append(formattedText);
            ConsoleUpdated?.Invoke(formattedText);
        }
    }
}
