using RollPunk.Entities;
using System.Data;
using System.Reflection;
using System.Linq;

namespace RollPunk.UIFields
{
    public static class EntityFactoryExtension
    {
        public static void RegisterLineFields(this EntityFactory factory)
        {
            var lineFieldTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(LineField)) && t.GetCustomAttribute<EntityTypeAttribute>() != null)
                .ToList();

            foreach (var type in lineFieldTypes)
            {
                if(type.GetCustomAttribute<EntityTypeAttribute>() != null);
                    factory.Register(type);
            }
        }
    }
}
