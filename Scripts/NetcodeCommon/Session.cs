using RollPunk.Entities;
using RollPunk.Fields;
using RollPunk.HierarchyFields;
using RollPunk.Players;

namespace NetcodeCommon
{
    public class Session
    {
        protected FieldsContainer<EntityField> Fields;
        protected FieldsRegistry FieldsRegistry;

        private Dictionary<Guid, Player> _players;
        public IReadOnlyDictionary<Guid, Player> Players => _players;

        protected EntityFactory EntityFactory;

        protected FieldsHierarchyReconstructor HierarchyReconstructor;

        public Session(EntityFactory entityFactory)
        {
            EntityFactory = entityFactory;
            HierarchyReconstructor = new(EntityFactory);

            Fields = new();
            FieldsRegistry = new(Fields);
            _players = new();
        }

        public virtual void ApplySessionPatch(SessionPatch patch)
        {
            EntityUpdater updater = new();
            
            foreach (var deletedField in patch.RemoveFields)
            {
                var field = FieldsRegistry.GetField(deletedField);

                if (field.Parent != null)
                    field.Parent.RemoveField(field);
                else
                    Fields.RemoveField(field);
            }

            foreach (var pendingField in patch.PendingFields)
                HierarchyReconstructor.ApplyFieldState(pendingField, Fields, null, FieldsRegistry);

            foreach (var pendingPlayer in patch.PendingPlayers)
            {
                if (_players.ContainsKey(pendingPlayer.Key))
                    updater.UpdateEntity(_players[pendingPlayer.Key], pendingPlayer.Value);
                else
                    _players.Add(pendingPlayer.Key, new Player(pendingPlayer.Value));
            }

            foreach (var removedPlayer in patch.RemovePlayers)
                _players.Remove(removedPlayer);
        }

        public virtual SessionState GetState()
        {
            SessionState state = new()
            {
                Fields = FieldStateExtractor.ExtractFieldsCollectionTreeState(Fields.Fields)
            };

            return state;
        }

        public virtual Player AddPlayer(Guid clientId, string name, bool isAdmin = false)
        {
            if(_players.ContainsKey(clientId))
                throw new InvalidOperationException($"Player for Client {clientId} already exists");

            Player player = new(name, new Guid(), isAdmin);
            _players.Add(clientId, player);

            return player;
        }

        public virtual Player RemovePlayer(Guid clientId)
        {
            if (!_players.ContainsKey(clientId))
                return null;

            Player removedPlayer = _players[clientId];
            _players.Remove(clientId);
            return removedPlayer;
        }

        public virtual void ApplyState(SessionState state)
        {
            List<FieldState> fields = state.Fields;
            ApplyFields(fields);

            _players.Clear();

            foreach(var player in state.Players)
                _players.Add(player.Key, new(player.Value));
        }

        private void ApplyFields(List<FieldState> fields)
        {
            HashSet<Guid> existedInStateFields = new();

            void HandleFieldState(FieldState fieldState)
            {
                existedInStateFields.Add(fieldState.State.ID);

                foreach (var childState in fieldState.Children)
                    HandleFieldState(childState);
            }

            foreach (var fieldState in fields)
                HandleFieldState(fieldState);

            foreach (var fieldState in fields)
                HierarchyReconstructor.ApplyFieldState(fieldState, Fields, fieldsRegistry: FieldsRegistry);

            foreach (var field in FieldsRegistry.Fields)
            {
                if (existedInStateFields.Contains(field.ID) == false)
                {
                    if (field.Parent != null)
                        field.Parent.RemoveField(field);
                    else
                        Fields.RemoveField(field);
                }
            }
        }
    }
}
