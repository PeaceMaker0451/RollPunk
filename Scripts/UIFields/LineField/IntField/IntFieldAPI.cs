namespace RollPunk.UIFields
{
    internal sealed class IntFieldAPI : LineFieldAPI
    {
        private IntField _field;

        public int MaxValue => _field.MaxValue;
        public int MinValue => _field.MinValue;

        public IntFieldAPI(IntField handler) : base(handler)
        {
            _field = handler;
        }

        public void setValue(int value)
        {
            _field.SetValue(value);
        }

        public void setMaxValue(int maxValue)
        {
            _field.SetMaxValue(maxValue);
        }

        public void setMinValue(int minValue)
        {
            _field.SetMinValue(minValue);
        }
    }
}
