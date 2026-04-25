namespace RollPunk.Fields
{
    public class ValueFieldAPI : FieldAPI
    {
        private ValueField _valueFieldHandler;

        public ValueFieldAPI(ValueField handler) : base(handler)
        {
            _valueFieldHandler = handler;
        }

        public object getValue() { return _valueFieldHandler.GetRawValue(); }
    }
}
