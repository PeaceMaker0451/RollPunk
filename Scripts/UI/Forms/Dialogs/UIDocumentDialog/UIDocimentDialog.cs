using Godot;
using RollPunk.Scripts.UI.DynamicUI;
using RollPunk.UI.DynamicUI;
using System;

namespace RollPunk.UI.Forms
{
    public partial class UIDocimentDialog : Form
    {
        public UIDocument? UIDocument { get; private set; }

        private UIDocumentRenderer _renderer = new();
        private Container? _container;

        public UIDocimentDialog()
        {
            API = new UIDocumentDialogAPI(this);
        }

        public void Initialize(string title, Container uiDocContainer)
        {
            Title = title;
            _container = uiDocContainer;
            UIDocument = new();
            UIDocument.Changed += OnDocumentChanged;
        }

        private void OnDocumentChanged()
        {
            if (UIDocument != null && _container != null)
                _renderer.Render(_container, UIDocument);
            else
                throw new Exception("Невозможно отобразить UIDocument");
        }
    }
}


