using Godot;
using NetcodeCommon;
using RollPunk.AccessPolicy;
using RollPunk.Client;
using RollPunk.Client.Game;
using RollPunk.Fields;
using RollPunk.HierarchyFields;
using RollPunk.Scripts.UI;
using RollPunk.Scripts.UI.SessionConsole;
using RollPunk.UI.Forms;
using RollPunk.UIFields;
using System;

namespace RollPunk.ClientSide.Runtime.UI
{
    [FormScene("res://Scenes/FormsScenes/GameView.tscn")]
    internal partial class SessionView : Form
    {
        [Export] private FieldsList _fieldsList;
        [Export] private PlayerList _playersList;
        [Export] private SessionEventConsole _console;
        [Export] private EntityView _entityView;

        private ClientSession _session;

        public Action<Field> FieldListFieldSelected;

        public override void _Ready()
        {
            base._Ready();

            _fieldsList.FieldSelected += (field) => FieldListFieldSelected?.Invoke(field);
        }

        public void SetFieldListContainer(IReadOnlyFieldsContainer container)
        {
            _fieldsList.SetContainer(container);
        }

        public void InitializeLogs(Session session)
        {
            _console.LogSession(session);
        }

        public void InitializePlayerList(Session session)
        {
            _playersList.Initialize(session);
        }

        public void InitializeEntityView(FieldControlsConstructor fieldControlsConstructor)
        {
            _entityView.Initialize(fieldControlsConstructor);
        }

        public void ShowEntity(EntityField field)
        {
            _entityView.DisplayField(field);
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
}
