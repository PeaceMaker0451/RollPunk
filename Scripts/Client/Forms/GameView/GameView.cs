using Godot;
using NetcodeCommon;
using RollPunk.Client;
using RollPunk.Client.Forms;
using RollPunk.Fields;
using RollPunk.HierarchyFields;
using RollPunk.Scripts.UI.SessionConsole;
using RollPunk.UI.Forms;
using RollPunk.UIFields;

namespace RollPunk.ClientSide.Runtime.UI
{
    internal partial class GameView : Form
    {
        [Export] private FieldsTree _fieldsTree;
        [Export] private SessionEventConsole _console;
        [Export] public EntityView EntityView;

        public override void _Ready()
        {
            base._Ready();
        }

        public void Initialize(Session session, FieldControlsConstructor fieldControlsConstructor, Serializator serializator)
        {
            _fieldsTree.SetContainer(session.Fields);
            EntityView.Initialize(fieldControlsConstructor, serializator);
            _console.LogSession(session);

            _fieldsTree.FieldSelected += (field) =>
            {
                if (field is EntityField entityField)
                    EntityView.DisplayField(entityField);
            };
        }
    }
}
