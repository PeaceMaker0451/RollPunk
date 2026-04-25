using Newtonsoft.Json.Linq;
using System.Reflection;

namespace RollPunk.Entities
{
    public abstract class Entity
    {
        public string Name { get; protected set; }
        public Guid ID { get; private set; }

        public event Action? Updated;

        public Entity(string name = "")
        {
            GetEntityType();

            RandomizeID();
            Name = name;
        }

        public Entity(EntityState data)
        {
            if (data.EntityType != GetEntityType())
                throw new Exception($"Data type mismatch object Type - {data.EntityType} - {GetType().ToString()}");

            ApplyPayload(data.Payload);
            Name = data.Name;
            ID = data.ID;
        }

        public EntityState GetState()
        {
            var payload = new Dictionary<string, JToken>();
            WritePayload(payload);

            return new EntityState
            {
                EntityType = GetEntityType(),
                ID = ID,
                Name = Name,
                Payload = payload
            };
        }

        internal void RandomizeID()
        {
            ID = Guid.NewGuid(); 
        }

        internal void UpdateFromState(EntityState data)
        {
            if (data.EntityType != GetEntityType())
                throw new Exception($"Data type mismatch object Type - {data.EntityType} - {GetType().ToString()}");

            ApplyPayload(data.Payload);
            Name = data.Name;
            ID = data.ID;

            Updated?.Invoke();
        }

        public string GetEntityType()
        {
            return GetEntityType(GetType());
        }

        protected static T Get<T>(Dictionary<string, JToken> payload, string key)
        {
            if (!payload.TryGetValue(key, out var token))
                throw new Exception($"Missing key {key}");

            return token.ToObject<T>()!;
        }

        protected static void Set<T>(Dictionary<string, JToken> payload, string key, T value)
        {
            payload[key] = JToken.FromObject(value!);
        }

        protected abstract void ApplyPayload(Dictionary<string, JToken> payload);
        protected abstract void WritePayload(Dictionary<string, JToken> payload);

        private static string GetEntityType(Type type)
        {
            var attr = type.GetCustomAttribute<EntityTypeAttribute>()
                ?? throw new Exception($"{type.Name} missing EntityTypeAttribute");

            return attr.Id;
        }
    }
}