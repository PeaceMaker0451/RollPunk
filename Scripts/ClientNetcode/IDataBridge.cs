using NetcodeCommon;

namespace RollPunk.ClientNetcode
{
    public interface IDataBridge
    {
        public event Action<string> ReceivedWelcome;
        public event Action<SessionPatch> ReceivedSessionPatch;
        public event Action<SessionState> ReceivedSessionState;
        public event Action SessionInitializeRequest;
        public event Action ClientInitializeRequest;
        public event Action<Exception> ConnectionErrored;

        public void SendClientData(string name, Guid id);

        public void SendSessionPatch(SessionPatch patch);

        public void RequestSessionState();
    }
}
