using RollPunk.AccessPolicy;
using RollPunk.Client.Runtime.Sessions;
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

namespace RollPunk.Client.Runtime
{
    public class Session : IDisposable, IAPIHandler
    {
        private string SessionInitializedHookName = "SessionInitialized";
        private string EntityInitializedHookName = "EntityInitialized";

        private IRuntimeData _runtimeData;
        private SessionAPI _api;

        private Dictionary<Guid, Player> _players = new();

        private Constructor _constructor;
        private ModHooker _hooker;
        private ModHookerRuleExecuter _ruleExecuter;
        private ModLoader _modLoader;
        private ModsContainer _loadedMods;

        private EntityFactory _entityFactory;
        private FieldsHierarchyReconstructor _fieldsHierarchyReconstructor;
        
        private FieldsContainer<EntityField> _fieldsContainer;
        private FieldsRegistry _fieldsRegistry;

        private MutationCatcher _mutationCatcher;
        private EntityInitializer _entityInitializer;
        private EntityValidator _entityValidator;

        public Guid ID { get; private set; }

        public IReadOnlyDictionary<Guid, Player> Players => _players;
        public Player CurrentPlayer => _players.ContainsKey(_runtimeData.ClientID) ? _players[_runtimeData.ClientID] : null;

        public GlobalAPIInjector APIInjector { get; private set; }

        public Serializator Serializator { get; private set; }

        public FieldsContainer<EntityField> Entities => _fieldsContainer;
        public EntityContainer<Rule> Rules { get; private set; } = new();
        public IReadOnlyFieldRegistry Registry => _fieldsRegistry;

        public EntityFieldsOwnersRegistry OwnersRegistry { get; private set; } = new();

        public Session(IRuntimeData runtimeData, IReadOnlyList<Mod> mods)
        {
            _runtimeData = runtimeData;
            RPDebug.Log($"[color=bisque]Creating session...[/color]");
            
            _api = new(this);

            ID = Guid.NewGuid();
            RPDebug.Log($"[color=bisque]Session {ID}[/color]");

            _entityFactory = new();
            _entityFactory.RegisterFields();
            _entityFactory.RegisterHierarchyFields();
            _entityFactory.RegisterRules();
            _entityFactory.RegisterLineFields();
            _entityFactory.RegisterPlayers();

            _fieldsHierarchyReconstructor = new FieldsHierarchyReconstructor(_entityFactory);
            
            Serializator = new(_entityFactory, _fieldsHierarchyReconstructor);

            LoadMods(mods);
            InitializeFieldsContainer();
        }

        public void InitializeSession()
        {
            APIInjector.AddGlobalAPI(GetAPI());
            BatchHook(SessionInitializedHookName);
        }

        public Player CreatePlayer(string name, bool isAdmin = false)
        {
            if (_players.ContainsKey(_runtimeData.ClientID))
                throw new InvalidOperationException($"Player for Client {_runtimeData.ClientID} already exists");
            
            Player player = new(name, new Guid(), isAdmin);
            _players.Add(_runtimeData.ClientID, player);
            return player;
        }

        public void Dispose() { }

        public API GetAPI()
        {
            return _api;
        }

        private void LoadMods(IReadOnlyList<Mod> modsToLoad)
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
            _ruleExecuter = new ModHookerRuleExecuter(_hooker);
            APIInjector = new GlobalAPIInjector(_loadedMods);

            _constructor = new Constructor(new ModHookerRuleExecuter(_hooker));

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
            _fieldsContainer = new();
            _fieldsRegistry = new(_fieldsContainer);

            _fieldsContainer.Added += (entity) => entity.SetRulesExecuter(_ruleExecuter);

            _mutationCatcher = new(_fieldsRegistry);

            _entityValidator = new(_fieldsRegistry, _hooker, _mutationCatcher);
            _entityInitializer = new(_fieldsRegistry, _hooker, _mutationCatcher);
        }
    }
}
