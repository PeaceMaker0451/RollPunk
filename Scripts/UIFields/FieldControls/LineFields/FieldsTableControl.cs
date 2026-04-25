using Godot;
using RollPunk.Debug;
using RollPunk.Fields;
using RollPunk.Scripts.UIFields;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace RollPunk.UIFields
{
    internal partial class FieldsTableControl : FieldControl
    {
        [Export] Control _fieldContainerRoot;
        [Export] Label _titleControl;
        [Export] Control _upperSeparator;

        private Dictionary<Field, FieldControl> _controls = new();
        private Container? _fieldsContainer;
        private FieldsGroup? _field;
        private FieldControlsConstructor? _constructor;

        public void Initialize(FieldsGroup field, FieldControlsConstructor constructor)
        {
            if (_titleControl == null)
                throw new InvalidOperationException("Title control not setted!!");

            if (_fieldContainerRoot == null)
                throw new InvalidOperationException("Container Root control not setted!!");

            _field = field;
            _constructor = constructor;

            if (_field.AdditionalData.TryGetValue("container_type", out var typeObject) && typeObject is string type && ContainerTypeMapper.TryParse(type, out ContainerType containerType))
                CreateContainer(containerType);
            else
                CreateContainer(ContainerType.VBox);

            if (_field.AdditionalData.TryGetValue("label_visible", out var labelVisibleObject) && labelVisibleObject is bool labelVisible && labelVisible == false)
            {
                _titleControl.Hide();
                _upperSeparator.Hide();
            }
            else
            {
                _titleControl.Show();
                _upperSeparator.Show();
            }
                

            UpdateName();
            CheckVisibility();
            CheckEditability();

            foreach (var innerField in _field.Fields)
                AddField(innerField);

            _field.ChildAdded += AddField;
            AddSubscriptions();

            ViewCheckChanged += () =>
            {
                foreach (var control in _controls.Values)
                    control.SetViewCheck(ViewCheck);
            };


            EditCheckChanged += () =>
            {
                foreach (var control in _controls.Values)
                    control.SetEditCheck(EditCheck);
            };

            if (GetField().AdditionalData.TryGetValue("stretch_ratio", out var stretchRatioObject))
            {
                var stretchRatio = Convert.ToSingle(stretchRatioObject);
                SizeFlagsStretchRatio = stretchRatio;
            }

            
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
        
        protected override void SetEditable(bool editable) { }

        private void AddField(Field field)
        {            
            FieldControl control = _constructor.CreateFieldControl(field);
            _controls.Add(field, control);
            _fieldsContainer.AddChild(control);
            control.SetViewCheck(ViewCheck);
            control.SetEditCheck(EditCheck);

            FieldControlsSorter.Sort(_fieldsContainer);
        }

        private void CreateContainer(ContainerType type)
        {
            Container container = null;

            switch (type)
            {
                case ContainerType.HBox:
                    container = new HBoxContainer();
                    break;

                case ContainerType.VBox:
                    container = new VBoxContainer();
                    break;

                case ContainerType.VFlow:
                    container = new VFlowContainer();
                    break;

                case ContainerType.HFlow:
                    container = new HFlowContainer();
                    break;

                case ContainerType.Scroll:
                    container = new ScrollContainer();

                    if(GetField().AdditionalData.TryGetValue("vertical_scroll", out var vericalScrollObject) && vericalScrollObject is bool isVerticalScrollOn)
                    {
                        if(isVerticalScrollOn)
                        (container as ScrollContainer).VerticalScrollMode = ScrollContainer.ScrollMode.Auto;
                        else
                        (container as ScrollContainer).VerticalScrollMode = ScrollContainer.ScrollMode.Disabled;
                    }


                    if (GetField().AdditionalData.TryGetValue("horizontal_scroll", out var horizontalScrollObject) && horizontalScrollObject is bool isHorizontalScrollOn)
                    {
                        if (isHorizontalScrollOn)
                            (container as ScrollContainer).HorizontalScrollMode = ScrollContainer.ScrollMode.Auto;
                        else
                            (container as ScrollContainer).HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled;
                    }

                    break;

                default:
                    container = new HBoxContainer();
                    break;
            }

            container.SizeFlagsVertical = SizeFlags.ExpandFill;
            container.SizeFlagsHorizontal = SizeFlags.ExpandFill;

            _fieldsContainer = container;
            _fieldContainerRoot.AddChild(_fieldsContainer);
        }

        protected override void SetName(string name)
        {
            _titleControl.Text = name;
        }

        protected override void UpdateValue()
        {
            throw new NotImplementedException();
        }

        protected override void OnExittingTree()
        {
            _field.ChildAdded -= AddField;
        }
    }
}