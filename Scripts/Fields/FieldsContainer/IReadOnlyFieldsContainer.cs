using System;
using System.Collections.Generic;

namespace RollPunk.Fields
{
    public interface IReadOnlyFieldsContainer<T> : IReadOnlyFieldsContainer
    where T : Field
{
    event Action<T> Added;
    event Action<T> Removed;

    IReadOnlyList<T> List { get; }
    IReadOnlyDictionary<Guid, T> Dictionary { get; }

    T? GetById(Guid id);
    T? GetByName(string name);

    new bool Contains(Guid id);
    new bool Contains(string name);
}

public interface IReadOnlyFieldsContainer
{
    event Action<Field> FieldAdded;
    event Action<Field> FieldRemoved;

    IReadOnlyList<Field> Fields { get; }
    IReadOnlyDictionary<Guid, Field> FieldsDictionary { get; }

    Field? GetFieldById(Guid id);
    Field? GetFieldByName(string name);

    bool Contains(Guid id);
    bool Contains(string name);
}
}