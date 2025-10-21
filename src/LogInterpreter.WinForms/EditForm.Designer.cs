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
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            leftPanel.SuspendLayout();
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
            splitContainer.SplitterDistance = 343;
            splitContainer.TabIndex = 0;
            // 
            // leftPanel
            // 
            leftPanel.Controls.Add(pipelineListBox);
            leftPanel.Controls.Add(pipelineLabel);
            leftPanel.Dock = DockStyle.Fill;
            leftPanel.Location = new Point(0, 0);
            leftPanel.Name = "leftPanel";
            leftPanel.Padding = new Padding(10);
            leftPanel.Size = new Size(343, 600);
            leftPanel.TabIndex = 0;
            // 
            // pipelineListBox
            // 
            pipelineListBox.Dock = DockStyle.Fill;
            pipelineListBox.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 238);
            pipelineListBox.FormattingEnabled = true;
            pipelineListBox.Items.AddRange(new object[] { "Item 1", "Item 2" });
            pipelineListBox.Location = new Point(10, 30);
            pipelineListBox.Name = "pipelineListBox";
            pipelineListBox.Size = new Size(323, 560);
            pipelineListBox.TabIndex = 1;
            // 
            // pipelineLabel
            // 
            pipelineLabel.AutoSize = true;
            pipelineLabel.Dock = DockStyle.Top;
            pipelineLabel.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 238);
            pipelineLabel.Location = new Point(10, 10);
            pipelineLabel.Name = "pipelineLabel";
            pipelineLabel.Size = new Size(64, 20);
            pipelineLabel.TabIndex = 0;
            pipelineLabel.Text = "Pipeline";
            // 
            // rightPanel
            // 
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.Location = new Point(0, 0);
            rightPanel.Name = "rightPanel";
            rightPanel.Padding = new Padding(10);
            rightPanel.Size = new Size(643, 600);
            rightPanel.TabIndex = 0;
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
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer;
        private Panel leftPanel;
        private ListBox pipelineListBox;
        private Label pipelineLabel;
        private Panel rightPanel;
    }
}
