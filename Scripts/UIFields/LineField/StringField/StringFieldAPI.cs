using RollPunk.Fields;

namespace RollPunk.UIFields
{
    internal sealed class StringFieldAPI : LineFieldAPI
    {
        private StringField _field;

        public StringFieldAPI(StringField handler) : base(handler)
        {
            _field = handler;
        }

        public void setValue(string value)
        {
            _field.SetValue(value);
        }
    }
}
