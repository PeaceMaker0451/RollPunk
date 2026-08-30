using MoonSharp.Interpreter;
using RollPunk.Modding;
using RollPunk.Modding.APIs;
using RollPunk.UIFields;

namespace RollPunk.UI.DynamicUI
{
    public class UIDocumentAPI : HeldAPI
    {
        private UIDocument _doc;

        public UIDocumentAPI(UIDocument handler) : base(handler)
        {
            _doc = handler;
        }

        public void addButton(string text, DynValue luaFunction)
        {
            _doc.AddButton(text, () => luaFunction.Function.Call());
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
    }
}
