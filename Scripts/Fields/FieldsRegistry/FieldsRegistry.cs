using System;
using System.Collections.Generic;
using System.Linq;

namespace RollPunk.Fields
{
    public class FieldsRegistry : IReadOnlyFieldRegistry
    {
        Dictionary<Guid, Field> _fields = new();
        Dictionary<Field, (Action NameChanged, Action<string> AdditionalDataChanged, Action Updated, Action Changed,
             Action<Field> ChildAdded, Action<Field> ChildRemoved, Action<Field> ParentChanged, Action<Field> ParentRemoved,
            Action ValueChanged,
            Action LinePriorityChanged, Action EditableChanged, Action VisibleChanged)> _handlers = new();

        public event Action<Field> FieldAdded;
        public event Action<Field> FieldRemoved;

        public event Action<Field> NameChanged;
        public event Action<Field, string> AdditionalDataChanged;
        public event Action<Field> Updated;
        public event Action<Field> ChildAdded;
        public event Action<Field, Field> ChildRemoved;
        public event Action<Field, Field> ParentChanged;
        public event Action<Field, Field> ParentRemoved;
        public event Action<Field> Changed;

        public event Action<ValueField> ValueChanged;

        //public event Action<LineField> LinePriorityChanged;
        //public event Action<LineField> EditableChanged;
        //public event Action<LineField> VisibleChanged;

        public IReadOnlyCollection<Field> Fields => _fields.Values;
        public IReadOnlyDictionary<Guid, Field> FieldsDictionary => _fields;

        public FieldsRegistry(IFieldsHandler rootHandler)
        {
            ConnectToFieldsHandler(rootHandler);
        }

        public Field GetField(Guid guid)
        {
            return _fields[guid];
        }

        public Field GetField(string name)
        {
            return _fields.Where(field => field.Value.Name == name).FirstOrDefault().Value;
        }

        private void ConnectToFieldsHandler(IFieldsHandler fieldsHandler)
        {
            AddFieldsHandlerFields(fieldsHandler);
        }

        private void AddField(Field field)
        {
            AddField(field, false);
        }

        private void AddField(Field field, bool silent = false)
        {
            if (_fields.ContainsKey(field.ID))
                throw new Exception("Поле уже содержится в этом реестре. Если оно попало сюда дважды, значит, что-то вовне пошло не так. Разберись, дурак.");

            _fields[field.ID] = field;

            SubscribeOnFieldEvents(field);

            if (silent == false)
                FieldAdded?.Invoke(field);

            AddFieldsHandlerFields(field);

        }

        private void AddFieldsHandlerFields(IFieldsHandler fieldsHandler)
        {
            foreach (var field in fieldsHandler.Fields)
                AddField(field, true);

            SubscribeToFieldsUpdates(fieldsHandler);
        }

        private void RemoveField(Field field)
        {
            if (_fields.ContainsKey(field.ID) == false)
                throw new Exception("Попытка удаления из реестра поля, которого там не было. Что за бардак, блять?");

            _fields.Remove(field.ID);

            UnsubscribeFromFieldEvents(field);

            RemoveChildren(field);
            UnSubscribeFromFieldsUpdates(field);

            FieldRemoved?.Invoke(field);
        }

        private void RemoveChildren(Field fieldOwner)
        {
            foreach (var field in fieldOwner.Fields)
                RemoveField(field);
        }

        private void SubscribeToFieldsUpdates(IFieldsHandler fieldOwner)
        {
            fieldOwner.ChildAdded += AddField;
            fieldOwner.ChildRemoved += RemoveField;
        }

        private void UnSubscribeFromFieldsUpdates(IFieldsHandler fieldOwner)
        {
            fieldOwner.ChildAdded -= AddField;
            fieldOwner.ChildRemoved -= RemoveField;
        }

