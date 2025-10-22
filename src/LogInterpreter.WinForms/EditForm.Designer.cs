namespace LogInterpreter.WinForms
{
    partial class EditForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            splitContainer = new SplitContainer();
            leftPanel = new Panel();
            pipelineListBox = new ListBox();
            pipelineLabel = new Label();
            rightPanel = new Panel();
            configurationLabel = new Label();
            propertiesPanel = new Panel();
            outputTypeTextBox = new TextBox();
            outputTypeLabel = new Label();
            inputTypeTextBox = new TextBox();
            inputTypeLabel = new Label();
            typeNameTextBox = new TextBox();
            typeNameLabel = new Label();
            propertiesLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            leftPanel.SuspendLayout();
            rightPanel.SuspendLayout();
            propertiesPanel.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer
            // 
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.Location = new Point(0, 0);
            splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(leftPanel);
            splitContainer.Panel1MinSize = 200;
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(rightPanel);
            splitContainer.Panel2MinSize = 300;
            splitContainer.Size = new Size(990, 600);
            splitContainer.SplitterDistance = 449;
            splitContainer.TabIndex = 0;
            // 
            // leftPanel
            // 
            leftPanel.Controls.Add(pipelineListBox);
            leftPanel.Controls.Add(pipelineLabel);
            leftPanel.Dock = DockStyle.Fill;
            leftPanel.Location = new Point(0, 0);
            leftPanel.Name = "leftPanel";
            leftPanel.Size = new Size(449, 600);
            leftPanel.TabIndex = 0;
            // 
            // pipelineListBox
            // 
            pipelineListBox.Dock = DockStyle.Fill;
            pipelineListBox.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 238);
            pipelineListBox.FormattingEnabled = true;
            pipelineListBox.Location = new Point(0, 20);
            pipelineListBox.Name = "pipelineListBox";
            pipelineListBox.Size = new Size(449, 580);
            pipelineListBox.TabIndex = 1;
            pipelineListBox.SelectedIndexChanged += pipelineListBox_SelectedIndexChanged;
            // 
            // pipelineLabel
            // 
            pipelineLabel.AutoSize = true;
            pipelineLabel.Dock = DockStyle.Top;
            pipelineLabel.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 238);
            pipelineLabel.Location = new Point(0, 0);
            pipelineLabel.Name = "pipelineLabel";
            pipelineLabel.Size = new Size(64, 20);
            pipelineLabel.TabIndex = 0;
            pipelineLabel.Text = "Pipeline";
            // 
            // rightPanel
            // 
            rightPanel.Controls.Add(configurationLabel);
            rightPanel.Controls.Add(propertiesPanel);
            rightPanel.Controls.Add(propertiesLabel);
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.Location = new Point(0, 0);
            rightPanel.Name = "rightPanel";
            rightPanel.Padding = new Padding(10);
            rightPanel.Size = new Size(537, 600);
            rightPanel.TabIndex = 0;
            // 
            // configurationLabel
            // 
            configurationLabel.AutoSize = true;
            configurationLabel.Dock = DockStyle.Top;
            configurationLabel.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 238);
            configurationLabel.Location = new Point(10, 121);
            configurationLabel.Name = "configurationLabel";
            configurationLabel.Size = new Size(106, 20);
            configurationLabel.TabIndex = 2;
            configurationLabel.Text = "Configuration";
            // 
            // propertiesPanel
            // 
            propertiesPanel.Controls.Add(outputTypeTextBox);
            propertiesPanel.Controls.Add(outputTypeLabel);
            propertiesPanel.Controls.Add(inputTypeTextBox);
            propertiesPanel.Controls.Add(inputTypeLabel);
            propertiesPanel.Controls.Add(typeNameTextBox);
            propertiesPanel.Controls.Add(typeNameLabel);
            propertiesPanel.Dock = DockStyle.Top;
            propertiesPanel.Location = new Point(10, 30);
            propertiesPanel.Name = "propertiesPanel";
            propertiesPanel.Size = new Size(517, 91);
            propertiesPanel.TabIndex = 0;
            // 
            // outputTypeTextBox
            // 
            outputTypeTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            outputTypeTextBox.Location = new Point(84, 61);
            outputTypeTextBox.Name = "outputTypeTextBox";
            outputTypeTextBox.ReadOnly = true;
            outputTypeTextBox.Size = new Size(429, 23);
            outputTypeTextBox.TabIndex = 5;
            // 
            // outputTypeLabel
            // 
            outputTypeLabel.AutoSize = true;
            outputTypeLabel.Location = new Point(3, 64);
            outputTypeLabel.Name = "outputTypeLabel";
            outputTypeLabel.Size = new Size(75, 15);
            outputTypeLabel.TabIndex = 4;
            outputTypeLabel.Text = "Output Type:";
            // 
            // inputTypeTextBox
            // 
            inputTypeTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            inputTypeTextBox.Location = new Point(84, 32);
            inputTypeTextBox.Name = "inputTypeTextBox";
            inputTypeTextBox.ReadOnly = true;
            inputTypeTextBox.Size = new Size(429, 23);
            inputTypeTextBox.TabIndex = 3;
            // 
            // inputTypeLabel
            // 
            inputTypeLabel.AutoSize = true;
            inputTypeLabel.Location = new Point(3, 35);
            inputTypeLabel.Name = "inputTypeLabel";
            inputTypeLabel.Size = new Size(65, 15);
            inputTypeLabel.TabIndex = 2;
            inputTypeLabel.Text = "Input Type:";
            // 
            // typeNameTextBox
            // 
            typeNameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            typeNameTextBox.Location = new Point(84, 3);
            typeNameTextBox.Name = "typeNameTextBox";
            typeNameTextBox.ReadOnly = true;
            typeNameTextBox.Size = new Size(429, 23);
            typeNameTextBox.TabIndex = 1;
            // 
            // typeNameLabel
            // 
            typeNameLabel.AutoSize = true;
            typeNameLabel.Location = new Point(3, 6);
            typeNameLabel.Name = "typeNameLabel";
            typeNameLabel.Size = new Size(34, 15);
            typeNameLabel.TabIndex = 0;
            typeNameLabel.Text = "Type:";
            // 
            // propertiesLabel
            // 
            propertiesLabel.AutoSize = true;
            propertiesLabel.Dock = DockStyle.Top;
            propertiesLabel.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 238);
            propertiesLabel.Location = new Point(10, 10);
            propertiesLabel.Name = "propertiesLabel";
            propertiesLabel.Size = new Size(81, 20);
            propertiesLabel.TabIndex = 1;
            propertiesLabel.Text = "Properties";
            // 
            // EditForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(990, 600);
            Controls.Add(splitContainer);
            MinimumSize = new Size(600, 400);
            Name = "EditForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Edit Pipeline";
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            leftPanel.ResumeLayout(false);
            leftPanel.PerformLayout();
            rightPanel.ResumeLayout(false);
            rightPanel.PerformLayout();
            propertiesPanel.ResumeLayout(false);
            propertiesPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer;
        private Panel leftPanel;
        private ListBox pipelineListBox;
        private Label pipelineLabel;
        private Panel rightPanel;
        private Label propertiesLabel;
        private Panel propertiesPanel;
        private TextBox outputTypeTextBox;
        private Label outputTypeLabel;
        private TextBox inputTypeTextBox;
        private Label inputTypeLabel;
        private TextBox typeNameTextBox;
        private Label typeNameLabel;
        private Label configurationLabel;
    }
}
