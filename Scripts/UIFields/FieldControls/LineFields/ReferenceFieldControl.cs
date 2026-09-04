using Godot;
using System;

namespace RollPunk.UIFields
{
    internal partial class ReferenceFieldControl : FieldControl
    {
        [Export] private Button _selectButton;

        private EntityReferenceField? _field;
        private Action<EntityReferenceField>? _onSelected;

        public void Initialize(EntityReferenceField field, Action<EntityReferenceField> onSelected)
        {
            if (_selectButton == null)
                throw new InvalidOperationException("Button control not setted!!");

            _field = field;
            _onSelected = onSelected;

            UpdateName();
            UpdateValue();
            CheckVisibility();
            CheckEditability();
            _selectButton.Pressed += () => _onSelected?.Invoke(_field);

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
            _selectButton.Disabled = editable == false;
        }

        protected override void SetName(string name)
        {
            _selectButton.Text = _field.VisibleName;
        }

        protected override void UpdateValue() { }
    }
}
