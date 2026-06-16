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
    }
}
