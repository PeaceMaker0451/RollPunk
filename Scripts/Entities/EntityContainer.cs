using Newtonsoft.Json.Linq;
using System.Data;
using System.Text;

namespace RollPunk.Entities
{
    public class EntityContainer<T> where T : Entity
    {
        private Dictionary<Guid, T> _objects = new();

        public event Action<T>? Added;
        public event Action<T>? Removed;

        public IReadOnlyList<T> List => _objects.Values.ToList();
        public IReadOnlyDictionary<Guid, T> Dictionary => _objects;

        public void Add(T newObject)
        {
            if (_objects.ContainsKey(newObject.ID))
                throw new Exception("Entity is already added to this container");

            bool shouldAdd = ValidateAdding(newObject);

            if (shouldAdd)
            {
                _objects.Add(newObject.ID, newObject);
                Added?.Invoke(newObject);
            }
            else
                throw new InvalidOperationException("Entity adding didn't pass validation");
        }

        public bool Remove(Guid id)
        {
            if (_objects.TryGetValue(id, out T serializableObject))
            {
                bool shouldRemove = ValidateRemoving(serializableObject);

                if(shouldRemove)
                {
                    bool isRemoved = _objects.Remove(id);
                    
                    if(isRemoved)
                    Removed?.Invoke(serializableObject);

                    return isRemoved;
                }
            }

            return false;
        }

        public bool Remove(T objectToRemove)
        {
            return Remove(objectToRemove.ID);
        }

        protected virtual bool ValidateAdding(T newObject) { return true; }
        protected virtual bool ValidateRemoving(T objectToRemove) { return true; }
    }
}
