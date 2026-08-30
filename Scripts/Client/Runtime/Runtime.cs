using Godot;
using RollPunk.Debug;
using System;

namespace RollPunk.Client.Game
{
    public enum RuntimeSessionState
    {
        None,
        Connecting,
        InSession
    }
    
    
    internal class Runtime
    {
        public event Action SessionStateChanged;
        
        public IRuntimeClientData ClientData { get; private set; }
        public ClientSession Session { get; private set; }
        public RuntimeSessionState SessionState { get; private set; }

        public Runtime()
        {
            Guid clientId = Root.Settings.LoadSettings().ClientID;
            Guid? overridedGuid = TryOverrideGuid();

            if (overridedGuid != null)
            {
                clientId = (Guid)overridedGuid;
                RPDebug.Log($"Client ID will be changed to {clientId}");
            }

            ClientData = new RuntimeClientData(clientId);
        }

        public void SetSession(ClientSession session)
        {
            Session = session;
            SessionState = RuntimeSessionState.InSession;
            SessionStateChanged?.Invoke();
        }

        public void SetSessionConnecting()
        {
            SessionState = RuntimeSessionState.Connecting;
            SessionStateChanged?.Invoke();
        }

        public void SetSessionClear()
        {
            Session = null;
            SessionState = RuntimeSessionState.None;
            SessionStateChanged?.Invoke();
        }

        private Guid? TryOverrideGuid()
        {
            const string ClientIdPrefix = "--clientId=";

            bool TryExtractClientId(string[] args, out Guid result)
            {
                result = Guid.Empty;

                string clientArg = Array.Find(args, arg => arg.StartsWith(ClientIdPrefix, StringComparison.OrdinalIgnoreCase));

                if (string.IsNullOrEmpty(clientArg))
                {
                    return false;
                }

                string guidString = clientArg.Substring(ClientIdPrefix.Length);

                return Guid.TryParse(guidString, out result);
            }

            string[] args = OS.GetCmdlineArgs();

            if (TryExtractClientId(args, out Guid parsedGuid))
                return parsedGuid;
            else
                return null;
        }

        public class RuntimeClientData : IRuntimeClientData
        {
            public Guid ClientID { get; private set; }

            public RuntimeClientData(Guid clientID)
            {
                ClientID = clientID;
            }
        }
    }
}
