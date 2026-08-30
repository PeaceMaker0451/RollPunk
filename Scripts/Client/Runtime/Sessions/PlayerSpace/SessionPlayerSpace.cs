using RollPunk.Client.Forms;
using RollPunk.ClientSide.Runtime.UI;
using RollPunk.HierarchyFields;
using RollPunk.Modding.APIs;
using RollPunk.UI.DynamicUI;
using System;

namespace RollPunk.Client.Game
{
    public class SessionPlayerSpace : IAPIHandler
    {
        private SessionPlayerSpaceAPI _api;

        private SessionViewController _sessionViewController;
        private SessionView _sessionView;

        private EditorController _editorController;
        private Editor _editor;

        private ClientSession _session;

        public event Action DisplayedEntityChanged;

        public string ActionsTabName { get; set; }
        public UIDocument Actions { get; private set; } = new();

        public EntityField DisplayedEntity { get; private set; }

        public SessionPlayerSpace(ClientSession session)
        {
            _session = session;
            _api = new(this);

            EnsureEditorState();
            _session.PatchInserted += EnsureEditorState;
            _session.StateInserted += EnsureEditorState;
        }

        public API GetAPI()
        {
            return _api;
        }

        public void DisplayEntity(EntityField field)
        {
            DisplayedEntity = field;
            DisplayedEntityChanged?.Invoke();
        }

        public void OpenGameView(bool newWindow)
        {
            FormDisplayMode window = newWindow ? FormDisplayMode.NewWindow : FormDisplayMode.MainTab;


            if (_sessionViewController == null)
            {
                _sessionViewController = new SessionViewController();
                _sessionView = Root.Forms.OpenWith(_sessionViewController, window);
            }
            else
            {
                if(newWindow)
                    _sessionView.MoveToNewWindow();
                else
                    _sessionView.MoveToMainTab();
            }

            _sessionViewController.SetSession(_session);
        }

        public void CloseGameView()
        {
            _sessionViewController.Close();
            _sessionViewController = null;
        }

        public void Dispose()
        {
            if (_sessionView != null)
                _sessionView.Close();
        }

        private void EnsureEditorState()
        {
            bool isPlayerAdmin = true;

            if(_session.CurrentPlayer != null && _session.CurrentPlayer.IsAdmin)
                isPlayerAdmin = true;

            if (isPlayerAdmin)
            {
                if (_editor == null)
                    CreateEditor();
            }
            else
            {
                if (_editor != null)
                    CloseEditor();
            }
        }

        private void CreateEditor()
        {
            _editorController = new EditorController();
            _editor = Root.Forms.OpenWith(_editorController, FormDisplayMode.MainTab);
            _editorController.SetSession(_session);
        }

        private void CloseEditor()
        {
            _editor.Close();
            _editor = null;
            _editorController = null;
        }
    }
}
