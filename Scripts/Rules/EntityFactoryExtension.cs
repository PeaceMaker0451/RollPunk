using RollPunk.Entities;

namespace RollPunk.Rules
{
    public static class EntityFactoryExtension
    {
        public static void RegisterRules(this EntityFactory factory)
        {
            factory.Register<Rule>();
        }
    }
}
