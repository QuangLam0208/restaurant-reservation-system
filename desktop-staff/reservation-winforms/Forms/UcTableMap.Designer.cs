namespace reservation_winforms.Forms
{
    partial class UcTableMap
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
            this.pnlSidebar = new System.Windows.Forms.Panel();
            this.btnWaitlist = new System.Windows.Forms.Button();
            this.btnCheckOut = new System.Windows.Forms.Button();
            this.btnSeatWalkIn = new System.Windows.Forms.Button();
            this.nudGuestCount = new System.Windows.Forms.NumericUpDown();
            this.lblGuestCount = new System.Windows.Forms.Label();
            this.lblSelectedTable = new System.Windows.Forms.Label();
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.btnFilterOverstay = new System.Windows.Forms.Button(); // Thêm nút Quá giờ
            this.btnFilterReserved = new System.Windows.Forms.Button();
            this.btnFilterOccupied = new System.Windows.Forms.Button();
            this.btnFilterAvailable = new System.Windows.Forms.Button();
            this.btnFilterAll = new System.Windows.Forms.Button();
            this.lblFilterText = new System.Windows.Forms.Label();
            this.flpTableMap = new System.Windows.Forms.FlowLayoutPanel(); // Xóa 3 nút bàn ảo đi
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlSidebar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudGuestCount)).BeginInit();
            this.pnlFilter.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSidebar
            // 
            this.pnlSidebar.BackColor = System.Drawing.Color.White;
            this.pnlSidebar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSidebar.Controls.Add(this.btnWaitlist);
            this.pnlSidebar.Controls.Add(this.btnCheckOut);
            this.pnlSidebar.Controls.Add(this.btnSeatWalkIn);
            this.pnlSidebar.Controls.Add(this.nudGuestCount);
            this.pnlSidebar.Controls.Add(this.lblGuestCount);
            this.pnlSidebar.Controls.Add(this.lblSelectedTable);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlSidebar.Location = new System.Drawing.Point(720, 60);
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.Size = new System.Drawing.Size(300, 669);
            this.pnlSidebar.TabIndex = 2;
            // 
            // btnWaitlist
            // 
            this.btnWaitlist.BackColor = System.Drawing.Color.White;
            this.btnWaitlist.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnWaitlist.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(156)))), ((int)(((byte)(18)))));
            this.btnWaitlist.FlatAppearance.BorderSize = 2;
            this.btnWaitlist.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnWaitlist.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnWaitlist.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(156)))), ((int)(((byte)(18)))));
            this.btnWaitlist.Location = new System.Drawing.Point(20, 310);
            this.btnWaitlist.Name = "btnWaitlist";
            this.btnWaitlist.Size = new System.Drawing.Size(260, 50);
            this.btnWaitlist.TabIndex = 5;
            this.btnWaitlist.Text = "ĐƯA VÀO HÀNG ĐỢI";
            this.btnWaitlist.UseVisualStyleBackColor = false;
            // 
            // btnCheckOut
            // 
            this.btnCheckOut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(165)))), ((int)(((byte)(166)))));
            this.btnCheckOut.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCheckOut.FlatAppearance.BorderSize = 0;
            this.btnCheckOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckOut.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCheckOut.ForeColor = System.Drawing.Color.White;
            this.btnCheckOut.Location = new System.Drawing.Point(20, 240);
            this.btnCheckOut.Name = "btnCheckOut";
            this.btnCheckOut.Size = new System.Drawing.Size(260, 50);
            this.btnCheckOut.TabIndex = 4;
            this.btnCheckOut.Text = "THANH TOÁN (CHECK-OUT)";
            this.btnCheckOut.UseVisualStyleBackColor = false;
            // 
            // btnSeatWalkIn
            // 
            this.btnSeatWalkIn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnSeatWalkIn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSeatWalkIn.FlatAppearance.BorderSize = 0;
            this.btnSeatWalkIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSeatWalkIn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSeatWalkIn.ForeColor = System.Drawing.Color.White;
            this.btnSeatWalkIn.Location = new System.Drawing.Point(20, 170);
            this.btnSeatWalkIn.Name = "btnSeatWalkIn";
            this.btnSeatWalkIn.Size = new System.Drawing.Size(260, 50);
            this.btnSeatWalkIn.TabIndex = 3;
            this.btnSeatWalkIn.Text = "XẾP BÀN (WALK-IN)";
            this.btnSeatWalkIn.UseVisualStyleBackColor = false;
            // 
            // nudGuestCount
            // 
            this.nudGuestCount.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudGuestCount.Location = new System.Drawing.Point(20, 110);
            this.nudGuestCount.Name = "nudGuestCount";
            this.nudGuestCount.Size = new System.Drawing.Size(260, 36);
            this.nudGuestCount.TabIndex = 2;
            this.nudGuestCount.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // lblGuestCount
            // 
            this.lblGuestCount.AutoSize = true;
            this.lblGuestCount.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGuestCount.ForeColor = System.Drawing.Color.Gray;
            this.lblGuestCount.Location = new System.Drawing.Point(20, 80);
            this.lblGuestCount.Name = "lblGuestCount";
            this.lblGuestCount.Size = new System.Drawing.Size(164, 20);
            this.lblGuestCount.TabIndex = 1;
            this.lblGuestCount.Text = "Số lượng khách Walk-in:";
            // 
            // lblSelectedTable
            // 
            this.lblSelectedTable.AutoSize = true;
            this.lblSelectedTable.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectedTable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.lblSelectedTable.Location = new System.Drawing.Point(20, 30);
            this.lblSelectedTable.Name = "lblSelectedTable";
            this.lblSelectedTable.Size = new System.Drawing.Size(186, 32);
            this.lblSelectedTable.TabIndex = 0;
            this.lblSelectedTable.Text = "Chưa chọn bàn";
            // 
            // pnlFilter
            // 
            this.pnlFilter.BackColor = System.Drawing.Color.White;
            this.pnlFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFilter.Controls.Add(this.btnFilterOverstay);
            this.pnlFilter.Controls.Add(this.btnFilterReserved);
            this.pnlFilter.Controls.Add(this.btnFilterOccupied);
            this.pnlFilter.Controls.Add(this.btnFilterAvailable);
            this.pnlFilter.Controls.Add(this.btnFilterAll);
            this.pnlFilter.Controls.Add(this.lblFilterText);
            this.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilter.Location = new System.Drawing.Point(0, 60);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(720, 60);
            this.pnlFilter.TabIndex = 4;
            // 
            // btnFilterOverstay
            // 
            this.btnFilterOverstay.BackColor = System.Drawing.Color.White;
            this.btnFilterOverstay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFilterOverstay.FlatAppearance.BorderColor = System.Drawing.Color.IndianRed;
            this.btnFilterOverstay.FlatAppearance.BorderSize = 2;
            this.btnFilterOverstay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilterOverstay.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFilterOverstay.ForeColor = System.Drawing.Color.IndianRed;
            this.btnFilterOverstay.Location = new System.Drawing.Point(495, 12);
            this.btnFilterOverstay.Name = "btnFilterOverstay";
            this.btnFilterOverstay.Size = new System.Drawing.Size(95, 34);
            this.btnFilterOverstay.TabIndex = 5;
            this.btnFilterOverstay.Text = "Quá giờ";
            this.btnFilterOverstay.UseVisualStyleBackColor = false;
            // 
            // btnFilterReserved
            // 
            this.btnFilterReserved.BackColor = System.Drawing.Color.White;
            this.btnFilterReserved.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFilterReserved.FlatAppearance.BorderColor = System.Drawing.Color.Orange;
            this.btnFilterReserved.FlatAppearance.BorderSize = 2;
            this.btnFilterReserved.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilterReserved.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFilterReserved.ForeColor = System.Drawing.Color.Orange;
            this.btnFilterReserved.Location = new System.Drawing.Point(395, 12);
            this.btnFilterReserved.Name = "btnFilterReserved";
            this.btnFilterReserved.Size = new System.Drawing.Size(90, 34);
            this.btnFilterReserved.TabIndex = 4;
            this.btnFilterReserved.Text = "Đã đặt";
            this.btnFilterReserved.UseVisualStyleBackColor = false;
            // 
            // btnFilterOccupied
            // 
            this.btnFilterOccupied.BackColor = System.Drawing.Color.White;
            this.btnFilterOccupied.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFilterOccupied.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnFilterOccupied.FlatAppearance.BorderSize = 2;
            this.btnFilterOccupied.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilterOccupied.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFilterOccupied.ForeColor = System.Drawing.Color.Gray;
            this.btnFilterOccupied.Location = new System.Drawing.Point(295, 12);
            this.btnFilterOccupied.Name = "btnFilterOccupied";
            this.btnFilterOccupied.Size = new System.Drawing.Size(90, 34);
            this.btnFilterOccupied.TabIndex = 3;
            this.btnFilterOccupied.Text = "Đang ăn";
            this.btnFilterOccupied.UseVisualStyleBackColor = false;
            // 
            // btnFilterAvailable
            // 
            this.btnFilterAvailable.BackColor = System.Drawing.Color.White;
            this.btnFilterAvailable.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFilterAvailable.FlatAppearance.BorderColor = System.Drawing.Color.MediumSeaGreen;
            this.btnFilterAvailable.FlatAppearance.BorderSize = 2;
            this.btnFilterAvailable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilterAvailable.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFilterAvailable.ForeColor = System.Drawing.Color.MediumSeaGreen;
            this.btnFilterAvailable.Location = new System.Drawing.Point(195, 12);
            this.btnFilterAvailable.Name = "btnFilterAvailable";
            this.btnFilterAvailable.Size = new System.Drawing.Size(90, 34);
            this.btnFilterAvailable.TabIndex = 2;
            this.btnFilterAvailable.Text = "Trống";
            this.btnFilterAvailable.UseVisualStyleBackColor = false;
            // 
            // btnFilterAll
            // 
            this.btnFilterAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnFilterAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFilterAll.FlatAppearance.BorderSize = 0;
            this.btnFilterAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilterAll.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFilterAll.ForeColor = System.Drawing.Color.White;
            this.btnFilterAll.Location = new System.Drawing.Point(95, 12);
            this.btnFilterAll.Name = "btnFilterAll";
            this.btnFilterAll.Size = new System.Drawing.Size(90, 34);
            this.btnFilterAll.TabIndex = 1;
            this.btnFilterAll.Text = "Tất cả";
            this.btnFilterAll.UseVisualStyleBackColor = false;
            // 
            // lblFilterText
            // 
            this.lblFilterText.AutoSize = true;
            this.lblFilterText.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilterText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblFilterText.Location = new System.Drawing.Point(15, 18);
            this.lblFilterText.Name = "lblFilterText";
            this.lblFilterText.Size = new System.Drawing.Size(73, 21);
            this.lblFilterText.TabIndex = 0;
            this.lblFilterText.Text = "Lọc bàn:";
            // 
            // flpTableMap
            // 
            this.flpTableMap.AutoScroll = true;
            this.flpTableMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpTableMap.Location = new System.Drawing.Point(0, 120);
            this.flpTableMap.Name = "flpTableMap";
            this.flpTableMap.Padding = new System.Windows.Forms.Padding(20);
            this.flpTableMap.Size = new System.Drawing.Size(720, 609);
            this.flpTableMap.TabIndex = 5;
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1020, 60);
            this.pnlHeader.TabIndex = 6;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.lblTitle.Location = new System.Drawing.Point(20, 14);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(315, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "SƠ ĐỒ BÀN (TABLE MAP)";
            // 
            // UcTableMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.flpTableMap);
            this.Controls.Add(this.pnlFilter);
            this.Controls.Add(this.pnlSidebar);
            this.Controls.Add(this.pnlHeader);
            this.Name = "UcTableMap";
            this.Size = new System.Drawing.Size(1020, 729);
            this.pnlSidebar.ResumeLayout(false);
            this.pnlSidebar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudGuestCount)).EndInit();
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlSidebar;
        private System.Windows.Forms.Button btnWaitlist;
        private System.Windows.Forms.Button btnCheckOut;
        private System.Windows.Forms.Button btnSeatWalkIn;
        private System.Windows.Forms.NumericUpDown nudGuestCount;
        private System.Windows.Forms.Label lblGuestCount;
        private System.Windows.Forms.Label lblSelectedTable;
        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.Button btnFilterOverstay; // Nút mới
        private System.Windows.Forms.Button btnFilterReserved;
        private System.Windows.Forms.Button btnFilterOccupied;
        private System.Windows.Forms.Button btnFilterAvailable;
        private System.Windows.Forms.Button btnFilterAll;
        private System.Windows.Forms.Label lblFilterText;
        private System.Windows.Forms.FlowLayoutPanel flpTableMap; // Đã xóa các nút bàn con
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
    }
}