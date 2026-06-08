namespace ArchitectureChecker
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.TableLayoutPanel pathPanel;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.TextBox txtRootPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Panel summaryPanel;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.FlowLayoutPanel metricsPanel;
        private System.Windows.Forms.PictureBox picErrors;
        private System.Windows.Forms.Label lblErrorsMetric;
        private System.Windows.Forms.PictureBox picWarnings;
        private System.Windows.Forms.Label lblWarningsMetric;
        private System.Windows.Forms.PictureBox picChecked;
        private System.Windows.Forms.Label lblCheckedMetric;
        private System.Windows.Forms.PictureBox picFindings;
        private System.Windows.Forms.Label lblFindingsMetric;
        private System.Windows.Forms.DataGridView gridViolations;
        private System.Windows.Forms.TableLayoutPanel detailPanel;
        private System.Windows.Forms.Panel detailHeader;
        private System.Windows.Forms.Label lblDetails;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.TextBox txtDetails;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.pathPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lblPath = new System.Windows.Forms.Label();
            this.txtRootPath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.summaryPanel = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.metricsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.picErrors = new System.Windows.Forms.PictureBox();
            this.lblErrorsMetric = new System.Windows.Forms.Label();
            this.picWarnings = new System.Windows.Forms.PictureBox();
            this.lblWarningsMetric = new System.Windows.Forms.Label();
            this.picChecked = new System.Windows.Forms.PictureBox();
            this.lblCheckedMetric = new System.Windows.Forms.Label();
            this.picFindings = new System.Windows.Forms.PictureBox();
            this.lblFindingsMetric = new System.Windows.Forms.Label();
            this.gridViolations = new System.Windows.Forms.DataGridView();
            this.detailPanel = new System.Windows.Forms.TableLayoutPanel();
            this.detailHeader = new System.Windows.Forms.Panel();
            this.lblDetails = new System.Windows.Forms.Label();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.txtDetails = new System.Windows.Forms.TextBox();
            this.rootLayout.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.pathPanel.SuspendLayout();
            this.summaryPanel.SuspendLayout();
            this.metricsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picErrors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picWarnings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picChecked)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFindings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViolations)).BeginInit();
            this.detailPanel.SuspendLayout();
            this.detailHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // rootLayout
            // 
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.headerPanel, 0, 0);
            this.rootLayout.Controls.Add(this.pathPanel, 0, 1);
            this.rootLayout.Controls.Add(this.summaryPanel, 0, 2);
            this.rootLayout.Controls.Add(this.gridViolations, 0, 3);
            this.rootLayout.Controls.Add(this.detailPanel, 0, 4);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(14, 14);
            this.rootLayout.Margin = new System.Windows.Forms.Padding(0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.RowCount = 5;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 88F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 58F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 42F));
            this.rootLayout.Size = new System.Drawing.Size(1236, 713);
            this.rootLayout.TabIndex = 0;
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(29)))), ((int)(((byte)(34)))));
            this.headerPanel.Controls.Add(this.lblTitle);
            this.headerPanel.Controls.Add(this.lblSubtitle);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Padding = new System.Windows.Forms.Padding(18, 10, 18, 8);
            this.headerPanel.Size = new System.Drawing.Size(1236, 78);
            this.headerPanel.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(18, 8);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(287, 35);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Architecture Checker";
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSubtitle.Font = new System.Drawing.Font("Microsoft JhengHei UI", 11F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(194)))), ((int)(((byte)(204)))));
            this.lblSubtitle.Location = new System.Drawing.Point(23, 48);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(1186, 24);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Check Scope boundaries, module dependencies, and architecture rule violations.";
            // 
            // pathPanel
            // 
            this.pathPanel.ColumnCount = 4;
            this.pathPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.pathPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pathPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 144F));
            this.pathPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 148F));
            this.pathPanel.Controls.Add(this.lblPath, 0, 0);
            this.pathPanel.Controls.Add(this.txtRootPath, 1, 0);
            this.pathPanel.Controls.Add(this.btnBrowse, 2, 0);
            this.pathPanel.Controls.Add(this.btnRun, 3, 0);
            this.pathPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pathPanel.Location = new System.Drawing.Point(0, 88);
            this.pathPanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.pathPanel.Name = "pathPanel";
            this.pathPanel.Padding = new System.Windows.Forms.Padding(12, 0, 12, 0);
            this.pathPanel.RowCount = 1;
            this.pathPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pathPanel.Size = new System.Drawing.Size(1236, 54);
            this.pathPanel.TabIndex = 1;
            // 
            // lblPath
            // 
            this.lblPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPath.Font = new System.Drawing.Font("Microsoft JhengHei UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(226)))), ((int)(((byte)(234)))));
            this.lblPath.Location = new System.Drawing.Point(15, 0);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(104, 54);
            this.lblPath.TabIndex = 0;
            this.lblPath.Text = "Project Root";
            this.lblPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtRootPath
            // 
            this.txtRootPath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(209)))), ((int)(((byte)(218)))));
            this.txtRootPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRootPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRootPath.Font = new System.Drawing.Font("Microsoft JhengHei UI", 11F);
            this.txtRootPath.Location = new System.Drawing.Point(122, 9);
            this.txtRootPath.Margin = new System.Windows.Forms.Padding(0, 9, 10, 9);
            this.txtRootPath.Name = "txtRootPath";
            this.txtRootPath.Size = new System.Drawing.Size(800, 26);
            this.txtRootPath.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(40)))), ((int)(((byte)(48)))));
            this.btnBrowse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBrowse.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(125)))), ((int)(((byte)(140)))));
            this.btnBrowse.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(56)))), ((int)(((byte)(66)))));
            this.btnBrowse.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(50)))), ((int)(((byte)(60)))));
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowse.Font = new System.Drawing.Font("Microsoft JhengHei UI", 11F);
            this.btnBrowse.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(238)))), ((int)(((byte)(244)))));
            this.btnBrowse.Location = new System.Drawing.Point(932, 7);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(0, 7, 10, 7);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(134, 40);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = false;
            // 
            // btnRun
            // 
            this.btnRun.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(111)))), ((int)(((byte)(204)))));
            this.btnRun.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRun.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(196)))), ((int)(((byte)(255)))));
            this.btnRun.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(86)))), ((int)(((byte)(166)))));
            this.btnRun.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(132)))), ((int)(((byte)(232)))));
            this.btnRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRun.Font = new System.Drawing.Font("Microsoft JhengHei UI", 11F);
            this.btnRun.ForeColor = System.Drawing.Color.White;
            this.btnRun.Location = new System.Drawing.Point(1076, 7);
            this.btnRun.Margin = new System.Windows.Forms.Padding(0, 7, 0, 7);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(148, 40);
            this.btnRun.TabIndex = 3;
            this.btnRun.Text = "Run Check";
            this.btnRun.UseVisualStyleBackColor = false;
            // 
            // summaryPanel
            // 
            this.summaryPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(40)))), ((int)(((byte)(46)))));
            this.summaryPanel.Controls.Add(this.lblStatus);
            this.summaryPanel.Controls.Add(this.metricsPanel);
            this.summaryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.summaryPanel.Location = new System.Drawing.Point(0, 152);
            this.summaryPanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.summaryPanel.Name = "summaryPanel";
            this.summaryPanel.Padding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.summaryPanel.Size = new System.Drawing.Size(1236, 102);
            this.summaryPanel.TabIndex = 2;
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(169)))), ((int)(((byte)(77)))));
            this.lblStatus.Font = new System.Drawing.Font("Microsoft JhengHei UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Location = new System.Drawing.Point(18, 18);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(190, 48);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // metricsPanel
            // 
            this.metricsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.metricsPanel.Controls.Add(this.picErrors);
            this.metricsPanel.Controls.Add(this.lblErrorsMetric);
            this.metricsPanel.Controls.Add(this.picWarnings);
            this.metricsPanel.Controls.Add(this.lblWarningsMetric);
            this.metricsPanel.Controls.Add(this.picChecked);
            this.metricsPanel.Controls.Add(this.lblCheckedMetric);
            this.metricsPanel.Controls.Add(this.picFindings);
            this.metricsPanel.Controls.Add(this.lblFindingsMetric);
            this.metricsPanel.Location = new System.Drawing.Point(230, 18);
            this.metricsPanel.Name = "metricsPanel";
            this.metricsPanel.Size = new System.Drawing.Size(968, 52);
            this.metricsPanel.TabIndex = 2;
            this.metricsPanel.WrapContents = false;
            // 
            // picErrors
            // 
            this.picErrors.Location = new System.Drawing.Point(3, 12);
            this.picErrors.Margin = new System.Windows.Forms.Padding(3, 12, 7, 0);
            this.picErrors.Name = "picErrors";
            this.picErrors.Size = new System.Drawing.Size(22, 22);
            this.picErrors.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picErrors.TabIndex = 0;
            this.picErrors.TabStop = false;
            // 
            // lblErrorsMetric
            // 
            this.lblErrorsMetric.AutoSize = true;
            this.lblErrorsMetric.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.lblErrorsMetric.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(229)))), ((int)(((byte)(236)))));
            this.lblErrorsMetric.Location = new System.Drawing.Point(32, 9);
            this.lblErrorsMetric.Margin = new System.Windows.Forms.Padding(0, 9, 30, 0);
            this.lblErrorsMetric.Name = "lblErrorsMetric";
            this.lblErrorsMetric.Size = new System.Drawing.Size(70, 20);
            this.lblErrorsMetric.TabIndex = 1;
            this.lblErrorsMetric.Text = "Errors: 0";
            // 
            // picWarnings
            // 
            this.picWarnings.Location = new System.Drawing.Point(135, 12);
            this.picWarnings.Margin = new System.Windows.Forms.Padding(3, 12, 7, 0);
            this.picWarnings.Name = "picWarnings";
            this.picWarnings.Size = new System.Drawing.Size(22, 22);
            this.picWarnings.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picWarnings.TabIndex = 2;
            this.picWarnings.TabStop = false;
            // 
            // lblWarningsMetric
            // 
            this.lblWarningsMetric.AutoSize = true;
            this.lblWarningsMetric.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.lblWarningsMetric.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(229)))), ((int)(((byte)(236)))));
            this.lblWarningsMetric.Location = new System.Drawing.Point(164, 9);
            this.lblWarningsMetric.Margin = new System.Windows.Forms.Padding(0, 9, 30, 0);
            this.lblWarningsMetric.Name = "lblWarningsMetric";
            this.lblWarningsMetric.Size = new System.Drawing.Size(98, 20);
            this.lblWarningsMetric.TabIndex = 3;
            this.lblWarningsMetric.Text = "Warnings: 0";
            // 
            // picChecked
            // 
            this.picChecked.Location = new System.Drawing.Point(295, 12);
            this.picChecked.Margin = new System.Windows.Forms.Padding(3, 12, 7, 0);
            this.picChecked.Name = "picChecked";
            this.picChecked.Size = new System.Drawing.Size(22, 22);
            this.picChecked.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picChecked.TabIndex = 4;
            this.picChecked.TabStop = false;
            // 
            // lblCheckedMetric
            // 
            this.lblCheckedMetric.AutoSize = true;
            this.lblCheckedMetric.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.lblCheckedMetric.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(229)))), ((int)(((byte)(236)))));
            this.lblCheckedMetric.Location = new System.Drawing.Point(324, 9);
            this.lblCheckedMetric.Margin = new System.Windows.Forms.Padding(0, 9, 30, 0);
            this.lblCheckedMetric.Name = "lblCheckedMetric";
            this.lblCheckedMetric.Size = new System.Drawing.Size(125, 20);
            this.lblCheckedMetric.TabIndex = 5;
            this.lblCheckedMetric.Text = "Checked files: 0";
            // 
            // picFindings
            // 
            this.picFindings.Location = new System.Drawing.Point(482, 12);
            this.picFindings.Margin = new System.Windows.Forms.Padding(3, 12, 7, 0);
            this.picFindings.Name = "picFindings";
            this.picFindings.Size = new System.Drawing.Size(22, 22);
            this.picFindings.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picFindings.TabIndex = 6;
            this.picFindings.TabStop = false;
            // 
            // lblFindingsMetric
            // 
            this.lblFindingsMetric.AutoSize = true;
            this.lblFindingsMetric.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.lblFindingsMetric.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(229)))), ((int)(((byte)(236)))));
            this.lblFindingsMetric.Location = new System.Drawing.Point(511, 9);
            this.lblFindingsMetric.Margin = new System.Windows.Forms.Padding(0, 9, 30, 0);
            this.lblFindingsMetric.Name = "lblFindingsMetric";
            this.lblFindingsMetric.Size = new System.Drawing.Size(89, 20);
            this.lblFindingsMetric.TabIndex = 7;
            this.lblFindingsMetric.Text = "Findings: 0";
            // 
            // gridViolations
            // 
            this.gridViolations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridViolations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridViolations.Location = new System.Drawing.Point(0, 272);
            this.gridViolations.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.gridViolations.Name = "gridViolations";
            this.gridViolations.RowHeadersWidth = 51;
            this.gridViolations.RowTemplate.Height = 27;
            this.gridViolations.Size = new System.Drawing.Size(1236, 244);
            this.gridViolations.TabIndex = 3;
            // 
            // detailPanel
            // 
            this.detailPanel.ColumnCount = 1;
            this.detailPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.detailPanel.Controls.Add(this.detailHeader, 0, 0);
            this.detailPanel.Controls.Add(this.txtDetails, 0, 1);
            this.detailPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailPanel.Location = new System.Drawing.Point(0, 524);
            this.detailPanel.Margin = new System.Windows.Forms.Padding(0);
            this.detailPanel.Name = "detailPanel";
            this.detailPanel.RowCount = 2;
            this.detailPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.detailPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.detailPanel.Size = new System.Drawing.Size(1236, 189);
            this.detailPanel.TabIndex = 4;
            // 
            // detailHeader
            // 
            this.detailHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(40)))), ((int)(((byte)(46)))));
            this.detailHeader.Controls.Add(this.lblDetails);
            this.detailHeader.Controls.Add(this.btnOpenFile);
            this.detailHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailHeader.Location = new System.Drawing.Point(0, 0);
            this.detailHeader.Margin = new System.Windows.Forms.Padding(0);
            this.detailHeader.Name = "detailHeader";
            this.detailHeader.Size = new System.Drawing.Size(1236, 38);
            this.detailHeader.TabIndex = 0;
            // 
            // lblDetails
            // 
            this.lblDetails.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblDetails.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblDetails.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(238)))), ((int)(((byte)(244)))));
            this.lblDetails.Location = new System.Drawing.Point(0, 0);
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new System.Drawing.Size(100, 38);
            this.lblDetails.TabIndex = 0;
            this.lblDetails.Text = "Details";
            this.lblDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(48)))), ((int)(((byte)(56)))));
            this.btnOpenFile.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnOpenFile.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(125)))), ((int)(((byte)(140)))));
            this.btnOpenFile.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(56)))), ((int)(((byte)(66)))));
            this.btnOpenFile.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(50)))), ((int)(((byte)(60)))));
            this.btnOpenFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenFile.Font = new System.Drawing.Font("Microsoft JhengHei UI", 11F);
            this.btnOpenFile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(238)))), ((int)(((byte)(244)))));
            this.btnOpenFile.Location = new System.Drawing.Point(1117, 0);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(119, 38);
            this.btnOpenFile.TabIndex = 1;
            this.btnOpenFile.Text = "Open File";
            this.btnOpenFile.UseVisualStyleBackColor = false;
            // 
            // txtDetails
            // 
            this.txtDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(15)))), ((int)(((byte)(18)))));
            this.txtDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDetails.Font = new System.Drawing.Font("Consolas", 12F);
            this.txtDetails.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(238)))));
            this.txtDetails.Location = new System.Drawing.Point(0, 38);
            this.txtDetails.Margin = new System.Windows.Forms.Padding(0);
            this.txtDetails.Multiline = true;
            this.txtDetails.Name = "txtDetails";
            this.txtDetails.ReadOnly = true;
            this.txtDetails.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtDetails.Size = new System.Drawing.Size(1236, 151);
            this.txtDetails.TabIndex = 1;
            this.txtDetails.WordWrap = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(35)))), ((int)(((byte)(41)))));
            this.ClientSize = new System.Drawing.Size(1264, 741);
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("Microsoft JhengHei UI", 11F);
            this.MinimumSize = new System.Drawing.Size(1100, 720);
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(14);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Architecture Checker";
            this.rootLayout.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.pathPanel.ResumeLayout(false);
            this.pathPanel.PerformLayout();
            this.summaryPanel.ResumeLayout(false);
            this.metricsPanel.ResumeLayout(false);
            this.metricsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picErrors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picWarnings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picChecked)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFindings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViolations)).EndInit();
            this.detailPanel.ResumeLayout(false);
            this.detailPanel.PerformLayout();
            this.detailHeader.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
