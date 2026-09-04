using RollPunk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RollPunk.Fields
{
    public class FieldsContainer<T> :
    IReadOnlyFieldsContainer<T>,
    IFieldsHandler
    where T : Field
    {
        private readonly Dictionary<Guid, T> _fieldsById = new();
        private readonly Dictionary<string, T> _fieldsByName = new();

        private readonly IReadOnlyDictionary<Guid, Field> _fieldsDictionaryView;

        public FieldsContainer()
        {
            _fieldsDictionaryView = new FieldDictionaryView<T>(_fieldsById);
        }

        public event Action<T>? Added;
        public event Action<T>? Removed;

        public event Action<Field>? FieldAdded;
        public event Action<Field>? FieldRemoved;

        public IReadOnlyList<T> List => _fieldsById.Values.ToList();

        public IReadOnlyDictionary<Guid, T> Dictionary => _fieldsById;

        public IReadOnlyList<Field> Fields => List;

        public IReadOnlyDictionary<Guid, Field> FieldsDictionary =>
            new FieldDictionaryView<T>(_fieldsById);

        public T? GetById(Guid id)
        {
            if (_fieldsById.TryGetValue(id, out var field))
                return field;
            else
                return null;
        }
        
        public Field? GetFieldById(Guid id)
        {
            return GetById(id);
        }

        public T? GetByName(string name)
        {
            if (_fieldsByName.TryGetValue(name, out var field))
                return field;
            else
                return null;
        }

        public Field? GetFieldByName(string name)
        {
            return GetByName(name);
        }

        public bool Contains(Guid id)
        {
            return _fieldsById.ContainsKey(id);
        }

        public bool Contains(string name)
        {
            return _fieldsByName.ContainsKey(name);
        }

        public void Add(T field)
        {
            ArgumentNullException.ThrowIfNull(field);

            if (_fieldsById.ContainsKey(field.ID))
            {
                throw new InvalidOperationException(
                    $"Field with ID '{field.ID}' is already contained.");
            }

            if (_fieldsByName.ContainsKey(field.Name))
            {
                throw new InvalidOperationException(
                    $"Field with name '{field.Name}' is already contained.");
            }

            _fieldsById.Add(field.ID, field);
            _fieldsByName.Add(field.Name, field);

            Added?.Invoke(field);
            FieldAdded?.Invoke(field);
        }

        public void AddField(Field field)
        {
            if (field is not T typedField)
            {
                throw new InvalidOperationException(
                    $"Field is not of type '{typeof(T).Name}'.");
            }

            Add(typedField);
        }

        public bool Remove(Guid id)
        {
            if (!_fieldsById.TryGetValue(id, out var field))
                return false;

            _fieldsById.Remove(id);
            _fieldsByName.Remove(field.Name);

            Removed?.Invoke(field);
            FieldRemoved?.Invoke(field);

            return true;
        }

        public bool Remove(T field)
        {
            ArgumentNullException.ThrowIfNull(field);

            if (!_fieldsById.TryGetValue(field.ID, out var existing))
                return false;

            if (!ReferenceEquals(existing, field))
                return false;

            return Remove(field.ID);
        }

        public bool RemoveField(Field field)
        {
            if (field is not T typedField)
            {
                throw new InvalidOperationException(
                    $"Field is not of type '{typeof(T).Name}'.");
            }

            return Remove(typedField);
        }
    }
}