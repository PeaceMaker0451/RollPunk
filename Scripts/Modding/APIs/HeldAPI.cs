using MoonSharp.Interpreter;

namespace RollPunk.Modding.APIs
{
    public abstract class HeldAPI : API
    {
        protected IAPIHandler _handler;

        public HeldAPI(IAPIHandler handler)
        {
            _handler = handler;
        }

        [MoonSharpHidden]
        public IAPIHandler GetHandler()
        {
            return _handler;
        }
    }
}