using Kavics.LogInterpreter.Abstractions;
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

            // Parse the pipeline definition
            var pipelineDefinition = @"LogSource
    LogPath=/var/log/app.log
    FirstFileName=/var/log/app-2025-12-01.txt
ConsoleWriter
OneLineLogFileReader
CompactJsonLogParser
ErrorAggregator
    AggregationFileName=/var/log/aggregation.txt
EntryCounter
    AggregationFileName=/var/log/summary.txt";

            var pipeline = Pipeline.Parse(pipelineDefinition);

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

            // Add cards from the parsed pipeline
            foreach (var item in pipeline.Items)
            {
                var itemType = item.GetType();
                var descriptor = Pipeline.AvailableItems.FirstOrDefault(d => d.Type == itemType);

                if (descriptor != null)
                {
                    // Get input and output types from the pipeline item
                    var pipelineItemInterface = itemType.GetInterfaces()
                        .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineItem<,>));

                    string inputType = "-";
                    string outputType = "-";

                    if (pipelineItemInterface != null)
                    {
                        var genericArgs = pipelineItemInterface.GetGenericArguments();
                        inputType = genericArgs[0].Name;
                        outputType = genericArgs[1].Name;
                    }

                    AddPipelineCard(descriptor.Name, descriptor.Description, inputType, outputType, cardWidth);
                }
            }
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
