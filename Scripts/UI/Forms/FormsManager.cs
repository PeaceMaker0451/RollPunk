using RollPunk.Modding.APIs;
using RollPunk.UI.Forms;
using RollPunk.UI.Frames;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RollPunk.Client.Forms
{
    internal class FormsManager : IFormsManager, IFormHost
    {
        private readonly HashSet<Form> _openForms = new();
        private readonly Dictionary<Form, Frame> _formFrames = new();
        private readonly Dictionary<Form, Action> _frameCloseSubscriptions = new();

        private readonly IFramesHost _framesHost;
        private readonly FormsLoader _formsLoader;
        private readonly DialogFactory _dialogFactory;
        private readonly FormsAPI _api;

        public IDialogFactory Dialogs => _dialogFactory;
        public IEnumerable<Form> OpenForms => _openForms;

        public FormsManager(IFramesHost framesHost, FormsLoader formsLoader)
        {
            _framesHost = framesHost ?? throw new ArgumentNullException(nameof(framesHost));
            _formsLoader = formsLoader ?? throw new ArgumentNullException(nameof(formsLoader));
            _dialogFactory = new DialogFactory(this);
            _api = new FormsAPI(this);
        }

        // ---------- Публичное API ----------

        public T Open<T>(FormDisplayMode mode = FormDisplayMode.NewWindow, int priority = 0)
            where T : Form
        {
            var form = _formsLoader.Instantiate<T>();
            return Open(form, mode, priority);
        }

        public T Open<T>(T form, FormDisplayMode mode = FormDisplayMode.NewWindow, int priority = 0)
            where T : Form
        {
            if (form == null) throw new ArgumentNullException(nameof(form));
            if (form.IsOpen) throw new InvalidOperationException($"Form '{form.GetType().Name}' is already open.");

            RegisterForm(form);
            PlaceForm(form, mode, priority);
            form.IsOpen = true;
            form.RaiseShown();
            return form;
        }

        public T OpenIn<T>(Frame frame) where T : Form
        {
            if (frame == null) throw new ArgumentNullException(nameof(frame));

            var form = _formsLoader.Instantiate<T>();
            if (form.IsOpen) throw new InvalidOperationException($"Form '{form.GetType().Name}' is already open.");

            RegisterForm(form);
            PlaceInExistingFrame(form, frame);
            form.IsOpen = true;
            form.RaiseShown();
            return form;
        }

        public TForm OpenWith<TForm>(IFormPresenter<TForm> presenter,
                                     FormDisplayMode mode = FormDisplayMode.NewWindow,
                                     int priority = 0)
            where TForm : Form
        {
            if (presenter == null) throw new ArgumentNullException(nameof(presenter));

            var form = Open<TForm>(mode, priority);
            presenter.Attach(form);
            return form;
        }

        public T FindOpen<T>() where T : Form
        {
            return _openForms.OfType<T>().FirstOrDefault();
        }

        public API GetAPI() => _api;

        // ---------- IFormHost (внутренний контракт с формой) ----------

        void IFormHost.RequestClose(Form form)
        {
            if (!_openForms.Contains(form)) return;

            RemoveFromCurrentContainer(form);
            form.IsOpen = false;
            form.Location = FormLocation.Closed;
            form.Host = null;
            _openForms.Remove(form);
            _formFrames.Remove(form);

            form.RaiseClosed();
            form.QueueFree();
        }

        void IFormHost.RequestMoveToNewWindow(Form form)
        {
            if (!_openForms.Contains(form)) return;
            if (form.Location == FormLocation.NewWindow) return;

            RemoveFromCurrentContainer(form);
            PlaceInNewFrame(form);
            form.RaiseLocationChanged();
        }

        void IFormHost.RequestMoveToMainTab(Form form, int priority)
        {
            if (!_openForms.Contains(form)) return;
            if (form.Location == FormLocation.MainTab) return;

            RemoveFromCurrentContainer(form);
            PlaceInMainTab(form, priority);
            form.RaiseLocationChanged();
        }

        Frame IFormHost.GetContainingFrame(Form form)
        {
            return _formFrames.TryGetValue(form, out var frame) ? frame : null;
        }

        // ---------- Внутренняя механика ----------

        private void RegisterForm(Form form)
        {
            form.Host = this;
            _openForms.Add(form);
        }

        private void PlaceForm(Form form, FormDisplayMode mode, int priority)
        {
            switch (mode)
            {
                case FormDisplayMode.NewWindow:
                    PlaceInNewFrame(form);
                    break;
                case FormDisplayMode.MainTab:
                    PlaceInMainTab(form, priority);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }
        }

        private void PlaceInNewFrame(Form form)
        {
            var frame = _framesHost.OpenFrame(form);
            AttachFrameToForm(form, frame);
            form.Location = FormLocation.NewWindow;
        }

        private void PlaceInExistingFrame(Form form, Frame frame)
        {
            frame.SetForm(form);
            frame.SetTitle(form.Title);
            AttachFrameToForm(form, frame);
            form.Location = FormLocation.NewWindow;
        }

        private void PlaceInMainTab(Form form, int priority)
        {
            _framesHost.MainFrame.AddTab(form, form.Title, priority);
            form.Location = FormLocation.MainTab;
        }

        private void AttachFrameToForm(Form form, Frame frame)
        {
            _formFrames[form] = frame;

            // Если пользователь закрыл фрейм крестиком — закрываем и форму.
            Action onFrameClose = () => ((IFormHost)this).RequestClose(form);
            frame.CloseButtonPressed += onFrameClose;
            _frameCloseSubscriptions[form] = onFrameClose;
        }

        private void RemoveFromCurrentContainer(Form form)
        {
            switch (form.Location)
            {
                case FormLocation.MainTab:
                    _framesHost.MainFrame.RemoveTab(form);
                    break;
                case FormLocation.NewWindow:
                    if (_formFrames.TryGetValue(form, out var frame))
                    {
                        DetachFrameFromForm(form, frame);
                        _framesHost.CloseFrame(frame);
                    }
                    break;
            }
            _formFrames.Remove(form);
        }

        private void DetachFrameFromForm(Form form, Frame frame)
        {
            if (_frameCloseSubscriptions.TryGetValue(form, out var handler))
            {
                frame.CloseButtonPressed -= handler;
                _frameCloseSubscriptions.Remove(form);
            }
        }
    }
}
