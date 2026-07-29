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

        public async void showStringDialog(string title, DynValue optionsTable = null, DynValue callback = null, params DynValue[] parameters) 
        { 
            try
            {
                // Разбор параметров из optionsTable
                string placeholder = "";
                bool allowCancel = true;
                int minWidth = 350;
                int minHeight = 150;
                string okText = "Ок";
                string cancelText = "Отмена";

                if (optionsTable != null && optionsTable.Type == DataType.Table)
                {
                    var table = optionsTable.Table;
                    placeholder = table.Get("placeholder").CastToString() ?? placeholder;
                    allowCancel = table.Get("allowCancel").CastToBool(true);
                    minWidth = (int)(table.Get("minWidth").CastToNumber() ?? minWidth);
                    minHeight = (int)(table.Get("minHeight").CastToNumber() ?? minHeight);
                    okText = table.Get("okText").CastToString() ?? okText;
                    cancelText = table.Get("cancelText").CastToString() ?? cancelText;
                }

                var result = await _formsManager.Dialogs.ShowStringInput(title, placeholder, allowCancel, new Godot.Vector2(minWidth, minHeight), okText, cancelText);

                if (callback.IsNil() == false && callback.Type == DataType.Function)
                {
                    callback.Function.Call(result.IsConfirmed, result.Value, parameters);
                }
            }
            catch(Exception ex)
            {
                LuaErrorsHandler.Handle(ex);
            }
        }

        public async void showIntDialog(string title, DynValue optionsTable = null, DynValue callback = null, params DynValue[] parameters)
        {
            try
            {
                int? defaultValue = null;
                bool allowCancel = true;
                int minValue = int.MinValue;
                int maxValue = int.MaxValue;
                int step = 1;
                int minWidth = 300;
                int minHeight = 130;
                string okText = "Ок";
                string cancelText = "Отмена";

                if (optionsTable != null && optionsTable.Type == DataType.Table)
                {
                    var table = optionsTable.Table;
                    defaultValue = (int?)table.Get("defaultValue").CastToNumber();
                    allowCancel = table.Get("allowCancel").CastToBool(true);
                    minValue = (int)(table.Get("minValue").CastToNumber() ?? minValue);
                    maxValue = (int)(table.Get("maxValue").CastToNumber() ?? maxValue);
                    step = (int)(table.Get("step").CastToNumber() ?? step);
                    minWidth = (int)(table.Get("minWidth").CastToNumber() ?? minWidth);
                    minHeight = (int)(table.Get("minHeight").CastToNumber() ?? minHeight);
                    okText = table.Get("okText").CastToString() ?? okText;
                    cancelText = table.Get("cancelText").CastToString() ?? cancelText;
                }

                var result = await _formsManager.Dialogs.ShowIntInput(title, defaultValue, minValue, maxValue, step, allowCancel, new Godot.Vector2(minWidth, minHeight), okText, cancelText);

                if (callback.IsNil() == false && callback.Type == DataType.Function)
                {
                    callback.Function.Call(result.IsConfirmed, result.Value, parameters);
                }
            }
            catch (Exception ex)
            {
                LuaErrorsHandler.Handle(ex);
            }
        }

        public async void showInformationDialog(string title, DynValue optionsTable = null, DynValue callback = null, params DynValue[] parameters)
        { 
            try
            {
                string message = "";
                bool allowCancel = true;
                int minWidth = 400;
                int minHeight = 250;
                string okText = "Продолжить";

                if (optionsTable != null && optionsTable.Type == DataType.Table)
                {
                    var table = optionsTable.Table;
                    message = table.Get("message").CastToString() ?? message;
                    allowCancel = table.Get("allowCancel").CastToBool(true);
                    minWidth = (int)(table.Get("minWidth").CastToNumber() ?? minWidth);
                    minHeight = (int)(table.Get("minHeight").CastToNumber() ?? minHeight);
                    okText = table.Get("okText").CastToString() ?? okText;
                }

                await _formsManager.Dialogs.ShowInformation(title, message, new Godot.Vector2(minWidth, minHeight), allowCancel, okText);

                if (callback.IsNil() == false && callback.Type == DataType.Function)
                {
                    callback.Function.Call(parameters);
                }
            }
            catch (Exception ex)
            {
                LuaErrorsHandler.Handle(ex);
            }
        }

        public async void showConfirmationDialog(string title, DynValue optionsTable = null, DynValue callback = null, params DynValue[] parameters)
        {
            try
            {
                string message = "";
                bool allowCancel = true;
                int minWidth = 300;
                int minHeight = 120;
                string yesText = "Да";
                string noText = "Нет";

                if (optionsTable != null && optionsTable.Type == DataType.Table)
                {
                    var table = optionsTable.Table;
                    message = table.Get("message").CastToString() ?? message;
                    allowCancel = table.Get("allowCancel").CastToBool(true);
                    minWidth = (int)(table.Get("minWidth").CastToNumber() ?? minWidth);
                    minHeight = (int)(table.Get("minHeight").CastToNumber() ?? minHeight);
                    yesText = table.Get("yesText").CastToString() ?? yesText;
                    noText = table.Get("noText").CastToString() ?? noText;
                }

                var result = await _formsManager.Dialogs.ShowConfirmation(title, message, allowCancel, new Godot.Vector2(minWidth, minHeight), yesText, noText);

                if (callback.IsNil() == false && callback.Type == DataType.Function)
                {
                    callback.Function.Call(result.IsConfirmed, parameters);
                }
            }
            catch (Exception ex)
            {
                LuaErrorsHandler.Handle(ex);
            }
        }
    }
}
