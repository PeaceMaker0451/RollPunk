using NetcodeCommon;
using Newtonsoft.Json;
using RollPunk.Debug;
using RollPunk.NetcodeCommon;
using RollPunk.Players;
using System.Xml.Linq;

namespace RollPunk.ClientNetcode
{
    internal class ClientHandle
    {
        private TcpClient _client;

        public ClientHandle(TcpClient client)
        {
            _client = client;
        }

        public void HandleWelcome(Packet packet)
        {
            string message = packet.ReadString();
            int clientId = packet.ReadInt();

            RPDebug.Log($"[color=dark_violet]Получен Welcome пакет от сервера - {clientId} {message}[/color]");
            _client.WelcomeReceived(clientId, message);
        }

        public void HandleSessionPatch(Packet packet)
        {
            string data = packet.ReadString();
            SessionPatch patch = JsonConvert.DeserializeObject<SessionPatch>(data);

            RPDebug.Log($"[color=dark_violet]Получен патч сессии от сервера[/color]");
            _client.PatchReceived(patch);
        }

        public void HandleSessionState(Packet packet)
        {
            string data = packet.ReadString();
            SessionState state = JsonConvert.DeserializeObject<SessionState>(data);

            RPDebug.Log($"[color=dark_violet]Получено состояние сессии от сервера[/color]");
            _client.SessionStateReceived(state);
        }
    }
}
