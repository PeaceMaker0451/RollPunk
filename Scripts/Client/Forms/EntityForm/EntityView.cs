using Godot;
using RollPunk.Client;
using RollPunk.Debug;
using RollPunk.Fields;
using RollPunk.HierarchyFields;
using RollPunk.Scripts.UIFields;
using RollPunk.UI.Forms;
using RollPunk.UIFields;
using System;
using System.Collections.Generic;
using System.Text.Json;

internal partial class EntityView : Form
{
    [Export] private Container _fieldsContainer;

    [Export] private LineEdit _id;
    [Export] private LineEdit _name;

    [Export] private Button _saveButton;

    private FieldControlsConstructor _controlsConstructor;
    private EntityField _entity;
	private Dictionary<Field, FieldControl> _controls = new Dictionary<Field, FieldControl>();

    private Serializator _serializator;

    private Func<LineField, bool> _viewCheck;
    private Func<LineField, bool> _editCheck;

	public event Action<Field> FieldChanged;

    public override void _Ready()
    {
        _saveButton.Pressed += () =>
        {
            if (_entity == null)
                return;

            var data = _serializator.SerializeFieldTree(_entity);
            Client.Instance.FileDebugUtils.SaveStringWithDialog(data);
        };
    }
    
    public void Initialize(FieldControlsConstructor fieldControlsConstructor, Serializator serializator)
	{
        _controlsConstructor = fieldControlsConstructor;
        _serializator = serializator;

    }

    public void DisplayField(EntityField entityField)
    {
        if (_controlsConstructor == null)
            throw new InvalidOperationException("EntityView is not initialized!");
        
        foreach (var control in _controls.Values)
        {
            control.QueueFree();
        }

        _controls.Clear();

        _entity = entityField;

        _id.Text = entityField.ID.ToString();
        _name.Text = entityField.Name;

        foreach (var field in entityField.Fields)
        {
            if (field is LineField lineField == false)
                continue;

            FieldControl fieldControl = _controlsConstructor.CreateFieldControl(field);
            _fieldsContainer.AddChild(fieldControl);
            field.Changed += () => FieldChanged?.Invoke(field);
            fieldControl.SetViewCheck(_viewCheck);
            fieldControl.SetEditCheck(_editCheck);

            _controls.Add(field, fieldControl);
        }

        FieldControlsSorter.Sort(_fieldsContainer);
    }

    public void SetViewRule(Func<LineField, bool> rule)
    {
        _viewCheck = rule;
    }

    public void SetEditRule(Func<LineField, bool> rule)
    {
        _editCheck = rule;
    }
}
