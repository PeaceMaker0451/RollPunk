using RollPunk.UI.Forms;

namespace RollPunk.Client.Forms
{
    public interface IFormController
    {
        void Initialize();
    }

    public interface IFormControllerBase : IFormController
    {
        string FormPath { get; }
        Form View { get; }

        void SetView(Form view);
    }

    public interface IFormController<T> : IFormControllerBase where T : Form
    {
        new T View { get; }

        Form IFormControllerBase.View => View;

        void IFormControllerBase.SetView(Form view) => SetView((T)view);

        void SetView(T view);
    }
}
