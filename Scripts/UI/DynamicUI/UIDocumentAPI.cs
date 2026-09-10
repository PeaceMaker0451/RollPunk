using MoonSharp.Interpreter;
using RollPunk.Modding;
using RollPunk.Modding.APIs;
using System;

namespace RollPunk.UI.DynamicUI
{
    public class UIDocumentAPI : HeldAPI
    {
        private UIDocument _doc;

        public UIDocumentAPI(UIDocument handler) : base(handler)
        {
            _doc = handler;
        }

        public void addButton(Script script, string text, DynValue luaFunction)
        {
            _doc.AddButton(text, () =>
            {
                try
                {
                    script.Call(luaFunction);
                }
                catch (ScriptRuntimeException ex)
                {
                    LuaErrorsHandler.Handle(ex, $"Button \"{text}\"' action throws an exception");
                }
                catch (Exception ex)
                {
                    LuaErrorsHandler.Handle(ex, $"Button \"{text}\"' action throws an exception");
                }
            });
        }

        public void addLabel(string text)
        {
            _doc.AddLabel(text);
        }

        public UIDocumentAPI addContainer(string text)
        {
            var containerData = _doc.AddContainer(text);
            return containerData.Content.GetAPI() as UIDocumentAPI;
        }

        public void clear() => _doc.Clear();
    }
}
