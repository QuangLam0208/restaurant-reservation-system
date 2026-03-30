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
            this.lblRate = new System.Windows.Forms.Label();
            this.lblRateTitle = new System.Windows.Forms.Label();
            this.lblNoShow = new System.Windows.Forms.Label();
            this.lblNoShowTitle = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblTotalTitle = new System.Windows.Forms.Label();
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
            this.pnlHeader.Size = new System.Drawing.Size(1530, 92);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(44, 62, 80);
            this.lblTitle.Location = new System.Drawing.Point(30, 22);
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
            this.pnlFilter.Size = new System.Drawing.Size(1470, 100);
            // 
            // lblFrom
            // 
            this.lblFrom.AutoSize = true;
            this.lblFrom.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblFrom.Location = new System.Drawing.Point(30, 35);
            this.lblFrom.Text = "Từ ngày:";
            // 
            // dtpFrom
            // 
            this.dtpFrom.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFrom.Location = new System.Drawing.Point(140, 30);
            this.dtpFrom.Size = new System.Drawing.Size(200, 39);
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTo.Location = new System.Drawing.Point(380, 35);
            this.lblTo.Text = "Đến ngày:";
            // 
            // dtpTo
            // 
            this.dtpTo.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTo.Location = new System.Drawing.Point(510, 30);
            this.dtpTo.Size = new System.Drawing.Size(200, 39);
            // 
            // btnFilter
            // 
            this.btnFilter.BackColor = System.Drawing.Color.FromArgb(41, 128, 185);
            this.btnFilter.ForeColor = System.Drawing.Color.White;
            this.btnFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilter.FlatAppearance.BorderSize = 0;
            this.btnFilter.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnFilter.Location = new System.Drawing.Point(750, 25);
            this.btnFilter.Size = new System.Drawing.Size(200, 50);
            this.btnFilter.Text = "🔍 LỌC DỮ LIỆU";
            this.btnFilter.Cursor = System.Windows.Forms.Cursors.Hand;
            // 
            // pnlCards
            // 
            this.pnlCards.BackColor = System.Drawing.Color.Transparent;
            this.pnlCards.Controls.Add(this.lblRate);
            this.pnlCards.Controls.Add(this.lblRateTitle);
            this.pnlCards.Controls.Add(this.lblNoShow);
            this.pnlCards.Controls.Add(this.lblNoShowTitle);
            this.pnlCards.Controls.Add(this.lblTotal);
            this.pnlCards.Controls.Add(this.lblTotalTitle);
            this.pnlCards.Location = new System.Drawing.Point(30, 250);
            this.pnlCards.Size = new System.Drawing.Size(1470, 160);
            // 
            // lblTotalTitle
            // 
            this.lblTotalTitle.BackColor = System.Drawing.Color.FromArgb(41, 128, 185);
            this.lblTotalTitle.ForeColor = System.Drawing.Color.White;
            this.lblTotalTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotalTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTotalTitle.Size = new System.Drawing.Size(470, 50);
            this.lblTotalTitle.Text = "TỔNG ĐƠN ĐẶT BÀN";
            this.lblTotalTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotal
            // 
            this.lblTotal.BackColor = System.Drawing.Color.White;
            this.lblTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Bold);
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(41, 128, 185);
            this.lblTotal.Location = new System.Drawing.Point(0, 50);
            this.lblTotal.Size = new System.Drawing.Size(470, 110);
            this.lblTotal.Text = "0";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNoShowTitle
            // 
            this.lblNoShowTitle.BackColor = System.Drawing.Color.FromArgb(243, 156, 18);
            this.lblNoShowTitle.ForeColor = System.Drawing.Color.White;
            this.lblNoShowTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblNoShowTitle.Location = new System.Drawing.Point(500, 0);
            this.lblNoShowTitle.Size = new System.Drawing.Size(470, 50);
            this.lblNoShowTitle.Text = "SỐ ĐƠN KHÁCH BỎ CHỖ (NO-SHOW)";
            this.lblNoShowTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNoShow
            // 
            this.lblNoShow.BackColor = System.Drawing.Color.White;
            this.lblNoShow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNoShow.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Bold);
            this.lblNoShow.ForeColor = System.Drawing.Color.FromArgb(243, 156, 18);
            this.lblNoShow.Location = new System.Drawing.Point(500, 50);
            this.lblNoShow.Size = new System.Drawing.Size(470, 110);
            this.lblNoShow.Text = "0";
            this.lblNoShow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblRateTitle
            // 
            this.lblRateTitle.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            this.lblRateTitle.ForeColor = System.Drawing.Color.White;
            this.lblRateTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblRateTitle.Location = new System.Drawing.Point(1000, 0);
            this.lblRateTitle.Size = new System.Drawing.Size(470, 50);
            this.lblRateTitle.Text = "TỶ LỆ NO-SHOW";
            this.lblRateTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblRate
            // 
            this.lblRate.BackColor = System.Drawing.Color.White;
            this.lblRate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblRate.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Bold);
            this.lblRate.ForeColor = System.Drawing.Color.FromArgb(231, 76, 60);
            this.lblRate.Location = new System.Drawing.Point(1000, 50);
            this.lblRate.Size = new System.Drawing.Size(470, 110);
            this.lblRate.Text = "0%";
            this.lblRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chartReservations
            // 
            chartArea1.Name = "ChartArea1";
            this.chartReservations.ChartAreas.Add(chartArea1);
            this.chartReservations.Location = new System.Drawing.Point(30, 440);
            this.chartReservations.Size = new System.Drawing.Size(1470, 550);
            series1.ChartArea = "ChartArea1";
            series1.Name = "Đơn đặt bàn";
            series1.Color = System.Drawing.Color.FromArgb(46, 204, 113);
            series1.IsValueShownAsLabel = true;
            this.chartReservations.Series.Add(series1);
            // 
            // UcReports
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(245, 246, 250);
            this.Controls.Add(this.chartReservations);
            this.Controls.Add(this.pnlCards);
            this.Controls.Add(this.pnlFilter);
            this.Controls.Add(this.pnlHeader);
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
        private System.Windows.Forms.Label lblRate;
        private System.Windows.Forms.Label lblRateTitle;
        private System.Windows.Forms.Label lblNoShow;
        private System.Windows.Forms.Label lblNoShowTitle;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblTotalTitle;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartReservations;
    }
}