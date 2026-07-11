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

        protected Dictionary<Guid, Player> Players;

        protected EntityFactory EntityFactory;

        protected FieldsHierarchyReconstructor HierarchyReconstructor;

        public Session(EntityFactory entityFactory)
        {
            EntityFactory = entityFactory;
            HierarchyReconstructor = new(EntityFactory);

            Fields = new();
            FieldsRegistry = new(Fields);
            Players = new();
        }

        protected void ApplySessionPatch(SessionPatch patch)
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
                if (Players.ContainsKey(pendingPlayer.Key))
                    updater.UpdateEntity(Players[pendingPlayer.Key], pendingPlayer.Value);
                else
                    Players.Add(pendingPlayer.Key, new Player(pendingPlayer.Value));
            }

            foreach (var removedPlayer in patch.RemovePlayers)
                Players.Remove(removedPlayer);
        }

        protected SessionState GetState()
        {
            SessionState state = new()
            {
                Fields = FieldStateExtractor.ExtractFieldsCollectionTreeState(Fields.Fields)
            };

            return state;
        }

        protected Player AddPlayer(Guid clientId, string name, bool isAdmin = false)
        {
            if(Players.ContainsKey(clientId))
                throw new InvalidOperationException($"Player for Client {clientId} already exists");

            Player player = new(name, new Guid(), isAdmin);
            Players.Add(clientId, player);

            return player;
        }

        protected Player RemovePlayer(Guid clientId)
        {
            if (!Players.ContainsKey(clientId))
                return null;

            Player removedPlayer = Players[clientId];
            Players.Remove(clientId);
            return removedPlayer;
        }

        protected void ApplyState(SessionState state)
        {
            List<FieldState> fields = state.Fields;
            ApplyFields(fields);

            Players.Clear();

            foreach(var player in state.Players)
                Players.Add(player.Key, new(player.Value));
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
