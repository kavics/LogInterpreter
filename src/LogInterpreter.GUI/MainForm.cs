using LogInterpreter.GUI.Controls;

namespace LogInterpreter.GUI
{
    public partial class MainForm : Form
    {
        private PipelineItemCard? _selectedCard;

        public MainForm()
        {
            InitializeComponent();
            InitializePipelineCards();

            // Handle resize to adjust card widths
            pipelineFlowLayoutPanel.Resize += PipelineFlowLayoutPanel_Resize;
        }

        private void PipelineFlowLayoutPanel_Resize(object? sender, EventArgs e)
        {
            // Adjust all card widths when panel resizes
            int cardWidth = pipelineFlowLayoutPanel.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 8;
            foreach (Control control in pipelineFlowLayoutPanel.Controls)
            {
                if (control is PipelineItemCard card)
                {
                    card.Width = cardWidth;
                }
            }
        }

        private void InitializePipelineCards()
        {
            int cardWidth = pipelineFlowLayoutPanel.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 8;

            // Add Pipeline card (always first)
            var pipelineCard = new PipelineItemCard
            {
                ItemType = "Pipeline",
                Description = "The main pipeline container",
                InputType = "Input: -",
                OutputType = "Output: -",
                IsFirstItem = true,
                Width = cardWidth
            };
            pipelineCard.Selected += PipelineCard_Selected;
            pipelineFlowLayoutPanel.Controls.Add(pipelineCard);

            // Add example pipeline items
            AddPipelineCard("LogSource", "Reads log files from a specified directory or a single file and emits their full paths.", "int", "string", cardWidth);
            AddPipelineCard("OneLineLogFileReader", "Reads log files line by line (one log entry per line format).", "string", "string", cardWidth);
            AddPipelineCard("CompactJsonLogParser", "Parses log entries from compact JSON format (one JSON object per line).", "string", "LogEntry", cardWidth);
            AddPipelineCard("Filter<LogEntry>", "Filters entries based on a custom predicate function.", "LogEntry", "LogEntry", cardWidth);
            AddPipelineCard("Formatter<LogEntry>", "Formats log entries into string representation using a custom function.", "LogEntry", "string", cardWidth);
            AddPipelineCard("ConsoleWriter", "Writes each log entry to the console output.", "string", "string", cardWidth);
        }

        private void AddPipelineCard(string itemType, string description, string inputType, string outputType, int cardWidth)
        {
            var card = new PipelineItemCard
            {
                ItemType = itemType,
                Description = description,
                InputType = $"Input: {inputType}",
                OutputType = $"Output: {outputType}",
                Width = cardWidth
            };
            card.Selected += PipelineCard_Selected;
            pipelineFlowLayoutPanel.Controls.Add(card);
        }

        private void PipelineCard_Selected(object? sender, EventArgs e)
        {
            if (sender is PipelineItemCard selectedCard)
            {
                // Deselect all other cards
                foreach (Control control in pipelineFlowLayoutPanel.Controls)
                {
                    if (control is PipelineItemCard card && card != selectedCard)
                    {
                        card.IsSelected = false;
                    }
                }

                _selectedCard = selectedCard;
                UpdatePropertiesPanel();
            }
        }

        private void UpdatePropertiesPanel()
        {
            if (_selectedCard != null)
            {
                propertiesContentPanel.Visible = true;
                propertyTypeLabel.Text = _selectedCard.ItemType;
                propertyDescriptionTextBox.Text = _selectedCard.Description;
                
                // Extract just the type from "Input: type" and "Output: type"
                string inputType = _selectedCard.InputType.Replace("Input: ", "");
                string outputType = _selectedCard.OutputType.Replace("Output: ", "");
                
                propertyInputTextBox.Text = inputType;
                propertyOutputTextBox.Text = outputType;
            }
            else
            {
                propertiesContentPanel.Visible = false;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void statusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }
    }
}
