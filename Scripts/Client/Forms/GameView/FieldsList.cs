using Godot;
using RollPunk.Fields;
using System;
using System.Collections.Generic;

namespace RollPunk.ClientSide.Runtime.UI
{
    internal partial class FieldsList : ItemList
    {
        private IReadOnlyFieldsContainer _container;
        private Dictionary<int, Field> _indexToField = new();
        private Dictionary<Field, Action> _updateNameActions = new();

        public event Action<Field> FieldSelected;

        public void SetContainer(IReadOnlyFieldsContainer container)
        {
            if (_container != null)
            {
                _container.ChildAdded -= OnFieldAdded;
                _container.ChildRemoved -= OnFieldRemoved;
            }

            _container = container;

            if (_container != null)
            {
                _container.ChildAdded += OnFieldAdded;
                _container.ChildRemoved += OnFieldRemoved;

                Clear();
                _indexToField.Clear();
                foreach (var field in _container.Fields)
                {
                    AddField(field);
                }
            }
        }

        private void OnFieldAdded(Field field)
        {
            AddField(field);
        }

        private void OnFieldRemoved(Field field)
        {
            RemoveField(field);
        }

        private void AddField(Field field)
        {
            var text = $"{field.Name}";
            int index = AddItem(text);
            _indexToField[index] = field;

            Action onNameUpdated = () => UpdateName(field);
            field.NameChanged += onNameUpdated;
            _updateNameActions.Add(field, onNameUpdated);
        }

        private void RemoveField(Field field)
        {
            var foundIndex = GetFieldIndex(field);

            if (foundIndex.HasValue)
            {
                RemoveItem(foundIndex.Value);
                _indexToField.Remove(foundIndex.Value);

                var newDict = new Dictionary<int, Field>();
                for (int i = 0; i < GetItemCount(); i++)
                {
                    newDict[i] = _indexToField.ContainsKey(i) ? _indexToField[i] : null;
                }
                _indexToField = newDict;

                field.NameChanged -= _updateNameActions[field];
                _updateNameActions.Remove(field);
            }
        }

        private void UpdateName(Field field)
        {
            var foundIndex = GetFieldIndex(field);

            string name = field.Name == string.Empty ? "[empty name]" : field.Name;

            if(foundIndex.HasValue)
            SetItemText(foundIndex.Value, field.Name);
        }

        public override void _Ready()
        {
            ItemSelected += OnItemSelected;
        }

        private void OnItemSelected(long index)
        {
            if (_indexToField.TryGetValue((int)index, out var field) && field != null)
            {
                FieldSelected?.Invoke(field);
            }
        }

        private int? GetFieldIndex(Field field)
        {
            int? foundIndex = null;
            foreach (var kvp in _indexToField)
            {
                if (kvp.Value == field)
                {
                    foundIndex = kvp.Key;
                    break;
                }
            }

            return foundIndex;
        }
    }
}
