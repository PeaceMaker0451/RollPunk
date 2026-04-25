using Discord;
using Newtonsoft.Json;
using RollPunk.Debug;
using RollPunk.Entities;
using RollPunk.Fields;
using RollPunk.Modding.APIs;

namespace RollPunk.Client
{
    public class Serializator: IAPIHandler
    {
        private SerializatorAPI _api;
        private EntityFactory _entityFactory;
        private FieldsHierarchyReconstructor _fieldsHierarchyReconstructor;
        
        public Serializator(EntityFactory entityFactory, FieldsHierarchyReconstructor fieldsHierarchyReconstructor) 
        {
            _api = new(this);
            
            _entityFactory = entityFactory;
            _fieldsHierarchyReconstructor = fieldsHierarchyReconstructor;
        }

        public string SerializeEntity(Entity entity)
        {
            var settings = new JsonSerializerSettings
            {
                //Formatting = Formatting.Indented
            };

            string output = JsonConvert.SerializeObject(entity.GetState(), settings); // Применяем настройки
            RPDebug.DebugLog($"Сериализация объекта {entity.GetState()} завершена: \n{output}");
            return output;
        }

        public string SerializeFieldTree(Field field)
        {
            var settings = new JsonSerializerSettings
            {
                //Formatting = Formatting.Indented
            };

            TreeState fieldState = FieldStateExtractor.ExtractFieldTreeState(field);
            
            string output = JsonConvert.SerializeObject(fieldState, settings); // Применяем настройки
            RPDebug.DebugLog($"Сериализация объекта {fieldState} завершена: \n{output}");
            return output;
        }

        public Entity DeserializeEntity(string json)
        {
            EntityState entityState = JsonConvert.DeserializeObject<EntityState>(json);
            var entity = _entityFactory.Create(entityState);
            RPDebug.DebugLog($"Десериализация строки \n{json}\n завершена: {entity.GetState()}");
            return entity;
        }

        public Field DeserializeFieldTree(string json)
        {
            TreeState fieldState = JsonConvert.DeserializeObject<TreeState>(json);
            Field field = _fieldsHierarchyReconstructor.CreateFieldsTree(fieldState);
            RPDebug.DebugLog($"Десериализация строки \n{json}\n завершена: {field.GetState()}");
            return field;
        }

        public API GetAPI()
        {
            return _api;
        }
    }
}
