using RollPunk.Client.Game;
using RollPunk.Modding;
using RollPunk.UI.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RollPunk.Client.Forms
{
    internal class MainMenuController : IFormController<MainMenu>
    {
        private readonly Runtime _runtime;
        private MainMenu _view;

        public MainMenu View => _view;
        public string FormPath => "res://Scenes/FormsScenes/MainMenu.tscn";
        public IFormHandle FormHandle {  get; private set; }

        public MainMenuController(Runtime runtime)
        {
            _runtime = runtime;
        }

        public void Initialize()
        {
            View.Initialize(_runtime.ReadedMods);
            View.CreateSessionRequested += () => _runtime.StartSession();
            View.EnterSessionRequested += (address) => _runtime.TryConnectToSession(address);
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
