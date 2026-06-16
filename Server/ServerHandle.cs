using NetcodeCommon;
using Newtonsoft.Json;
using RollPunk.Debug;
using RollPunk.NetcodeCommon;

namespace RollPunk.Server
{
    internal class ServerHandle
    {
        private Server _server;

        public ServerHandle(Server server) { _server = server; }

        public void HandleClientInitialization(int fromClient, Packet packet)
        {
            int clientIdCheck = packet.ReadInt();
            string name = packet.ReadString();
            Guid id = Guid.Parse(packet.ReadString());

            RPDebug.Log($"{_server.Clients[fromClient].Tcp.Socket.Client.RemoteEndPoint} connected succesfully: ({fromClient}) {name} | {id}");

            if (clientIdCheck != fromClient)
                RPDebug.LogError($"{_server.Clients[fromClient].Tcp.Socket.Client.RemoteEndPoint} has assumed the wrong client ID: ({fromClient}:{clientIdCheck})");
        }

        public void HandleClientSessionPatch(int fromClient, Packet packet)
        {
            string data = packet.ReadString();
            SessionPatch patch = JsonConvert.DeserializeObject<SessionPatch>(data);
            _server.ApplySessionPatch(fromClient, patch);

            RPDebug.Log($"{_server.Clients[fromClient].Tcp.Socket.Client.RemoteEndPoint} - Session patch received.");
        }

        public void HandleClientSessionStateRequest(int fromClient, Packet packet)
        {
            _server.RequestSessionState(fromClient);

            RPDebug.Log($"{_server.Clients[fromClient].Tcp.Socket.Client.RemoteEndPoint} has requested session state.");
        }
    }
}
