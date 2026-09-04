using RollPunk.HierarchyFields;
using RollPunk.Modding.APIs;
using RollPunk.UI.DynamicUI;

namespace RollPunk.Client.Game
{
    public class SessionPlayerSpaceAPI : HeldAPI
    {
        private SessionPlayerSpace _space;

        public UIDocumentAPI Actions => _space.Actions.GetAPI() as UIDocumentAPI;

        public SessionPlayerSpaceAPI(SessionPlayerSpace handler) : base(handler)
        {
            _space = handler;
        }

        public void setActionsTabName(string name)
        {
            _space.SetActionsTabName(name);
        }

        public void setDisplayedEntity(EntityFieldAPI field)
        {
            _space.DisplayEntity(field.GetField() as EntityField);
        }

        public void openSessionView(bool new_window)
        {
            _space.OpenGameView(new_window);
        }

        public void closeSessionView()
        {
            _space.CloseGameView();
        }
    }
}
