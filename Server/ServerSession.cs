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


        public void SetSessionInitialized()
        {
            SessionInitialized = true;
        }
    }
}
