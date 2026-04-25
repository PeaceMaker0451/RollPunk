namespace RollPunk.UIFields
{
    internal sealed class FieldsGroupAPI : LineFieldAPI
    {
        private readonly FieldsGroup fieldsGroupHandler;
        public FieldsGroupAPI(LineField handler) : base(handler)
        {
            fieldsGroupHandler = (FieldsGroup)handler;
        }

        public void addField(LineFieldAPI field)
        {
            fieldsGroupHandler.AddField((LineField)field.GetHandler());
        }

        public void removeField(LineFieldAPI field)
        {
            fieldsGroupHandler.RemoveField((LineField)field.GetField());
        }
    }
}