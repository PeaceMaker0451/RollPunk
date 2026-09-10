using RollPunk.Modding.APIs;

namespace RollPunk.UI.Forms
{
    public class FormAPI : HeldAPI
    {
        private Form _form;

        public string title => _form.Title;
        public bool is_open => _form.IsOpen;
        
        public FormAPI(Form handler) : base(handler)
        {
            _form = handler;
        }

        public void close()
        {
            _form.Close();
        }

        public void moveToNewWindow()
        {
            _form.MoveToNewWindow();
        }

        public void moveToMainTab(int priority)
        {
            _form.MoveToMainTab(priority);
        }

        public void setTitle(string title)
        {
            _form.SetTitle(title);
        }
    }
}
