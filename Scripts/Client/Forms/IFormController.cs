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
        Form View { get; set; }
    }

    public interface IFormController<T> : IFormControllerBase where T : Form
    {
        new T View { get; set; }

        Form IFormControllerBase.View
        {
            get => View;
            set => View = (T)value;
        }
    }
}
