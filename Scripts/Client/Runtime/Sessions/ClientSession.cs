using NetcodeCommon;
using RollPunk.AccessPolicy;
using RollPunk.Client.Game.Sessions;
using RollPunk.ClientNetcode;
using RollPunk.ClientSide.Runtime;
using RollPunk.Debug;
using RollPunk.Entities;
using RollPunk.Fields;
using RollPunk.HierarchyFields;
using RollPunk.Modding;
using RollPunk.Modding.APIs;
using RollPunk.Players;
using RollPunk.Rules;
using RollPunk.UIFields;
using System;
using System.Collections.Generic;

namespace RollPunk.Client.Game
{
    public class ClientSession : Session, IDisposable, IAPIHandler
    {
        private string SessionInitializedHookName = "SessionInitialized";

        private IRuntimeData _runtimeData;
        private SessionAPI _api;

        private Constructor _constructor;
        private ModHooker _hooker;
        private ModHookerRuleExecuter _ruleExecuter;
        private ModLoader _modLoader;
        private ModsContainer _loadedMods;

        private EntityInitializer _entityInitializer;
        private EntityValidator _entityValidator;

        private IDataBridge _dataBridge;
        private MutationCatcher _mutationCatcher;
        private OwnershipIntegrityManager _ownershipManager;

        public Guid ID { get; private set; }

        public Player CurrentPlayer => Players.ContainsKey(_runtimeData.ClientID) ? Players[_runtimeData.ClientID] : null;

        public GlobalAPIInjector APIInjector { get; private set; }

        public Serializator Serializator { get; private set; }

        public FieldsContainer<EntityField> Entities => FieldsContainer;
        public EntityContainer<Rule> Rules { get; private set; } = new();
        public IReadOnlyFieldRegistry Registry => base.FieldsRegistry;

        public EntityFieldsOwnersRegistry OwnersRegistry { get; private set; }

        public ClientSession(EntityFactory entityFactory, IRuntimeData runtimeData, IReadOnlyCollection<Mod> mods, IDataBridge dataBridge = null)
            :base(entityFactory)
        {
            _runtimeData = runtimeData;
            RPDebug.Log($"[color=bisque]Creating session...[/color]");
            
            _api = new(this);

            ID = Guid.NewGuid();
            RPDebug.Log($"[color=bisque]Session {ID}[/color]");
            
            Serializator = new(EntityFactory, HierarchyReconstructor);

            if (dataBridge != null)
            {
                _dataBridge = dataBridge;
                InitializeNetworking();
            }

            LoadMods(mods);
            InitializeFieldsContainer();
            InitializeOwnershipSystem();
            APIInjector.AddGlobalAPI(GetAPI());
        }

        public void InitializeSession()
        {
            BatchHook(SessionInitializedHookName);
        }

        public void CreatePlayer(string name, bool isAdmin = false)
        {
            AddPlayer(_runtimeData.ClientID, name, isAdmin);
        }

        public void Dispose() 
        {
            if (_dataBridge != null)
            {
                _dataBridge.ReceivedSessionPatch -= ApplySessionPatch;
                _dataBridge.ReceivedSessionState -= ApplyState;
                _dataBridge.SessionInitializeRequest -= InitializeSession;
                
                if (_dataBridge is TcpClient tcpClient)
                {
                    tcpClient.Disconnect();
                }
            }
        }

        public API GetAPI()
        {
            return _api;
        }

        private void LoadMods(IReadOnlyCollection<Mod> modsToLoad)
        {
            RPDebug.Log($"[color=bisque]Loading Mods...[/color]");

            _loadedMods = new ModsContainer();

            foreach(var mod in modsToLoad)
            {
                RPDebug.Log($"[color=bisque] - Mod {mod.modData.Name} ({mod.modPath})[/color]");
                _loadedMods.AddMod(mod);
            }
                
            _modLoader = new ModLoader();
            _hooker = new ModHooker();
            _ruleExecuter = new ModHookerRuleExecuter(_hooker, _mutationCatcher);
            APIInjector = new GlobalAPIInjector(_loadedMods);

            _constructor = new Constructor(_ruleExecuter);

            APIInjector.AddGlobalAPI(_hooker.GetAPI());
            APIInjector.AddGlobalAPI(_constructor.GetAPI());
            APIInjector.AddGlobalAPI(new RollPunkAPI());
            APIInjector.AddGlobalAPI(Serializator.GetAPI());

            foreach (Mod mod in _loadedMods.Mods)
                _modLoader.LoadMod(mod);
        }

        private object[] BatchHook(string eventName, params object[] args)
        {
            if (_mutationCatcher != null)
                return _hooker.BatchHook(_mutationCatcher, eventName, args);
            else
                return _hooker.CallHook(eventName, args);
        }

        private void InitializeFieldsContainer()
        {
            FieldsContainer.Added += (entity) => entity.SetRulesExecuter(_ruleExecuter);

            _entityValidator = new(FieldsRegistry, _hooker, _mutationCatcher);
            _entityInitializer = new(FieldsRegistry, _hooker, _mutationCatcher);
        }

        private void InitializeNetworking()
        {
            RPDebug.Log($"[color=bisque]Network initializing...[/color]");

            _mutationCatcher = new(this, _dataBridge);

            _dataBridge.ReceivedSessionPatch += ApplySessionPatch;
            _dataBridge.ReceivedSessionState += ApplyState;
            _dataBridge.SessionInitializeRequest += InitializeSession;
        }

        public override void ApplySessionPatch(SessionPatch patch)
        {
            _mutationCatcher.StartIgnore();
            _entityValidator.StartIgnore();
            _entityInitializer.StartIgnore();
            _ownershipManager.StartIgnore();
            base.ApplySessionPatch(patch);
            _mutationCatcher.StopIgnore();
            _entityValidator.StopIgnore();
            _entityInitializer.StopIgnore();
            _ownershipManager.StopIgnore();
        }

        public override void ApplyState(SessionState state)
        {
            _mutationCatcher.StartIgnore();
            _entityValidator.StartIgnore();
            _entityInitializer.StartIgnore();
            _ownershipManager.StartIgnore();
            base.ApplyState(state);
            _mutationCatcher.StopIgnore();
            _entityValidator.StopIgnore();
            _entityInitializer.StopIgnore();
            _ownershipManager.StopIgnore();
        }

        private void InitializeOwnershipSystem()
        {
            OwnersRegistry = new EntityFieldsOwnersRegistry(Ownerships);
            _ownershipManager = new OwnershipIntegrityManager(this);
        }
    }
}
