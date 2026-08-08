using Godot;
using NetcodeCommon;

namespace RollPunk.Scripts.UI.SessionConsole
{
    public partial class SessionEventConsole : Node
    {
        [Export] EventConsole _eventConsole;

        private Session _session;

        public void LogSession(Session session)
        {
            _session = session;
            
            session.LogAdded += OnLogAdded;
            session.StateInserted += InsertLogs;

            InsertLogs();
            _eventConsole.AddLog(new("Console", Logs.SourceType.System, "Это консоль сессии. Здесь будет разная информация о происходящем в игре", System.DateTime.UtcNow));
        }

        private void InsertLogs()
        {
            _eventConsole.InsertLogCollection(_session.Logs);
        }

        private void OnLogAdded(Logs.Event log)
        {
            _eventConsole.AddLog(log);
        }
    }
}
