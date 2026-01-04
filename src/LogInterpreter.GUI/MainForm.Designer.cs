namespace LogInterpreter.GUI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            saveToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            undoToolStripMenuItem = new ToolStripMenuItem();
            redoToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            cutToolStripMenuItem = new ToolStripMenuItem();
            copyToolStripMenuItem = new ToolStripMenuItem();
            pasteToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            selectAllToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            statusBarToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            statusStrip = new StatusStrip();
            toolStripStatusLabel = new ToolStripStatusLabel();
            navigationPanel = new Panel();
            navigationLabel = new Label();
            splitContainer = new SplitContainer();
            pipelinePanel = new Panel();
            pipelineFlowLayoutPanel = new FlowLayoutPanel();
            samplePipelineCard = new LogInterpreter.GUI.Controls.PipelineItemCard();
            sampleItemCard = new LogInterpreter.GUI.Controls.PipelineItemCard();
            propertiesPanel = new Panel();
            propertiesContentPanel = new Panel();
            propertyOutputValueLabel = new Label();
            propertyOutputLabel = new Label();
            propertyInputValueLabel = new Label();
            propertyInputLabel = new Label();
            propertyDescriptionTextBox = new TextBox();
            propertyTypeLabel = new Label();
            menuStrip.SuspendLayout();
            statusStrip.SuspendLayout();
            navigationPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            pipelinePanel.SuspendLayout();
            pipelineFlowLayoutPanel.SuspendLayout();
            propertiesPanel.SuspendLayout();
            propertiesContentPanel.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, viewToolStripMenuItem, helpToolStripMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(826, 24);
            menuStrip.TabIndex = 0;
            menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, toolStripSeparator1, saveToolStripMenuItem, saveAsToolStripMenuItem, toolStripSeparator2, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            newToolStripMenuItem.Size = new Size(146, 22);
            newToolStripMenuItem.Text = "&New";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            openToolStripMenuItem.Size = new Size(146, 22);
            openToolStripMenuItem.Text = "&Open";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(143, 6);
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveToolStripMenuItem.Size = new Size(146, 22);
            saveToolStripMenuItem.Text = "&Save";
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new Size(146, 22);
            saveAsToolStripMenuItem.Text = "Save &As...";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(143, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(146, 22);
            exitToolStripMenuItem.Text = "E&xit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { undoToolStripMenuItem, redoToolStripMenuItem, toolStripSeparator3, cutToolStripMenuItem, copyToolStripMenuItem, pasteToolStripMenuItem, toolStripSeparator4, selectAllToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(39, 20);
            editToolStripMenuItem.Text = "&Edit";
            // 
            // undoToolStripMenuItem
            // 
            undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            undoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
            undoToolStripMenuItem.Size = new Size(164, 22);
            undoToolStripMenuItem.Text = "&Undo";
            // 
            // redoToolStripMenuItem
            // 
            redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            redoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Y;
            redoToolStripMenuItem.Size = new Size(164, 22);
            redoToolStripMenuItem.Text = "&Redo";
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(161, 6);
            // 
            // cutToolStripMenuItem
            // 
            cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            cutToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.X;
            cutToolStripMenuItem.Size = new Size(164, 22);
            cutToolStripMenuItem.Text = "Cu&t";
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.C;
            copyToolStripMenuItem.Size = new Size(164, 22);
            copyToolStripMenuItem.Text = "&Copy";
            // 
            // pasteToolStripMenuItem
            // 
            pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            pasteToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.V;
            pasteToolStripMenuItem.Size = new Size(164, 22);
            pasteToolStripMenuItem.Text = "&Paste";
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(161, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            selectAllToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.A;
            selectAllToolStripMenuItem.Size = new Size(164, 22);
            selectAllToolStripMenuItem.Text = "Select &All";
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { statusBarToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(44, 20);
            viewToolStripMenuItem.Text = "&View";
            // 
            // statusBarToolStripMenuItem
            // 
            statusBarToolStripMenuItem.Checked = true;
            statusBarToolStripMenuItem.CheckOnClick = true;
            statusBarToolStripMenuItem.CheckState = CheckState.Checked;
            statusBarToolStripMenuItem.Name = "statusBarToolStripMenuItem";
            statusBarToolStripMenuItem.Size = new Size(126, 22);
            statusBarToolStripMenuItem.Text = "&Status Bar";
            statusBarToolStripMenuItem.Click += statusBarToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(116, 22);
            aboutToolStripMenuItem.Text = "&About...";
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel });
            statusStrip.Location = new Point(0, 428);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(826, 22);
            statusStrip.TabIndex = 1;
            statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new Size(39, 17);
            toolStripStatusLabel.Text = "Ready";
            // 
            // navigationPanel
            // 
            navigationPanel.BackColor = Color.LightSteelBlue;
            navigationPanel.Controls.Add(navigationLabel);
            navigationPanel.Dock = DockStyle.Top;
            navigationPanel.Location = new Point(0, 24);
            navigationPanel.Name = "navigationPanel";
            navigationPanel.Size = new Size(826, 60);
            navigationPanel.TabIndex = 2;
            // 
            // navigationLabel
            // 
            navigationLabel.AutoSize = true;
            navigationLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            navigationLabel.Location = new Point(12, 18);
            navigationLabel.Name = "navigationLabel";
            navigationLabel.Size = new Size(96, 21);
            navigationLabel.TabIndex = 0;
            navigationLabel.Text = "Navigation";
            // 
            // splitContainer
            // 
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.Location = new Point(0, 84);
            splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(pipelinePanel);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(propertiesPanel);
            splitContainer.Size = new Size(826, 344);
            splitContainer.SplitterDistance = 485;
            splitContainer.TabIndex = 3;
            // 
            // pipelinePanel
            // 
            pipelinePanel.BackColor = Color.WhiteSmoke;
            pipelinePanel.Controls.Add(pipelineFlowLayoutPanel);
            pipelinePanel.Dock = DockStyle.Fill;
            pipelinePanel.Location = new Point(0, 0);
            pipelinePanel.Name = "pipelinePanel";
            pipelinePanel.Size = new Size(485, 344);
            pipelinePanel.TabIndex = 0;
            // 
            // pipelineFlowLayoutPanel
            // 
            pipelineFlowLayoutPanel.AutoScroll = true;
            pipelineFlowLayoutPanel.Controls.Add(samplePipelineCard);
            pipelineFlowLayoutPanel.Controls.Add(sampleItemCard);
            pipelineFlowLayoutPanel.Dock = DockStyle.Fill;
            pipelineFlowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            pipelineFlowLayoutPanel.Location = new Point(0, 0);
            pipelineFlowLayoutPanel.Name = "pipelineFlowLayoutPanel";
            pipelineFlowLayoutPanel.Padding = new Padding(4);
            pipelineFlowLayoutPanel.Size = new Size(485, 344);
            pipelineFlowLayoutPanel.TabIndex = 1;
            pipelineFlowLayoutPanel.WrapContents = false;
            // 
            // samplePipelineCard
            // 
            samplePipelineCard.BackColor = Color.White;
            samplePipelineCard.BorderStyle = BorderStyle.FixedSingle;
            samplePipelineCard.Cursor = Cursors.Hand;
            samplePipelineCard.Description = "The main pipeline container";
            samplePipelineCard.InputType = "Input: -";
            samplePipelineCard.IsSelected = false;
            samplePipelineCard.ItemType = "Pipeline";
            samplePipelineCard.Location = new Point(4, 4);
            samplePipelineCard.Margin = new Padding(0, 0, 0, 4);
            samplePipelineCard.Name = "samplePipelineCard";
            samplePipelineCard.OutputType = "Output: -";
            samplePipelineCard.Padding = new Padding(0, 0, 0, 4);
            samplePipelineCard.Size = new Size(455, 60);
            samplePipelineCard.TabIndex = 0;
            // 
            // sampleItemCard
            // 
            sampleItemCard.BackColor = Color.White;
            sampleItemCard.BorderStyle = BorderStyle.FixedSingle;
            sampleItemCard.Cursor = Cursors.Hand;
            sampleItemCard.Description = "Reads log entries from a source";
            sampleItemCard.InputType = "Input: LogSource";
            sampleItemCard.IsSelected = false;
            sampleItemCard.ItemType = "LogReader";
            sampleItemCard.Location = new Point(16, 68);
            sampleItemCard.Margin = new Padding(12, 0, 0, 4);
            sampleItemCard.Name = "sampleItemCard";
            sampleItemCard.OutputType = "Output: String";
            sampleItemCard.Padding = new Padding(0, 0, 0, 4);
            sampleItemCard.Size = new Size(443, 85);
            sampleItemCard.TabIndex = 1;
            // 
            // propertiesPanel
            // 
            propertiesPanel.BackColor = Color.Honeydew;
            propertiesPanel.Controls.Add(propertiesContentPanel);
            propertiesPanel.Dock = DockStyle.Fill;
            propertiesPanel.Location = new Point(0, 0);
            propertiesPanel.Margin = new Padding(0);
            propertiesPanel.Name = "propertiesPanel";
            propertiesPanel.Size = new Size(337, 344);
            propertiesPanel.TabIndex = 0;
            // 
            // propertiesContentPanel
            // 
            propertiesContentPanel.AutoScroll = true;
            propertiesContentPanel.Controls.Add(propertyOutputValueLabel);
            propertiesContentPanel.Controls.Add(propertyOutputLabel);
            propertiesContentPanel.Controls.Add(propertyInputValueLabel);
            propertiesContentPanel.Controls.Add(propertyInputLabel);
            propertiesContentPanel.Controls.Add(propertyDescriptionTextBox);
            propertiesContentPanel.Controls.Add(propertyTypeLabel);
            propertiesContentPanel.Dock = DockStyle.Fill;
            propertiesContentPanel.Location = new Point(0, 0);
            propertiesContentPanel.Margin = new Padding(0);
            propertiesContentPanel.Name = "propertiesContentPanel";
            propertiesContentPanel.Size = new Size(337, 344);
            propertiesContentPanel.TabIndex = 1;
            propertiesContentPanel.Visible = false;
            // 
            // propertyOutputValueLabel
            // 
            propertyOutputValueLabel.AutoSize = true;
            propertyOutputValueLabel.Font = new Font("Segoe UI", 8F);
            propertyOutputValueLabel.ForeColor = Color.Black;
            propertyOutputValueLabel.Location = new Point(90, 108);
            propertyOutputValueLabel.Name = "propertyOutputValueLabel";
            propertyOutputValueLabel.Size = new Size(115, 13);
            propertyOutputValueLabel.TabIndex = 7;
            propertyOutputValueLabel.Text = "<output type name>";
            // 
            // propertyOutputLabel
            // 
            propertyOutputLabel.AutoSize = true;
            propertyOutputLabel.Font = new Font("Segoe UI", 9F);
            propertyOutputLabel.Location = new Point(3, 108);
            propertyOutputLabel.Name = "propertyOutputLabel";
            propertyOutputLabel.Size = new Size(76, 15);
            propertyOutputLabel.TabIndex = 6;
            propertyOutputLabel.Text = "Output Type:";
            // 
            // propertyInputValueLabel
            // 
            propertyInputValueLabel.AutoSize = true;
            propertyInputValueLabel.Font = new Font("Segoe UI", 8F);
            propertyInputValueLabel.ForeColor = Color.Black;
            propertyInputValueLabel.Location = new Point(90, 90);
            propertyInputValueLabel.Name = "propertyInputValueLabel";
            propertyInputValueLabel.Size = new Size(107, 13);
            propertyInputValueLabel.TabIndex = 5;
            propertyInputValueLabel.Text = "<input type name>";
            // 
            // propertyInputLabel
            // 
            propertyInputLabel.AutoSize = true;
            propertyInputLabel.Font = new Font("Segoe UI", 9F);
            propertyInputLabel.Location = new Point(3, 88);
            propertyInputLabel.Name = "propertyInputLabel";
            propertyInputLabel.Size = new Size(66, 15);
            propertyInputLabel.TabIndex = 4;
            propertyInputLabel.Text = "Input Type:";
            // 
            // propertyDescriptionTextBox
            // 
            propertyDescriptionTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            propertyDescriptionTextBox.BackColor = Color.Honeydew;
            propertyDescriptionTextBox.BorderStyle = BorderStyle.None;
            propertyDescriptionTextBox.Font = new Font("Segoe UI", 9F);
            propertyDescriptionTextBox.ForeColor = Color.DimGray;
            propertyDescriptionTextBox.Location = new Point(3, 25);
            propertyDescriptionTextBox.Multiline = true;
            propertyDescriptionTextBox.Name = "propertyDescriptionTextBox";
            propertyDescriptionTextBox.ReadOnly = true;
            propertyDescriptionTextBox.ScrollBars = ScrollBars.Vertical;
            propertyDescriptionTextBox.Size = new Size(331, 60);
            propertyDescriptionTextBox.TabIndex = 1;
            propertyDescriptionTextBox.TabStop = false;
            propertyDescriptionTextBox.Text = "Pipeline item description";
            // 
            // propertyTypeLabel
            // 
            propertyTypeLabel.AutoSize = true;
            propertyTypeLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            propertyTypeLabel.Location = new Point(3, 0);
            propertyTypeLabel.Name = "propertyTypeLabel";
            propertyTypeLabel.Padding = new Padding(0, 0, 0, 4);
            propertyTypeLabel.Size = new Size(119, 24);
            propertyTypeLabel.TabIndex = 0;
            propertyTypeLabel.Text = "<PipelineItem>";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(826, 450);
            Controls.Add(splitContainer);
            Controls.Add(navigationPanel);
            Controls.Add(statusStrip);
            Controls.Add(menuStrip);
            KeyPreview = true;
            MainMenuStrip = menuStrip;
            Name = "MainForm";
            Text = "Log Interpreter";
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            navigationPanel.ResumeLayout(false);
            navigationPanel.PerformLayout();
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            pipelinePanel.ResumeLayout(false);
            pipelineFlowLayoutPanel.ResumeLayout(false);
            propertiesPanel.ResumeLayout(false);
            propertiesContentPanel.ResumeLayout(false);
            propertiesContentPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripMenuItem redoToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem cutToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem selectAllToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem statusBarToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel toolStripStatusLabel;
        private Panel navigationPanel;
        private Label navigationLabel;
        private SplitContainer splitContainer;
        private Panel pipelinePanel;
        private FlowLayoutPanel pipelineFlowLayoutPanel;
        private Panel propertiesPanel;
        private Panel propertiesContentPanel;
        private Label propertyTypeLabel;
        private TextBox propertyDescriptionTextBox;
        private Label propertyInputLabel;
        private Label propertyInputValueLabel;
        private Label propertyOutputLabel;
        private Label propertyOutputValueLabel;
        private Controls.PipelineItemCard samplePipelineCard;
        private Controls.PipelineItemCard sampleItemCard;
    }
}
