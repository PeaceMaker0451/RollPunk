using RollPunk.UI.Forms;

namespace RollPunk.Client.Forms
{
    public interface IFormController
    {
        public IFormHandle FormHandle { get; }
        
        public void Initialize();
        public void SetFormHandle(IFormHandle handle);
    }

    public interface IFormControllerBase : IFormController
    {
        public string FormPath { get; }
        public Form View { get; }

        public void SetView(Form view);
    }

    public interface IFormController<T> : IFormControllerBase where T : Form
    {
        public new T View { get; }
        Form IFormControllerBase.View => View;

        public void SetView(T view);
        void IFormControllerBase.SetView(Form view) => SetView((T)view);
    }
}
