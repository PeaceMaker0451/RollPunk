using NetcodeCommon;
using Newtonsoft.Json;
using RollPunk.NetcodeCommon;

namespace RollPunk.Server
{
    internal class ServerSend
    {
        private Server _server;

        public ServerSend(Server server)
        {
            _server = server;
        }
        
        public void SendWelcome(int clientId, string message)
        {
            using (Packet packet = new((int)ServerPackets.Welcome))
            {
                packet.Write(message);
                packet.Write(clientId);

                SendTcpData(clientId, packet);
            }
        }

        public void SendInitializeSessionRequest(int clientId)
        {
            using (Packet packet = new((int)ServerPackets.SessionInitialize))
            {
                SendTcpData(clientId, packet);
            }
        }

        public void SendSessionPatch(SessionPatch patch)
        {
            SendSessionPatch(-1, patch);
        }

        public void SendSessionPatch(int exceptClientId, SessionPatch patch)
        {
            using (Packet packet = new((int)ServerPackets.SessionPatch))
            {
                string data = JsonConvert.SerializeObject(patch);
                packet.Write(data);

                if (exceptClientId == -1)
                    SendTcpDataToAll(packet);
                else
                    SendTcpDataToAll(exceptClientId, packet);
            }
        }

        public void SendSessionState(int clientId, SessionState sessionState)
        {
            using (Packet packet = new((int)ServerPackets.SessionState))
            {
                string data = JsonConvert.SerializeObject(sessionState);
                packet.Write(data);

                SendTcpData(clientId, packet);
            }
        }

        private void SendTcpData(int clientId, Packet packet)
        {
            packet.WriteLength();
            _server.Clients[clientId].Tcp.SendData(packet);
        }

        private void SendTcpDataToAll(Packet packet)
        {
            packet.WriteLength();

            foreach(var client in _server.Clients)
            {
                client.Value.Tcp.SendData(packet);
            }
        }

        private void SendTcpDataToAll(int exceptClientId, Packet packet)
        {
            packet.WriteLength();

            foreach (var client in _server.Clients)
            {
                if (client.Key == exceptClientId)
                    continue;
                
                client.Value.Tcp.SendData(packet);
            }
        }
    }
}
