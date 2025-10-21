namespace LogInterpreter.WinForms
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
            components = new System.ComponentModel.Container();
            menuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openLogFileToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exportToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            refreshToolStripMenuItem = new ToolStripMenuItem();
            clearLogToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            toolbarToolStripMenuItem = new ToolStripMenuItem();
            statusBarToolStripMenuItem = new ToolStripMenuItem();
            toolsToolStripMenuItem = new ToolStripMenuItem();
            filtersToolStripMenuItem = new ToolStripMenuItem();
            searchToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            toolStrip = new ToolStrip();
            openToolStripButton = new ToolStripButton();
            editToolStripButton = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            refreshToolStripButton = new ToolStripButton();
            clearToolStripButton = new ToolStripButton();
            toolStripSeparator5 = new ToolStripSeparator();
            searchToolStripButton = new ToolStripButton();
            filtersToolStripButton = new ToolStripButton();
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            progressBar = new ToolStripProgressBar();
            toolStripImageList = new ImageList(components);
            logDataGridView = new DataGridView();
            logBindingSource = new BindingSource(components);
            menuStrip.SuspendLayout();
            toolStrip.SuspendLayout();
            statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)logDataGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)logBindingSource).BeginInit();
            SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, viewToolStripMenuItem, toolsToolStripMenuItem, helpToolStripMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(1286, 24);
            menuStrip.TabIndex = 0;
            menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openLogFileToolStripMenuItem, toolStripSeparator1, exportToolStripMenuItem, toolStripSeparator2, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // openLogFileToolStripMenuItem
            // 
            openLogFileToolStripMenuItem.Image = Properties.Resources.OpenIcon;
            openLogFileToolStripMenuItem.Name = "openLogFileToolStripMenuItem";
            openLogFileToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            openLogFileToolStripMenuItem.Size = new Size(186, 22);
            openLogFileToolStripMenuItem.Text = "&Open Log File...";
            openLogFileToolStripMenuItem.Click += OpenLogFileToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(183, 6);
            // 
            // exportToolStripMenuItem
            // 
            exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            exportToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.E;
            exportToolStripMenuItem.Size = new Size(186, 22);
            exportToolStripMenuItem.Text = "&Export...";
            exportToolStripMenuItem.Click += ExportToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(183, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
            exitToolStripMenuItem.Size = new Size(186, 22);
            exitToolStripMenuItem.Text = "E&xit";
            exitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { refreshToolStripMenuItem, clearLogToolStripMenuItem, toolStripSeparator3, toolbarToolStripMenuItem, statusBarToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(44, 20);
            viewToolStripMenuItem.Text = "&View";
            // 
            // refreshToolStripMenuItem
            // 
            refreshToolStripMenuItem.Image = Properties.Resources.RefreshIcon;
            refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            refreshToolStripMenuItem.ShortcutKeys = Keys.F5;
            refreshToolStripMenuItem.Size = new Size(146, 22);
            refreshToolStripMenuItem.Text = "&Refresh";
            refreshToolStripMenuItem.Click += RefreshToolStripMenuItem_Click;
            // 
            // clearLogToolStripMenuItem
            // 
            clearLogToolStripMenuItem.Image = Properties.Resources.ClearIcon;
            clearLogToolStripMenuItem.Name = "clearLogToolStripMenuItem";
            clearLogToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.L;
            clearLogToolStripMenuItem.Size = new Size(146, 22);
            clearLogToolStripMenuItem.Text = "&Clear Log";
            clearLogToolStripMenuItem.Click += ClearLogToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(143, 6);
            // 
            // toolbarToolStripMenuItem
            // 
            toolbarToolStripMenuItem.Checked = true;
            toolbarToolStripMenuItem.CheckState = CheckState.Checked;
            toolbarToolStripMenuItem.Name = "toolbarToolStripMenuItem";
            toolbarToolStripMenuItem.Size = new Size(146, 22);
            toolbarToolStripMenuItem.Text = "&Toolbar";
            toolbarToolStripMenuItem.Click += ToolbarToolStripMenuItem_Click;
            // 
            // statusBarToolStripMenuItem
            // 
            statusBarToolStripMenuItem.Checked = true;
            statusBarToolStripMenuItem.CheckState = CheckState.Checked;
            statusBarToolStripMenuItem.Name = "statusBarToolStripMenuItem";
            statusBarToolStripMenuItem.Size = new Size(146, 22);
            statusBarToolStripMenuItem.Text = "&Status Bar";
            statusBarToolStripMenuItem.Click += StatusBarToolStripMenuItem_Click;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { filtersToolStripMenuItem, searchToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new Size(46, 20);
            toolsToolStripMenuItem.Text = "&Tools";
            // 
            // filtersToolStripMenuItem
            // 
            filtersToolStripMenuItem.Image = Properties.Resources.FilterIcon;
            filtersToolStripMenuItem.Name = "filtersToolStripMenuItem";
            filtersToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.F;
            filtersToolStripMenuItem.Size = new Size(144, 22);
            filtersToolStripMenuItem.Text = "&Filters...";
            filtersToolStripMenuItem.Click += FiltersToolStripMenuItem_Click;
            // 
            // searchToolStripMenuItem
            // 
            searchToolStripMenuItem.Image = Properties.Resources.SearchIcon;
            searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            searchToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.H;
            searchToolStripMenuItem.Size = new Size(144, 22);
            searchToolStripMenuItem.Text = "&Search...";
            searchToolStripMenuItem.Click += SearchToolStripMenuItem_Click;
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
            aboutToolStripMenuItem.Size = new Size(107, 22);
            aboutToolStripMenuItem.Text = "&About";
            aboutToolStripMenuItem.Click += AboutToolStripMenuItem_Click;
            // 
            // toolStrip
            // 
            toolStrip.ImageList = toolStripImageList;
            toolStrip.Items.AddRange(new ToolStripItem[] { openToolStripButton, editToolStripButton, toolStripSeparator4, refreshToolStripButton, clearToolStripButton, toolStripSeparator5, searchToolStripButton, filtersToolStripButton });
            toolStrip.Location = new Point(0, 24);
            toolStrip.Name = "toolStrip";
            toolStrip.Size = new Size(1286, 25);
            toolStrip.TabIndex = 1;
            toolStrip.Text = "toolStrip";
            // 
            // openToolStripButton
            // 
            openToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            openToolStripButton.ImageTransparentColor = Color.Magenta;
            openToolStripButton.Name = "openToolStripButton";
            openToolStripButton.Size = new Size(56, 22);
            openToolStripButton.Text = "Open";
            openToolStripButton.Click += OpenLogFileToolStripMenuItem_Click;
            // 
            // editToolStripButton
            // 
            editToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            editToolStripButton.ImageTransparentColor = Color.Magenta;
            editToolStripButton.Name = "editToolStripButton";
            editToolStripButton.Size = new Size(47, 22);
            editToolStripButton.Text = "Edit";
            editToolStripButton.Click += EditToolStripButton_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 25);
            // 
            // refreshToolStripButton
            // 
            refreshToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            refreshToolStripButton.ImageTransparentColor = Color.Magenta;
            refreshToolStripButton.Name = "refreshToolStripButton";
            refreshToolStripButton.Size = new Size(66, 22);
            refreshToolStripButton.Text = "Refresh";
            refreshToolStripButton.Click += RefreshToolStripMenuItem_Click;
            // 
            // clearToolStripButton
            // 
            clearToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            clearToolStripButton.ImageTransparentColor = Color.Magenta;
            clearToolStripButton.Name = "clearToolStripButton";
            clearToolStripButton.Size = new Size(54, 22);
            clearToolStripButton.Text = "Clear";
            clearToolStripButton.Click += ClearLogToolStripMenuItem_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(6, 25);
            // 
            // searchToolStripButton
            // 
            searchToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            searchToolStripButton.ImageTransparentColor = Color.Magenta;
            searchToolStripButton.Name = "searchToolStripButton";
            searchToolStripButton.Size = new Size(62, 22);
            searchToolStripButton.Text = "Search";
            searchToolStripButton.Click += SearchToolStripMenuItem_Click;
            // 
            // filtersToolStripButton
            // 
            filtersToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            filtersToolStripButton.ImageTransparentColor = Color.Magenta;
            filtersToolStripButton.Name = "filtersToolStripButton";
            filtersToolStripButton.Size = new Size(61, 22);
            filtersToolStripButton.Text = "Filters";
            filtersToolStripButton.Click += FiltersToolStripMenuItem_Click;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel, progressBar });
            statusStrip.Location = new Point(0, 588);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(1286, 22);
            statusStrip.TabIndex = 2;
            statusStrip.Text = "statusStrip";
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(39, 17);
            statusLabel.Text = "Ready";
            // 
            // progressBar
            // 
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(100, 16);
            progressBar.Visible = false;
            // 
            // toolStripImageList
            // 
            toolStripImageList.ColorDepth = ColorDepth.Depth32Bit;
            toolStripImageList.ImageSize = new Size(16, 16);
            // 
            // logDataGridView
            // 
            logDataGridView.AllowUserToAddRows = false;
            logDataGridView.AllowUserToDeleteRows = false;
            logDataGridView.AutoGenerateColumns = false;
            logDataGridView.BackgroundColor = Color.White;
            logDataGridView.BorderStyle = BorderStyle.None;
            logDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            logDataGridView.DataSource = logBindingSource;
            logDataGridView.Dock = DockStyle.Fill;
            logDataGridView.Location = new Point(0, 49);
            logDataGridView.MultiSelect = false;
            logDataGridView.Name = "logDataGridView";
            logDataGridView.ReadOnly = true;
            logDataGridView.RowHeadersVisible = false;
            logDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            logDataGridView.Size = new Size(1286, 539);
            logDataGridView.TabIndex = 3;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1286, 610);
            Controls.Add(logDataGridView);
            Controls.Add(statusStrip);
            Controls.Add(toolStrip);
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;
            Name = "MainForm";
            Text = "LogInterpreter";
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)logDataGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)logBindingSource).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openLogFileToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exportToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem refreshToolStripMenuItem;
        private ToolStripMenuItem clearLogToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem toolbarToolStripMenuItem;
        private ToolStripMenuItem statusBarToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem filtersToolStripMenuItem;
        private ToolStripMenuItem searchToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStrip toolStrip;
        private ToolStripButton openToolStripButton;
        private ToolStripButton editToolStripButton;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton refreshToolStripButton;
        private ToolStripButton clearToolStripButton;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripButton searchToolStripButton;
        private ToolStripButton filtersToolStripButton;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private ToolStripProgressBar progressBar;
        private ImageList toolStripImageList;
        private DataGridView logDataGridView;
        private BindingSource logBindingSource;
    }
}
