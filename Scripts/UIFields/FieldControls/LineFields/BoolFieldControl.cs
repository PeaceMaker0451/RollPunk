using Godot;
using RollPunk.Fields;
using System;

namespace RollPunk.UIFields
{
    internal partial class BoolFieldControl : FieldControl
    {
        [Export] private Button _checkerButton;

        private BoolField? _field;

        public void Initialize(BoolField field)
        {
            if (_checkerButton == null)
                throw new InvalidOperationException("Button control not setted!!");

            _field = field;

            UpdateName();
            UpdateValue();
            CheckVisibility();
            CheckEditability();
            _checkerButton.Toggled += (isOn) => _field.SetValue(isOn);

            AddSubscriptions();
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
            _checkerButton.Disabled = editable == false;
        }

        protected override void SetName(string name)
        {
            _checkerButton.Text = _field.VisibleName;
        }

        protected override void UpdateValue()
        {
            _checkerButton.SetPressedNoSignal(_field.Value);
        }
    }
}
