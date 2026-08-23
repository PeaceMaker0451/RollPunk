using System;
using System.Collections.Generic;

namespace RollPunk.Fields
{
    public interface IReadOnlyFieldsContainer<T> : IReadOnlyFieldsContainer where T : Field
    {
        public event Action<T> Added;
        public event Action<T> Removed;

        public IReadOnlyList<T> List { get; }
        public IReadOnlyDictionary<Guid, T> Dictionary { get; }
    }

    public interface IReadOnlyFieldsContainer
    {
        public event Action<Field> FieldAdded;
        public event Action<Field> FieldRemoved;

        public IReadOnlyList<Field> Fields { get; }
        public IReadOnlyDictionary<Guid, Field> FieldsDictionary { get; }
    }
}