using NetcodeCommon;
using Newtonsoft.Json;
using RollPunk.Debug;
using RollPunk.NetcodeCommon;
using RollPunk.Players;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace RollPunk.ClientNetcode
{
    public class ClientSend
    {
        private TcpClient _client;

        public ClientSend(TcpClient client)
        {
            _client = client;
        }
        
        public void SendClientInitialize(int clientId, string name, Guid playerId)
        {
            using(Packet packet = new((int)ClientPackets.ClientInitialize))
            {
                packet.Write(clientId);
                packet.Write(name);
                packet.Write(playerId.ToString());

                RPDebug.Log($"[color=web_purple]Отправляем инициализацию... {name} - ({playerId})[/color]");
                SendTcpData(packet);
            }
        }

        public void SendClientSessionPatch(SessionPatch patch)
        {
            using(Packet packet = new((int)ClientPackets.SessionPatch))
            {
                string data = JsonConvert.SerializeObject(patch);
                packet.Write(data);

                RPDebug.Log($"[color=web_purple]Отправляем патч сессии..." +
                    $"\n{data}[/color]");
                SendTcpData(packet);
            }
        }

        public void SendSessionStateRequest()
        {
            using (Packet packet = new((int)ClientPackets.SessionStateRequest))
            {
                RPDebug.Log($"[color=web_purple]Отправляем запрос на состояние сессии...[/color]");
                SendTcpData(packet);
            }
        }
        
        private void SendTcpData(Packet packet)
        {
            packet.WriteLength();
            _client.Tcp.SendData(packet);
        }
    }
}
