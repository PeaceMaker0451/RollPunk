using RollPunk.Entities;
using RollPunk.Fields;
using RollPunk.HierarchyFields;
using RollPunk.Logs;
using RollPunk.Players;
using RollPunk.AccessPolicy;
using System;
using System.Collections.Generic;

namespace NetcodeCommon
{
    public class Session
    {
        private Dictionary<Guid, Player> _players;
        private List<Event> _sessionLog;

        protected FieldsContainer<EntityField> FieldsContainer;
        protected FieldsRegistry FieldsRegistry;
        
        public OwnershipContainer Ownerships { get; private set; } = new();

        protected EntityFactory EntityFactory;
        protected FieldsHierarchyReconstructor HierarchyReconstructor;

        public event Action<Guid> PlayerAdded;
        public event Action<Guid> PlayerRemoved;
        public event Action<Event> LogAdded;
        public event Action StateInserted;

        public IReadOnlyDictionary<Guid, Player> Players => _players;
        public IReadOnlyFieldRegistry Registry => FieldsRegistry;
        public IReadOnlyFieldsContainer Fields => FieldsContainer;
        public IReadOnlyList<Event> Logs => _sessionLog;

        public Session(EntityFactory entityFactory)
        {
            EntityFactory = entityFactory;
            HierarchyReconstructor = new(EntityFactory);

            FieldsContainer = new();
            FieldsRegistry = new(FieldsContainer);
            _players = new();
            _sessionLog = new();
        }

        public void AddLog(Event log)
        {
            _sessionLog.Add(log);
            LogAdded?.Invoke(log);
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
                    FieldsContainer.RemoveField(field);
            }

            foreach (var pendingField in patch.PendingFields)
                HierarchyReconstructor.ApplyFieldState(pendingField, FieldsContainer, null, FieldsRegistry);

            foreach (var pendingPlayer in patch.PendingPlayers)
            {
                if (_players.ContainsKey(pendingPlayer.Key))
                    updater.UpdateEntity(_players[pendingPlayer.Key], pendingPlayer.Value);
                else
                    AddPlayer(pendingPlayer.Key, new Player(pendingPlayer.Value));
            }

            foreach (var removedPlayer in patch.RemovePlayers)
                RemovePlayer(removedPlayer);

            foreach (var pendingOwnership in patch.PendingOwnerships)
            {
                var existingOwnership = Ownerships.GetByID(pendingOwnership.Key);
                if (existingOwnership != null)
                    updater.UpdateEntity(existingOwnership, pendingOwnership.Value);
                else
                    Ownerships.Add(new EntityOwnership(pendingOwnership.Value));
            }

            foreach (var removedOwnership in patch.RemoveOwnerships)
            {
                var ownership = Ownerships.GetByID(removedOwnership);
                if (ownership != null)
                    Ownerships.Remove(ownership);
            }

            foreach (var pendingLog in patch.PendingLogs)
                AddLog(new(pendingLog));
        }

        public virtual SessionState GetState()
        {
            SessionState state = new()
            {
                Fields = FieldStateExtractor.ExtractFieldsCollectionTreeState(FieldsContainer.Fields),
                Players = new Dictionary<Guid, EntityState>(),
                Ownerships = new Dictionary<Guid, EntityState>()
            };

            // Добавляем игроков в состояние
            foreach (var player in _players)
            {
                state.Players.Add(player.Key, player.Value.GetState());
            }

            // Добавляем владения в состояние
            state.Ownerships = Ownerships.GetStates();

            foreach (var log in _sessionLog)
                state.Logs.Add(log.GetState());

            return state;
        }

        public virtual Player AddPlayer(Guid clientId, string name, bool isAdmin = false)
        {
            if(_players.ContainsKey(clientId))
                throw new InvalidOperationException($"Player for Client {clientId} already exists");

            Player player = new(name, new Guid(), isAdmin);
            return AddPlayer(clientId, player);
        }

        public virtual Player RemovePlayer(Guid clientId)
        {
            if (!_players.ContainsKey(clientId))
                return null;

            Player removedPlayer = _players[clientId];
            _players.Remove(clientId);

            PlayerRemoved?.Invoke(clientId);
            return removedPlayer;
        }

        public virtual void ApplyState(SessionState state)
        {
            List<FieldState> fields = state.Fields;
            ApplyFields(fields);

            _players.Clear();
            foreach(var player in state.Players)
                AddPlayer(player.Key, new(player.Value));

            Ownerships.Clear();
            Ownerships.UpdateFromState(state.Ownerships);

            _sessionLog.Clear();
            foreach (var log in state.Logs)
                AddLog(new(log));

            StateInserted?.Invoke();
        }

        private Player AddPlayer(Guid clientId, Player player)
        {
            _players.Add(clientId, player);

            PlayerAdded?.Invoke(clientId);
            return player;
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
                HierarchyReconstructor.ApplyFieldState(fieldState, FieldsContainer, fieldsRegistry: FieldsRegistry);

            foreach (var field in FieldsRegistry.Fields)
            {
                if (existedInStateFields.Contains(field.ID) == false)
                {
                    if (field.Parent != null)
                        field.Parent.RemoveField(field);
                    else
                        FieldsContainer.RemoveField(field);
                }
            }
        }
    }
}
