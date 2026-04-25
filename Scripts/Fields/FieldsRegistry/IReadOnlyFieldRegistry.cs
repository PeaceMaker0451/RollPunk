using System;
using System.Collections.Generic;

namespace RollPunk.Fields
{
    public interface IReadOnlyFieldRegistry
    {
        public event Action<Field> NameChanged;
        public event Action<Field, string> AdditionalDataChanged;
        public event Action<Field> Updated;
        public event Action<Field> Changed;

        public event Action<ValueField> ValueChanged;

        //public event Action<LineField> LinePriorityChanged;
        //public event Action<LineField> EditableChanged;
        //public event Action<LineField> VisibleChanged;

        public IReadOnlyCollection<Field> Fields { get; }
        public IReadOnlyDictionary<Guid, Field> FieldsDictionary { get; }

        public Field GetField(Guid guid);
        public Field GetField(string name);
    }
}
