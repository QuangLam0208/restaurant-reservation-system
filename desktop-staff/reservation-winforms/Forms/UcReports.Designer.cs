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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.btnFilter = new System.Windows.Forms.Button();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.lblTo = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.lblFrom = new System.Windows.Forms.Label();
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
            this.pnlFilter.BackColor = System.Drawing.Color.White;
            this.pnlFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFilter.Controls.Add(this.btnFilter);
            this.pnlFilter.Controls.Add(this.dtpTo);
            this.pnlFilter.Controls.Add(this.lblTo);
            this.pnlFilter.Controls.Add(this.dtpFrom);
            this.pnlFilter.Controls.Add(this.lblFrom);
            this.pnlFilter.Location = new System.Drawing.Point(30, 120);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(1470, 100);
            this.pnlFilter.TabIndex = 2;
            // 
            // btnFilter
            // 
            this.btnFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnFilter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFilter.FlatAppearance.BorderSize = 0;
            this.btnFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilter.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnFilter.ForeColor = System.Drawing.Color.White;
            this.btnFilter.Location = new System.Drawing.Point(750, 25);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(234, 50);
            this.btnFilter.TabIndex = 0;
            this.btnFilter.Text = "LỌC DỮ LIỆU";
            this.btnFilter.UseVisualStyleBackColor = false;
            // 
            // dtpTo
            // 
            this.dtpTo.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTo.Location = new System.Drawing.Point(510, 30);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(200, 39);
            this.dtpTo.TabIndex = 1;
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTo.Location = new System.Drawing.Point(380, 35);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(130, 32);
            this.lblTo.TabIndex = 2;
            this.lblTo.Text = "Đến ngày:";
            // 
            // dtpFrom
            // 
            this.dtpFrom.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFrom.Location = new System.Drawing.Point(140, 30);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(200, 39);
            this.dtpFrom.TabIndex = 3;
            // 
            // lblFrom
            // 
            this.lblFrom.AutoSize = true;
            this.lblFrom.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblFrom.Location = new System.Drawing.Point(30, 35);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(114, 32);
            this.lblFrom.TabIndex = 4;
            this.lblFrom.Text = "Từ ngày:";
            // 
            // pnlCards
            // 
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
            this.pnlCards.Location = new System.Drawing.Point(30, 250);
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
            chartArea1.Name = "ChartArea1";
            this.chartReservations.ChartAreas.Add(chartArea1);
            this.chartReservations.Location = new System.Drawing.Point(30, 440);
            this.chartReservations.Name = "chartReservations";
            series1.ChartArea = "ChartArea1";
            series1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            series1.IsValueShownAsLabel = true;
            series1.Name = "Đơn đặt bàn";
            this.chartReservations.Series.Add(series1);
            this.chartReservations.Size = new System.Drawing.Size(1470, 550);
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
            this.pnlFilter.PerformLayout();
            this.pnlCards.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartReservations)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.Panel pnlCards;

        // Cập nhật các Label
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