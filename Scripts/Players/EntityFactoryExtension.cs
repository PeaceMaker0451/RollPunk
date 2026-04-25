using RollPunk.Entities;

namespace RollPunk.Players
{
    public static class EntityFactoryExtension
    {
        public static void RegisterPlayers(this EntityFactory factory)
        {
            factory.Register<Player>();
        }
    }
}
