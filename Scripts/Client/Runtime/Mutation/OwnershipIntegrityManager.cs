using RollPunk.AccessPolicy;
using RollPunk.HierarchyFields;

namespace RollPunk.Client.Game
{
    internal class OwnershipIntegrityManager
    {
        private ClientSession _session;
        private bool _isIgnoringChanges = false;

        public OwnershipIntegrityManager(ClientSession session)
        {
            _session = session;
            _session.Entities.Added += OnEntityFieldAdded;
            _session.Entities.Removed += OnEntityFieldRemoved;
        }

        public void StartIgnore() => _isIgnoringChanges = true;
        public void StopIgnore() => _isIgnoringChanges = false;

        private void OnEntityFieldAdded(EntityField entity)
        {
            if (_isIgnoringChanges) return;

            // Автоматически создаем EntityOwnership для новой EntityField
            var ownership = new EntityOwnership(entity.ID, $"Ownership_{entity.Name}");
            _session.Ownerships.Add(ownership);
        }

        private void OnEntityFieldRemoved(EntityField entity)
        {
            if (_isIgnoringChanges) return;

            // Каскадное удаление связанных EntityOwnership
            var ownership = _session.OwnersRegistry.GetOwnership(entity.ID);
            if (ownership != null)
                _session.Ownerships.Remove(ownership);
        }
    }
}
