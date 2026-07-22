using Godot;
using RollPunk.Fields;
using System;
using System.Collections.Generic;

namespace RollPunk.ClientSide.Runtime.UI
{
    internal partial class FieldsTree : Tree
    {
        [Export] private Texture2D _defaultFieldIcon;
        [Export] private Texture2D _entityFieldIcon;
        [Export] private Texture2D _containerFieldIcon;
        
        private IReadOnlyFieldsContainer _container;
        private Dictionary<TreeItem, Field> _itemToField = new();
        private Dictionary<Field, TreeItem> _fieldToItem = new();
        private Dictionary<Field, Action> _updateNameActions = new();
        private Dictionary<Field, Action<Field>> _childAddedActions = new();
        private Dictionary<Field, Action<Field>> _childRemovedActions = new();
        private TreeItem _root;

        public event Action<Field> FieldSelected;

        public void SetContainer(IReadOnlyFieldsContainer container)
        {
            ClearSubscriptions();
            
            if (_container != null)
            {
                _container.ChildAdded -= OnContainerFieldAdded;
                _container.ChildRemoved -= OnContainerFieldRemoved;
            }
            
            _container = container;
            
            Clear();
            _itemToField.Clear();
            _fieldToItem.Clear();
            
            if (_container != null)
            {
                _container.ChildAdded += OnContainerFieldAdded;
                _container.ChildRemoved += OnContainerFieldRemoved;
                
                _root = CreateItem();
                _root.SetText(0, "Fields");
                
                foreach (var field in _container.Fields)
                {
                    AddFieldRecursive(field, _root);
                }
            }
        }

        public override void _Ready()
        {
            ItemSelected += OnItemSelected;
            SetHideRoot(false);
        }

        private void OnItemSelected()
        {
            var selectedItem = GetSelected();
            if (selectedItem != null && _itemToField.TryGetValue(selectedItem, out var field))
            {
                FieldSelected?.Invoke(field);
            }
        }

        private void AddFieldRecursive(Field field, TreeItem parent)
        {
            var item = CreateItem(parent);
            item.SetText(0, GetFieldDisplayName(field));
            
            SetFieldIcon(item, field);
            
            _itemToField[item] = field;
            _fieldToItem[field] = item;
            
            SubscribeToFieldEvents(field);
            
            foreach (var childField in field.Fields)
            {
                AddFieldRecursive(childField, item);
            }
        }

        private void RemoveFieldRecursive(Field field)
        {
            if (_fieldToItem.TryGetValue(field, out var item))
            {
                UnsubscribeFromFieldEvents(field);
                
                foreach (var childField in field.Fields)
                {
                    RemoveFieldRecursive(childField);
                }
                
                _itemToField.Remove(item);
                _fieldToItem.Remove(field);
                
                item.Free();
            }
        }

        private void SubscribeToFieldEvents(Field field)
        {
            Action onNameChanged = () => UpdateFieldName(field);
            field.NameChanged += onNameChanged;
            field.Changed += onNameChanged;
            _updateNameActions[field] = onNameChanged;

            Action<Field> onChildAdded = (childField) => OnFieldChildAdded(field, childField);
            field.ChildAdded += onChildAdded;
            _childAddedActions[field] = onChildAdded;

            Action<Field> onChildRemoved = (childField) => OnFieldChildRemoved(field, childField);
            field.ChildRemoved += onChildRemoved;
            _childRemovedActions[field] = onChildRemoved;
        }

        private void UnsubscribeFromFieldEvents(Field field)
        {
            if (_updateNameActions.TryGetValue(field, out var nameAction))
            {
                field.NameChanged -= nameAction;
                field.Changed -= nameAction;
                _updateNameActions.Remove(field);
            }

            if (_childAddedActions.TryGetValue(field, out var addedAction))
            {
                field.ChildAdded -= addedAction;
                _childAddedActions.Remove(field);
            }

            if (_childRemovedActions.TryGetValue(field, out var removedAction))
            {
                field.ChildRemoved -= removedAction;
                _childRemovedActions.Remove(field);
            }
        }

        private void OnFieldChildAdded(Field parentField, Field childField)
        {
            if (_fieldToItem.TryGetValue(parentField, out var parentItem))
            {
                AddFieldRecursive(childField, parentItem);
            }
        }

        private void OnFieldChildRemoved(Field parentField, Field childField)
        {
            RemoveFieldRecursive(childField);
        }

        private void UpdateFieldName(Field field)
        {
            if (_fieldToItem.TryGetValue(field, out var item))
            {
                item.SetText(0, GetFieldDisplayName(field));
            }
        }

        private string GetFieldDisplayName(Field field)
        {
            return string.IsNullOrEmpty(field.Name) ? "[empty name]" : field.Name;
        }

        private void SetFieldIcon(TreeItem item, Field field)
        {
            Texture2D icon = GetIconForField(field);
            if (icon != null)
            {
                item.SetIcon(0, icon);
            }
        }

        private Texture2D GetIconForField(Field field)
        {
            // Проверяем тип поля и возвращаем соответствующую иконку
            string fieldTypeName = field.GetType().Name;
            
            return fieldTypeName switch
            {
                "EntityField" => _entityFieldIcon,
                _ when field.Fields.Count > 0 => _containerFieldIcon,
                _ => _defaultFieldIcon
            };
        }

        private void OnContainerFieldAdded(Field field)
        {
            if (_root != null)
            {
                AddFieldRecursive(field, _root);
            }
        }

        private void OnContainerFieldRemoved(Field field)
        {
            RemoveFieldRecursive(field);
        }

        private void ClearSubscriptions()
        {
            if (_fieldToItem != null)
            {
                foreach (var field in _fieldToItem.Keys)
                {
                    UnsubscribeFromFieldEvents(field);
                }
            }
            
            _updateNameActions.Clear();
            _childAddedActions.Clear();
            _childRemovedActions.Clear();
        }

        public override void _ExitTree()
        {
            ClearSubscriptions();
            base._ExitTree();
        }
    }
}
