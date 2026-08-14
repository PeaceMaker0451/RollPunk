using RollPunk.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RollPunk.AccessPolicy
{
    [EntityType("EntityOwnership")]
    public class EntityOwnership : Entity
    {
        public Guid EntityFieldId { get; private set; }
        public HashSet<Guid> OwnerIds { get; private set; } = new();
        public HashSet<Guid> TeamIds { get; private set; } = new();

        public event Action<EntityOwnership> Changed;

        public EntityOwnership(Guid entityFieldId, string name = "") : base(name)
        {
            EntityFieldId = entityFieldId;
        }

        public EntityOwnership(EntityState data) : base(data)
        {
        }

        protected override void ApplyPayload(Dictionary<string, JToken> payload)
        {
            EntityFieldId = Get<Guid>(payload, nameof(EntityFieldId));
            OwnerIds = Get<HashSet<Guid>>(payload, nameof(OwnerIds)) ?? new();
            TeamIds = Get<HashSet<Guid>>(payload, nameof(TeamIds)) ?? new();
        }

        protected override void WritePayload(Dictionary<string, JToken> payload)
        {
            Set(payload, nameof(EntityFieldId), EntityFieldId);
            Set(payload, nameof(OwnerIds), OwnerIds);
            Set(payload, nameof(TeamIds), TeamIds);
        }

        public void AddOwner(Guid ownerId)
        {
            OwnerIds.Add(ownerId);
            OnChanged();
        }

        public void RemoveOwner(Guid ownerId)
        {
            OwnerIds.Remove(ownerId);
            OnChanged();
        }

        public bool HasOwner(Guid ownerId)
        {
            return OwnerIds.Contains(ownerId);
        }

        public void AddTeam(Guid teamId)
        {
            TeamIds.Add(teamId);
            OnChanged();
        }

        public void RemoveTeam(Guid teamId)
        {
            TeamIds.Remove(teamId);
            OnChanged();
        }

        public bool HasTeam(Guid teamId)
        {
            return TeamIds.Contains(teamId);
        }

        private void OnChanged()
        {
            Changed?.Invoke(this);
        }
    }
}
