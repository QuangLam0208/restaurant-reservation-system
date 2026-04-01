namespace reservation_winforms.Forms
{
    partial class UcReports
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.tabFilter = new System.Windows.Forms.TabControl();
            this.tabPageDate = new System.Windows.Forms.TabPage();
            this.lblToDate = new System.Windows.Forms.Label();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.lblFromDate = new System.Windows.Forms.Label();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.tabPageMonth = new System.Windows.Forms.TabPage();
            this.lblToMonth = new System.Windows.Forms.Label();
            this.dtpToMonth = new System.Windows.Forms.DateTimePicker();
            this.lblFromMonth = new System.Windows.Forms.Label();
            this.dtpFromMonth = new System.Windows.Forms.DateTimePicker();
            this.tabPageYear = new System.Windows.Forms.TabPage();
            this.lblFromYear = new System.Windows.Forms.Label();
            this.numFromYear = new System.Windows.Forms.NumericUpDown();
            this.lblToYear = new System.Windows.Forms.Label();
            this.numToYear = new System.Windows.Forms.NumericUpDown();
            this.btnFilter = new System.Windows.Forms.Button();
            this.pnlCards = new System.Windows.Forms.Panel();
            this.lblTotalAllTitle = new System.Windows.Forms.Label();
            this.lblTotalAll = new System.Windows.Forms.Label();
            this.lblTotalOnlineTitle = new System.Windows.Forms.Label();
            this.lblTotalOnline = new System.Windows.Forms.Label();
            this.lblTotalWalkInTitle = new System.Windows.Forms.Label();
            this.lblTotalWalkIn = new System.Windows.Forms.Label();
            this.lblNoShowTitle = new System.Windows.Forms.Label();
            this.lblNoShow = new System.Windows.Forms.Label();
            this.lblRateTitle = new System.Windows.Forms.Label();
            this.lblRate = new System.Windows.Forms.Label();
            this.chartReservations = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.pnlHeader.SuspendLayout();
            this.pnlFilter.SuspendLayout();
            this.tabFilter.SuspendLayout();
            this.tabPageDate.SuspendLayout();
            this.tabPageMonth.SuspendLayout();
            this.tabPageYear.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFromYear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numToYear)).BeginInit();
            this.pnlCards.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartReservations)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1530, 92);
            this.pnlHeader.TabIndex = 3;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.lblTitle.Location = new System.Drawing.Point(30, 22);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(441, 48);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "📈 BÁO CÁO & THỐNG KÊ";
            // 
            // pnlFilter
            // 
            this.pnlFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFilter.BackColor = System.Drawing.Color.White;
            this.pnlFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFilter.Controls.Add(this.tabFilter);
            this.pnlFilter.Controls.Add(this.btnFilter);
            this.pnlFilter.Location = new System.Drawing.Point(30, 100);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(1470, 160);
            this.pnlFilter.TabIndex = 2;
            // 
            // tabFilter
            // 
            this.tabFilter.Controls.Add(this.tabPageDate);
            this.tabFilter.Controls.Add(this.tabPageMonth);
            this.tabFilter.Controls.Add(this.tabPageYear);
            this.tabFilter.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.tabFilter.ItemSize = new System.Drawing.Size(275, 35);
            this.tabFilter.Location = new System.Drawing.Point(20, 10);
            this.tabFilter.Name = "tabFilter";
            this.tabFilter.SelectedIndex = 0;
            this.tabFilter.Size = new System.Drawing.Size(835, 135);
            this.tabFilter.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabFilter.TabIndex = 0;
            // 
            // tabPageDate
            // 
            this.tabPageDate.Controls.Add(this.lblToDate);
            this.tabPageDate.Controls.Add(this.dtpToDate);
            this.tabPageDate.Controls.Add(this.lblFromDate);
            this.tabPageDate.Controls.Add(this.dtpFromDate);
            this.tabPageDate.Location = new System.Drawing.Point(4, 39);
            this.tabPageDate.Name = "tabPageDate";
            this.tabPageDate.Size = new System.Drawing.Size(827, 92);
            this.tabPageDate.TabIndex = 0;
            this.tabPageDate.Text = "Lọc theo Ngày";
            this.tabPageDate.UseVisualStyleBackColor = true;
            // 
            // lblToDate
            // 
            this.lblToDate.AutoSize = true;
            this.lblToDate.Location = new System.Drawing.Point(338, 13);
            this.lblToDate.Name = "lblToDate";
            this.lblToDate.Size = new System.Drawing.Size(122, 32);
            this.lblToDate.TabIndex = 2;
            this.lblToDate.Text = "Đến ngày:";
            // 
            // dtpToDate
            // 
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpToDate.Location = new System.Drawing.Point(468, 10);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(180, 39);
            this.dtpToDate.TabIndex = 3;
            // 
            // lblFromDate
            // 
            this.lblFromDate.AutoSize = true;
            this.lblFromDate.Location = new System.Drawing.Point(18, 13);
            this.lblFromDate.Name = "lblFromDate";
            this.lblFromDate.Size = new System.Drawing.Size(105, 32);
            this.lblFromDate.TabIndex = 0;
            this.lblFromDate.Text = "Từ ngày:";
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFromDate.Location = new System.Drawing.Point(128, 10);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(180, 39);
            this.dtpFromDate.TabIndex = 1;
            // 
            // tabPageMonth
            // 
            this.tabPageMonth.Controls.Add(this.lblToMonth);
            this.tabPageMonth.Controls.Add(this.dtpToMonth);
            this.tabPageMonth.Controls.Add(this.lblFromMonth);
            this.tabPageMonth.Controls.Add(this.dtpFromMonth);
            this.tabPageMonth.Location = new System.Drawing.Point(4, 39);
            this.tabPageMonth.Name = "tabPageMonth";
            this.tabPageMonth.Size = new System.Drawing.Size(827, 92);
            this.tabPageMonth.TabIndex = 1;
            this.tabPageMonth.Text = "Lọc theo Tháng";
            this.tabPageMonth.UseVisualStyleBackColor = true;
            // 
            // lblToMonth
            // 
            this.lblToMonth.AutoSize = true;
            this.lblToMonth.Location = new System.Drawing.Point(319, 15);
            this.lblToMonth.Name = "lblToMonth";
            this.lblToMonth.Size = new System.Drawing.Size(132, 32);
            this.lblToMonth.TabIndex = 2;
            this.lblToMonth.Text = "Đến tháng:";
            // 
            // dtpToMonth
            // 
            this.dtpToMonth.CustomFormat = "MM/yyyy";
            this.dtpToMonth.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToMonth.Location = new System.Drawing.Point(459, 12);
            this.dtpToMonth.Name = "dtpToMonth";
            this.dtpToMonth.ShowUpDown = true;
            this.dtpToMonth.Size = new System.Drawing.Size(150, 39);
            this.dtpToMonth.TabIndex = 3;
            // 
            // lblFromMonth
            // 
            this.lblFromMonth.AutoSize = true;
            this.lblFromMonth.Location = new System.Drawing.Point(19, 15);
            this.lblFromMonth.Name = "lblFromMonth";
            this.lblFromMonth.Size = new System.Drawing.Size(115, 32);
            this.lblFromMonth.TabIndex = 0;
            this.lblFromMonth.Text = "Từ tháng:";
            // 
            // dtpFromMonth
            // 
            this.dtpFromMonth.CustomFormat = "MM/yyyy";
            this.dtpFromMonth.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromMonth.Location = new System.Drawing.Point(139, 12);
            this.dtpFromMonth.Name = "dtpFromMonth";
            this.dtpFromMonth.ShowUpDown = true;
            this.dtpFromMonth.Size = new System.Drawing.Size(150, 39);
            this.dtpFromMonth.TabIndex = 1;
            // 
            // tabPageYear
            // 
            this.tabPageYear.Controls.Add(this.lblFromYear);
            this.tabPageYear.Controls.Add(this.numFromYear);
            this.tabPageYear.Controls.Add(this.lblToYear);
            this.tabPageYear.Controls.Add(this.numToYear);
            this.tabPageYear.Location = new System.Drawing.Point(4, 39);
            this.tabPageYear.Name = "tabPageYear";
            this.tabPageYear.Size = new System.Drawing.Size(827, 92);
            this.tabPageYear.TabIndex = 2;
            this.tabPageYear.Text = "Lọc theo Năm";
            this.tabPageYear.UseVisualStyleBackColor = true;
            // 
            // lblFromYear
            // 
            this.lblFromYear.AutoSize = true;
            this.lblFromYear.Location = new System.Drawing.Point(16, 16);
            this.lblFromYear.Name = "lblFromYear";
            this.lblFromYear.Size = new System.Drawing.Size(100, 32);
            this.lblFromYear.TabIndex = 0;
            this.lblFromYear.Text = "Từ năm:";
            // 
            // numFromYear
            // 
            this.numFromYear.Location = new System.Drawing.Point(122, 13);
            this.numFromYear.Maximum = new decimal(new int[] {
            2100,
            0,
            0,
            0});
            this.numFromYear.Minimum = new decimal(new int[] {
            2020,
            0,
            0,
            0});
            this.numFromYear.Name = "numFromYear";
            this.numFromYear.Size = new System.Drawing.Size(100, 39);
            this.numFromYear.TabIndex = 1;
            this.numFromYear.Value = new decimal(new int[] {
            2024,
            0,
            0,
            0});
            // 
            // lblToYear
            // 
            this.lblToYear.AutoSize = true;
            this.lblToYear.Location = new System.Drawing.Point(249, 16);
            this.lblToYear.Name = "lblToYear";
            this.lblToYear.Size = new System.Drawing.Size(117, 32);
            this.lblToYear.TabIndex = 2;
            this.lblToYear.Text = "Đến năm:";
            // 
            // numToYear
            // 
            this.numToYear.Location = new System.Drawing.Point(376, 13);
            this.numToYear.Maximum = new decimal(new int[] {
            2100,
            0,
            0,
            0});
            this.numToYear.Minimum = new decimal(new int[] {
            2020,
            0,
            0,
            0});
            this.numToYear.Name = "numToYear";
            this.numToYear.Size = new System.Drawing.Size(100, 39);
            this.numToYear.TabIndex = 3;
            this.numToYear.Value = new decimal(new int[] {
            2026,
            0,
            0,
            0});
            // 
            // btnFilter
            // 
            this.btnFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnFilter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFilter.FlatAppearance.BorderSize = 0;
            this.btnFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilter.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnFilter.ForeColor = System.Drawing.Color.White;
            this.btnFilter.Location = new System.Drawing.Point(890, 55);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(274, 50);
            this.btnFilter.TabIndex = 1;
            this.btnFilter.Text = "LỌC DỮ LIỆU";
            this.btnFilter.UseVisualStyleBackColor = false;
            // 
            // pnlCards
            // 
            this.pnlCards.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlCards.BackColor = System.Drawing.Color.Transparent;
            this.pnlCards.Controls.Add(this.lblTotalAllTitle);
            this.pnlCards.Controls.Add(this.lblTotalAll);
            this.pnlCards.Controls.Add(this.lblTotalOnlineTitle);
            this.pnlCards.Controls.Add(this.lblTotalOnline);
            this.pnlCards.Controls.Add(this.lblTotalWalkInTitle);
            this.pnlCards.Controls.Add(this.lblTotalWalkIn);
            this.pnlCards.Controls.Add(this.lblNoShowTitle);
            this.pnlCards.Controls.Add(this.lblNoShow);
            this.pnlCards.Controls.Add(this.lblRateTitle);
            this.pnlCards.Controls.Add(this.lblRate);
            this.pnlCards.Location = new System.Drawing.Point(30, 280);
            this.pnlCards.Name = "pnlCards";
            this.pnlCards.Size = new System.Drawing.Size(1470, 160);
            this.pnlCards.TabIndex = 1;
            // 
            // lblTotalAllTitle
            // 
            this.lblTotalAllTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.lblTotalAllTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotalAllTitle.ForeColor = System.Drawing.Color.White;
            this.lblTotalAllTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTotalAllTitle.Name = "lblTotalAllTitle";
            this.lblTotalAllTitle.Size = new System.Drawing.Size(280, 50);
            this.lblTotalAllTitle.TabIndex = 0;
            this.lblTotalAllTitle.Text = "TỔNG ĐƠN ĐÃ ĐẶT";
            this.lblTotalAllTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotalAll
            // 
            this.lblTotalAll.BackColor = System.Drawing.Color.White;
            this.lblTotalAll.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTotalAll.Font = new System.Drawing.Font("Segoe UI", 26F, System.Drawing.FontStyle.Bold);
            this.lblTotalAll.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.lblTotalAll.Location = new System.Drawing.Point(0, 50);
            this.lblTotalAll.Name = "lblTotalAll";
            this.lblTotalAll.Size = new System.Drawing.Size(280, 110);
            this.lblTotalAll.TabIndex = 1;
            this.lblTotalAll.Text = "0";
            this.lblTotalAll.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotalOnlineTitle
            // 
            this.lblTotalOnlineTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.lblTotalOnlineTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotalOnlineTitle.ForeColor = System.Drawing.Color.White;
            this.lblTotalOnlineTitle.Location = new System.Drawing.Point(295, 0);
            this.lblTotalOnlineTitle.Name = "lblTotalOnlineTitle";
            this.lblTotalOnlineTitle.Size = new System.Drawing.Size(280, 50);
            this.lblTotalOnlineTitle.TabIndex = 2;
            this.lblTotalOnlineTitle.Text = "ĐƠN ONLINE";
            this.lblTotalOnlineTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotalOnline
            // 
            this.lblTotalOnline.BackColor = System.Drawing.Color.White;
            this.lblTotalOnline.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTotalOnline.Font = new System.Drawing.Font("Segoe UI", 26F, System.Drawing.FontStyle.Bold);
            this.lblTotalOnline.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.lblTotalOnline.Location = new System.Drawing.Point(295, 50);
            this.lblTotalOnline.Name = "lblTotalOnline";
            this.lblTotalOnline.Size = new System.Drawing.Size(280, 110);
            this.lblTotalOnline.TabIndex = 3;
            this.lblTotalOnline.Text = "0";
            this.lblTotalOnline.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotalWalkInTitle
            // 
            this.lblTotalWalkInTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.lblTotalWalkInTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotalWalkInTitle.ForeColor = System.Drawing.Color.White;
            this.lblTotalWalkInTitle.Location = new System.Drawing.Point(590, 0);
            this.lblTotalWalkInTitle.Name = "lblTotalWalkInTitle";
            this.lblTotalWalkInTitle.Size = new System.Drawing.Size(280, 50);
            this.lblTotalWalkInTitle.TabIndex = 4;
            this.lblTotalWalkInTitle.Text = "ĐƠN WALK-IN";
            this.lblTotalWalkInTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotalWalkIn
            // 
            this.lblTotalWalkIn.BackColor = System.Drawing.Color.White;
            this.lblTotalWalkIn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTotalWalkIn.Font = new System.Drawing.Font("Segoe UI", 26F, System.Drawing.FontStyle.Bold);
            this.lblTotalWalkIn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.lblTotalWalkIn.Location = new System.Drawing.Point(590, 50);
            this.lblTotalWalkIn.Name = "lblTotalWalkIn";
            this.lblTotalWalkIn.Size = new System.Drawing.Size(280, 110);
            this.lblTotalWalkIn.TabIndex = 5;
            this.lblTotalWalkIn.Text = "0";
            this.lblTotalWalkIn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNoShowTitle
            // 
            this.lblNoShowTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(156)))), ((int)(((byte)(18)))));
            this.lblNoShowTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblNoShowTitle.ForeColor = System.Drawing.Color.White;
            this.lblNoShowTitle.Location = new System.Drawing.Point(885, 0);
            this.lblNoShowTitle.Name = "lblNoShowTitle";
            this.lblNoShowTitle.Size = new System.Drawing.Size(280, 50);
            this.lblNoShowTitle.TabIndex = 6;
            this.lblNoShowTitle.Text = "SỐ KHÁCH BỎ CHỖ";
            this.lblNoShowTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNoShow
            // 
            this.lblNoShow.BackColor = System.Drawing.Color.White;
            this.lblNoShow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNoShow.Font = new System.Drawing.Font("Segoe UI", 26F, System.Drawing.FontStyle.Bold);
            this.lblNoShow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(156)))), ((int)(((byte)(18)))));
            this.lblNoShow.Location = new System.Drawing.Point(885, 50);
            this.lblNoShow.Name = "lblNoShow";
            this.lblNoShow.Size = new System.Drawing.Size(280, 110);
            this.lblNoShow.TabIndex = 7;
            this.lblNoShow.Text = "0";
            this.lblNoShow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblRateTitle
            // 
            this.lblRateTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.lblRateTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblRateTitle.ForeColor = System.Drawing.Color.White;
            this.lblRateTitle.Location = new System.Drawing.Point(1180, 0);
            this.lblRateTitle.Name = "lblRateTitle";
            this.lblRateTitle.Size = new System.Drawing.Size(290, 50);
            this.lblRateTitle.TabIndex = 8;
            this.lblRateTitle.Text = "TỶ LỆ NO-SHOW";
            this.lblRateTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblRate
            // 
            this.lblRate.BackColor = System.Drawing.Color.White;
            this.lblRate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblRate.Font = new System.Drawing.Font("Segoe UI", 26F, System.Drawing.FontStyle.Bold);
            this.lblRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.lblRate.Location = new System.Drawing.Point(1180, 50);
            this.lblRate.Name = "lblRate";
            this.lblRate.Size = new System.Drawing.Size(290, 110);
            this.lblRate.TabIndex = 9;
            this.lblRate.Text = "0%";
            this.lblRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chartReservations
            // 
            this.chartReservations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            chartArea2.Name = "ChartArea1";
            this.chartReservations.ChartAreas.Add(chartArea2);
            this.chartReservations.Location = new System.Drawing.Point(30, 460);
            this.chartReservations.Name = "chartReservations";
            series2.ChartArea = "ChartArea1";
            series2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            series2.IsValueShownAsLabel = true;
            series2.Name = "Đơn đặt bàn";
            this.chartReservations.Series.Add(series2);
            this.chartReservations.Size = new System.Drawing.Size(1470, 520);
            this.chartReservations.TabIndex = 0;
            // 
            // UcReports
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.chartReservations);
            this.Controls.Add(this.pnlCards);
            this.Controls.Add(this.pnlFilter);
            this.Controls.Add(this.pnlHeader);
            this.Name = "UcReports";
            this.Size = new System.Drawing.Size(1530, 1122);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlFilter.ResumeLayout(false);
            this.tabFilter.ResumeLayout(false);
            this.tabPageDate.ResumeLayout(false);
            this.tabPageDate.PerformLayout();
            this.tabPageMonth.ResumeLayout(false);
            this.tabPageMonth.PerformLayout();
            this.tabPageYear.ResumeLayout(false);
            this.tabPageYear.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFromYear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numToYear)).EndInit();
            this.pnlCards.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartReservations)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.TabControl tabFilter;
        private System.Windows.Forms.TabPage tabPageDate;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.Label lblToDate;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.Label lblFromDate;
        private System.Windows.Forms.TabPage tabPageMonth;
        private System.Windows.Forms.DateTimePicker dtpToMonth;
        private System.Windows.Forms.Label lblToMonth;
        private System.Windows.Forms.DateTimePicker dtpFromMonth;
        private System.Windows.Forms.Label lblFromMonth;
        private System.Windows.Forms.TabPage tabPageYear;
        private System.Windows.Forms.NumericUpDown numToYear;
        private System.Windows.Forms.Label lblToYear;
        private System.Windows.Forms.NumericUpDown numFromYear;
        private System.Windows.Forms.Label lblFromYear;

        private System.Windows.Forms.Panel pnlCards;
        private System.Windows.Forms.Label lblTotalAllTitle;
        private System.Windows.Forms.Label lblTotalAll;
        private System.Windows.Forms.Label lblTotalOnlineTitle;
        private System.Windows.Forms.Label lblTotalOnline;
        private System.Windows.Forms.Label lblTotalWalkInTitle;
        private System.Windows.Forms.Label lblTotalWalkIn;
        private System.Windows.Forms.Label lblNoShowTitle;
        private System.Windows.Forms.Label lblNoShow;
        private System.Windows.Forms.Label lblRateTitle;
        private System.Windows.Forms.Label lblRate;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartReservations;
    }
}