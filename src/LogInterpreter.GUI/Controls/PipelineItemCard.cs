namespace LogInterpreter.GUI.Controls
{
    /// <summary>
    /// A card control representing a pipeline item with type, description, and input/output types.
    /// </summary>
    public partial class PipelineItemCard : UserControl
    {
        private bool _isSelected;
        private Color _normalBackColor = Color.White;
        private Color _selectedBackColor = Color.FromArgb(230, 240, 255);
        private Color _firstItemBackColor = Color.LightBlue;

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
            set => inputTypeLabel.Text = value;
        }

        public string OutputType
        {
            get => outputTypeLabel.Text;
            set => outputTypeLabel.Text = value;
        }

        public bool IsFirstItem
        {
            set
            {
                if (value)
                {
                    _normalBackColor = _firstItemBackColor;
                    UpdateBackColor();
                }
            }
        }

        private void UpdateBackColor()
        {
            this.BackColor = _isSelected ? _selectedBackColor : _normalBackColor;
        }
    }
}
