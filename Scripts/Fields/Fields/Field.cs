using Godot;
using Newtonsoft.Json.Linq;
using RollPunk.Debug;
using RollPunk.Entities;
using RollPunk.Modding.APIs;
using System;
using System.Collections.Generic;

namespace RollPunk.Fields
{
    public abstract class Field : Entity, IAPIHandler, IFieldsHandler
    {
        protected readonly FieldAPI _api;

        private readonly List<Field> _children = new();

        private Dictionary<string, Field> _childrenByNames = new();
        private Dictionary<Guid, Field> _childrenByIds = new();

        public event Action NameChanged;
        public event Action<string> AdditionalDataChanged;
        public event Action<Field> FieldAdded;
        public event Action<Field> FieldRemoved;
        public event Action<Field> DescendantAdded;
        public event Action<Field> DescendantRemoved;
        public event Action<Field> ParentChanged;
        public event Action<Field> ParentRemoved;
        public event Action Changed;

        public Dictionary<string, object> AdditionalData { get; private set; } = new();
        public Field Parent { get; private set; }
        public IReadOnlyList<Field> Fields => _children;

        public Field(string name, Type apiType, Dictionary<string, object> additionalData = null) : base(name)
        {
            if (additionalData != null)
                AdditionalData = additionalData;

            FieldAPI api = CreateAPI(apiType);
            _api = api;
        }

        public Field(EntityState data, Type apiType) : base(data)
        {
            FieldAPI api = CreateAPI(apiType);
            _api = api;
        }

        public void SetName(string newName)
        {
            Name = newName;
            NameChanged?.Invoke();
            RaiseChanged();
        }

        public void SetAdditionalDataField(string name, object value)
        {
            if (value == null)
            {
                if (AdditionalData.ContainsKey(name))
                {
                    AdditionalData.Remove(name);

                    AdditionalDataChanged?.Invoke(name);
                    RaiseChanged();
                }
            }
            else
            {
                if (AdditionalData.ContainsKey(name))
                    AdditionalData[name] = value;
                else
                    AdditionalData.Add(name, value);

                AdditionalDataChanged?.Invoke(name);
                RaiseChanged();
            }
        }

        public object GetAdditionalDataField(string fieldName)
        {
            if (AdditionalData.ContainsKey(fieldName))
                return AdditionalData[fieldName];
            else
                return null;
        }

        public void AddField(Field child)
        {
            if (child == null) 
                throw new ArgumentNullException(nameof(child));

            if (_children.Contains(child)) 
                return;

            if(child.Parent != null)
                throw new InvalidOperationException($"Unnable to child field {child.Name} [{child.ID}]: remove child field from it's parent first!");

            if (child.IsAncestorOf(this))  
                throw new InvalidOperationException($"Unnable to child field {child.Name} [{child.ID}]: operation would create ownership cycle.");

            //if(child.TryGetField(child.Name, out var field) == true)
            //    throw new InvalidOperationException($"Unnable to child field {child.Name} [{child.ID}]: childs hierar");

            var names = new HashSet<string>(_childrenByNames.Keys);
            names.IntersectWith(child._childrenByNames.Keys);

            if(names.Count > 0)
                throw new InvalidOperationException($"Unnable to child field {child.Name} [{child.ID}]: childs hierarchy has intersections by fields names: ({string.Join(", ", names)})");

            var ids = new HashSet<string>(_childrenByNames.Keys);
            ids.IntersectWith(child._childrenByNames.Keys);

            if (ids.Count > 0)
                throw new InvalidOperationException($"Unnable to child field {child.Name} [{child.ID}]: childs hierarchy has intersections by fields Ids: ({string.Join(", ", ids)})");

            ValidateChild(child);
            ThrowValidateChildOnParent(child);

            _children.Add(child);
            child.SetParent(this);
            AddFieldToRegistry(child);

            foreach (var childsChild in child._childrenByIds.Values)
                AddFieldToRegistry(childsChild);

            FieldAdded?.Invoke(child);
        }

        public bool RemoveField(Field child)
        {
            if (child == null) return false;
            bool removed = _children.Remove(child);
            if (removed)
            {
                child.ClearParent();
                RemoveFieldFromRegistry(child);

                FieldRemoved?.Invoke(child);
            }
            return removed;
        }

        public Field GetField(string name)
        {
            return _childrenByNames[name];
        }

        public Field GetField(Guid id)
        {
            return _childrenByIds[id];
        }
        
        public bool TryGetField(string name, out Field field)
        {
            return _childrenByNames.TryGetValue(name, out field);
        }

        public bool TryGetField(Guid id, out Field field)
        {
            return _childrenByIds.TryGetValue(id, out field);
        }

        public FieldAPI GetFieldAPI()
        {
            return _api;
        }

        public API GetAPI()
        {
            return GetFieldAPI();
        }

        protected void RaiseChanged()
        {
            Changed?.Invoke();
        }

        protected virtual void ValidateChild(Field field) { }

        protected override void ApplyPayload(Dictionary<string, JToken> payload)
        {
            AdditionalData = Get<Dictionary<string, object>>(payload, nameof(AdditionalData));
        }

        protected override void WritePayload(Dictionary<string, JToken> payload)
        {
            Set(payload, nameof(AdditionalData), AdditionalData);
        }

        private void AddFieldToRegistry(Field field)
        {
            _childrenByIds.Add(field.ID, field);
            _childrenByNames.Add(field.Name, field);

            field.FieldAdded += OnDescendantAdded;
            field.FieldRemoved += OnDescendantRemoved;
        }

        private void OnDescendantAdded(Field field)
        {
            AddFieldToRegistry(field);
            DescendantAdded?.Invoke(field);
        }

        private void OnDescendantRemoved(Field field)
        {
            RemoveFieldFromRegistry(field);
            DescendantRemoved?.Invoke(field);
        }

        private void RemoveFieldFromRegistry(Field field)
        {
            _childrenByIds.Remove(field.ID);
            _childrenByNames.Remove(field.Name);

            field.FieldAdded -= AddFieldToRegistry;
            field.FieldRemoved -= RemoveFieldFromRegistry;
        }

        private void ThrowValidateChildOnParent(Field field)
        {            
            if (_childrenByNames.ContainsKey(field.Name))
                throw new InvalidOperationException($"Unnable to child field {field.Name} [{field.ID}]: Field with such name already contains in the hierarchy tree of {Name} [{ID}]");

            if (_childrenByIds.ContainsKey(field.ID))
                throw new InvalidOperationException($"Unnable to child field {field.Name} [{field.ID}]: Field with such ID already contains in the hierarchy tree of {Name} [{ID}]");

            if (Parent != null)
                Parent.ThrowValidateChildOnParent(field);
        }

        private void SetParent(Field owner)
        {
            Parent = owner;
            ParentChanged?.Invoke(owner);
        }

        private void ClearParent()
        {
            Field oldParent = Parent;
            Parent = null;
            ParentRemoved?.Invoke(oldParent);
        }

        private FieldAPI CreateAPI(Type apiType)
        {
            FieldAPI api = null;

            if (apiType == null)
                return null;

            api = (FieldAPI)Activator.CreateInstance(apiType, this);

            if (api == null)
                throw new InvalidOperationException($"Type '{apiType}' isn't inherits FieldAPI class");

            return api;
        }
    }
}

