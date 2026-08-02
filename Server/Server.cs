using NetcodeCommon;
using Newtonsoft.Json;
using RollPunk.Debug;
using RollPunk.NetcodeCommon;
using System;
using System.Net;
using System.Net.Sockets;
namespace RollPunk.Server
{
    internal class Server
    {
        public int MaxPlayers { get; private set; }
        public int Port { get; private set; }

        public IReadOnlyDictionary<int, Client> Clients => _clients;

        private Dictionary<int, Action<int, Packet>> _handlers;
        private Dictionary<int, Client> _clients = new();
        private TcpListener _tcpListener;

        private ServerHandle _handle;
        private ServerSend _send;

        private ServerSession _session;

        private ThreadManager _threadManager;

        public Server(ThreadManager threadManager)
        {
            _threadManager = threadManager;
        }

        public void Start(int maxPlayers, int port)
        {
            MaxPlayers = maxPlayers;
            Port = port;

            _session = new();

            _send = new(this);
            _handle = new(this);

            _handlers = new()
            {
                { (int)ClientPackets.ClientInitialize, _handle.HandleClientInitialization },
                { (int)ClientPackets.SessionPatch, _handle.HandleClientSessionPatch  },
                { (int)ClientPackets.SessionStateRequest, _handle.HandleClientSessionStateRequest  },
            };

            RPDebug.DebugLog($"Starting server on {Port}...");
            InitializeServerData();

            _tcpListener = new TcpListener(IPAddress.Any, Port);
            _tcpListener.Start();
            _tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            RPDebug.DebugLog($"Server started on {Port}");
        }

        public void InitializePlayer(int clientConnectionId, string name, Guid clientId)
        {
            if (_clients.Where(client => client.Value.ClientId == clientId).Any())
            {
                RPDebug.LogError($"Client with ClientID {clientId} already exists - disconnecting..");
                _clients[clientConnectionId].Tcp.Disconnect();
                return;
            }

            _clients[clientConnectionId].ClientId = clientId;

            _threadManager.ExecuteOnMainThread(() =>
            {
                var player = _session.AddPlayer(clientId, name);

                SessionPatch newPlayerPatch = new()
                {
                    PendingPlayers = new() { { clientId, player.GetState() } }
                };

                if (_session.SessionInitialized == false)
                {
                    _send.SendInitializeSessionRequest(clientConnectionId);
                    _session.SetSessionInitialized();
                }
                else
                    _send.SendSessionState(clientConnectionId, _session.GetState());

                _send.SendSessionPatch(newPlayerPatch);
            });
        }

        public void ApplySessionPatch(int fromClient, SessionPatch sessionPatch)
        {
            _session.ApplySessionPatch(sessionPatch);
            _send.SendSessionPatch(fromClient, sessionPatch);
        }

        public void RequestSessionState(int fromClient)
        {
            _send.SendSessionState(fromClient, _session.GetState());
        }

        public void HandleClientDisconnect(int clientId)
        {
            if (!_clients.ContainsKey(clientId) || _clients[clientId].ClientId == Guid.Empty)
                return;

            var guid = _clients[clientId].ClientId;
            _clients[clientId].ClientId = Guid.Empty;

            _threadManager.ExecuteOnMainThread(() =>
            {
                var removedPlayer = _session.RemovePlayer(guid);
                if (removedPlayer != null)
                {
                    SessionPatch disconnectPatch = new()
                    {
                        RemovePlayers = new() { guid }
                    };

                    _send.SendSessionPatch(disconnectPatch);
                }
                else
                {
                    RPDebug.LogError($"Client {guid} not removed!");
                }

                RPDebug.Log($"Client {clientId} ({guid}) disconnected");
            });
        }

        private void TCPConnectCallback(IAsyncResult result)
        {
            TcpClient client = _tcpListener.EndAcceptTcpClient(result);
            _tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            RPDebug.DebugLog($"Incoming connection from: {client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (_clients[i].Tcp.Socket == null)
                {
                    _clients[i].Tcp.Connect(client);
                    return;
                }
            }

            RPDebug.LogError($"{client.Client.RemoteEndPoint} - failed to connect: server is full!");
        }

        private void InitializeServerData()
        {
            for(int i = 0; i <= MaxPlayers; i++)
            {
                var client = new Client(i, _handlers, _threadManager);
                client.Tcp.Connected += () => _send.SendWelcome(client.Id, "Gooool!");
                client.Tcp.Disconnected += HandleClientDisconnect;
                _clients.Add(i, client);
                
            }
        }
    }
}
