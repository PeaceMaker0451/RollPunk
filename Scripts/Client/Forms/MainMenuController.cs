using RollPunk.Client.Game;

namespace RollPunk.Client.Forms
{
    internal class MainMenuController : IFormPresenter<MainMenu>
    {
        private readonly Runtime _runtime;
        private MainMenu _view;

        public MainMenuController(Runtime runtime)
        {
            _runtime = runtime;
        }

        private void OnStateChanged()
        {
            _view.SetInSession(_runtime.State == RollPunkState.Session);
        }

        public void Attach(MainMenu form)
        {
            _view = form;

            _view.Initialize(_runtime.ReadedMods);
            _view.CreateSessionRequested += () => _runtime.StartSession();
            _view.EnterSessionRequested += (address) => _runtime.TryConnectToSession(address);
            _view.ExitSessionRequested += () => _runtime.KillSession();

            _runtime.StateChanged += OnStateChanged;
            OnStateChanged();
        }
    }
}
