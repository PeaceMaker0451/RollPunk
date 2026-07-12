using RollPunk.UI.Forms;

namespace RollPunk.Client.Forms
{
    public interface IFormController
    {
        void Initialize();
    }

    public interface IFormController<T> : IFormController where T : Form
    {
        T View { get; set; }
        string FormPath { get; }
    }
}
