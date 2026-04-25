using MoonSharp.Interpreter;
using RollPunk.Modding;
using RollPunk.Modding.APIs;
using System;

namespace RollPunk.Client
{
    internal class UIAPI : HeldAPI
    {
        UIController _ui;
        
        public UIAPI(UIController handler) : base(handler)
        {
            _ui = handler;
        }

        public void setMainFrameInputActive(bool active) => _ui.SetMainFrameInputActive(active);
        public async void openStringDialogue(string title, DynValue callback = null, params DynValue[] parameters) 
        { 
            try
            {
                string result = await _ui.OpenStringDialogue(title);

                if (callback.IsNil() == false && callback.Type == DataType.Function)
                    callback.Function.Call(result, parameters);
            }
            catch(Exception ex)
            {
                LuaErrorsHandler.Handle(ex);
            }

        }

        public async void openIntDialogue(string title, DynValue callback = null, params DynValue[] parameters)
        {
            try
            {
                int result = Int32.Parse(await _ui.OpenStringDialogue(title));

                if (callback.IsNil() == false && callback.Type == DataType.Function)
                    callback.Function.Call(result, parameters);
            }
            catch (Exception ex)
            {
                LuaErrorsHandler.Handle(ex);
            }
        }

        public async void openInformationDialogue(string title, string information, DynValue callback = null, params DynValue[] parameters)
        { 
            try
            {
                await _ui.OpenInformationDialogue(title, information);

                if (callback.IsNil() == false && callback.Type == DataType.Function)
                    callback.Function.Call(parameters);
            }
            catch (Exception ex)
            {
                LuaErrorsHandler.Handle(ex);
            }
        }
    }
}
