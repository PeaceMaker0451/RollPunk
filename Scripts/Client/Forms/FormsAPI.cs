using MoonSharp.Interpreter;
using RollPunk.Modding;
using RollPunk.Modding.APIs;
using System;

namespace RollPunk.Client.Forms
{
    internal class FormsAPI : HeldAPI
    {
        private readonly IFormsManager _formsManager;
        
        public FormsAPI(IFormsManager formsManager) : base(formsManager)
        {
            _formsManager = formsManager;
        }

        public async void showStringDialog(string title, string placeholder = "", DynValue callback = null, params DynValue[] parameters) 
        { 
            try
            {
                string result = await _formsManager.Dialogs.ShowStringInput(title, placeholder);

                if (result != null && callback.IsNil() == false && callback.Type == DataType.Function)
                    callback.Function.Call(result, parameters);
            }
            catch(Exception ex)
            {
                LuaErrorsHandler.Handle(ex);
            }
        }

        public async void showIntDialog(string title, int? defaultValue = null, DynValue callback = null, params DynValue[] parameters)
        {
            try
            {
                int? result = await _formsManager.Dialogs.ShowIntInput(title, defaultValue);
                
                if (result.HasValue && callback.IsNil() == false && callback.Type == DataType.Function)
                    callback.Function.Call(result.Value, parameters);
            }
            catch (Exception ex)
            {
                LuaErrorsHandler.Handle(ex);
            }
        }

        public async void showInformationDialog(string title, string message, DynValue callback = null, params DynValue[] parameters)
        { 
            try
            {
                await _formsManager.Dialogs.ShowInformation(title, message);

                if (callback.IsNil() == false && callback.Type == DataType.Function)
                    callback.Function.Call(parameters);
            }
            catch (Exception ex)
            {
                LuaErrorsHandler.Handle(ex);
            }
        }

        public async void showConfirmationDialog(string title, string message, DynValue callback = null, params DynValue[] parameters)
        {
            try
            {
                bool result = await _formsManager.Dialogs.ShowConfirmation(title, message);

                if (callback.IsNil() == false && callback.Type == DataType.Function)
                    callback.Function.Call(result, parameters);
            }
            catch (Exception ex)
            {
                LuaErrorsHandler.Handle(ex);
            }
        }
    }
}
