using Godot;
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

    private FieldControlsConstructor _controlsConstructor;
    private EntityField _entity;
	private Dictionary<Field, FieldControl> _controls = new Dictionary<Field, FieldControl>();

    private Func<LineField, bool> _viewCheck;
    private Func<LineField, bool> _editCheck;

	public event Action<Field> FieldChanged;
    
    public void Initialize(FieldControlsConstructor fieldControlsConstructor)
	{
        _controlsConstructor = fieldControlsConstructor;
    }

    public void DisplayField(EntityField entityField)
    {
        if(_entity != null)
        {
            _entity.DescendantAdded -= OnEntityDescendantAdded;
            _entity.DescendantRemoved -= OnEntityDescendantRemoved;
            _entity.FieldAdded -= OnEntityDescendantAdded;
            _entity.FieldRemoved -= OnEntityDescendantRemoved;
        }
        
        _entity = entityField;
        _entity.DescendantAdded += OnEntityDescendantAdded;
        _entity.DescendantRemoved += OnEntityDescendantRemoved;
        _entity.FieldAdded += OnEntityDescendantAdded;
        _entity.FieldRemoved += OnEntityDescendantRemoved;

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
