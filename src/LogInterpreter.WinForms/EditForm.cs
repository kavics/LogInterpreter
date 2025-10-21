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
            this.Load += EditForm_Load;
            pipelineListBox.DisplayMember = "Name";
        }

        public EditForm(Pipeline pipeline) : this()
        {
            this.pipeline = pipeline;
        }

        private void EditForm_Load(object? sender, EventArgs e)
        {
            LoadPipelineItems();
        }

        private void LoadPipelineItems()
        {
            pipelineListBox.Items.Clear();
            
            if (pipeline?.Items != null)
            {
                foreach (var item in pipeline.Items)
                {
                    pipelineListBox.Items.Add(item);
                }
            }
        }
    }
}
