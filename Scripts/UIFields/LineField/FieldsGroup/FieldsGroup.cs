using RollPunk.AccessPolicy;
using RollPunk.Entities;
using RollPunk.Fields;
using System;
using System.Collections.Generic;

namespace RollPunk.UIFields
{
    [EntityType("FieldsGroup")]
    public sealed class FieldsGroup : LineField
    {
        public FieldsGroup(string name, string visibleName, PlayerRole viewAccessLevel, PlayerRole editAcessLevel, int linePriority = 0, Dictionary<string, object> additionalData = null)
            : base(name, visibleName, viewAccessLevel, editAcessLevel, typeof(FieldsGroupAPI), linePriority, additionalData) { }

        public FieldsGroup(EntityState fieldData) : base(fieldData, typeof(FieldsGroupAPI)) { }

        public override object GetRawValue()
        {
            return Fields as IReadOnlyList<LineField>;
        }

        protected override void ValidateChild(Field field)
        {
            if (field is not LineField)
                throw new InvalidOperationException("FieldsGroup can contain only LineFields.");
        }
    }
}
