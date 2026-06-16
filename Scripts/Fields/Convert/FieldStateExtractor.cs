using System;
using System.Collections.Generic;

namespace RollPunk.Fields
{
    public static class FieldStateExtractor
    {
        public static FieldState ExtractFieldTreeState(Field field)
        {
            FieldState fieldState = new FieldState();
            fieldState.State = field.GetState();

            if(field.Parent != null)
                fieldState.ParentID = field.Parent.ID;
            else
                fieldState.ParentID = Guid.Empty;

            if (fieldState.Children == null)
                throw new Exception("Children null");

            foreach (var child in field.Fields)
            {
                if (child == null)
                    throw new Exception("Child null");

                fieldState.Children.Add(ExtractFieldTreeState(child));
            }
                

            return fieldState;
        }

        public static List<FieldState> ExtractFieldsCollectionTreeState(IReadOnlyCollection<Field> fields)
        {
            List<FieldState> states = new List<FieldState>();

            foreach (var child in fields)
                states.Add(ExtractFieldTreeState(child));

            return states;
        }
    }
}
