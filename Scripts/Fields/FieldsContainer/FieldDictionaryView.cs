using System;
using System.Collections.Generic;

namespace RollPunk.Fields
{
    public sealed class FieldDictionaryView<T> : IReadOnlyDictionary<Guid, Field>
    where T : Field
    {
        private readonly IReadOnlyDictionary<Guid, T> _source;

        public FieldDictionaryView(IReadOnlyDictionary<Guid, T> source)
        {
            _source = source;
        }

        public Field this[Guid key] => _source[key];

        public IEnumerable<Guid> Keys => _source.Keys;

        public IEnumerable<Field> Values => _source.Values;

        public int Count => _source.Count;

        public bool ContainsKey(Guid key)
        {
            return _source.ContainsKey(key);
        }

        public bool TryGetValue(Guid key, out Field? value)
        {
            if (_source.TryGetValue(key, out var field))
            {
                value = field;
                return true;
            }

            value = null;
            return false;
        }

        public IEnumerator<KeyValuePair<Guid, Field>> GetEnumerator()
        {
            foreach (var pair in _source)
                yield return new KeyValuePair<Guid, Field>(
                    pair.Key,
                    pair.Value);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
