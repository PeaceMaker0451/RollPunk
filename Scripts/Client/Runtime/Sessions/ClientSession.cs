using NetcodeCommon;
using RollPunk.AccessPolicy;
using RollPunk.Client.Game.Sessions;
using RollPunk.ClientNetcode;
using RollPunk.Debug;
using RollPunk.Entities;
using RollPunk.HierarchyFields;
using RollPunk.Modding;
using RollPunk.Modding.APIs;
using RollPunk.Players;
using System;
using System.Collections.Generic;

namespace RollPunk.Client.Game
{
    public class ClientSession : Session, IDisposable, IAPIHandler
    {
        private string SessionInitializedHookName = "SessionInitialized";
        private string ClientInitializedHookName = "ClientInitialized";
        private string StateAppliedHookName = "StateApplied";
        private string PatchAppliedHookName = "PatchApplied";

        private IRuntimeClientData _runtimeData;
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


        internal SessionPlayerSpace PlayerSpace { get; private set; }
        public Player? CurrentPlayer => Players.ContainsKey(_runtimeData.ClientID) ? Players[_runtimeData.ClientID] : null;
        public GlobalAPIInjector APIInjector { get; private set; }
        public Serializator Serializator { get; private set; }
        public EntityFieldsOwnersRegistry OwnersRegistry { get; private set; }

        public ClientSession(EntityFactory entityFactory, IRuntimeClientData runtimeData, IReadOnlyCollection<Mod> mods, IDataBridge dataBridge = null)
            :base(entityFactory)
        {
            _runtimeData = runtimeData;
            RPDebug.Log($"[color=bisque]Creating session...[/color]");
            
            Serializator = new(EntityFactory, HierarchyReconstructor);
            PlayerSpace = new(this);

            if (dataBridge != null)
            {
                _dataBridge = dataBridge;
                InitializeNetworking();
            }

            InitializeOwnershipSystem();
            LoadMods(mods);
            InitializeFieldsContainer();
        }

        public void AddEntity(EntityField field)
        {
            Container.Add(field);
        }

        public bool RemoveEntity(EntityField field)
        {
            return Container.Remove(field);
        }

        public void InitializeSession()
        {
            BatchHook(SessionInitializedHookName);
        }

        public void InitializeClient()
        {
            BatchHook(ClientInitializedHookName);
        }

        public void CreatePlayer(bool isAdmin = false)
        {
            AddPlayer(_runtimeData.ClientID, _runtimeData.Name, isAdmin);
        }

        public void Dispose() 
        {
            if (_dataBridge != null)
            {
                _dataBridge.ReceivedSessionPatch -= ApplySessionPatch;
                _dataBridge.ReceivedSessionState -= ApplyState;
                _dataBridge.SessionInitializeRequest -= InitializeSession;
                _dataBridge.ClientInitializeRequest -= InitializeClient;

                if (_dataBridge is TcpClient tcpClient)
                {
                    tcpClient.Disconnect();
                }
            }

            PlayerSpace.Dispose();
        }

        public API GetAPI()
        {
            return _api;
        }

        protected override void OnPatchApplied()
        {
            _hooker.CallHook(PatchAppliedHookName);
        }

        protected override void OnStateApplied()
        {
            _hooker.CallHook(StateAppliedHookName);
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

            foreach (Mod mod in _loadedMods.Mods)
                _modLoader.LoadMod(mod);

            _api = new(this);
            APIInjector.AddGlobalAPI(GetAPI());
            APIInjector.AddGlobalAPI(Serializator.GetAPI());
            APIInjector.AddGlobalAPI(_constructor.GetAPI());
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
            Container.Added += (entity) => entity.SetRulesExecuter(_ruleExecuter);

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
            _dataBridge.ClientInitializeRequest += InitializeClient;
        }

        public override void ApplySessionPatch(SessionPatch patch)
        {
            _mutationCatcher.StartIgnore();
            _entityValidator.StartIgnore();
            _entityInitializer.StartIgnore();
            _ownershipManager.StartIgnore();
            
            try
            {
                base.ApplySessionPatch(patch);
            }
            catch (Exception ex)
            {
                RPDebug.LogError($"Error applying session patch - {ex.Message} \n{ex.StackTrace}");
                RPDebug.LogError($"Requesting session state..");
                _dataBridge.RequestSessionState();
            }
            
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
