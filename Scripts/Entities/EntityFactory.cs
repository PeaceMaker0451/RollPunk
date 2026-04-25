using System.Reflection;

namespace RollPunk.Entities
{
    public sealed class EntityFactory
    {
        private readonly Dictionary<string, Type> types = new();

        public void Register<T>() where T : Entity
        {
            var type = typeof(T);
            Register(type);
        }

        public void Register(Type type)
        {
            var attr = type.GetCustomAttribute<EntityTypeAttribute>()
                ?? throw new Exception($"{type.Name} missing EntityTypeAttribute");

            if (types.ContainsKey(attr.Id))
                throw new Exception($"Duplicate entity type id: {attr.Id}");

            types.Add(attr.Id, type);
        }

        public Entity Create(EntityState state)
        {
            if(state == null)
                throw new NullReferenceException($"EntityState is null!!");

            if (state.EntityType == null)
                throw new NullReferenceException($"entity type is null!!");

            if (types.TryGetValue(state.EntityType, out var type) == false)
                throw new Exception($"Unknown entity type: {state.EntityType}");

            return (Entity)Activator.CreateInstance(type, state)!;
        }

        public Entity Clone(EntityState state)
        {
            Entity entity = Create(state);
            entity.RandomizeID();
            return entity;
        }
    }
}
