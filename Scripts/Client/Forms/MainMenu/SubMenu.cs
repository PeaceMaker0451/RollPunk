using Godot;
using RollPunk.Client.Game;
using System;
using System.Security.Cryptography.X509Certificates;

namespace RollPunk.Client
{
    internal abstract partial class SubMenu : Control
    {
        protected MainMenu Menu {  get; private set; }

        public void Initialize(MainMenu menu)
        {
            Menu = menu;
            OnInitialize();
        }

        protected virtual void OnInitialize() { }

        public void Open() { OnOpen(); this.Show(); }
        protected virtual void OnOpen() { }
    }
}
