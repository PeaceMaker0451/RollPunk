using Godot;
using System;

namespace RollPunk.UIFields
{
    internal partial class StringFieldControl : FieldControl
    {
        [Export] private Label _titleControl;
        [Export] protected LineEdit _valueControl;
        [Export] protected TextEdit _multiLineValueControl;

        private StringField? _field;
        private string _oldText;

        public void Initialize(StringField field)
        {
            if (_titleControl == null)
                throw new InvalidOperationException("Title control not setted!!");

            if (_valueControl == null && _multiLineValueControl == null)
                throw new InvalidOperationException("Value control not setted!!");

            _field = field;

            UpdateName();
            UpdateValue();
            CheckVisibility();
            CheckEditability();

            AddSubscriptions();
            
            if(_valueControl != null)
            {
                _valueControl.FocusExited += () =>
                {
                    if (_oldText == _valueControl.Text)
                        return;

                    field.SetValue(_valueControl.Text);
                    _oldText = _valueControl.Text;
                };
            }

            if(_multiLineValueControl != null)
            {
                _multiLineValueControl.FocusExited += () =>
                {
                    if (_oldText == _multiLineValueControl.Text)
                        return;

                    field.SetValue(_multiLineValueControl.Text);
                    _oldText = _multiLineValueControl.Text;
                }; 
            }

            UpdateWrap();
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
            if (_valueControl != null)
                _valueControl.Editable = editable;

            if (_multiLineValueControl != null)
                _multiLineValueControl.Editable = editable;
        }

        protected override void SetName(string name)
        {
            _titleControl.Text = name;
        }

        protected override void UpdateValue()
        {
            if (_valueControl != null)
                _valueControl.Text = _field.Value;

            if (_multiLineValueControl != null)
                _multiLineValueControl.Text = _field.Value;
        }

        protected override void OnAdditionalDataChanged(string dataName)
        {
            if (_multiLineValueControl != null && dataName == "is_wrap_enabled")
                UpdateWrap();
        }

        private void UpdateWrap()
        {
            if (_multiLineValueControl != null
            && GetField().AdditionalData.TryGetValue("is_wrap_enabled", out var isWrapEnabledObject) && isWrapEnabledObject is bool isWrapEnabled)
            {
                if (isWrapEnabled)
                    _multiLineValueControl.WrapMode = TextEdit.LineWrappingMode.Boundary;
                else
                    _multiLineValueControl.WrapMode = TextEdit.LineWrappingMode.None;
            }
        }
    }
}
