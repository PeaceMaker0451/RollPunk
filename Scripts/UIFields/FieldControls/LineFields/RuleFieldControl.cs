using Godot;
using System;

namespace RollPunk.UIFields
{
    public partial class RuleFieldControl : FieldControl
    {
        [Export] private Button _button;

        private RuleField? _field;

        public void Initialize(RuleField field)
        {
            if (_button == null)
                throw new InvalidOperationException("Button control not setted!!");

            _field = field;

            UpdateName();
            CheckVisibility();
            CheckEditability();

            AddSubscriptions();
            _button.Pressed += _field.Execute;
        }

        public override LineField GetField()
        {
            return _field;
        }

        protected override void SetVisible(bool visible)
        {
            (this as Control).SetVisible(visible);
        }
        protected override void SetEditable(bool editable)
        {
            _button.Disabled = !editable;
        }

        protected override void SetName(string name)
        {
            _button.Text = name;
        }

        protected override void UpdateValue()
        {
            throw new NotImplementedException();
        }
    }
}
