using RollPunk.ClientNetcode;
using RollPunk.Debug;
using RollPunk.Entities;
using RollPunk.Modding;
using RollPunk.Scripts.Client.Settings;
using System;
using System.Threading.Tasks;

namespace RollPunk.Client.Game
{
    internal class SessionLifeCycle
    {
        private EntityFactory _entityFactory;
        private ModsContainer _readedMods;
        private RollpunkRootApis _apis;

        public SessionLifeCycle(EntityFactory entityFactory, ModsContainer loadedMods)
        {
            _entityFactory = entityFactory;
            _readedMods = loadedMods;
            _apis = new RollpunkRootApis();
        }
        
        public ClientSession Create(IRuntimeClientData runtimeData)
        {
            var session = new ClientSession(_entityFactory, runtimeData, UserModsLoader.GetUserMods(_readedMods).Mods);
            session.CreatePlayer(true);


            foreach(var api in _apis.Apis)
                session.APIInjector.AddGlobalAPI(api);

            session.InitializeSession();
            session.InitializeClient();

            return session;
        }

        public async Task<ClientSession> TryConnect(string adress, IRuntimeClientData runtimeData)
        {
            ClientSession session = null;
            var adressParts = adress.Split(new char[] { ':' });

            if (adressParts.Length != 2)
                throw new Exception("Невозможно подключиться к хосту: Неправильный формат адресной строки.");

            try
            {
                TcpClient client = new(adressParts[0], Convert.ToInt32(adressParts[1]), Root.ThreadManager.ThreadManager);

                string message = string.Empty;
                int maxConnectionTimeMs = 20000;
                int checkConnectionDelay = 20;
                ConnectionWait wait = new();

                client.ReceivedWelcome += (serverMessage) =>
                {
                    message = serverMessage;
                    wait.IsWelcomeReceived = true;
                };

                client.ConnectToServer();

                while (wait.IsWelcomeReceived == false && wait.ConnectionTime < maxConnectionTimeMs)
                {
                    await Task.Delay(checkConnectionDelay);
                    wait.ConnectionTime += checkConnectionDelay;
                }

                if (wait.IsWelcomeReceived == false)
                    throw new Exception("Невозможно подключиться к хосту: Превышено время ожидания");

                RPDebug.Log($"Сервер передал нам: {message}");

                session = new ClientSession(_entityFactory, runtimeData, UserModsLoader.GetUserMods(_readedMods).Mods, client);

                foreach (var api in _apis.Apis)
                    session.APIInjector.AddGlobalAPI(api);

                client.SendClientData(runtimeData.Name, runtimeData.ClientID);
                client.ConnectionErrored += (ex) => Root.ThreadManager.ExecuteOnMainThread(() => Destroy(session));
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }

            return session;
        }

        public void Destroy(ClientSession session)
        {
            session.Dispose();
        }

        private class ConnectionWait
        {
            public bool IsWelcomeReceived = false;
            public int ConnectionTime = 0;
        }
    }
}
