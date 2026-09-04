using RollPunk.Entities;
using RollPunk.Fields;
using RollPunk.HierarchyFields;
using RollPunk.Logs;
using RollPunk.Players;
using RollPunk.AccessPolicy;
using System;
using System.Collections.Generic;
using System.Text;
using RollPunk.Debug;

namespace NetcodeCommon
{
    public class Session
    {
        private Dictionary<Guid, Player> _players;
        private List<Event> _sessionLog;

        protected FieldsContainer<EntityField> Container;
        protected FieldsRegistry FieldsRegistry;
        
        public OwnershipContainer Ownerships { get; private set; } = new();

        protected EntityFactory EntityFactory;
        protected FieldsHierarchyReconstructor HierarchyReconstructor;

        public event Action<Guid> PlayerAdded;
        public event Action<Guid> PlayerRemoved;

        public event Action<Event> LogAdded;
        public event Action PatchInserted;
        public event Action StateInserted;

        public IReadOnlyDictionary<Guid, Player> Players => _players;
        public IReadOnlyFieldRegistry Registry => FieldsRegistry;
        public IReadOnlyFieldsContainer<EntityField> Entities => Container;
        public IReadOnlyList<Event> Logs => _sessionLog;

        public Session(EntityFactory entityFactory)
        {
            EntityFactory = entityFactory;
            HierarchyReconstructor = new(EntityFactory);

            Container = new();
            FieldsRegistry = new(Container);
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
                    Container.RemoveField(field);
            }

            foreach (var pendingField in patch.PendingFields)
                HierarchyReconstructor.ApplyFieldState(pendingField, Container, null, FieldsRegistry);

            foreach (var pendingPlayer in patch.PendingPlayers)
            {
                if (_players.ContainsKey(pendingPlayer.Key))
                    updater.UpdateEntity(_players[pendingPlayer.Key], pendingPlayer.Value);
                else
                    AddPlayer(pendingPlayer.Key, new Player(pendingPlayer.Value));
            }

            foreach (var removedPlayer in patch.RemovePlayers)
                RemovePlayer(removedPlayer);

            Ownerships.UpdateFromState(patch.PendingOwnerships);

            foreach (var removedOwnership in patch.RemoveOwnerships)
            {
                Ownerships.Remove(removedOwnership);
            }

            foreach (var pendingLog in patch.PendingLogs)
            {
                RPDebug.Log($"pending log - {pendingLog.Name}");
                AddLog(new(pendingLog));
            }

            OnPatchApplied();
            PatchInserted?.Invoke();
        }

        public virtual SessionState GetState()
        {
            SessionState state = new()
            {
                Fields = FieldStateExtractor.ExtractFieldsCollectionTreeState(Container.Fields),
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

            if(clientId == Guid.Empty)
                throw new InvalidOperationException($"Client ID is empty!");

            Player player = new(name, clientId, isAdmin);
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

            OnStateApplied();
            StateInserted?.Invoke();
        }

        public string GetSessionData()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"Players:");

            foreach (var player in _players.Values)
                stringBuilder.AppendLine($"- {player.Name} [{player.ClientId}]");

            stringBuilder.AppendLine($"Fields:");

            foreach (var field in Container.Fields)
            {
                stringBuilder.AppendLine($"- {field.Name} [{field.ID}] ({field.Fields.Count} children)");

                if(field is EntityField entityField)
                {
                    EntityOwnership record;

                    foreach(var entityOwnership in Ownerships.Objects)
                    {
                        if(entityOwnership.EntityFieldId == entityField.ID)
                        {
                            foreach(var ownerId in entityOwnership.OwnerIds)
                            {
                                if (_players.TryGetValue(ownerId, out var owner))
                                    stringBuilder.AppendLine($"\\_ {owner.Name} [{owner.ClientId}]");
                                else
                                    stringBuilder.AppendLine($"\\_ unknown player [{ownerId}]");
                            }
                        }
                    }    
                }
            }

            stringBuilder.AppendLine($"Events:");

            foreach (var eventRecord in _sessionLog)
                stringBuilder.AppendLine($"- {eventRecord.Name} - {eventRecord.Data}");

            stringBuilder.AppendLine($"OwnershipRecords:");

            foreach (var ownerRecord in Ownerships.Objects)
            {
                stringBuilder.AppendLine($"{ownerRecord.EntityFieldId} - ({string.Join(',', ownerRecord.OwnerIds)}) [{ownerRecord.ID}] ");
            }

            return stringBuilder.ToString();
        }

        protected virtual void OnStateApplied() { }

        protected virtual void OnPatchApplied() { }

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
                HierarchyReconstructor.ApplyFieldState(fieldState, Container, fieldsRegistry: FieldsRegistry);

            foreach (var field in FieldsRegistry.Fields)
            {
                if (existedInStateFields.Contains(field.ID) == false)
                {
                    if (field.Parent != null)
                        field.Parent.RemoveField(field);
                    else
                        Container.RemoveField(field);
                }
            }
        }
    }
}
