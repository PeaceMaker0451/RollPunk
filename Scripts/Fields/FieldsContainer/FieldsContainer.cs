using RollPunk.Entities;
using System;
using System.Collections.Generic;

namespace RollPunk.Fields
{
    public class FieldsContainer<T> : EntityContainer<T>, IReadOnlyFieldsContainer, IFieldsHandler where T : Field
    {
        public event Action<Field> ChildAdded;
        public event Action<Field> ChildRemoved;

        public IReadOnlyList<Field> Fields => List;
        public IReadOnlyDictionary<Guid, Field> FieldsDictionary => Dictionary as IReadOnlyDictionary<Guid, Field>;

        public FieldsContainer()
        {
            Added += (field) => ChildAdded?.Invoke(field);
            Removed += (field) => ChildRemoved?.Invoke(field);
        }
        
        public void AddField(Field field)
        {
            if (field is T typedField == false)
                throw new Exception($"Field is not of type ({nameof(T)})");

            Add(typedField);
        }

        public bool RemoveField(Field field)
        {
            if (field is T typedField == false)
                throw new Exception($"Field is not of type ({nameof(T)})");

            return Remove(typedField);
        }
    }
}