using System.Reflection;
using System.ComponentModel;
using LogInterpreter.Abstractions;
using LogInterpreter.Abstractions.DefaultImplementations;

namespace LogInterpreter.WinForms
{
    public partial class MainForm : Form
    {
        private BindingList<ILogEntry> logEntries;
        private string? currentLogFilePath;
        private Pipeline? currentPipeline;
        private Counter? pipelineCounter;
        private ErrorAggregator? pipelineErrorAggregator;
        private LogEntryCollector? pipelineCollector;

        public MainForm()
        {
            InitializeComponent();
            SetupDataGridView();
            SetupToolbarIcons();
            SetWindowTitle();
            SetupPipeline();
            UpdateStatusBar("Kész");
        }

        private void SetupPipeline()
        {
            // Pipeline elemek létrehozása
            pipelineCounter = new Counter();
            pipelineErrorAggregator = new ErrorAggregator();
            pipelineCollector = new LogEntryCollector();

            // Pipeline összeállítása (LogSource nélkül egyelõre)
            currentPipeline = new Pipeline();
            // Pipeline újraépítése az új log fájllal
            pipelineCounter = new Counter();
            pipelineErrorAggregator = new ErrorAggregator();
            pipelineCollector = new LogEntryCollector();

            currentPipeline = new Pipeline()
                .AddItem(new LogSource(string.Empty))
                .AddItem(new OneLineLogFileReader())
                .AddItem(new CompactJsonLogParser())
                .AddItem(pipelineCounter)
                .AddItem(pipelineErrorAggregator)
                .AddItem(new Filter<LogEntry>(e =>
                {
                    if (e.Message.StartsWith("Updating rental status"))
                        return true;
                    if (e.Message.StartsWith("Rental {RentalId} started for user {UserEmail}"))
                        return true;
                    if (e.Message.StartsWith("Rental {RentalId} initiated for user {UserEmail}"))
                        return true;
                    return false;
                }))
                .AddItem(pipelineCollector);
        }

        private void SetupDataGridView()
        {
            // BindingList létrehozása ILogEntry objektumokhoz
            logEntries = new BindingList<ILogEntry>();
            
            // BindingSource beállítása
            logBindingSource.DataSource = logEntries;

            // Oszlopok manuális konfigurálása
            SetupColumns();

            // Dupla kattintás eseménykezelõ hozzáadása
            logDataGridView.CellDoubleClick += LogDataGridView_CellDoubleClick;

            // Példa adatok betöltése kezdéskor NEM kell
            // LoadSampleData();
        }

        /// <summary>
        /// Pipeline futtatása és eredmények betöltése a DataGridView-ba
        /// </summary>
        private void LoadFromPipeline(string logFilePath)
        {
            try
            {
                ShowProgress(true);
                UpdateStatusBar("Log fájl feldolgozása...");
                
                currentLogFilePath = logFilePath;
//currentPipeline.??? = logFilePath;

                // Pipeline futtatása
                RunPipeline();
            }
            catch (Exception ex)
            {
                ShowProgress(false);
                MessageBox.Show(
                    $"Hiba a log fájl feldolgozása közben:\n{ex.Message}",
                    "Hiba",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                UpdateStatusBar("Hiba történt");
            }
        }

        /// <summary>
        /// A pipeline futtatása és az eredmények betöltése
        /// </summary>
        private void RunPipeline()
        {
            if (currentPipeline == null || pipelineCollector == null || pipelineCounter == null)
            {
                ShowProgress(false);
                return;
            }

            try
            {
                // Collector kiürítése az újrafuttatás elõtt
                pipelineCollector.CollectedEntries.Clear();

                // Pipeline futtatása
                currentPipeline.Run();

                // Eredmények betöltése a DataGridView-ba
                logEntries.Clear();
                foreach (var entry in pipelineCollector.CollectedEntries)
                {
                    logEntries.Add(entry);
                }
                
                // Státusz frissítése a statisztikákkal
                UpdateStatusBar($"Betöltve: {logEntries.Count} bejegyzés, " +
                              $"Összes: {pipelineCounter.Entries}, " +
                              $"Hibák: {pipelineCounter.Errors}, " +
                              $"Figyelmeztetések: {pipelineCounter.Warnings}");
                
                ShowProgress(false);
            }
            catch (Exception ex)
            {
                ShowProgress(false);
                MessageBox.Show(
                    $"Hiba a pipeline futtatása közben:\n{ex.Message}",
                    "Hiba",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                UpdateStatusBar("Hiba történt");
            }
        }

        private void SetupColumns()
        {
            logDataGridView.Columns.Clear();

            // Idõbélyeg oszlop
            var timeColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ILogEntry.Time),
                HeaderText = "Idõpont",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "yyyy-MM-dd HH:mm:ss.fff"
                }
            };
            logDataGridView.Columns.Add(timeColumn);

