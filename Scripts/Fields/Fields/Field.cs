using Newtonsoft.Json.Linq;
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
        private readonly FieldsRegistry _registry;

        public event Action NameChanged;
        public event Action<string> AdditionalDataChanged;
        public event Action<Field> ChildAdded;
        public event Action<Field> ChildRemoved;
        public event Action<Field> ParentChanged;
        public event Action<Field> ParentRemoved;
        public event Action Changed;

        public Dictionary<string, object> AdditionalData { get; private set; } = new();
        public Field Parent { get; private set; }
        public IReadOnlyList<Field> Fields => _children;
        public IReadOnlyFieldRegistry Registry => _registry;

        public Field(string name, Type apiType, Dictionary<string, object> additionalData = null) : base(name)
        {
            if (additionalData != null)
                AdditionalData = additionalData;

            FieldAPI api = CreateAPI(apiType);
            _api = api;

            _registry = new(this);
        }

        public Field(EntityState data, Type apiType) : base(data)
        {
            FieldAPI api = CreateAPI(apiType);
            _api = api;

            _registry = new(this);
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
                throw new InvalidOperationException("Cannot add field: remove child field from it's parent first!");

            ValidateChild(child);

            if (child.IsAncestorOf(this))  
                throw new InvalidOperationException("Cannot add field: operation would create ownership cycle.");

            _children.Add(child);
            child.SetParent(this);
            ChildAdded?.Invoke(child);
        }

        public bool RemoveField(Field child)
        {
            if (child == null) return false;
            bool removed = _children.Remove(child);
            if (removed)
            {
                child.ClearParent();
                ChildRemoved?.Invoke(child);
            }
            return removed;
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

        protected virtual void ValidateChild(Field field)
        {

        }

        protected override void ApplyPayload(Dictionary<string, JToken> payload)
        {
            AdditionalData = Get<Dictionary<string, object>>(payload, nameof(AdditionalData));
        }

        protected override void WritePayload(Dictionary<string, JToken> payload)
        {
            Set(payload, nameof(AdditionalData), AdditionalData);
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

