using RollPunk.Modding.APIs;

namespace RollPunk.Client.Forms
{
    internal class FormsAPI : HeldAPI
    {
        private readonly IFormsManager _formsManager;

        public DialogFactoryAPI Dialogs => _formsManager.Dialogs.GetAPI() as DialogFactoryAPI;
        
        public FormsAPI(IFormsManager formsManager) : base(formsManager)
        {
            _formsManager = formsManager;
        }
    }
}