            // Log szint oszlop
            var levelColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ILogEntry.Level),
                HeaderText = "Szint",
                Width = 80
            };
            logDataGridView.Columns.Add(levelColumn);

            // LineId oszlop
            var lineIdColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ILogEntry.LineId),
                HeaderText = "Line",
                Width = 60
            };
            logDataGridView.Columns.Add(lineIdColumn);

            // Kategória oszlop
            var categoryColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ILogEntry.Category),
                HeaderText = "Kategória",
                Width = 120
            };
            logDataGridView.Columns.Add(categoryColumn);

            // Program Flow ID oszlop
            var programFlowColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ILogEntry.ProgramFlowId),
                HeaderText = "PF",
                Width = 60
            };
            logDataGridView.Columns.Add(programFlowColumn);

            // Operation ID oszlop
            var opIdColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ILogEntry.OpId),
                HeaderText = "Op",
                Width = 50
            };
            logDataGridView.Columns.Add(opIdColumn);

            // Status oszlop
            var statusColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ILogEntry.Status),
                HeaderText = "Státusz",
                Width = 80
            };
            logDataGridView.Columns.Add(statusColumn);

            // Idõtartam oszlop
            var durationColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ILogEntry.Duration),
                HeaderText = "Idõtartam",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = @"hh\:mm\:ss\.fff"
                }
            };
            logDataGridView.Columns.Add(durationColumn);

            // UserId oszlop (unbound - Properties dictionary-ból töltjük)
            var userIdColumn = new DataGridViewTextBoxColumn
            {
                Name = "UserId",
                HeaderText = "UserId",
                Width = 80,
                ReadOnly = true
            };
            logDataGridView.Columns.Add(userIdColumn);

            // UserEmail oszlop (unbound - Properties dictionary-ból töltjük)
            var userEmailColumn = new DataGridViewTextBoxColumn
            {
                Name = "UserEmail",
                HeaderText = "UserEmail",
                Width = 150,
                ReadOnly = true
            };
            logDataGridView.Columns.Add(userEmailColumn);

            // Üzenet oszlop (kitölti a fennmaradó helyet)
            var messageColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ILogEntry.Message),
                HeaderText = "Üzenet",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 200
            };
            logDataGridView.Columns.Add(messageColumn);

            // Sor színezés beállítása a log szint alapján
            logDataGridView.CellFormatting += LogDataGridView_CellFormatting;
        }

        private void LogDataGridView_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= logEntries.Count)
                return;

            var entry = logEntries[e.RowIndex];
            var row = logDataGridView.Rows[e.RowIndex];

            // UserId és UserEmail oszlopok feltöltése Properties-bõl
            if (logDataGridView.Columns[e.ColumnIndex].Name == "UserId")
            {
                if (entry.Properties != null && entry.Properties.TryGetValue("UserId", out var userId))
                {
                    e.Value = userId;
                }
                else
                {
                    e.Value = string.Empty;
                }
                e.FormattingApplied = true;
            }
            else if (logDataGridView.Columns[e.ColumnIndex].Name == "UserEmail")
            {
                if (entry.Properties != null && entry.Properties.TryGetValue("UserEmail", out var userEmail))
                {
                    e.Value = userEmail;
                }
                else
                {
                    e.Value = string.Empty;
                }
                e.FormattingApplied = true;
            }

            // Sor színezése a log szint alapján
            switch (entry.Level)
            {
                case LogLevel.Error:
                case LogLevel.Critical:
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 230, 230); // Világos piros
                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                    break;
                case LogLevel.Warning:
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 250, 220); // Világos sárga
                    row.DefaultCellStyle.ForeColor = Color.DarkOrange;
                    break;
                case LogLevel.Information:
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    break;
                case LogLevel.Debug:
                case LogLevel.Trace:
                    row.DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240); // Világos szürke
                    row.DefaultCellStyle.ForeColor = Color.Gray;
                    break;
            }
        }

        private void LoadSampleData()
        {
            // Példa adatok hozzáadása demonstrációs célból
            var sampleEntries = new List<ILogEntry>
            {
                new LogEntry
                {
                    Time = DateTime.Now.AddMinutes(-10),
                    Level = LogLevel.Information,
                    LineId = 1,
                    Category = "System",
                    ProgramFlowId = 1001,
                    OpId = 1,
                    Status = "Start",
                    Duration = TimeSpan.FromMilliseconds(150),
                    Message = "Alkalmazás elindítva"
                },
                new LogEntry
                {
                    Time = DateTime.Now.AddMinutes(-8),
                    Level = LogLevel.Warning,
                    LineId = 2,
                    Category = "Database",
                    ProgramFlowId = 1002,
                    OpId = 2,
                    Status = "End",
                    Duration = TimeSpan.FromMilliseconds(2500),
                    Message = "Adatbázis kapcsolat lassú"
                },
                new LogEntry
                {
                    Time = DateTime.Now.AddMinutes(-5),
                    Level = LogLevel.Error,
                    LineId = 3,
                    Category = "FileSystem",
                    ProgramFlowId = 1003,
                    OpId = 3,
                    Status = "ERROR",
                    Duration = TimeSpan.FromMilliseconds(50),
                    Message = "Fájl nem található: config.xml"
                },
                new LogEntry
                {
                    Time = DateTime.Now.AddMinutes(-2),
                    Level = LogLevel.Debug,
                    LineId = 4,
                    Category = "UserInterface",
                    ProgramFlowId = 1004,
                    OpId = 4,
                    Status = "End",
                    Duration = TimeSpan.FromMilliseconds(25),
                    Message = "Felhasználói interakció feldolgozva"
                },
                new LogEntry
                {
                    Time = DateTime.Now,
                    Level = LogLevel.Information,
                    LineId = 5,
                    Category = "ContentOperation",
                    ProgramFlowId = 1005,
                    OpId = 5,
                    Status = "End",
                    Duration = TimeSpan.FromMilliseconds(333),
                    Message = "NODE.SAVE Id: 1158, VersionId: 171, Version: V1.0.A"
                }
            };

            foreach (var entry in sampleEntries)
            {
                logEntries.Add(entry);
            }

            UpdateStatusBar($"{logEntries.Count} bejegyzés betöltve");
        }

        private void SetWindowTitle()
        {
            try
            {
                var version = GetApplicationVersion();
                this.Text = $"LogInterpreter v{version}";
            }
            catch (Exception ex)
            {
                // Ha hiba van a verzió kiolvasásában, alapértelmezett cím
                this.Text = "LogInterpreter";
                UpdateStatusBar($"Version error: {ex.Message}");
            }
        }

        private void SetupToolbarIcons()
        {
            try
            {
                // Set icons directly on toolbar buttons
                openToolStripButton.Image = Properties.Resources.OpenIcon;
                refreshToolStripButton.Image = Properties.Resources.RefreshIcon;
                clearToolStripButton.Image = Properties.Resources.ClearIcon;
                searchToolStripButton.Image = Properties.Resources.SearchIcon;
                filtersToolStripButton.Image = Properties.Resources.FilterIcon;
                
                // Also populate the ImageList as backup
                toolStripImageList.Images.Add("open", Properties.Resources.OpenIcon);
                toolStripImageList.Images.Add("refresh", Properties.Resources.RefreshIcon);
                toolStripImageList.Images.Add("clear", Properties.Resources.ClearIcon);
                toolStripImageList.Images.Add("search", Properties.Resources.SearchIcon);
                toolStripImageList.Images.Add("filter", Properties.Resources.FilterIcon);
            }
            catch (Exception ex)
            {
                UpdateStatusBar($"Icon setup error: {ex.Message}");
            }
        }

        #region Menu Event Handlers

        private void OpenLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Title = "Log fájl megnyitása",
                Filter = "Log Files (*.log;*.txt)|*.log;*.txt|All Files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadFromPipeline(openFileDialog.FileName);
            }
        }

        private void EditToolStripButton_Click(object sender, EventArgs e)
        {
            if (currentPipeline == null)
            {
                MessageBox.Show(
                    "Nincs betöltött pipeline. Elõször nyiss meg egy log fájlt.",
                    "Edit Pipeline",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            using var editForm = new EditForm(currentPipeline);
            editForm.ShowDialog(this);

            RunPipeline();
        }

        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (logEntries.Count == 0)
            {
                MessageBox.Show(
                    "Nincs exportálható adat.",
                    "Export",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            using var saveFileDialog = new SaveFileDialog
            {
                Title = "Log adatok exportálása",
                Filter = "CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = $"LogExport_{DateTime.Now:yyyyMMdd_HHmmss}"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ShowProgress(true);
                    UpdateStatusBar("Exportálás folyamatban...");

                    var extension = Path.GetExtension(saveFileDialog.FileName).ToLowerInvariant();
                    
                    if (extension == ".csv")
                    {
                        ExportToCsv(saveFileDialog.FileName);
                    }
                    else
                    {
                        ExportToText(saveFileDialog.FileName);
                    }

                    ShowProgress(false);
                    UpdateStatusBar($"Exportálva: {logEntries.Count} bejegyzés");
                    
                    MessageBox.Show(
                        $"{logEntries.Count} bejegyzés sikeresen exportálva:\n{saveFileDialog.FileName}",
                        "Export sikeres",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    ShowProgress(false);
                    MessageBox.Show(
                        $"Hiba az exportálás közben:\n{ex.Message}",
                        "Export hiba",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    UpdateStatusBar("Export sikertelen");
                }
            }
        }

        private void ExportToCsv(string filePath)
        {
            using var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8);
            
            // Fejléc
            writer.WriteLine("Time,Level,LineId,Category,ProgramFlowId,OpId,Status,Duration,UserId,UserEmail,Message");
            
            // Adatok
            foreach (var entry in logEntries)
            {
                var userId = entry.Properties?.TryGetValue("UserId", out var uid) == true ? uid : string.Empty;
                var userEmail = entry.Properties?.TryGetValue("UserEmail", out var email) == true ? email : string.Empty;

                writer.WriteLine($"\"{entry.Time:yyyy-MM-dd HH:mm:ss.fff}\"," +
                               $"\"{entry.Level}\"," +
                               $"{entry.LineId}," +
                               $"\"{EscapeCsv(entry.Category)}\"," +
                               $"{entry.ProgramFlowId}," +
                               $"{entry.OpId}," +
                               $"\"{EscapeCsv(entry.Status)}\"," +
                               $"\"{entry.Duration:hh\\:mm\\:ss\\.fff}\"," +
                               $"\"{EscapeCsv(userId)}\"," +
                               $"\"{EscapeCsv(userEmail)}\"," +
                               $"\"{EscapeCsv(entry.Message)}\"");
            }
        }

        private void ExportToText(string filePath)
        {
            using var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8);
            
            foreach (var entry in logEntries)
            {
                var userId = entry.Properties?.TryGetValue("UserId", out var uid) == true ? uid : "N/A";
                var userEmail = entry.Properties?.TryGetValue("UserEmail", out var email) == true ? email : string.Empty;

                writer.WriteLine($"{entry.Time:yyyy-MM-dd HH:mm:ss.fff} " +
                               $"[{entry.Level,-11}] " +
                               $"{entry.Category,-15} " +
                               $"PF:{entry.ProgramFlowId,-8} " +
                               $"Op:{entry.OpId,-5} " +
                               $"User:{userId,-8} " +
                               $"{userEmail,-25} " +
                               $"{entry.Message}");
            }
        }

        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
                
            // Idézõjelek megduplázása CSV szabvány szerint
            return value.Replace("\"", "\"\"");
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RefreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentPipeline == null || string.IsNullOrEmpty(currentLogFilePath))
            {
                MessageBox.Show(
                    "Nincs betöltött log fájl, amit frissíteni lehetne.",
                    "Frissítés",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (!File.Exists(currentLogFilePath))
            {
                MessageBox.Show(
                    $"A log fájl már nem létezik:\n{currentLogFilePath}",
                    "Frissítés",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ShowProgress(true);
                UpdateStatusBar("Frissítés...");
                
                // Pipeline újrafuttatása
                RunPipeline();
            }
            catch (Exception ex)
            {
                ShowProgress(false);
                MessageBox.Show(
                    $"Hiba a frissítés közben:\n{ex.Message}",
                    "Hiba",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                UpdateStatusBar("Frissítés sikertelen");
            }
        }

        private void ClearLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // DataGridView törlése
            ClearLogEntries();
            currentLogFilePath = null;
            currentPipeline = null;
        }

        private void ToolbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolbarToolStripMenuItem.Checked = !toolbarToolStripMenuItem.Checked;
            toolStrip.Visible = toolbarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusBarToolStripMenuItem.Checked = !statusBarToolStripMenuItem.Checked;
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void FiltersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Implement filters dialog
            UpdateStatusBar("Filters dialog (not implemented)");
        }

        private void SearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Implement search dialog
            UpdateStatusBar("Search dialog (not implemented)");
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var version = GetApplicationVersion();
            MessageBox.Show(
                $"LogInterpreter v{version}\n\nA tool for analyzing and interpreting log files.",
                "About LogInterpreter",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Új log bejegyzés hozzáadása a DataGridView-hoz
        /// </summary>
        public void AddLogEntry(ILogEntry entry)
        {
            logEntries.Add(entry);
            UpdateStatusBar($"{logEntries.Count} bejegyzés");
        }

        /// <summary>
        /// Több log bejegyzés hozzáadása egyszerre
        /// </summary>
        public void AddLogEntries(IEnumerable<ILogEntry> entries)
        {
            foreach (var entry in entries)
            {
                logEntries.Add(entry);
            }
            UpdateStatusBar($"{logEntries.Count} bejegyzés");
        }

        /// <summary>
        /// Összes log bejegyzés törlése
        /// </summary>
        public void ClearLogEntries()
        {
            logEntries.Clear();
            UpdateStatusBar("Log törlve");
        }

        /// <summary>
        /// Log bejegyzések szûrése szint szerint
        /// </summary>
        public void FilterByLevel(LogLevel level)
        {
            if (level == LogLevel.NotParsed)
            {
                logBindingSource.RemoveFilter();
            }
            else
            {
                logBindingSource.Filter = $"Level = {(int)level}";
            }
        }

        /// <summary>
        /// Szûrés eltávolítása
        /// </summary>
        public void RemoveFilter()
        {
            logBindingSource.RemoveFilter();
            UpdateStatusBar($"{logEntries.Count} bejegyzés (szûrõ eltávolítva)");
        }

        private string GetApplicationVersion()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;
                
                if (version != null)
                {
                    return $"{version.Major}.{version.Minor}.{version.Build}";
                }
            }
            catch
            {
                // Hiba esetén alapértelmezett verzió
            }
            
            return "1.0.0";
        }

        private void UpdateStatusBar(string message)
        {
            statusLabel.Text = message;
        }

        private void ShowProgress(bool show)
        {
            progressBar.Visible = show;
        }

        private void LogDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Fejléc sorát kihagyjuk
            if (e.RowIndex < 0 || e.RowIndex >= logEntries.Count)
                return;

            var entry = logEntries[e.RowIndex];
            ShowRawLogEntryDialog(entry);
        }

        private void ShowRawLogEntryDialog(ILogEntry entry)
        {
            // Modal dialog létrehozása
            using var dialog = new Form
            {
                Text = $"Raw Log Entry - {entry.Time:yyyy-MM-dd HH:mm:ss.fff}",
                Width = 800,
                Height = 600,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.Sizable,
                MinimumSize = new Size(600, 400),
                MaximizeBox = true,
                MinimizeBox = false,
                ShowInTaskbar = false
            };

            // TextBox a Raw adatok megjelenítéséhez
            var textBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                WordWrap = false,
                Font = new Font("Consolas", 10),
                BackColor = Color.White
            };

            // Raw adatok formázása
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== LOG ENTRY DETAILS ===");
            sb.AppendLine();
            sb.AppendLine($"Time:          {entry.Time:yyyy-MM-dd HH:mm:ss.fff}");
            sb.AppendLine($"Level:         {entry.Level}");
            sb.AppendLine($"LineId:        {entry.LineId}");
            sb.AppendLine($"Category:      {entry.Category}");
            sb.AppendLine($"ProgramFlowId: {entry.ProgramFlowId}");
            sb.AppendLine($"OpId:          {entry.OpId}");
            sb.AppendLine($"Status:        {entry.Status}");
            sb.AppendLine($"Duration:      {entry.Duration:hh\\:mm\\:ss\\.fff}");
            sb.AppendLine($"Message:       {entry.Message}");
            sb.AppendLine();
            
            // Properties
            if (entry.Properties != null && entry.Properties.Count > 0)
            {
                sb.AppendLine("=== PROPERTIES ===");
                sb.AppendLine();
                foreach (var prop in entry.Properties.OrderBy(p => p.Key))
                {
                    sb.AppendLine($"{prop.Key,-30} = {prop.Value}");
                }
                sb.AppendLine();
            }

            // Raw sorok
            if (entry.Raw != null && entry.Raw.Length > 0)
            {
                sb.AppendLine("=== RAW LOG DATA ===");
                sb.AppendLine();
                for (int i = 0; i < entry.Raw.Length; i++)
                {
                    sb.AppendLine($"[{i}] {entry.Raw[i]}");
                    if (i < entry.Raw.Length - 1)
                        sb.AppendLine();
                }
            }

            textBox.Text = sb.ToString();
            textBox.SelectionStart = 0;
            textBox.SelectionLength = 0;

            // Panel a gombok számára
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(10)
            };

            // Close gomb
            var closeButton = new Button
            {
                Text = "Bezárás",
                DialogResult = DialogResult.OK,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Width = 100,
                Height = 30
            };
            closeButton.Location = new Point(buttonPanel.Width - closeButton.Width - 10, 10);

            // Copy to Clipboard gomb
            var copyButton = new Button
            {
                Text = "Vágólapra",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Width = 100,
                Height = 30
            };
            copyButton.Location = new Point(closeButton.Left - copyButton.Width - 10, 10);
            copyButton.Click += (s, e) =>
            {
                try
                {
                    Clipboard.SetText(textBox.Text);
                    MessageBox.Show("A Raw log adat a vágólapra másolva.", "Vágólapra másolás", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba a vágólapra másolás során:\n{ex.Message}", "Hiba", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            buttonPanel.Controls.Add(closeButton);
            buttonPanel.Controls.Add(copyButton);

            dialog.Controls.Add(textBox);
            dialog.Controls.Add(buttonPanel);
            dialog.AcceptButton = closeButton;

            // Dialog megjelenítése
            dialog.ShowDialog(this);
        }

        #endregion
    }
}
