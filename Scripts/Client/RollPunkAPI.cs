using Godot;
using RollPunk.Debug;
using RollPunk.Modding.APIs;

namespace RollPunk.ClientSide.Runtime
{
    public partial class RollPunkAPI : API
    {
        public RollPunkAPI() { }

        public void log(string log)
        {
            RPDebug.Log(log);
        }

        public void logError(string log)
        {
            RPDebug.LogError(log);
        }

        public void print(string log)
        {
            GD.Print(log);
        }
    }
}