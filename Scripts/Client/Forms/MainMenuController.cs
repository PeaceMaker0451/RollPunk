using RollPunk.Client.Runtime;
using RollPunk.UI.Forms;
using System;

namespace RollPunk.Client.Forms
{
    internal class MainMenuController : IFormController<MainMenu>
    {
        private readonly RollPunkRuntime _runtime;
        private MainMenu _view;

        public MainMenu View => _view;
        public string FormPath => "res://Scenes/FormsScenes/MainMenu.tscn";
        public IFormHandle FormHandle {  get; private set; }

        public MainMenuController(RollPunkRuntime runtime)
        {
            _runtime = runtime;
        }

        public void Initialize()
        {
            View.CreateSessionRequested += () => _runtime.StartSession(_runtime.ReadedMods);
            View.EnterSessionRequested += (address) => _runtime.TryConnectToSession(address, _runtime.ReadedMods);
            View.ExitSessionRequested += () => _runtime.KillSession();
            
            _runtime.StateChanged += OnStateChanged;
            OnStateChanged();
        }

        public void SetView(MainMenu view)
        {
            _view = view;
        }

        public void SetFormHandle(IFormHandle handle)
        {
            FormHandle = handle;
        }

        private void OnStateChanged()
        {
            View.SetInSession(_runtime.State == RollPunkState.Session);
        }
    }
}
