using System;
using System.Collections.Generic;

namespace RollPunk.Fields
{
    public interface IFieldsHandler
    {
        public event Action<Field> ChildAdded;
        public event Action<Field> ChildRemoved;

        public IReadOnlyList<Field> Fields { get; }

        public void AddField(Field field);
        public bool RemoveField(Field field);
    }
}
