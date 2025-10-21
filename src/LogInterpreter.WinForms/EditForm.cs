using LogInterpreter.Abstractions;
using LogInterpreter.Abstractions.DefaultImplementations;

namespace LogInterpreter.WinForms
{
    public partial class EditForm : Form
    {
        private Pipeline? pipeline;

        public EditForm()
        {
            InitializeComponent();
        }

        public EditForm(Pipeline pipeline) : this()
        {
            this.pipeline = pipeline;
        }
    }
}
