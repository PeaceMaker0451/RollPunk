namespace RollPunk.Fields
{
    public interface IFieldsContainer: IReadOnlyFieldsContainer, IFieldsHandler
    {
        public void AddField(Field field);
        public bool RemoveField(Field field);
    }
}
