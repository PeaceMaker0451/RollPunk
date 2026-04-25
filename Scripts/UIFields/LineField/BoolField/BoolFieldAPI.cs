namespace RollPunk.UIFields
{
    public sealed class BoolFieldAPI : LineFieldAPI
    {
        private BoolField _field;

        public BoolFieldAPI(BoolField handler) : base(handler)
        {
            _field = handler;
        }

        public void setValue(bool value)
        {
            _field.SetValue(value);
        }
    }
}
