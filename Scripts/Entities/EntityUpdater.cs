namespace RollPunk.Entities
{
    public class EntityUpdater
    {
        public void UpdateEntity(Entity entity, EntityState entityState)
        {
            if (entity.ID != entityState.ID)
                throw new InvalidOperationException("Entities ID's missmatch!");

            entity.UpdateFromState(entityState);
        }
    }
}
