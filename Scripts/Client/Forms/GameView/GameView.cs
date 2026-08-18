using Godot;
using NetcodeCommon;
using RollPunk.Client;
using RollPunk.Client.Forms;
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
    internal partial class GameView : Form
    {
        [Export] private FieldsList _fieldsList;
        [Export] private PlayerList _playersList;
        [Export] private SessionEventConsole _console;
        [Export] public EntityView EntityView;

        public override void _Ready()
        {
            base._Ready();
        }

        public void Initialize(ClientSession session, FieldControlsConstructor fieldControlsConstructor)
        {
            _fieldsList.SetContainer(session.Entities);
            EntityView.Initialize(fieldControlsConstructor, session.Serializator);
            _console.LogSession(session);

            _fieldsList.FieldSelected += (field) =>
            {
                if (field is EntityField entityField)
                    EntityView.DisplayField(entityField);
            };

            _playersList.Initialize(session);
        }
    }
}
