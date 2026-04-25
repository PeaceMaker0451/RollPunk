using Godot;
using RollPunk.Client.Runtime;

namespace RollPunk.Client
{
    internal abstract partial class SubMenu : Control
    {
        protected MainMenu Menu {  get; private set; }

        public void Initialize(MainMenu menu)
        {
            Menu = menu;
        }
    }
}
