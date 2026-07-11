using Godot;
using RollPunk.Client.Forms;
using RollPunk.Fields;
using RollPunk.HierarchyFields;
using RollPunk.UI.Forms;
using RollPunk.UIFields;

namespace RollPunk.ClientSide.Runtime.UI
{
    internal partial class GameView : Form
    {
        [Export] private FieldsList _fieldsList;
        [Export] public EntityView EntityView;

        private SessionViewController _controller;

        public override void _Ready()
        {
            base._Ready();
            
            // Контроллер будет создан и настроен извне через SetController
        }

        public void SetController(SessionViewController controller)
        {
            _controller = controller;
            _controller.Initialize();
        }

        public void Initialize(IReadOnlyFieldsContainer fieldsContainer, FieldControlsConstructor fieldControlsConstructor, Serializator serializator)
        {
            _fieldsList.SetContainer(fieldsContainer);
            EntityView.Initialize(fieldControlsConstructor, serializator);

            _fieldsList.FieldSelected += (field) =>
            {
                if (field is EntityField entityField)
                    EntityView.DisplayField(entityField);
            };
        }
    }
}
