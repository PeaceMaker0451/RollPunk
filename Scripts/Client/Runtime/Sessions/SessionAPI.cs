using MoonSharp.Interpreter;
using RollPunk.AccessPolicy;
using RollPunk.Debug;
using RollPunk.HierarchyFields;
using RollPunk.Modding;
using RollPunk.Modding.APIs;
using RollPunk.Players;
using System;

namespace RollPunk.Client.Runtime.Sessions
{
    public class SessionAPI : HeldAPI
    {
        private Session _session;

        public OwnersRegistryAPI OwnersRegistry { get; private set; }

        public PlayerAPI current_player => (PlayerAPI)_session.CurrentPlayer.GetAPI();
        
        public SessionAPI(Session handler) : base(handler)
        {
            _session = handler;
            OwnersRegistry = new OwnersRegistryAPI(_session.OwnersRegistry);
        }

        public void addEntityField(EntityFieldAPI field)
        {
            try
            {
                _session.Entities.Add((EntityField)field.GetField());
            }
            catch(Exception ex)
            {
                LuaErrorsHandler.Handle(ex);
                throw;
            }
        }

        public bool removeEntityField(EntityFieldAPI field)
        {
            try
            {
                return _session.Entities.Remove((EntityField)field.GetField());
            }
            catch (Exception ex)
            {
                LuaErrorsHandler.Handle(ex);
                throw;
            }
        }

        public void saveString(string value)
        {
            try
            {
                Client.Instance.FileDebugUtils.SaveStringWithDialog(value);
            }
            catch (Exception e)
            {
                LuaErrorsHandler.Handle(e);
                throw;
            }
        }

        public void loadString(DynValue function)
        {
            try
            {
                if (function == null || function.Type != DataType.Function)
                    throw new InvalidOperationException("Передана хуйня, должна быть функция");

                Client.Instance.FileDebugUtils.LoadStringWithDialog((data) =>
                {
                    RPDebug.Log($"Файл загружен - \n{data}\n - передаем в lua");
                    function.Function.Call(data);
                });
            }
            catch(Exception e)
            {
                LuaErrorsHandler.Handle(e);
                throw;
            }
        }
    }
}
