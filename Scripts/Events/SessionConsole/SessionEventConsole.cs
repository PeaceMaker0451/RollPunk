using Godot;
using RollPunk.Logs;
using System;
using System.Collections.Generic;

namespace RollPunk.Scripts.UI.SessionConsole
{
    public partial class SessionEventConsole : Node
    {
        [Export] RichTextLabel _text;
        
        public void AddLog(Event log)
        {
            WriteEvent(log);
        }

        public void InsertLogCollection(ICollection<Event> logCollection)
        {
            _text.Clear();
            
            foreach (Event log in logCollection)
                WriteEvent(log);
        }

        private void WriteEvent(Event log)
        {
            string errorPrefix = "[color=red]";
            string userPrefix = "[color=cyan]";
            string systemPrefix = "[color=yellow]";

            string prefix = "";

            switch(log.Type)
            {
                case SourceType.User:
                    prefix = userPrefix;
                    break;

                case SourceType.System:
                    prefix = systemPrefix;
                    break;

                case SourceType.Error:
                    prefix = errorPrefix;
                    break;
            }

            _text.AppendText($"[i][{log.Date.ToLocalTime().ToShortTimeString()}][/i] {prefix}[b]{log.Source}[/b][/color] - {log.Data}\n");
        }
    }
}
