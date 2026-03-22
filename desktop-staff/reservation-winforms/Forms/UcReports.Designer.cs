namespace reservation_winforms.Forms
{
    partial class UcReports
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.btnFilter = new System.Windows.Forms.Button();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.lblTo = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.lblFrom = new System.Windows.Forms.Label();
            this.lblFilterTitle = new System.Windows.Forms.Label();
            this.pnlCards = new System.Windows.Forms.Panel();
            this.pnlCard3 = new System.Windows.Forms.Panel();
            this.lblValNoShow = new System.Windows.Forms.Label();
            this.lblTitleNoShow = new System.Windows.Forms.Label();
            this.pnlCard2 = new System.Windows.Forms.Panel();
            this.lblValSeated = new System.Windows.Forms.Label();
            this.lblTitleSeated = new System.Windows.Forms.Label();
            this.pnlCard1 = new System.Windows.Forms.Panel();
            this.lblValTotal = new System.Windows.Forms.Label();
            this.lblTitleTotal = new System.Windows.Forms.Label();
            this.pnlList = new System.Windows.Forms.Panel();
            this.dgvReports = new System.Windows.Forms.DataGridView();
            this.colDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSeated = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCancelled = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNoShow = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblListTitle = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlFilter.SuspendLayout();
            this.pnlCards.SuspendLayout();
            this.pnlCard3.SuspendLayout();
            this.pnlCard2.SuspendLayout();
            this.pnlCard1.SuspendLayout();
            this.pnlList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReports)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1530, 92);
            this.pnlHeader.TabIndex = 4;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.lblTitle.Location = new System.Drawing.Point(30, 22);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(473, 48);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "BÁO CÁO & THỐNG KÊ (KPI)";
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
            this.pnlFilter.Controls.Add(this.lblFilterTitle);
            this.pnlFilter.Location = new System.Drawing.Point(30, 123);
            this.pnlFilter.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(1469, 122);
            this.pnlFilter.TabIndex = 5;
            // 
            // btnFilter
            // 
            this.btnFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnFilter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFilter.FlatAppearance.BorderSize = 0;
            this.btnFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilter.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFilter.ForeColor = System.Drawing.Color.White;
            this.btnFilter.Location = new System.Drawing.Point(1230, 31);
            this.btnFilter.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(210, 62);
            this.btnFilter.TabIndex = 5;
            this.btnFilter.Text = "LỌC DỮ LIỆU";
            this.btnFilter.UseVisualStyleBackColor = false;
            // 
            // dtpTo
            // 
            this.dtpTo.CalendarFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpTo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTo.Location = new System.Drawing.Point(810, 37);
            this.dtpTo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(238, 45);
            this.dtpTo.TabIndex = 4;
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblTo.Location = new System.Drawing.Point(675, 46);
            this.lblTo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(122, 32);
            this.lblTo.TabIndex = 3;
            this.lblTo.Text = "Đến ngày:";
            // 
            // dtpFrom
            // 
            this.dtpFrom.CalendarFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpFrom.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFrom.Location = new System.Drawing.Point(390, 37);
            this.dtpFrom.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(238, 45);
            this.dtpFrom.TabIndex = 2;
            // 
            // lblFrom
            // 
            this.lblFrom.AutoSize = true;
            this.lblFrom.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFrom.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblFrom.Location = new System.Drawing.Point(270, 46);
            this.lblFrom.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(105, 32);
            this.lblFrom.TabIndex = 1;
            this.lblFrom.Text = "Từ ngày:";
            // 
            // lblFilterTitle
            // 
            this.lblFilterTitle.AutoSize = true;
            this.lblFilterTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilterTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.lblFilterTitle.Location = new System.Drawing.Point(30, 42);
            this.lblFilterTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFilterTitle.Name = "lblFilterTitle";
            this.lblFilterTitle.Size = new System.Drawing.Size(195, 38);
            this.lblFilterTitle.TabIndex = 0;
            this.lblFilterTitle.Text = "Thời gian lọc:";
            // 
            // pnlCards
            // 
            this.pnlCards.Controls.Add(this.pnlCard3);
            this.pnlCards.Controls.Add(this.pnlCard2);
            this.pnlCards.Controls.Add(this.pnlCard1);
            this.pnlCards.Location = new System.Drawing.Point(30, 277);
            this.pnlCards.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlCards.Name = "pnlCards";
            this.pnlCards.Size = new System.Drawing.Size(1470, 215);
            this.pnlCards.TabIndex = 6;
            // 
            // pnlCard3
            // 
            this.pnlCard3.BackColor = System.Drawing.Color.White;
            this.pnlCard3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCard3.Controls.Add(this.lblValNoShow);
            this.pnlCard3.Controls.Add(this.lblTitleNoShow);
            this.pnlCard3.Location = new System.Drawing.Point(1005, 0);
            this.pnlCard3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlCard3.Name = "pnlCard3";
            this.pnlCard3.Size = new System.Drawing.Size(464, 214);
            this.pnlCard3.TabIndex = 2;
            // 
            // lblValNoShow
            // 
            this.lblValNoShow.AutoSize = true;
            this.lblValNoShow.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValNoShow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.lblValNoShow.Location = new System.Drawing.Point(30, 77);
            this.lblValNoShow.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblValNoShow.Name = "lblValNoShow";
            this.lblValNoShow.Size = new System.Drawing.Size(245, 96);
            this.lblValNoShow.TabIndex = 1;
            this.lblValNoShow.Text = "12.5%";
            // 
            // lblTitleNoShow
            // 
            this.lblTitleNoShow.AutoSize = true;
            this.lblTitleNoShow.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleNoShow.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleNoShow.Location = new System.Drawing.Point(30, 23);
            this.lblTitleNoShow.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitleNoShow.Name = "lblTitleNoShow";
            this.lblTitleNoShow.Size = new System.Drawing.Size(360, 38);
            this.lblTitleNoShow.TabIndex = 0;
            this.lblTitleNoShow.Text = "Tỉ lệ Không đến (No-Show)";
            // 
            // pnlCard2
            // 
            this.pnlCard2.BackColor = System.Drawing.Color.White;
            this.pnlCard2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCard2.Controls.Add(this.lblValSeated);
            this.pnlCard2.Controls.Add(this.lblTitleSeated);
            this.pnlCard2.Location = new System.Drawing.Point(502, 0);
            this.pnlCard2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlCard2.Name = "pnlCard2";
            this.pnlCard2.Size = new System.Drawing.Size(464, 214);
            this.pnlCard2.TabIndex = 1;
            // 
            // lblValSeated
            // 
            this.lblValSeated.AutoSize = true;
            this.lblValSeated.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValSeated.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.lblValSeated.Location = new System.Drawing.Point(30, 77);
            this.lblValSeated.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblValSeated.Name = "lblValSeated";
            this.lblValSeated.Size = new System.Drawing.Size(163, 96);
            this.lblValSeated.TabIndex = 1;
            this.lblValSeated.Text = "150";
            // 
            // lblTitleSeated
            // 
            this.lblTitleSeated.AutoSize = true;
            this.lblTitleSeated.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleSeated.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleSeated.Location = new System.Drawing.Point(30, 23);
            this.lblTitleSeated.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitleSeated.Name = "lblTitleSeated";
            this.lblTitleSeated.Size = new System.Drawing.Size(312, 38);
            this.lblTitleSeated.TabIndex = 0;
            this.lblTitleSeated.Text = "Đã phục vụ thành công";
            // 
            // pnlCard1
            // 
            this.pnlCard1.BackColor = System.Drawing.Color.White;
            this.pnlCard1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCard1.Controls.Add(this.lblValTotal);
            this.pnlCard1.Controls.Add(this.lblTitleTotal);
            this.pnlCard1.Location = new System.Drawing.Point(0, 0);
            this.pnlCard1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlCard1.Name = "pnlCard1";
            this.pnlCard1.Size = new System.Drawing.Size(464, 214);
            this.pnlCard1.TabIndex = 0;
            // 
            // lblValTotal
            // 
            this.lblValTotal.AutoSize = true;
            this.lblValTotal.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValTotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.lblValTotal.Location = new System.Drawing.Point(30, 77);
            this.lblValTotal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblValTotal.Name = "lblValTotal";
            this.lblValTotal.Size = new System.Drawing.Size(163, 96);
            this.lblValTotal.TabIndex = 1;
            this.lblValTotal.Text = "184";
            // 
            // lblTitleTotal
            // 
            this.lblTitleTotal.AutoSize = true;
            this.lblTitleTotal.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleTotal.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleTotal.Location = new System.Drawing.Point(30, 23);
            this.lblTitleTotal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitleTotal.Name = "lblTitleTotal";
            this.lblTitleTotal.Size = new System.Drawing.Size(224, 38);
            this.lblTitleTotal.TabIndex = 0;
            this.lblTitleTotal.Text = "Tổng số yêu cầu";
            // 
            // pnlList
            // 
            this.pnlList.BackColor = System.Drawing.Color.White;
            this.pnlList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlList.Controls.Add(this.dgvReports);
            this.pnlList.Controls.Add(this.lblListTitle);
            this.pnlList.Location = new System.Drawing.Point(30, 523);
            this.pnlList.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlList.Name = "pnlList";
            this.pnlList.Padding = new System.Windows.Forms.Padding(30, 31, 30, 31);
            this.pnlList.Size = new System.Drawing.Size(1469, 568);
            this.pnlList.TabIndex = 7;
            // 
            // dgvReports
            // 
            this.dgvReports.AllowUserToAddRows = false;
            this.dgvReports.AllowUserToDeleteRows = false;
            this.dgvReports.AllowUserToResizeRows = false;
            this.dgvReports.BackgroundColor = System.Drawing.Color.White;
            this.dgvReports.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvReports.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvReports.ColumnHeadersHeight = 40;
            this.dgvReports.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colDate,
            this.colTotal,
            this.colSeated,
            this.colCancelled,
            this.colNoShow,
            this.colRate});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(245)))), ((int)(((byte)(251)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvReports.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvReports.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvReports.EnableHeadersVisualStyles = false;
            this.dgvReports.Location = new System.Drawing.Point(30, 92);
            this.dgvReports.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvReports.Name = "dgvReports";
            this.dgvReports.ReadOnly = true;
            this.dgvReports.RowHeadersVisible = false;
            this.dgvReports.RowHeadersWidth = 62;
            this.dgvReports.RowTemplate.Height = 45;
            this.dgvReports.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReports.Size = new System.Drawing.Size(1407, 443);
            this.dgvReports.TabIndex = 2;
            // 
            // colDate
            // 
            this.colDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDate.HeaderText = "Ngày";
            this.colDate.MinimumWidth = 8;
            this.colDate.Name = "colDate";
            this.colDate.ReadOnly = true;
            // 
            // colTotal
            // 
            this.colTotal.HeaderText = "Tổng Yêu Cầu";
            this.colTotal.MinimumWidth = 8;
            this.colTotal.Name = "colTotal";
            this.colTotal.ReadOnly = true;
            this.colTotal.Width = 150;
            // 
            // colSeated
            // 
            this.colSeated.HeaderText = "Phục vụ";
            this.colSeated.MinimumWidth = 8;
            this.colSeated.Name = "colSeated";
            this.colSeated.ReadOnly = true;
            this.colSeated.Width = 150;
            // 
            // colCancelled
            // 
            this.colCancelled.HeaderText = "Khách Hủy";
            this.colCancelled.MinimumWidth = 8;
            this.colCancelled.Name = "colCancelled";
            this.colCancelled.ReadOnly = true;
            this.colCancelled.Width = 150;
            // 
            // colNoShow
            // 
            this.colNoShow.HeaderText = "Không đến";
            this.colNoShow.MinimumWidth = 8;
            this.colNoShow.Name = "colNoShow";
            this.colNoShow.ReadOnly = true;
            this.colNoShow.Width = 150;
            // 
            // colRate
            // 
            this.colRate.HeaderText = "Tỉ lệ No-show";
            this.colRate.MinimumWidth = 8;
            this.colRate.Name = "colRate";
            this.colRate.ReadOnly = true;
            this.colRate.Width = 150;
            // 
            // lblListTitle
            // 
            this.lblListTitle.AutoSize = true;
            this.lblListTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblListTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblListTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.lblListTitle.Location = new System.Drawing.Point(30, 31);
            this.lblListTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblListTitle.Name = "lblListTitle";
            this.lblListTitle.Padding = new System.Windows.Forms.Padding(0, 0, 0, 23);
            this.lblListTitle.Size = new System.Drawing.Size(401, 61);
            this.lblListTitle.TabIndex = 1;
            this.lblListTitle.Text = "CHI TIẾT THEO NGÀY (DAILY)";
            // 
            // UcReports
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.pnlList);
            this.Controls.Add(this.pnlCards);
            this.Controls.Add(this.pnlFilter);
            this.Controls.Add(this.pnlHeader);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "UcReports";
            this.Size = new System.Drawing.Size(1530, 1122);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            this.pnlCards.ResumeLayout(false);
            this.pnlCard3.ResumeLayout(false);
            this.pnlCard3.PerformLayout();
            this.pnlCard2.ResumeLayout(false);
            this.pnlCard2.PerformLayout();
            this.pnlCard1.ResumeLayout(false);
            this.pnlCard1.PerformLayout();
            this.pnlList.ResumeLayout(false);
            this.pnlList.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReports)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.Label lblFilterTitle;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Panel pnlCards;
        private System.Windows.Forms.Panel pnlCard1;
        private System.Windows.Forms.Label lblTitleTotal;
        private System.Windows.Forms.Label lblValTotal;
        private System.Windows.Forms.Panel pnlCard3;
        private System.Windows.Forms.Label lblValNoShow;
        private System.Windows.Forms.Label lblTitleNoShow;
        private System.Windows.Forms.Panel pnlCard2;
        private System.Windows.Forms.Label lblValSeated;
        private System.Windows.Forms.Label lblTitleSeated;
        private System.Windows.Forms.Panel pnlList;
        private System.Windows.Forms.Label lblListTitle;
        private System.Windows.Forms.DataGridView dgvReports;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotal;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSeated;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCancelled;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNoShow;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRate;
    }
}