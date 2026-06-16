using NetcodeCommon;
using RollPunk.Debug;
using RollPunk.NetcodeCommon;
using System.Net.Sockets;

namespace RollPunk.ClientNetcode
{
    public class TcpClient : IDataBridge
    {
        public static int DataBufferSize = 4096;
        
        private string _ip;
        private int _port;
        private int _myId;

        private ClientHandle _handle;
        private ClientSend _send;
        private Dictionary<int, Action<Packet>> _packetHandlers;

        internal TcpConnection Tcp;

        public event Action<string> ReceivedWelcome;
        public event Action<SessionPatch> ReceivedSessionPatch;
        public event Action<SessionState> ReceivedSessionState;

        public TcpClient(string ip, int port, ThreadManager threadManager)
        {
            _ip = ip;
            _port = port;

            _handle = new(this);
            _send = new(this);

            _packetHandlers = new Dictionary<int, Action<Packet>>()
            {
                { (int)ServerPackets.Welcome, _handle.HandleWelcome },
            };

            Tcp = new(_packetHandlers, threadManager);
        }

        public void ConnectToServer()
        {
            Tcp.Connect(_ip, _port);
        }

        public void SendClientData(string name, Guid id)
        {
            _send.SendClientInitialize(_myId, name, id);
        }

        public void SendSessionPatch(SessionPatch patch)
        {
            _send.SendClientSessionPatch(patch);
        }

        public void RequestSessionState()
        {
            _send.SendSessionStateRequest();
        }

        public void WelcomeReceived(int clientId, string message)
        {
            _myId = clientId;
            ReceivedWelcome?.Invoke(message);
        }

        public void PatchReceived(SessionPatch patch)
        {
            ReceivedSessionPatch?.Invoke(patch);
        }

        public void SessionStateReceived(SessionState state)
        {
            ReceivedSessionState?.Invoke(state);
        }

        internal class TcpConnection
        {
            public System.Net.Sockets.TcpClient Socket;

            private NetworkStream _stream;
            private Packet _receivedData = new();
            private byte[] _receiveBuffer;

            private IReadOnlyDictionary<int, Action<Packet>> _packetHandlers;
            private ThreadManager _threadManager;

            public TcpConnection(IReadOnlyDictionary<int, Action<Packet>> packetHandlers, ThreadManager threadManager)
            {
                _packetHandlers = packetHandlers;
                _threadManager = threadManager;
            }
            
            public void Connect(string ip, int port)
            {
                Socket = new System.Net.Sockets.TcpClient
                {
                    ReceiveBufferSize = DataBufferSize,
                    SendBufferSize = DataBufferSize,
                };

                _receiveBuffer = new byte[DataBufferSize];
                Socket.BeginConnect(ip, port, ConnectCallback, Socket);
            }

            public void SendData(Packet packet)
            {
                try
                {
                    if(Socket == null) return;

                    RPDebug.Log($"Отправляем данные по TCP {packet.Length()}");
                    _stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
                catch (Exception e)
                {
                    RPDebug.LogError($"Error sending data to server: {e.Message}");
                }
            }

            private void ConnectCallback(IAsyncResult result)
            {
                try
                {
                    Socket.EndConnect(result);

                    if (Socket.Connected == false)
                        return;

                    _stream = Socket.GetStream();

                    _stream.BeginRead(_receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
                }
                catch(Exception e)
                {
                    RPDebug.LogError($"Connection error: {e}");
                }
            }

            private void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    int length = _stream.EndRead(result);

                    if (length <= 0)
                    {
                        return;
                    }

                    byte[] data = new byte[length];
                    Array.Copy(_receiveBuffer, data, length);

                    _receivedData.Reset(HandleData(data));
                    _stream.BeginRead(_receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
                }
                catch (Exception ex)
                {
                    RPDebug.LogError($"Error receiving TCP Data: {ex}");
                }
            }

            private bool HandleData(byte[] data)
            {
                int packetLength = 0;

                _receivedData.SetBytes(data);

                if(_receivedData.UnreadLength() >= 4)
                {
                    packetLength = _receivedData.ReadInt();

                    if( packetLength <= 0)
                        return true;
                }

                while( packetLength > 0 && packetLength <= _receivedData.UnreadLength())
                {
                    byte[] packetBytes = _receivedData.ReadBytes(packetLength);

                    _threadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            _packetHandlers[_packetId](_packet);
                        }
                    });

                    packetLength = 0;
                    if(_receivedData.UnreadLength() >= 4)
                    {
                        packetLength = _receivedData.ReadInt();
                        if (packetLength <= 0)
                            return true;
                    }
                }

                if (packetLength <= 1)
                    return true;

                return false;
            }
        }
    }
}
