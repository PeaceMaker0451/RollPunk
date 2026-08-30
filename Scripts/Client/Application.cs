using RollPunk.Client.Forms;
using RollPunk.Modding;
using RollPunk.Scripts.Client.Forms;

namespace RollPunk.Client.Game
{
    internal class Application
    {
        private MainMenuController _mainMenuController;
        private ConsoleController _consoleController;

        public Application()
        {
            LuaErrorsHandler.ErrorLogged += (error) => _ = Root.Forms.Dialogs.ShowInformation("LuaError", error);
            CreateControllers();

            Root.Runtime.SessionStateChanged += OnSessionStateChanged;
        }

        private void OnSessionStateChanged()
        {
            switch(Root.Runtime.SessionState)
            {
                case RuntimeSessionState.None:

                    break;

                case RuntimeSessionState.InSession:

                    break;
            }
        }

        private void CreateControllers()
        {
            _mainMenuController = new MainMenuController();
            Root.Forms.OpenWith<MainMenu>(_mainMenuController, FormDisplayMode.MainTab, int.MaxValue);

            _consoleController = new ConsoleController(Root.Console);
            Root.Forms.OpenWith(_consoleController, FormDisplayMode.MainTab, int.MinValue);
        }
    }
}
