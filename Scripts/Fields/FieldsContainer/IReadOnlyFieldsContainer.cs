using System;
using System.Collections.Generic;

namespace RollPunk.Fields
{
    public interface IReadOnlyFieldsContainer
    {
        public event Action<Field> ChildAdded;
        public event Action<Field> ChildRemoved;

        public IReadOnlyList<Field> Fields { get; }
        public IReadOnlyDictionary<Guid, Field> FieldsDictionary { get; }
    }
}