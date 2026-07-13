using RollPunk.Client.Runtime;
using RollPunk.UI.Forms;
using System;

namespace RollPunk.Client.Forms
{
    internal class MainMenuController : IFormController<MainMenu>
    {
        private MainMenu _view;
        public MainMenu View => _view;

        public void SetView(MainMenu view)
        {
            _view = view;
        }

        public string FormPath => "res://Scenes/FormsScenes/MainMenu.tscn";
        
        private readonly RollPunkRuntime _runtime;

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

        private void OnStateChanged()
        {
            View.SetInSession(_runtime.State == RollPunkState.Session);
        }
    }
}
