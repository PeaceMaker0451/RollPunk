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
        private readonly FramesManager _framesManager;
        private readonly FormsFactory _formsFactory;
        private readonly DialogFactory _dialogFactory;
        private readonly FormsAPI _api;

        public IDialogFactory Dialogs => _dialogFactory;

        public FormsManager(FramesManager framesManager, FormsFactory formsFactory)
        {
            _framesManager = framesManager;
            _formsFactory = formsFactory;
            _dialogFactory = new DialogFactory(this);
            _api = new FormsAPI(this);
        }

        public IFormHandle ShowInNewWindow(string formPath)
        {
            var context = LoadForm(formPath);
            var frame = _framesManager.OpenInNewFrame(context.Form);
            
            context.Location = FormLocation.NewWindow;
            context.Container = frame;
            
            return context.Handle;
        }

        public IFormHandle ShowInMainTab(string formPath, int priority = 0)
        {
            var context = LoadForm(formPath);
            
            _framesManager.MainFrame.AddTab(context.Form, context.Form.Title, priority);
            context.Location = FormLocation.MainTab;
            context.Container = _framesManager.MainFrame;
            
            return context.Handle;
        }

        public T GetForm<T>(IFormHandle handle) where T : Form
        {
            var context = GetContext(handle);
            return context.Form as T;
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

        public IFormHandle ShowController<T>(T controller, FormDisplayMode mode = FormDisplayMode.MainTab, int priority = 0) 
            where T : IFormControllerBase
        {
            var context = LoadForm(controller.FormPath);
            controller.View = context.Form;
            context.Controller = controller;

            switch (mode)
            {
                case FormDisplayMode.NewWindow:
                    var frame = _framesManager.OpenInNewFrame(context.Form);
                    context.Location = FormLocation.NewWindow;
                    context.Container = frame;
                    break;
                case FormDisplayMode.MainTab:
                default:
                    _framesManager.MainFrame.AddTab(context.Form, context.Form.Title, priority);
                    context.Location = FormLocation.MainTab;
                    context.Container = _framesManager.MainFrame;
                    break;
            }

            controller.Initialize();

            return context.Handle;
        }

        public T GetController<T>(IFormHandle handle) where T : class, IFormController
        {
            var context = GetContext(handle);
            return context.Controller as T;
        }

        public API GetAPI()
        {
            return _api;
        }

        private FormContext LoadForm(string formPath)
        {
            if (!_formsFactory.TryLoadForm(formPath, out Form form))
                throw new InvalidOperationException($"Failed to load form: {formPath}");
            
            var handle = new FormHandle(Guid.NewGuid().ToString());
            var context = new FormContext(handle, form);
            
            _forms[handle.Id] = context;
            return context;
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
