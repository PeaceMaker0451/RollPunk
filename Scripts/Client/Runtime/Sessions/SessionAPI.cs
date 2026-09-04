using MoonSharp.Interpreter;
using RollPunk.AccessPolicy;
using RollPunk.Debug;
using RollPunk.Fields;
using RollPunk.HierarchyFields;
using RollPunk.Modding;
using RollPunk.Modding.APIs;
using RollPunk.Players;
using System;
using System.Linq;

namespace RollPunk.Client.Game.Sessions
{
    public class SessionAPI : HeldAPI
    {
        private ClientSession _session;

        public OwnersRegistryAPI OwnersRegistry { get; private set; }
        public SessionPlayerSpaceAPI PlayerSpace => _session.PlayerSpace.GetAPI() as SessionPlayerSpaceAPI;

        public PlayerAPI current_player => (PlayerAPI)_session.CurrentPlayer.GetAPI();
        public EntityFieldAPI[] entities => _session.Entities.List.Select(field => field.GetAPI() as EntityFieldAPI).ToArray();


        public SessionAPI(ClientSession handler) : base(handler)
        {
            _session = handler;
            OwnersRegistry = new OwnersRegistryAPI(_session.OwnersRegistry);
        }

        public void addEntity(EntityFieldAPI field)
        {
            try
            {
                _session.AddEntity((EntityField)field.GetField());
            }
            catch(Exception ex)
            {
                LuaErrorsHandler.Handle(ex);
                throw;
            }
        }

        public bool removeEntity(EntityFieldAPI field)
        {
            try
            {
                return _session.RemoveEntity((EntityField)field.GetField());
            }
            catch (Exception ex)
            {
                LuaErrorsHandler.Handle(ex);
                throw;
            }
        }

        public EntityFieldAPI? getEntity(string name)
        {
            return _session.Entities.GetByName(name)?.GetAPI() as EntityFieldAPI;
        }

        public EntityFieldAPI? getEntityById(string id)
        {
            var guid = Guid.Parse(id);

            if (guid == Guid.Empty)
                return null;

            return _session.Entities.GetById(guid)?.GetAPI() as EntityFieldAPI;
        }

        public void saveString(string value)
        {
            try
            {
                Root.FileDebugUtils.SaveStringWithDialog(value);
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

                Root.FileDebugUtils.LoadStringWithDialog((data) =>
                {
                    try
                    {
                        RPDebug.Log($"Файл загружен - \n{data}\n - передаем в lua");
                        function.Function.Call(data);
                    }
                    catch(Exception ex)
                    {
                        LuaErrorsHandler.Handle(ex);
                        throw;
                    }
                });
            }
            catch(Exception e)
            {
                LuaErrorsHandler.Handle(e);
                throw;
            }
        }

        public void addUserLog(string source, string message)
        {
            _session.AddLog(new(source, Logs.SourceType.User, message, DateTime.Now));
        }
        public void addSystemLog(string source, string message)
        {
            _session.AddLog(new(source, Logs.SourceType.System, message, DateTime.Now));
        }
    }
}
