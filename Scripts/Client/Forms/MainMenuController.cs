using RollPunk.Client.Runtime;
using System;

namespace RollPunk.Client.Forms
{
    internal class MainMenuController : IFormController<MainMenu>
    {
        public MainMenu View { get; set; }
        Form IFormControllerBase.View
        {
            get => View;
            set => View = (MainMenu)value;
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
            
            _runtime.StateChanged += OnStateChanged;
            OnStateChanged();
        }

        private void OnStateChanged()
        {
            View.SetInSession(_runtime.State == RollPunkState.Session);
        }
    }
}
