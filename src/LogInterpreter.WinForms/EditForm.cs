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

        private void pipelineListBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (pipelineListBox.SelectedItem is IPipelineItem selectedItem)
            {
                UpdateDetailsPanel(selectedItem);
            }
            else
            {
                ClearDetailsPanel();
            }
        }

        private void UpdateDetailsPanel(IPipelineItem item)
        {
            // Fully qualified type name
            typeNameTextBox.Text = item.GetType().FullName ?? item.GetType().Name;

            // Get generic type parameters for input and output types
            var itemType = item.GetType();
            var pipelineInterface = itemType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && 
                                     i.GetGenericTypeDefinition() == typeof(IPipelineItem<,>));

            if (pipelineInterface != null)
            {
                var genericArgs = pipelineInterface.GetGenericArguments();
                inputTypeTextBox.Text = GetFriendlyTypeName(genericArgs[0]);
                outputTypeTextBox.Text = GetFriendlyTypeName(genericArgs[1]);
            }
            else
            {
                inputTypeTextBox.Text = "N/A";
                outputTypeTextBox.Text = "N/A";
            }
        }

        private void ClearDetailsPanel()
        {
            typeNameTextBox.Text = string.Empty;
            inputTypeTextBox.Text = string.Empty;
            outputTypeTextBox.Text = string.Empty;
        }

        private string GetFriendlyTypeName(Type type)
        {
            if (!type.IsGenericType)
            {
                return type.FullName ?? type.Name;
            }

            var genericTypeName = type.GetGenericTypeDefinition().FullName ?? type.GetGenericTypeDefinition().Name;
            var backtickIndex = genericTypeName.IndexOf('`');
            if (backtickIndex > 0)
            {
                genericTypeName = genericTypeName.Substring(0, backtickIndex);
            }

            var genericArgs = type.GetGenericArguments();
            var genericArgNames = string.Join(", ", genericArgs.Select(GetFriendlyTypeName));
            
            return $"{genericTypeName}<{genericArgNames}>";
        }
    }
}
