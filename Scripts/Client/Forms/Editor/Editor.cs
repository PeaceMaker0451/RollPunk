using Godot;
using RollPunk.ClientSide.Runtime.UI;
using RollPunk.Fields;
using RollPunk.HierarchyFields;
using RollPunk.MembersExposing;
using RollPunk.UI;
using RollPunk.UI.Forms;
using RollPunk.UIFields;
using System;

[FormScene("res://Scenes/FormsScenes/Editor.tscn")]
public partial class Editor : Form
{
    [Export] private EntityView _entityView;
    [Export] private ExposedObjectRenderer _renderer;
    [Export] private Container _rendererContainer;
    [Export] private FieldsTree _fieldsTree;

    public event Action<Field> FieldsTreeFieldSelected;
    public event Action<object> ExposedObjectCollectionMemberSelected;

    public override void _Ready()
    {
        _fieldsTree.FieldSelected += (field) => FieldsTreeFieldSelected?.Invoke(field);
        _renderer.CollectionObjectSelected += (obj) => ExposedObjectCollectionMemberSelected?.Invoke(obj);
    }
    
    public void SetFieldsContainer(IReadOnlyFieldsContainer container)
    {
        _fieldsTree.SetContainer(container);
        
    }

    public void InitializeEntityView(FieldControlsConstructor fieldControlsConstructor)
    {
        _entityView.Initialize(fieldControlsConstructor);
    }

    public void ShowEntity(EntityField field)
    {
        _rendererContainer.Visible = false;
        _entityView.Visible = true;

        _entityView.DisplayField(field);
    }

    public void ShowRawData(ExposedObject obj)
    {
        _entityView.Visible = false;
        _rendererContainer.Visible = true;

        _renderer.Render(obj);
    }

    public void SetEntityViewVisibiblityRule(Func<LineField, bool> rule)
    {
        _entityView.SetViewRule(rule);
    }
    public void SetEntityViewEditabilityRule(Func<LineField, bool> rule)
    {
        _entityView.SetEditRule(rule);
    }
}
