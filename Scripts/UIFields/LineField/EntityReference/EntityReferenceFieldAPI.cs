using RollPunk.Fields;
using RollPunk.HierarchyFields;

namespace RollPunk.UIFields
{
    public class EntityReferenceFieldAPI : LineFieldAPI
    {
        private EntityReferenceField _field;

        public string reference_id => _field.ReferenceId.ToString();

        public EntityReferenceFieldAPI(EntityReferenceField handler) : base(handler)
        {
            _field = handler;
        }

        public void setEntity(EntityFieldAPI entity)
        {
            _field.SetReference((EntityField)entity.GetField());
        }

        public EntityFieldAPI? getEntity()
        {
            var field = _field.GetEntityField();

            if (field == null)
                return null;

            return (EntityFieldAPI)field.GetAPI();
        }
    }
}