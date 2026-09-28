using RollPunk.UI.DynamicUI;

namespace RollPunk.UI.Forms
{
    internal class UIDocumentDialogAPI : FormAPI
    {
        UIDocimentDialog _dialog;

        public UIDocumentAPI? Document => _dialog.UIDocument?.GetAPI() as UIDocumentAPI;
        
        public UIDocumentDialogAPI(UIDocimentDialog handler) : base(handler)
        {
            _dialog = handler;
        }
    }
}
