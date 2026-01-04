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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (_selectedCard == null)
                return base.ProcessCmdKey(ref msg, keyData);

            // Only handle Up and Down keys
            if (keyData != Keys.Down && keyData != Keys.Up)
                return base.ProcessCmdKey(ref msg, keyData);

            int currentIndex = pipelineFlowLayoutPanel.Controls.IndexOf(_selectedCard);
            PipelineItemCard? nextCard = null;

            if (keyData == Keys.Down)
            {
                // Move to next card
                if (currentIndex < pipelineFlowLayoutPanel.Controls.Count - 1)
                {
                    nextCard = pipelineFlowLayoutPanel.Controls[currentIndex + 1] as PipelineItemCard;
                }
            }
            else if (keyData == Keys.Up)
            {
                // Move to previous card
                if (currentIndex > 0)
                {
                    nextCard = pipelineFlowLayoutPanel.Controls[currentIndex - 1] as PipelineItemCard;
                }
            }

            if (nextCard != null)
            {
                nextCard.IsSelected = true;
                // Scroll the card into view
                pipelineFlowLayoutPanel.ScrollControlIntoView(nextCard);
            }

            // Always return true for Up/Down keys to prevent default scrolling behavior
            return true;
        }

        private void PipelineFlowLayoutPanel_Resize(object? sender, EventArgs e)
        {
            // Adjust all card widths when panel resizes
            int baseCardWidth = pipelineFlowLayoutPanel.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 8;
            foreach (Control control in pipelineFlowLayoutPanel.Controls)
            {
                if (control is PipelineItemCard card)
                {
                    // Pipeline card (first item) gets full width, others are indented by 12px
                    if (card.Margin.Left == 0)
                    {
                        // Pipeline card - no indent
                        card.Width = baseCardWidth;
                    }
                    else
                    {
                        // Pipeline item cards - reduce width by 12px to account for left margin
                        card.Width = baseCardWidth - 12;
                    }
                }
            }
        }

        private void InitializePipelineCards()
        {
            // Clear designer sample cards
            pipelineFlowLayoutPanel.Controls.Clear();
            
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
                Width = cardWidth,
                Margin = new Padding(0, 0, 0, 4), // No left margin for Pipeline card
                Tag = null // Pipeline card has no PipelineItem
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

                    AddPipelineCard(descriptor.Name, descriptor.Description, inputType, outputType, cardWidth, item, descriptor);
                }
            }

            // Select the Pipeline card by default
            pipelineCard.IsSelected = true;
        }

        private void AddPipelineCard(string itemType, string description, string inputType, string outputType, 
            int cardWidth, IPipelineItem pipelineItem, PipelineItemDescriptor descriptor)
        {
            var card = new PipelineItemCard
            {
                ItemType = itemType,
                Description = description,
                InputType = $"Input: {inputType}",
                OutputType = $"Output: {outputType}",
                Width = cardWidth - 12, // Reduce width to account for left margin
                Margin = new Padding(12, 0, 0, 4), // 12px left margin for pipeline items
                Tag = new { Item = pipelineItem, Descriptor = descriptor }
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
                
                propertyInputValueLabel.Text = inputType;
                propertyOutputValueLabel.Text = outputType;

                // Clear existing dynamic property controls
                ClearDynamicPropertyControls();

                // Add dynamic property controls if Tag contains pipeline item info
                if (_selectedCard.Tag is { } tagObj)
                {
                    var item = tagObj.GetType().GetProperty("Item")?.GetValue(tagObj) as IPipelineItem;
                    var descriptor = tagObj.GetType().GetProperty("Descriptor")?.GetValue(tagObj) as PipelineItemDescriptor;

                    if (item != null && descriptor != null)
                    {
                        AddDynamicPropertyControls(item, descriptor);
                    }
                }
            }
            else
            {
                propertiesContentPanel.Visible = false;
            }
        }

        private int _dynamicControlsStartY = 138;
        private readonly List<Control> _dynamicControls = new();

        private void ClearDynamicPropertyControls()
        {
            foreach (var control in _dynamicControls)
            {
                propertiesContentPanel.Controls.Remove(control);
                control.Dispose();
            }
            _dynamicControls.Clear();
        }

        private void AddDynamicPropertyControls(IPipelineItem item, PipelineItemDescriptor descriptor)
        {
            int yPosition = _dynamicControlsStartY;
            int panelWidth = propertiesContentPanel.Width - 6;

            foreach (var config in descriptor.Configurations)
            {
                var property = item.GetType().GetProperty(config.Name);
                if (property == null)
                    continue;

                // Property Name Label (bold, readonly)
                var nameLabel = new Label
                {
                    Text = config.Name,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Location = new Point(3, yPosition),
                    AutoSize = true,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left
                };
                propertiesContentPanel.Controls.Add(nameLabel);
                _dynamicControls.Add(nameLabel);
                yPosition += 20;

                // Description TextBox (3 rows, scrollable, readonly)
                var descTextBox = new TextBox
                {
                    Text = config.Description,
                    Font = new Font("Segoe UI", 9F),
                    ForeColor = Color.DimGray,
                    BackColor = Color.Honeydew,
                    BorderStyle = BorderStyle.None,
                    Location = new Point(3, yPosition),
                    Width = panelWidth,
                    Height = 45,
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    TabStop = false,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                };
                propertiesContentPanel.Controls.Add(descTextBox);
                _dynamicControls.Add(descTextBox);
                yPosition += 50;

                // Value TextBox (editable)
                var valueTextBox = new TextBox
                {
                    Text = property.GetValue(item)?.ToString() ?? string.Empty,
                    Font = new Font("Segoe UI", 9F),
                    Location = new Point(3, yPosition),
                    Width = panelWidth,
                    BorderStyle = BorderStyle.FixedSingle,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    Tag = new { Property = property, Item = item }
                };
                valueTextBox.TextChanged += ValueTextBox_TextChanged;
                propertiesContentPanel.Controls.Add(valueTextBox);
                _dynamicControls.Add(valueTextBox);
                yPosition += 30;
            }
        }

        private void ValueTextBox_TextChanged(object? sender, EventArgs e)
        {
            if (sender is TextBox textBox && textBox.Tag is { } tagObj)
            {
                var property = tagObj.GetType().GetProperty("Property")?.GetValue(tagObj) as System.Reflection.PropertyInfo;
                var item = tagObj.GetType().GetProperty("Item")?.GetValue(tagObj);

                if (property != null && item != null)
                {
                    try
                    {
                        var convertedValue = Convert.ChangeType(textBox.Text, property.PropertyType);
                        property.SetValue(item, convertedValue);
                    }
                    catch
                    {
                        // Ignore conversion errors
                    }
                }
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
