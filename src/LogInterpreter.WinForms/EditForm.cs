using LogInterpreter.Abstractions;
using LogInterpreter.Abstractions.DefaultImplementations;
using System.Reflection;

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
            // Clear configuration panel first
            ClearConfigurationPanel();

            if (pipelineListBox.SelectedItem is IPipelineItem selectedItem)
            {
                UpdateDetailsPanel(selectedItem);
                GenerateConfigurationControls(selectedItem);
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

        private void ClearConfigurationPanel()
        {
            configurationPanel.Controls.Clear();
        }

        private void GenerateConfigurationControls(IPipelineItem item)
        {
            var itemType = item.GetType();
            var properties = itemType.GetProperties()
                .Where(p => p.GetCustomAttribute<ConfigurableAttribute>() != null)
                .ToList();

            if (!properties.Any())
            {
                return;
            }

            configurationPanel.SuspendLayout();

            int yPosition = 10;
            const int labelHeight = 20;
            const int controlHeight = 23;
            const int spacing = 10;
            const int leftMargin = 10;
            const int rightMargin = 10;

            foreach (var property in properties)
            {
                var configurableAttr = property.GetCustomAttribute<ConfigurableAttribute>()!;
                
                // Create label for property name
                var label = new Label
                {
                    Text = property.Name + ":",
                    Location = new Point(leftMargin, yPosition),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 9F, FontStyle.Regular)
                };
                configurationPanel.Controls.Add(label);
                yPosition += labelHeight + 5;

                // Handle Func types
                if (IsFuncType(property.PropertyType))
                {
                    var funcTypeInfo = GetFuncTypeInfo(property.PropertyType);
                    
                    // Display parameter and return types
                    var typeInfoLabel = new Label
                    {
                        Text = funcTypeInfo,
                        Location = new Point(leftMargin + 20, yPosition),
                        AutoSize = true,
                        MaximumSize = new Size(configurationPanel.Width - leftMargin - rightMargin - 40, 0),
                        Font = new Font("Segoe UI", 8.25F, FontStyle.Italic)
                    };
                    configurationPanel.Controls.Add(typeInfoLabel);
                    yPosition += typeInfoLabel.Height + 5;

                    // Create multiline textbox for Func - now editable
                    var textBox = new TextBox
                    {
                        Location = new Point(leftMargin, yPosition),
                        Width = configurationPanel.Width - leftMargin - rightMargin - 20,
                        Height = 200, // Approximately 10 lines
                        Multiline = true,
                        ScrollBars = ScrollBars.Both,
                        Font = new Font("Consolas", 9F),
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                        ReadOnly = false // Make it editable
                    };

                    // Try to get current value and display it
                    try
                    {
                        var funcValue = property.GetValue(item);
                        if (funcValue != null)
                        {
                            textBox.Text = $"// Function: {property.Name}\n// Type: {property.PropertyType.Name}\n// Note: Function editing is for documentation/reference only.\n// The actual function behavior cannot be modified at runtime.";
                        }
                    }
                    catch { }

                    // Store property reference for later update
                    textBox.Tag = new Tuple<IPipelineItem, PropertyInfo>(item, property);
                    
                    // Add event handler for focus loss
                    textBox.Leave += FuncTextBox_Leave;

                    configurationPanel.Controls.Add(textBox);
                    yPosition += textBox.Height + spacing;
                }
                // Handle Path configuration type
                else if (configurableAttr.Type == ConfigurationType.Path)
                {
                    var textBox = new TextBox
                    {
                        Location = new Point(leftMargin, yPosition),
                        Width = configurationPanel.Width - leftMargin - rightMargin - 60,
                        Height = controlHeight,
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                    };

                    // Set current value
                    try
                    {
                        var value = property.GetValue(item);
                        if (value != null)
                        {
                            textBox.Text = value.ToString();
                        }
                    }
                    catch { }

                    // Store property reference for later update
                    textBox.Tag = new Tuple<IPipelineItem, PropertyInfo>(item, property);
                    
                    // Add event handler for focus loss
                    textBox.Leave += PathTextBox_Leave;

                    var browseButton = new Button
                    {
                        Text = "...",
                        Location = new Point(configurationPanel.Width - rightMargin - 50, yPosition),
                        Width = 50,
                        Height = controlHeight,
                        Anchor = AnchorStyles.Top | AnchorStyles.Right
                    };

                    // Store property reference in button tag
                    browseButton.Tag = new Tuple<IPipelineItem, PropertyInfo, TextBox>(item, property, textBox);
                    browseButton.Click += BrowseButton_Click;

                    configurationPanel.Controls.Add(textBox);
                    configurationPanel.Controls.Add(browseButton);
                    yPosition += controlHeight + spacing;
                }
                // Handle default types (string, int, etc.)
                else
                {
                    Control inputControl;

                    if (property.PropertyType == typeof(bool))
                    {
                        var checkBox = new CheckBox
                        {
                            Location = new Point(leftMargin, yPosition),
                            AutoSize = true
                        };

                        try
                        {
                            var value = property.GetValue(item);
                            if (value is bool boolValue)
                            {
                                checkBox.Checked = boolValue;
                            }
                        }
                        catch { }

                        // Store property reference
                        checkBox.Tag = new Tuple<IPipelineItem, PropertyInfo>(item, property);
                        checkBox.CheckedChanged += CheckBox_CheckedChanged;

                        inputControl = checkBox;
                    }
                    else if (property.PropertyType == typeof(int) || 
                             property.PropertyType == typeof(long) ||
                             property.PropertyType == typeof(double) ||
                             property.PropertyType == typeof(decimal))
                    {
                        var numericUpDown = new NumericUpDown
                        {
                            Location = new Point(leftMargin, yPosition),
                            Width = configurationPanel.Width - leftMargin - rightMargin - 20,
                            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                            Minimum = decimal.MinValue,
                            Maximum = decimal.MaxValue
                        };

                        try
                        {
                            var value = property.GetValue(item);
                            if (value != null)
                            {
                                numericUpDown.Value = Convert.ToDecimal(value);
                            }
                        }
                        catch { }

                        // Store property reference
                        numericUpDown.Tag = new Tuple<IPipelineItem, PropertyInfo>(item, property);
                        numericUpDown.Leave += NumericUpDown_Leave;

                        inputControl = numericUpDown;
                    }
                    else
                    {
                        var textBox = new TextBox
                        {
                            Location = new Point(leftMargin, yPosition),
                            Width = configurationPanel.Width - leftMargin - rightMargin - 20,
                            Height = controlHeight,
                            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                        };

                        try
                        {
                            var value = property.GetValue(item);
                            if (value != null)
                            {
                                textBox.Text = value.ToString();
                            }
                        }
                        catch { }

                        // Store property reference
                        textBox.Tag = new Tuple<IPipelineItem, PropertyInfo>(item, property);
                        textBox.Leave += DefaultTextBox_Leave;

                        inputControl = textBox;
                    }

                    configurationPanel.Controls.Add(inputControl);
                    yPosition += (inputControl is CheckBox ? controlHeight : controlHeight) + spacing;
                }
            }

            configurationPanel.ResumeLayout();
        }

        private void BrowseButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is Tuple<IPipelineItem, PropertyInfo, TextBox> data)
            {
                var (item, property, textBox) = data;

                using var openFileDialog = new OpenFileDialog
                {
                    Title = $"Select {property.Name}",
                    Filter = "All Files (*.*)|*.*",
                    CheckFileExists = false,
                    CheckPathExists = false
                };

                // Set initial directory if path already exists
                if (!string.IsNullOrWhiteSpace(textBox.Text))
                {
                    try
                    {
                        if (File.Exists(textBox.Text))
                        {
                            openFileDialog.InitialDirectory = Path.GetDirectoryName(textBox.Text);
                            openFileDialog.FileName = Path.GetFileName(textBox.Text);
                        }
                        else if (Directory.Exists(textBox.Text))
                        {
                            openFileDialog.InitialDirectory = textBox.Text;
                        }
                    }
                    catch { }
                }

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox.Text = openFileDialog.FileName;
                    
                    // Update the property value immediately after dialog closes
                    try
                    {
                        if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(item, openFileDialog.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating property '{property.Name}': {ex.Message}", 
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void PathTextBox_Leave(object? sender, EventArgs e)
        {
            if (sender is TextBox textBox && textBox.Tag is Tuple<IPipelineItem, PropertyInfo> data)
            {
                var (item, property) = data;
                UpdatePropertyValue(item, property, textBox.Text);
            }
        }

        private void DefaultTextBox_Leave(object? sender, EventArgs e)
        {
            if (sender is TextBox textBox && textBox.Tag is Tuple<IPipelineItem, PropertyInfo> data)
            {
                var (item, property) = data;
                UpdatePropertyValue(item, property, textBox.Text);
            }
        }

        private void FuncTextBox_Leave(object? sender, EventArgs e)
        {
            if (sender is TextBox textBox && textBox.Tag is Tuple<IPipelineItem, PropertyInfo> data)
            {
                var (item, property) = data;
                // For Func properties, we store the text but can't actually modify the function
                // This is mainly for documentation/reference purposes
                // You might want to add logging or other handling here
            }
        }

        private void NumericUpDown_Leave(object? sender, EventArgs e)
        {
            if (sender is NumericUpDown numericUpDown && numericUpDown.Tag is Tuple<IPipelineItem, PropertyInfo> data)
            {
                var (item, property) = data;
                UpdatePropertyValue(item, property, numericUpDown.Value);
            }
        }

        private void CheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.Tag is Tuple<IPipelineItem, PropertyInfo> data)
            {
                var (item, property) = data;
                UpdatePropertyValue(item, property, checkBox.Checked);
            }
        }

        private void UpdatePropertyValue(IPipelineItem item, PropertyInfo property, object? value)
        {
            try
            {
                if (value == null)
                {
                    if (property.PropertyType.IsValueType && 
                        Nullable.GetUnderlyingType(property.PropertyType) == null)
                    {
                        // Cannot set null to non-nullable value type
                        return;
                    }
                    property.SetValue(item, null);
                    return;
                }

                // Handle type conversion
                if (property.PropertyType == typeof(string))
                {
                    property.SetValue(item, value.ToString());
                }
                else if (property.PropertyType == typeof(int))
                {
                    property.SetValue(item, Convert.ToInt32(value));
                }
                else if (property.PropertyType == typeof(long))
                {
                    property.SetValue(item, Convert.ToInt64(value));
                }
                else if (property.PropertyType == typeof(double))
                {
                    property.SetValue(item, Convert.ToDouble(value));
                }
                else if (property.PropertyType == typeof(decimal))
                {
                    property.SetValue(item, Convert.ToDecimal(value));
                }
                else if (property.PropertyType == typeof(bool))
                {
                    property.SetValue(item, Convert.ToBoolean(value));
                }
                else if (property.PropertyType == typeof(int?))
                {
                    property.SetValue(item, string.IsNullOrWhiteSpace(value.ToString()) ? null : (int?)Convert.ToInt32(value));
                }
                else if (property.PropertyType == typeof(long?))
                {
                    property.SetValue(item, string.IsNullOrWhiteSpace(value.ToString()) ? null : (long?)Convert.ToInt64(value));
                }
                else if (property.PropertyType == typeof(double?))
                {
                    property.SetValue(item, string.IsNullOrWhiteSpace(value.ToString()) ? null : (double?)Convert.ToDouble(value));
                }
                else if (property.PropertyType == typeof(decimal?))
                {
                    property.SetValue(item, string.IsNullOrWhiteSpace(value.ToString()) ? null : (decimal?)Convert.ToDecimal(value));
                }
                else if (property.PropertyType == typeof(bool?))
                {
                    property.SetValue(item, string.IsNullOrWhiteSpace(value.ToString()) ? null : (bool?)Convert.ToBoolean(value));
                }
                else
                {
                    // Try direct assignment
                    property.SetValue(item, value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating property '{property.Name}': {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private bool IsFuncType(Type type)
        {
            if (!type.IsGenericType)
                return false;

            var genericTypeDef = type.GetGenericTypeDefinition();
            
            // Check if it's a Func type (Func<> has up to 17 type parameters in .NET)
            // But more commonly we check by name prefix
            return genericTypeDef.FullName?.StartsWith("System.Func`") == true;
        }

        private string GetFuncTypeInfo(Type funcType)
        {
            if (!funcType.IsGenericType)
                return "Unknown function type";

            var genericArgs = funcType.GetGenericArguments();
            if (genericArgs.Length == 0)
                return "No parameters, No return";

            var parameters = genericArgs.Take(genericArgs.Length - 1).ToList();
            var returnType = genericArgs.Last();

            var paramText = parameters.Any() 
                ? string.Join(", ", parameters.Select(GetFriendlyTypeName))
                : "No parameters";

            return $"Parameters: {paramText}\nReturn: {GetFriendlyTypeName(returnType)}";
        }
    }
}
