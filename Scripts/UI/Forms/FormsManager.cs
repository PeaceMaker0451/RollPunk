using Godot;
using RollPunk.Modding.APIs;
using RollPunk.UI.Forms;
using RollPunk.UI.Frames;
using System;
using System.Collections.Generic;

namespace RollPunk.Client.Forms
{
    internal class FormsManager : IFormsManager
    {
        private readonly Dictionary<string, FormContext> _forms = new();
        private readonly FramesHost _framesManager;
        private readonly FormsLoader _formLoader;
        private readonly DialogFactory _dialogFactory;
        private readonly FormsAPI _api;

        public IDialogFactory Dialogs => _dialogFactory;

        public FormsManager(FramesHost framesManager, FormsLoader formsFactory)
        {
            _framesManager = framesManager;
            _formLoader = formsFactory;
            _dialogFactory = new DialogFactory(this);
            _api = new FormsAPI(this);
        }

        public T GetForm<T>(IFormHandle handle) where T : Form
        {
            var context = GetContext(handle);
            return context.Form as T;
        }

        public IFormHandle ShowForm(Form form, FormDisplayMode mode = FormDisplayMode.NewWindow, int priority = 0)
        {
            var handle = new FormHandle(Guid.NewGuid().ToString());
            var context = new FormContext(handle, form);
            _forms[handle.Id] = context;

            switch (mode)
            {
                case FormDisplayMode.NewWindow:
                    var frame = _framesManager.OpenInNewFrame(form);
                    context.Location = FormLocation.NewWindow;
                    context.Container = frame;
                    break;
                case FormDisplayMode.MainTab:
                default:
                    _framesManager.MainFrame.AddTab(form, form.Title, priority);
                    context.Location = FormLocation.MainTab;
                    context.Container = _framesManager.MainFrame;
                    break;
            }

            return handle;
        }

        public IFormHandle ShowForm(string scenePath, FormDisplayMode mode = FormDisplayMode.NewWindow, int priority = 0)
        {
            if (_formLoader.TryLoadForm(scenePath, out var form))
                return ShowForm(form, mode, priority);
            else
                throw new Exception("Unnable to load form.");
        }

        IFormHandle IFormsManager.ShowController<T>(T controller, FormDisplayMode mode = FormDisplayMode.MainTab, int priority = 0)
        {
            if (controller is IFormControllerBase baseController)
            {
                var handle = ShowForm(baseController.FormPath, mode, priority);
                var context = GetContext(handle);
                context.Controller = controller;
                baseController.SetView(context.Form);

                baseController.Initialize();
                return handle;
            }
            else
            {
                throw new InvalidOperationException($"Controller {typeof(T).Name} must implement IFormControllerBase");
            }
        }

        public T GetController<T>(IFormHandle handle) where T : class, IFormController
        {
            var context = GetContext(handle);
            return context.Controller as T;
        }

        public void MoveToNewWindow(IFormHandle handle)
        {
            var context = GetContext(handle);
            RemoveFromCurrentContainer(context);
            
            var frame = _framesManager.OpenInNewFrame(context.Form);
            context.Location = FormLocation.NewWindow;
            context.Container = frame;
        }

        public void MoveToMainTab(IFormHandle handle, int priority = 0)
        {
            var context = GetContext(handle);
            RemoveFromCurrentContainer(context);
            
            _framesManager.MainFrame.AddTab(context.Form, context.Form.Title, priority);
            context.Location = FormLocation.MainTab;
            context.Container = _framesManager.MainFrame;
        }

        public void CloseForm(IFormHandle handle)
        {
            var context = GetContext(handle);
            RemoveFromCurrentContainer(context);
            
            context.Form.QueueFree();
            context.Location = FormLocation.Closed;
            _forms.Remove(handle.Id);
        }

        public API GetAPI()
        {
            return _api;
        }

        private FormContext GetContext(IFormHandle handle)
        {
            if (!_forms.TryGetValue(handle.Id, out var context))
                throw new InvalidOperationException($"Form handle {handle.Id} not found");
            
            return context;
        }

        private void RemoveFromCurrentContainer(FormContext context)
        {
            switch (context.Location)
            {
                case FormLocation.MainTab:
                    _framesManager.MainFrame.RemoveTab(context.Form);
                    break;
                case FormLocation.NewWindow:
                    if (context.Container is Frame frame)
                        _framesManager.CloseFrame(frame);
                    break;
            }
        }
    }

    internal class FormContext
    {
        public IFormHandle Handle { get; }
        public Form Form { get; }
        public FormLocation Location { get; set; }
        public object Container { get; set; }
        public IFormController Controller { get; set; }

        public FormContext(IFormHandle handle, Form form)
        {
            Handle = handle;
            Form = form;
            Location = FormLocation.Closed;
        }
    }

    internal class FormHandle : IFormHandle
    {
        public string Id { get; }
        public bool IsValid => true;
        public FormLocation Location { get; internal set; }

        public FormHandle(string id)
        {
            Id = id;
            Location = FormLocation.Closed;
        }
    }
}
