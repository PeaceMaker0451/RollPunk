using Newtonsoft.Json.Linq;
using RollPunk.AccessPolicy;
using RollPunk.Entities;
using RollPunk.HierarchyFields;
using RollPunk.MembersExposing;
using System;
using System.Collections.Generic;

namespace RollPunk.UIFields
{
    [EntityType("EntityReference")]
    public class EntityReferenceField : LineField
    {
        private Func<Guid, EntityField?>? _searchEntityFieldFunc;

        [ExposedProperty] public Guid ReferenceId { get; private set; }

        public EntityReferenceField(string name, string visibleName, PlayerRole viewAccessLevel, PlayerRole editAcessLevel, int linePriority = 0, Dictionary<string, object> additionalData = null) 
            : base(name, visibleName, viewAccessLevel, editAcessLevel, typeof(EntityReferenceFieldAPI), linePriority, additionalData)
        { }

        public void SetReference(EntityField entity)
        {
            ReferenceId = entity.ID;
            RaiseChanged();
        }

        public EntityField? GetEntityField()
        {
            if (_searchEntityFieldFunc != null)
                return _searchEntityFieldFunc(ReferenceId);
            else
                throw new NullReferenceException("Entity search function is not setted!");
        }

        public void SetSearchFunc(Func<Guid, EntityField?> searchFunc)
        {
            _searchEntityFieldFunc = searchFunc;
        }

        protected override void ApplyPayload(Dictionary<string, JToken> payload)
        {
            base.ApplyPayload(payload);
            ReferenceId = Guid.Parse(Get<string>(payload, nameof(ReferenceId)));
        }

        protected override void WritePayload(Dictionary<string, JToken> payload)
        {
            base.WritePayload(payload);
            Set<string>(payload, nameof(ReferenceId), ReferenceId.ToString());
        }

        public override object GetRawValue()
        {
            return ReferenceId;
        }
    }
}