using RollPunk.Entities;
using System;

namespace RollPunk.Fields
{
    public class FieldsHierarchyReconstructor
    {
        private EntityUpdater _updater = new EntityUpdater();
        private EntityFactory _factory;
        
        public FieldsHierarchyReconstructor(EntityFactory entityFactory)
        {
            _factory = entityFactory;
        }
        
        public void ApplyFieldState(TreeState stateToApply, IFieldsHandler fieldsContainer, IFieldsHandler defaultRoot = null, FieldsRegistry fieldsRegistry = null)
        {
            if (defaultRoot == null)
                defaultRoot = fieldsContainer;

            if(fieldsRegistry == null)
                fieldsRegistry = new FieldsRegistry(fieldsContainer);
            
            if(TryUpdateField(stateToApply.State, fieldsRegistry) == false)
            {
                if(stateToApply.ParentID == Guid.Empty)
                {
                    var field = CreateField(stateToApply.State);
                    fieldsContainer.AddField(field);
                }
                    
                else
                {
                    if (fieldsRegistry.FieldsDictionary.TryGetValue(stateToApply.ParentID, out Field parent))
                    {
                        var field = CreateField(stateToApply.State);
                        parent.AddField(field);
                    } 
                    else
                        throw new Exception($"Parent with ID {stateToApply.ParentID} not found for field with ID {stateToApply.State.ID}");
                }
            }

            Field parentField = fieldsRegistry.GetField(stateToApply.State.ID);

            foreach (var fieldState in stateToApply.Children)
                ApplyFieldState(fieldState, fieldsContainer, defaultRoot, fieldsRegistry);
        }

        public Field CreateFieldsTree(TreeState stateToApply)
        {
            Field field = CreateField(stateToApply.State);

            foreach(var fieldState in stateToApply.Children)
                field.AddField(CreateFieldsTree(fieldState));

            return field;
        }
        
        private Field CreateField(EntityState state)
        {
            return (Field)_factory.Create(state);
        }

        private bool TryUpdateField(EntityState state, FieldsRegistry fieldsRegistry)
        {
            if(fieldsRegistry.FieldsDictionary.TryGetValue(state.ID, out Field field))
            {
                _updater.UpdateEntity(field, state);
                return true;
            }

            return false;
        }
    }
}
