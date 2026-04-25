using MoonSharp.Interpreter;
using RollPunk.Modding;
using RollPunk.Modding.APIs;
using System;
using System.Linq;

namespace RollPunk.Fields
{
    public class FieldAPI : HeldAPI
    {
        readonly private Field _fieldHandler;

        public string name => _fieldHandler.Name;
        public string id => _fieldHandler.ID.ToString();
        public FieldAPI parent => _fieldHandler.Parent?.GetAPI() as FieldAPI;
        public FieldAPI[] children => _fieldHandler.Fields.Select(field => field.GetAPI() as FieldAPI).ToArray();

        public FieldAPI(Field handler) : base(handler)
        {
            _fieldHandler = handler;
        }

        [MoonSharpHidden]
        public Field GetField()
        {
            return _fieldHandler;
        }

        public void setName(string name)
        {
            _fieldHandler.SetName(name);
        }

        public FieldAPI getParentField()
        {
            if (_fieldHandler.Parent != null)
                return _fieldHandler.Parent.GetFieldAPI();

            return null;
        }

        public void setAdditionalDataField(string name, object value)
        {
            _fieldHandler.SetAdditionalDataField(name, value);
        }

        public object getAdditionalDataField(string name)
        {
            return _fieldHandler.GetAdditionalDataField(name);
        }

        public void addField(FieldAPI field)
        {
            try
            {
                _fieldHandler.AddField(field.GetField());
            }
            catch(Exception ex)
            {
                LuaErrorsHandler.Handle(ex);
            }
        }

        public bool removeField(FieldAPI field)
        {
            try
            {
                return _fieldHandler.RemoveField(field.GetField());
            }
            catch (Exception ex)
            {
                LuaErrorsHandler.Handle(ex);
                return false;
            }
        }

        public FieldAPI getField(string name)
        {
            try
            {
                var field = _fieldHandler.Registry.GetField(name);

                if (field == null)
                    throw new Exception($"Unnable to find field \"{name}\"");

                return field.GetFieldAPI();
            }
            catch (Exception ex)
            {
                LuaErrorsHandler.Handle(ex);
                return null;
            }
        }
    }
}