        private void SubscribeOnFieldEvents(Field field)
        {
            (Action NameChanged, Action<string> AdditionalDataChanged, Action Updated, Action Changed,
             Action<Field> ChildAdded, Action<Field> ChildRemoved, Action<Field> ParentChanged, Action<Field> ParentRemoved,
            Action ValueChanged,
            Action LinePriorityChanged, Action EditableChanged, Action VisibleChanged) handlers = new();

            Action onNameChanged = () => NameChanged?.Invoke(field);
            handlers.NameChanged = onNameChanged;
            field.NameChanged += onNameChanged;

            Action<string> onAdditionalDataChanged = (name) => AdditionalDataChanged?.Invoke(field, name);
            handlers.AdditionalDataChanged = onAdditionalDataChanged;
            field.AdditionalDataChanged += onAdditionalDataChanged;

            Action onChanged = () => Changed?.Invoke(field);
            handlers.Changed = onChanged;
            field.Changed += onChanged;

            Action onUpdated = () => Updated?.Invoke(field);
            handlers.Updated = onUpdated;
            field.Updated += onUpdated;

            Action<Field> onChildAdded = (throwedField) => ChildAdded?.Invoke(throwedField);
            handlers.ChildAdded = onChildAdded;
            field.ChildAdded += onChildAdded;

            Action<Field> onChildRemoved = (throwedField) => ChildRemoved?.Invoke(field, throwedField);
            handlers.ChildRemoved = onChildRemoved;
            field.ChildRemoved += onChildRemoved;

            Action<Field> onParentChanged = (throwedField) => ParentChanged?.Invoke(field, throwedField);
            handlers.ParentChanged = onParentChanged;
            field.ParentChanged += onParentChanged;

            Action<Field> onParentRemoved = (throwedField) => ParentRemoved?.Invoke(field, throwedField);
            handlers.ParentRemoved = onParentRemoved;
            field.ParentRemoved += onParentRemoved;

            if (field is ValueField valueField)
            {
                Action onValueChanged = () => ValueChanged?.Invoke(valueField);
                handlers.ValueChanged = onValueChanged;
                valueField.ValueChanged += onValueChanged;

                //if (valueField is LineField lineField)
                //{
                //    Action onLinePriorityChanged = () => LinePriorityChanged?.Invoke(lineField);
                //    handlers.LinePriorityChanged = onLinePriorityChanged;
                //    lineField.LinePriorityChanged += onLinePriorityChanged;

                //    Action onEditableChanged = () => EditableChanged?.Invoke(lineField);
                //    handlers.EditableChanged = onEditableChanged;
                //    lineField.EditableChanged += onEditableChanged;

                //    Action onVisibleChanged = () => VisibleChanged?.Invoke(lineField);
                //    handlers.VisibleChanged = onVisibleChanged;
                //    lineField.VisibleChanged += onVisibleChanged;
                //}
            }

            _handlers.Add(field, handlers);
        }

        private void UnsubscribeFromFieldEvents(Field field)
        {
            if (!_handlers.TryGetValue(field, out var handlers))
                return;

            if (handlers.NameChanged != null)
                field.NameChanged -= handlers.NameChanged;
            if (handlers.AdditionalDataChanged != null)
                field.AdditionalDataChanged -= handlers.AdditionalDataChanged;
            if (handlers.Changed != null)
                field.Changed -= handlers.Changed;
            if (handlers.Updated != null)
                field.Updated -= handlers.Updated;
            if(handlers.ChildAdded != null)
                field.ChildAdded -= handlers.ChildAdded;
            if (handlers.ChildRemoved != null)
                field.ChildRemoved -= handlers.ChildRemoved;
            if(handlers.ParentChanged != null)
                field.ParentChanged -= handlers.ParentChanged;
            if(handlers.ParentRemoved != null)
                field.ParentRemoved -= handlers.ParentRemoved;

            if (field is ValueField valueField && handlers.ValueChanged != null)
                valueField.ValueChanged -= handlers.ValueChanged;

            //if (field is LineField lineField)
            //{
            //    if (handlers.LinePriorityChanged != null)
            //        lineField.LinePriorityChanged -= handlers.LinePriorityChanged;
            //    if (handlers.EditableChanged != null)
            //        lineField.EditableChanged -= handlers.EditableChanged;
            //    if (handlers.VisibleChanged != null)
            //        lineField.VisibleChanged -= handlers.VisibleChanged;
            //}

            _handlers.Remove(field);
        }
    }
}
