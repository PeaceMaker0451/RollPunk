using Godot;
using RollPunk.Client.Game;

namespace RollPunk.Client
{
    internal abstract partial class SubMenu : Control
    {
        protected MainMenu Menu {  get; private set; }

        public void Initialize(MainMenu menu)
        {
            Menu = menu;
        }

        public void Open() { OnOpen(); this.Show(); }
        protected virtual void OnOpen() { }
    }
}
