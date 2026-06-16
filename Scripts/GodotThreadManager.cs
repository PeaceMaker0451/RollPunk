using Godot;
using RollPunk.NetcodeCommon;
using System;

namespace RollPunk.ClientNetcode
{
    public partial class GodotThreadManager : Node
    {
        public ThreadManager ThreadManager {  get; private set; }

        public GodotThreadManager()
        {
            ThreadManager = new ThreadManager();
        }

        public override void _Process(double delta)
        {
            ThreadManager.UpdateMain();
        }

        public void ExecuteOnMainThread(Action action)
        {
            ThreadManager.ExecuteOnMainThread(action);
        }
    }
}
