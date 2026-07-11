using RollPunk.Debug;
using RollPunk.NetcodeCommon;
using System.Net.Sockets;

namespace RollPunk.Server
{
    internal class Client
    {
        public static int DataBufferSize = 4096;
        
        public int Id;
        public TCP Tcp;
        public Guid ClientId;

        public Client(int clientId, IReadOnlyDictionary<int, Action<int, Packet>> packetHandlers, ThreadManager threadManager)
        {
            Id = clientId;
            Tcp = new TCP(Id, packetHandlers, threadManager);
        }

        public class TCP
        {
            public TcpClient Socket;

            public event Action Connected;
            public event Action<int> Disconnected;

            private readonly int _id;
            private NetworkStream _stream;
            private byte[] _receiveBuffer;
            private Packet _receivedData;

            private IReadOnlyDictionary<int, Action<int, Packet>> _packetHandlers;
            private ThreadManager _threadManager;

            public TCP(int id, IReadOnlyDictionary<int, Action<int, Packet>> packetHandlers, ThreadManager threadManager)
            {
                _id = id;
                _packetHandlers = packetHandlers;
                _threadManager = threadManager;
            }

            public void Connect(TcpClient socket)
            {
                Socket = socket;
                Socket.ReceiveBufferSize = DataBufferSize;
                Socket.SendBufferSize = DataBufferSize;

                _stream = Socket.GetStream();
                _receiveBuffer = new byte[DataBufferSize];
                _receivedData = new Packet();

                RPDebug.Log($"начинаем чтение потока.. {_id}");
                _stream.BeginRead(_receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);

                Connected?.Invoke();
            }

            public void SendData(Packet packet)
            {
                try
                {
                    if (Socket == null)
                        return;

                    _stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
                catch(Exception ex)
                {
                    RPDebug.LogError($"Error sending data to client {_id}: {ex.Message}");
                    Disconnect();
                }
            }

            public void Disconnect()
            {
                try
                {
                    Socket?.Close();
                    _stream?.Close();
                }
                catch (Exception ex)
                {
                    RPDebug.LogError($"Error disconnecting client {_id}: {ex.Message}");
                }
                finally
                {
                    Socket = null;
                    _stream = null;
                    Disconnected?.Invoke(_id);
                }
            }

            private bool HandleData(byte[] data)
            {
                int packetLength = 0;

                _receivedData.SetBytes(data);

                if (_receivedData.UnreadLength() >= 4)
                {
                    packetLength = _receivedData.ReadInt();

                    if (packetLength <= 0)
                        return true;
                }

                while (packetLength > 0 && packetLength <= _receivedData.UnreadLength())
                {
                    byte[] packetBytes = _receivedData.ReadBytes(packetLength);

                    _threadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            _packetHandlers[_packetId](_id, _packet);
                        }
                    });

                    packetLength = 0;
                    if (_receivedData.UnreadLength() >= 4)
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

            private void ReceiveCallback(IAsyncResult result)
            {
                Console.WriteLine($"Пришли данные от клиента {_id}");
                
                try
                {
                    int length = _stream.EndRead(result);
                    
                    if(length <= 0)
                    {
                        RPDebug.Log($"Client {_id} disconnected (length: {length})");
                        Disconnect();
                        return;
                    }

                    byte[] data = new byte[length];
                    Array.Copy(_receiveBuffer, data, length);

                    _receivedData.Reset(HandleData(data));
                    _stream.BeginRead(_receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
                }
                catch (Exception ex)
                {
                    RPDebug.LogError($"Error receiving TCP Data from client {_id}: {ex}");
                    Disconnect();
                }
            }
        }
    }
}
