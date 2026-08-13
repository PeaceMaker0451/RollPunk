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
    }
}
