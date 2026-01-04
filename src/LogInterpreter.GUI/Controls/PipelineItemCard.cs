namespace LogInterpreter.GUI.Controls
{
    /// <summary>
    /// A card control representing a pipeline item with type, description, and input/output types.
    /// </summary>
    public partial class PipelineItemCard : UserControl
    {
        private bool _isSelected;
        private Color _normalBackColor = Color.White;
        private Color _selectedBackColor = Color.LightBlue;

        public event EventHandler? Selected;

        public PipelineItemCard()
        {
            InitializeComponent();
            
            // Subscribe to click events for all controls to enable selection
            this.Click += PipelineItemCard_Click;
            foreach (Control control in this.Controls)
            {
                control.Click += PipelineItemCard_Click;
                foreach (Control childControl in control.Controls)
                {
                    childControl.Click += PipelineItemCard_Click;
                }
            }
            
            this.Cursor = Cursors.Hand;
        }

        private void PipelineItemCard_Click(object? sender, EventArgs e)
        {
            IsSelected = true;
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    UpdateBackColor();
                    if (_isSelected)
                    {
                        Selected?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        public string ItemType
        {
            get => itemTypeLabel.Text;
            set => itemTypeLabel.Text = value;
        }

        public string Description
        {
            get => descriptionLabel.Text;
            set => descriptionLabel.Text = value;
        }

        public string InputType
        {
            get => inputTypeLabel.Text;
            set
            {
                // Remove "Input: " prefix if present
                var cleanValue = value.Replace("Input: ", "").Trim();
                inputTypeLabel.Text = $"{cleanValue} -->";
            }
        }

        public string OutputType
        {
            get => outputTypeLabel.Text;
            set
            {
                // Remove "Output: " prefix if present
                var cleanValue = value.Replace("Output: ", "").Trim();
                outputTypeLabel.Text = $"--> {cleanValue}";
            }
        }

        public bool IsFirstItem
        {
            set
            {
                if (value)
                {
                    // Pipeline card has same background as other cards
                    _normalBackColor = Color.White;
                    UpdateBackColor();
                    // Hide the input/output panel for the Pipeline card
                    ioPanel.Visible = false;
                    // Reduce the height of the card when IO panel is hidden
                    this.Height = typePanel.Height + this.Padding.Vertical;
                }
            }
        }

        public bool ShowDescription
        {
            set
            {
                descriptionLabel.Visible = value;
                if (!value)
                {
                    // Hide the description and adjust layout
                    typePanel.Padding = new Padding(8, 8, 8, 8);
                    typePanel.Height = itemTypeLabel.Height + typePanel.Padding.Vertical;
                    // Recalculate card height: typePanel + ioPanel + card padding
                    this.Height = typePanel.Height + ioPanel.Height + this.Padding.Vertical;
                }
                else
                {
                    // Show the description with normal layout
                    typePanel.Padding = new Padding(8, 8, 8, 0);
                    typePanel.Height = 55; // Default height with description
                    this.Height = 80; // Default total height
                }
            }
        }

        private void UpdateBackColor()
        {
            this.BackColor = _isSelected ? _selectedBackColor : _normalBackColor;
        }
    }
}
