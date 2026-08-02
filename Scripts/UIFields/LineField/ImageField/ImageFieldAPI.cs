using RollPunk.Fields;
using RollPunk.Modding.APIs;
using System;

namespace RollPunk.UIFields
{
    public class ImageFieldAPI : FieldAPI
    {
        private readonly ImageField _field;

        public ImageFieldAPI(ImageField field) : base (field)
        {
            _field = field ?? throw new ArgumentNullException(nameof(field));
        }

        [MoonSharp.Interpreter.MoonSharpHidden]
        public ImageField GetField()
        {
            return _field;
        }
    }
}
