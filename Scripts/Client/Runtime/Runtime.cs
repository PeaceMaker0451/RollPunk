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

            string name = Root.Settings.LoadSettings().Name;
            string? overridedName = TryOverrideName();

            if (overridedName != null)
            {
                name = (string)overridedName;
                RPDebug.Log($"Name will be changed to {name}");
            }

            ClientData = new RuntimeClientData(clientId, name);
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

        private string? TryOverrideName()
        {
            const string NamePrefix = "--name=";

            bool TryExtractName(string[] args, out string result)
            {
                result = string.Empty;

                string clientArg = Array.Find(args, arg => arg.StartsWith(NamePrefix, StringComparison.OrdinalIgnoreCase));

                if (string.IsNullOrEmpty(clientArg))
                {
                    return false;
                }

                result = clientArg.Substring(NamePrefix.Length);
                return true;
            }

            string[] args = OS.GetCmdlineArgs();

            if (TryExtractName(args, out string name))
                return name;
            else
                return null;
        }

        public class RuntimeClientData : IRuntimeClientData
        {
            public Guid ClientID { get; private set; }
            public string Name { get; private set; }

            public RuntimeClientData(Guid clientID, string name)
            {
                ClientID = clientID;
                Name = name;
            }
        }
    }
}
