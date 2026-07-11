using NetcodeCommon;
using RollPunk.Entities;
using RollPunk.Fields;
using RollPunk.HierarchyFields;
using RollPunk.Players;
using RollPunk.Rules;
using RollPunk.UIFields;

namespace RollPunk.Server
{
    internal class ServerSession : Session
    {
        public bool SessionInitialized { get; private set; } = false;
        
        public ServerSession() : base(new())
        {
            
            EntityFactory.RegisterFields();
            EntityFactory.RegisterHierarchyFields();
            EntityFactory.RegisterRules();
            EntityFactory.RegisterLineFields();
            EntityFactory.RegisterPlayers();
        }

        new public void ApplySessionPatch(SessionPatch patch) => base.ApplySessionPatch(patch);
        new public SessionState GetState() => base.GetState();
        new public void ApplyState(SessionState state) => base.ApplyState(state);
        new public Player AddPlayer(Guid clientId, string name, bool isAdmin = false) => base.AddPlayer(clientId, name, isAdmin);
        new public Player RemovePlayer(Guid clientId) => base.RemovePlayer(clientId);

        public void SetSessionInitialized()
        {
            SessionInitialized = true;
        }
    }
}
