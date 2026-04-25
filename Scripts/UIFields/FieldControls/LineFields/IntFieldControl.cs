using Godot;
using System;

namespace RollPunk.UIFields
{
    internal partial class IntFieldControl : FieldControl
    {
        [Export] private Label _titleControl;
        [Export] private SpinBox _valueControl;
        [Export] private BoxContainer _container;

        private IntField? _field;
        
        public void Initialize(IntField field)
        {
            if (_titleControl == null)
                throw new InvalidOperationException("Title control not setted!!");

            if (_valueControl == null)
                throw new InvalidOperationException("Value control not setted!!");

            _field = field;

            UpdateValueControl();
            UpdateName();
            UpdateValue();
            CheckVisibility();
            CheckEditability();

            AddSubscriptions();
            _valueControl.ValueChanged += (value) => field.SetValue((int)value);
            _field.MaxValueChanged += UpdateValueControl;
            _field.MinValueChanged += UpdateValueControl;
        }

        public override void _Ready()
        {
            _ = PlayLabelAnimation(_titleControl);
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
            _valueControl.Editable = editable;
        }

        private void UpdateValueControl()
        {
            if (_field.AdditionalData.TryGetValue("show_max", out var showMaxPrefixObject) && showMaxPrefixObject is bool showMax && showMax)
                _valueControl.Suffix = $"/{_field.MaxValue}";

            if (_field.AdditionalData.TryGetValue("vertical", out var verticalDisplayObject) && verticalDisplayObject is bool shouldVericalDisplay)
            {
                _container.Vertical = shouldVericalDisplay;
                _titleControl.HorizontalAlignment = shouldVericalDisplay? HorizontalAlignment.Left : HorizontalAlignment.Right;
            }        

            _valueControl.MaxValue = _field.MaxValue;
            _valueControl.MinValue = _field.MinValue;
        }

        protected override void SetName(string name)
        {
            _titleControl.Text = name;
        }

        protected override void UpdateValue()
        {
            _valueControl.SetValueNoSignal(_field.Value);
        }

        protected override void OnAdditionalDataChanged(string dataName)
        {
            UpdateValueControl();
        }

        protected override void OnExittingTree()
        {
            _field.MaxValueChanged -= UpdateValueControl;
            _field.MinValueChanged -= UpdateValueControl;
        }
    }
}