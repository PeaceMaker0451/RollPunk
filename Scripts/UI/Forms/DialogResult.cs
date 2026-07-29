namespace RollPunk.Client.Forms
{
    public class DialogResult<T>
    {
        public bool IsConfirmed { get; }
        public T Value { get; }

        public DialogResult(bool isConfirmed, T value)
        {
            IsConfirmed = isConfirmed;
            Value = value;
        }
    }
}
