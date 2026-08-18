using Godot;
using RollPunk.Client;
using RollPunk.Fields;
using RollPunk.HierarchyFields;
using RollPunk.Scripts.UIFields;
using RollPunk.UI.Forms;
using RollPunk.UIFields;
using System;
using System.Collections.Generic;

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
            ClientRoot.FileDebugUtils.SaveStringWithDialog(data);
        };
    }
    
    public void Initialize(FieldControlsConstructor fieldControlsConstructor, Serializator serializator)
	{
        _controlsConstructor = fieldControlsConstructor;
        _serializator = serializator;

    }

    public void DisplayField(EntityField entityField)
    {
        if(_entity != null)
        {
            _entity.DescendantAdded -= OnEntityDescendantAdded;
            _entity.DescendantRemoved -= OnEntityDescendantRemoved;
            _entity.ChildAdded -= OnEntityDescendantAdded;
            _entity.ChildRemoved -= OnEntityDescendantRemoved;
        }
        
        _entity = entityField;
        _entity.DescendantAdded += OnEntityDescendantAdded;
        _entity.DescendantRemoved += OnEntityDescendantRemoved;
        _entity.ChildAdded += OnEntityDescendantAdded;
        _entity.ChildRemoved += OnEntityDescendantRemoved;

        UpdateView();
    }

    public void SetViewRule(Func<LineField, bool> rule)
    {
        _viewCheck = rule;
    }

    public void SetEditRule(Func<LineField, bool> rule)
    {
        _editCheck = rule;
    }

    private void OnEntityDescendantAdded(Field child)
    {
        UpdateView();
    }

    private void OnEntityDescendantRemoved(Field child)
    {
        UpdateView();
    }

    private void UpdateView()
    {
        if (_controlsConstructor == null)
            throw new InvalidOperationException("EntityView is not initialized!");

        foreach (var control in _controls.Values)
            control.QueueFree();

        _controls.Clear();

        _id.Text = _entity.ID.ToString();
        _name.Text = _entity.Name;

        foreach (var field in _entity.Fields)
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
}
