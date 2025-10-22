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
            detailsPanel = new Panel();
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
            detailsPanel.SuspendLayout();
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
            pipelineListBox.Location = new Point(0, 15);
            pipelineListBox.Name = "pipelineListBox";
            pipelineListBox.Size = new Size(449, 585);
            pipelineListBox.TabIndex = 1;
            pipelineListBox.SelectedIndexChanged += pipelineListBox_SelectedIndexChanged;
            // 
            // pipelineLabel
            // 
            pipelineLabel.AutoSize = true;
            pipelineLabel.Dock = DockStyle.Top;
            pipelineLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 238);
            pipelineLabel.Location = new Point(0, 0);
            pipelineLabel.Name = "pipelineLabel";
            pipelineLabel.Size = new Size(51, 15);
            pipelineLabel.TabIndex = 0;
            pipelineLabel.Text = "Pipeline";
            // 
            // rightPanel
            // 
            rightPanel.Controls.Add(detailsPanel);
            rightPanel.Controls.Add(propertiesLabel);
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.Location = new Point(0, 0);
            rightPanel.Name = "rightPanel";
            rightPanel.Size = new Size(537, 600);
            rightPanel.TabIndex = 0;
            // 
            // detailsPanel
            // 
            detailsPanel.Controls.Add(outputTypeTextBox);
            detailsPanel.Controls.Add(outputTypeLabel);
            detailsPanel.Controls.Add(inputTypeTextBox);
            detailsPanel.Controls.Add(inputTypeLabel);
            detailsPanel.Controls.Add(typeNameTextBox);
            detailsPanel.Controls.Add(typeNameLabel);
            detailsPanel.Dock = DockStyle.Top;
            detailsPanel.Location = new Point(0, 15);
            detailsPanel.Name = "detailsPanel";
            detailsPanel.Size = new Size(537, 89);
            detailsPanel.TabIndex = 0;
            // 
            // outputTypeTextBox
            // 
            outputTypeTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            outputTypeTextBox.Location = new Point(90, 58);
            outputTypeTextBox.Name = "outputTypeTextBox";
            outputTypeTextBox.ReadOnly = true;
            outputTypeTextBox.Size = new Size(444, 23);
            outputTypeTextBox.TabIndex = 5;
            // 
            // outputTypeLabel
            // 
            outputTypeLabel.AutoSize = true;
            outputTypeLabel.Location = new Point(4, 61);
            outputTypeLabel.Name = "outputTypeLabel";
            outputTypeLabel.Size = new Size(75, 15);
            outputTypeLabel.TabIndex = 4;
            outputTypeLabel.Text = "Output Type:";
            // 
            // inputTypeTextBox
            // 
            inputTypeTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            inputTypeTextBox.Location = new Point(90, 29);
            inputTypeTextBox.Name = "inputTypeTextBox";
            inputTypeTextBox.ReadOnly = true;
            inputTypeTextBox.Size = new Size(444, 23);
            inputTypeTextBox.TabIndex = 3;
            // 
            // inputTypeLabel
            // 
            inputTypeLabel.AutoSize = true;
            inputTypeLabel.Location = new Point(3, 32);
            inputTypeLabel.Name = "inputTypeLabel";
            inputTypeLabel.Size = new Size(65, 15);
            inputTypeLabel.TabIndex = 2;
            inputTypeLabel.Text = "Input Type:";
            // 
            // typeNameTextBox
            // 
            typeNameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            typeNameTextBox.Location = new Point(90, 3);
            typeNameTextBox.Name = "typeNameTextBox";
            typeNameTextBox.ReadOnly = true;
            typeNameTextBox.Size = new Size(444, 23);
            typeNameTextBox.TabIndex = 1;
            // 
            // typeNameLabel
            // 
            typeNameLabel.AutoSize = true;
            typeNameLabel.Location = new Point(4, 6);
            typeNameLabel.Name = "typeNameLabel";
            typeNameLabel.Size = new Size(34, 15);
            typeNameLabel.TabIndex = 0;
            typeNameLabel.Text = "Type:";
            // 
            // propertiesLabel
            // 
            propertiesLabel.AutoSize = true;
            propertiesLabel.Dock = DockStyle.Top;
            propertiesLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 238);
            propertiesLabel.Location = new Point(0, 0);
            propertiesLabel.Name = "propertiesLabel";
            propertiesLabel.Size = new Size(65, 15);
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
            detailsPanel.ResumeLayout(false);
            detailsPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer;
        private Panel leftPanel;
        private ListBox pipelineListBox;
        private Label pipelineLabel;
        private Panel rightPanel;
        private Label propertiesLabel;
        private Panel detailsPanel;
        private TextBox outputTypeTextBox;
        private Label outputTypeLabel;
        private TextBox inputTypeTextBox;
        private Label inputTypeLabel;
        private TextBox typeNameTextBox;
        private Label typeNameLabel;
    }
}
