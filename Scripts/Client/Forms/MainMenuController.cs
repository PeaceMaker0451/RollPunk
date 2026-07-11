using RollPunk.Client.Runtime;
using System;

namespace RollPunk.Client.Forms
{
    internal class MainMenuController : IFormController
    {
        private readonly MainMenu _view;
        private readonly RollPunkRuntime _runtime;

        public MainMenuController(MainMenu view, RollPunkRuntime runtime)
        {
            _view = view;
            _runtime = runtime;
        }

        public void Initialize()
        {
            _view.CreateSessionRequested += () => _runtime.StartSession(_runtime.ReadedMods);
            _view.EnterSessionRequested += (address) => _runtime.TryConnectToSession(address, _runtime.ReadedMods);
            
            _runtime.StateChanged += OnStateChanged;
            OnStateChanged();
        }

        private void OnStateChanged()
        {
            _view.SetInSession(_runtime.State == RollPunkState.Session);
        }
    }
}
