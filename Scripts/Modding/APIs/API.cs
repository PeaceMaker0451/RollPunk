namespace RollPunk.Modding.APIs
{
    public abstract class API
    {
        public readonly string type;

        public API()
        {
            type = GetType().Name;
        }
    }
}
