using Godot;
using RollPunk.UI.DynamicUI;
using RollPunk.UIFields;

namespace RollPunk.Scripts.UI.DynamicUI
{
    internal class UIDocumentRenderer
    {
        public void Render(Node rootNode, UIDocument document)
        {
            foreach(var uiObject in document.UIObjects)
            {
                CreateNode(rootNode, uiObject);
            }
        }

        private void CreateNode(Node rootNode, UIObjectData objectData)
        {
            switch (objectData)
            {
                case ButtonData buttonData:
                    Button button = new();
                    rootNode.AddChild(button);
                    button.Text = buttonData.Text;
                    button.Pressed += buttonData.OnPress;
                    break;

                case LabelData labelData:
                    Label label = new();
                    rootNode.AddChild(label);
                    label.Text = labelData.Text;
                    break;

                case ContainerData containerData:
                    Container containerHandler = null;
                    Container contentContainer = null;

                    containerHandler = new FoldableContainer();
                    contentContainer = new VBoxContainer();
                    containerHandler.AddChild(contentContainer);
                    (containerHandler as FoldableContainer).Title = containerData.Text;

                    rootNode.AddChild(containerHandler);
                    Render(contentContainer, containerData.Content);
                    break;
            }
        }
    }
}
