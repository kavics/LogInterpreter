namespace LogInterpreter.GUI.Controls
{
    partial class PipelineItemCard
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            itemTypeLabel = new Label();
            descriptionLabel = new Label();
            inputTypeLabel = new Label();
            outputTypeLabel = new Label();
            typePanel = new Panel();
            ioPanel = new Panel();
            typePanel.SuspendLayout();
            ioPanel.SuspendLayout();
            SuspendLayout();
            // 
            // itemTypeLabel
            // 
            itemTypeLabel.AutoSize = true;
            itemTypeLabel.Dock = DockStyle.Top;
            itemTypeLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            itemTypeLabel.Location = new Point(8, 8);
            itemTypeLabel.Name = "itemTypeLabel";
            itemTypeLabel.Padding = new Padding(0, 0, 0, 4);
            itemTypeLabel.Size = new Size(79, 24);
            itemTypeLabel.TabIndex = 0;
            itemTypeLabel.Text = "ItemType";
            // 
            // descriptionLabel
            // 
            descriptionLabel.AutoSize = true;
            descriptionLabel.Dock = DockStyle.Top;
            descriptionLabel.Font = new Font("Segoe UI", 9F);
            descriptionLabel.ForeColor = Color.DimGray;
            descriptionLabel.Location = new Point(8, 32);
            descriptionLabel.Name = "descriptionLabel";
            descriptionLabel.Padding = new Padding(0, 0, 0, 4);
            descriptionLabel.Size = new Size(167, 19);
            descriptionLabel.TabIndex = 1;
            descriptionLabel.Text = "Description of the pipeline item";
            // 
            // inputTypeLabel
            // 
            inputTypeLabel.AutoSize = true;
            inputTypeLabel.Dock = DockStyle.Left;
            inputTypeLabel.Font = new Font("Segoe UI", 8F);
            inputTypeLabel.ForeColor = Color.Gray;
            inputTypeLabel.Location = new Point(8, 2);
            inputTypeLabel.Name = "inputTypeLabel";
            inputTypeLabel.Padding = new Padding(0, 2, 0, 0);
            inputTypeLabel.Size = new Size(64, 15);
            inputTypeLabel.TabIndex = 2;
            inputTypeLabel.Text = "Input: string";
            // 
            // outputTypeLabel
            // 
            outputTypeLabel.AutoSize = true;
            outputTypeLabel.Dock = DockStyle.Right;
            outputTypeLabel.Font = new Font("Segoe UI", 8F);
            outputTypeLabel.ForeColor = Color.Gray;
            outputTypeLabel.Location = new Point(212, 2);
            outputTypeLabel.Name = "outputTypeLabel";
            outputTypeLabel.Padding = new Padding(0, 2, 8, 0);
            outputTypeLabel.Size = new Size(76, 15);
            outputTypeLabel.TabIndex = 3;
            outputTypeLabel.Text = "Output: LogEntry";
            // 
            // typePanel
            // 
            typePanel.Controls.Add(descriptionLabel);
            typePanel.Controls.Add(itemTypeLabel);
            typePanel.Dock = DockStyle.Top;
            typePanel.Location = new Point(0, 0);
            typePanel.Name = "typePanel";
            typePanel.Padding = new Padding(8, 8, 8, 0);
            typePanel.Size = new Size(288, 55);
            typePanel.TabIndex = 4;
            // 
            // ioPanel
            // 
            ioPanel.Controls.Add(outputTypeLabel);
            ioPanel.Controls.Add(inputTypeLabel);
            ioPanel.Dock = DockStyle.Top;
            ioPanel.Location = new Point(0, 55);
            ioPanel.Name = "ioPanel";
            ioPanel.Padding = new Padding(8, 2, 0, 4);
            ioPanel.Size = new Size(288, 20);
            ioPanel.TabIndex = 5;
            // 
            // PipelineItemCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(ioPanel);
            Controls.Add(typePanel);
            Margin = new Padding(0, 0, 0, 4);
            Name = "PipelineItemCard";
            Padding = new Padding(0, 0, 0, 4);
            Size = new Size(288, 80);
            typePanel.ResumeLayout(false);
            typePanel.PerformLayout();
            ioPanel.ResumeLayout(false);
            ioPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label itemTypeLabel;
        private Label descriptionLabel;
        private Label inputTypeLabel;
        private Label outputTypeLabel;
        private Panel typePanel;
        private Panel ioPanel;
    }
}
