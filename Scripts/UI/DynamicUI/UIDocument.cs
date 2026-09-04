using RollPunk.Modding.APIs;
using RollPunk.UIFields;
using System;
using System.Collections.Generic;

namespace RollPunk.UI.DynamicUI
{
    public class UIDocument : IAPIHandler
    {
        private List<UIObjectData> _uiObjects = new();
        private UIDocumentAPI _docAPI;

        public IReadOnlyList<UIObjectData> UIObjects => _uiObjects;

        public event Action Changed;

        public UIDocument()
        {
            _docAPI = new(this);
        }
        
        public void AddButton(string text, Action onPress)
        {
            _uiObjects.Add(new ButtonData(text, onPress));
            Changed?.Invoke(); 
        }

        public void AddLabel(string text)
        {
            _uiObjects.Add(new LabelData(text));
            Changed?.Invoke();
        }

        public ContainerData AddContainer(string name)
        {
            var containerData = new ContainerData(name);
            _uiObjects.Add(containerData);
            containerData.Content.Changed += () => Changed?.Invoke();

            return containerData;
        }

        public void Clear()
        {
            _uiObjects.Clear();
            Changed?.Invoke();
        }

        public API GetAPI()
        {
            return _docAPI;
        }
    }

    public class UIObjectData { }

    public class ButtonData : UIObjectData
    {
        public readonly string Text;
        public readonly Action OnPress;

        public ButtonData(string text, Action onPress)
        {
            Text = text;
            OnPress = onPress;
        }
    }

    public class LabelData : UIObjectData
    {
        public readonly string Text;

        public LabelData(string text)
        {
            Text = text;
        }
    }

    public class ContainerData : UIObjectData
    {
        public readonly string Text;
        public readonly UIDocument Content;

        public ContainerData(string text)
        {
            Text = text;
            Content = new UIDocument();
        }
    }
}
