using System;
using System.Threading.Tasks;

namespace RollPunk.Client.Game
{
    internal class SessionManager
    {
        private Runtime _runtime;
        private SessionLifeCycle _sessionLifeCycle;
        
        public SessionManager(Runtime runtime, SessionLifeCycle sessionLifeCycle)
        {
            _runtime = runtime;
            _sessionLifeCycle = sessionLifeCycle;
        }

        public void CreateLocal()
        {
            var session = _sessionLifeCycle.Create(_runtime.ClientData);
            _runtime.SetSession(session);
        }

        public async Task<bool> CreateOnline(string adress)
        {
            _runtime.SetSessionConnecting();
            
            try
            {
                var session = await _sessionLifeCycle.TryConnect(adress, _runtime.ClientData);

                if(session != null)
                {
                    _runtime.SetSession(session);
                    return true;
                }
                else
                {
                    _ = Root.Forms.Dialogs.ShowInformation("Ошибка подключения к сессии", "Не удалось подключиться к сессии");
                    _runtime.SetSessionClear();
                    return false;
                }
            }
            catch(Exception ex)
            {
                _ = Root.Forms.Dialogs.ShowInformation("Ошибка подключения к сессии", ex.Message);
                _runtime.SetSessionClear();
                return false;
            }
        }

        public void Destroy()
        {
            _sessionLifeCycle.Destroy(_runtime.Session);
            _runtime.SetSessionClear();
        }
    }
}
