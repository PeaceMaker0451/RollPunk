using RollPunk.Debug;
using RollPunk.Entities;
using RollPunk.Fields;
using RollPunk.Modding;
using RollPunk.Modding.APIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RollPunk.Client
{
    public class SerializatorAPI : HeldAPI
    {
        private Serializator _serializator;
        
        public SerializatorAPI(Serializator handler) : base(handler)
        {
            _serializator = handler;
        }

        public string serialize(HeldAPI heldAPI)
        {
            try
            {
                if (heldAPI.GetHandler() is Entity entity == false)
                    throw new InvalidOperationException("API is not handled by Entity!!");

                return _serializator.SerializeEntity(entity);
            }
            catch (Exception e)
            {
                LuaErrorsHandler.Handle(e);
                throw;
            }
        }

        public API deserialize(string entityJson)
        {
            try
            {
                Entity entity = _serializator.DeserializeEntity(entityJson);

                if (entity is IAPIHandler apiHandler == false)
                    throw new InvalidOperationException("Converterd entity is not APIHandler!");

                return apiHandler.GetAPI();
            }
            catch (Exception e)
            {
                LuaErrorsHandler.Handle(e);
                throw;
            }
        }

        public string serializeField(FieldAPI fieldAPI)
        {
            try
            {
                var field = fieldAPI.GetField();
                return _serializator.SerializeFieldTree(field);
            }
            catch (Exception e)
            {
                LuaErrorsHandler.Handle(e);
                throw;
            }
        }

        public API deserializeField(string fieldJson)
        {
            try
            {
                Field field = _serializator.DeserializeFieldTree(fieldJson);
                return field.GetAPI();
            }
            catch (Exception e)
            {
                LuaErrorsHandler.Handle(e);
                throw;
            }
        }
    }
}
