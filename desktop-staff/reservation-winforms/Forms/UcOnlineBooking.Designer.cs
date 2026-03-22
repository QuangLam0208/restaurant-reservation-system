namespace reservation_winforms.Forms
{
    partial class UcOnlineBooking
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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearchHint = new System.Windows.Forms.Label();
            this.pnlDetails = new System.Windows.Forms.Panel();
            this.lblValTimeStatus = new System.Windows.Forms.Label();
            this.lblValTableStatus = new System.Windows.Forms.Label();
            this.lblTitleTableStatus = new System.Windows.Forms.Label();
            this.lblValTable = new System.Windows.Forms.Label();
            this.lblTitleTable = new System.Windows.Forms.Label();
            this.lblValGuests = new System.Windows.Forms.Label();
            this.lblTitleGuests = new System.Windows.Forms.Label();
            this.lblValTime = new System.Windows.Forms.Label();
            this.lblTitleTime = new System.Windows.Forms.Label();
            this.lblValPhone = new System.Windows.Forms.Label();
            this.lblTitlePhone = new System.Windows.Forms.Label();
            this.lblValName = new System.Windows.Forms.Label();
            this.lblTitleName = new System.Windows.Forms.Label();
            this.lblDetailTitle = new System.Windows.Forms.Label();
            this.btnCheckIn = new System.Windows.Forms.Button();
            this.btnCancelBooking = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            this.pnlDetails.SuspendLayout();
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
            this.pnlHeader.TabIndex = 1;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.lblTitle.Location = new System.Drawing.Point(30, 22);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(593, 48);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "XÁC NHẬN ĐẶT TRƯỚC (ONLINE)";
            // 
            // pnlSearch
            // 
            this.pnlSearch.BackColor = System.Drawing.Color.White;
            this.pnlSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSearch.Controls.Add(this.btnSearch);
            this.pnlSearch.Controls.Add(this.txtSearch);
            this.pnlSearch.Controls.Add(this.lblSearchHint);
            this.pnlSearch.Location = new System.Drawing.Point(39, 138);
            this.pnlSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Size = new System.Drawing.Size(1451, 153);
            this.pnlSearch.TabIndex = 2;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearch.FlatAppearance.BorderSize = 0;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(960, 46);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(225, 62);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "TÌM KIẾM";
            this.btnSearch.UseVisualStyleBackColor = false;
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.Location = new System.Drawing.Point(390, 49);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(538, 50);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.Text = "0987654321";
            // 
            // lblSearchHint
            // 
            this.lblSearchHint.AutoSize = true;
            this.lblSearchHint.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSearchHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblSearchHint.Location = new System.Drawing.Point(60, 55);
            this.lblSearchHint.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSearchHint.Name = "lblSearchHint";
            this.lblSearchHint.Size = new System.Drawing.Size(319, 38);
            this.lblSearchHint.TabIndex = 0;
            this.lblSearchHint.Text = "Số điện thoại / Mã đơn:";
            // 
            // pnlDetails
            // 
            this.pnlDetails.BackColor = System.Drawing.Color.White;
            this.pnlDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDetails.Controls.Add(this.lblValTimeStatus);
            this.pnlDetails.Controls.Add(this.lblValTableStatus);
            this.pnlDetails.Controls.Add(this.lblTitleTableStatus);
            this.pnlDetails.Controls.Add(this.lblValTable);
            this.pnlDetails.Controls.Add(this.lblTitleTable);
            this.pnlDetails.Controls.Add(this.lblValGuests);
            this.pnlDetails.Controls.Add(this.lblTitleGuests);
            this.pnlDetails.Controls.Add(this.lblValTime);
            this.pnlDetails.Controls.Add(this.lblTitleTime);
            this.pnlDetails.Controls.Add(this.lblValPhone);
            this.pnlDetails.Controls.Add(this.lblTitlePhone);
            this.pnlDetails.Controls.Add(this.lblValName);
            this.pnlDetails.Controls.Add(this.lblTitleName);
            this.pnlDetails.Controls.Add(this.lblDetailTitle);
            this.pnlDetails.Location = new System.Drawing.Point(39, 338);
            this.pnlDetails.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlDetails.Name = "pnlDetails";
            this.pnlDetails.Size = new System.Drawing.Size(1451, 399);
            this.pnlDetails.TabIndex = 3;
            // 
            // lblValTimeStatus
            // 
            this.lblValTimeStatus.AutoSize = true;
            this.lblValTimeStatus.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValTimeStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(156)))), ((int)(((byte)(18)))));
            this.lblValTimeStatus.Location = new System.Drawing.Point(420, 215);
            this.lblValTimeStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblValTimeStatus.Name = "lblValTimeStatus";
            this.lblValTimeStatus.Size = new System.Drawing.Size(180, 32);
            this.lblValTimeStatus.TabIndex = 13;
            this.lblValTimeStatus.Text = "(Đến sớm 15p)";
            // 
            // lblValTableStatus
            // 
            this.lblValTableStatus.AutoSize = true;
            this.lblValTableStatus.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValTableStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.lblValTableStatus.Location = new System.Drawing.Point(315, 323);
            this.lblValTableStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblValTableStatus.Name = "lblValTableStatus";
            this.lblValTableStatus.Size = new System.Drawing.Size(515, 45);
            this.lblValTableStatus.TabIndex = 12;
            this.lblValTableStatus.Text = "🟢 TRỐNG - Sẵn sàng đón khách";
            // 
            // lblTitleTableStatus
            // 
            this.lblTitleTableStatus.AutoSize = true;
            this.lblTitleTableStatus.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleTableStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleTableStatus.Location = new System.Drawing.Point(60, 328);
            this.lblTitleTableStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitleTableStatus.Name = "lblTitleTableStatus";
            this.lblTitleTableStatus.Size = new System.Drawing.Size(212, 38);
            this.lblTitleTableStatus.TabIndex = 11;
            this.lblTitleTableStatus.Text = "Tình trạng bàn :";
            // 
            // lblValTable
            // 
            this.lblValTable.AutoSize = true;
            this.lblValTable.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValTable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.lblValTable.Location = new System.Drawing.Point(315, 265);
            this.lblValTable.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblValTable.Name = "lblValTable";
            this.lblValTable.Size = new System.Drawing.Size(104, 45);
            this.lblValTable.TabIndex = 10;
            this.lblValTable.Text = "Bàn 5";
            // 
            // lblTitleTable
            // 
            this.lblTitleTable.AutoSize = true;
            this.lblTitleTable.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleTable.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleTable.Location = new System.Drawing.Point(60, 269);
            this.lblTitleTable.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitleTable.Name = "lblTitleTable";
            this.lblTitleTable.Size = new System.Drawing.Size(159, 38);
            this.lblTitleTable.TabIndex = 9;
            this.lblTitleTable.Text = "Bàn đã xếp:";
            // 
            // lblValGuests
            // 
            this.lblValGuests.AutoSize = true;
            this.lblValGuests.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValGuests.ForeColor = System.Drawing.Color.Black;
            this.lblValGuests.Location = new System.Drawing.Point(870, 211);
            this.lblValGuests.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblValGuests.Name = "lblValGuests";
            this.lblValGuests.Size = new System.Drawing.Size(120, 38);
            this.lblValGuests.TabIndex = 8;
            this.lblValGuests.Text = "4 người";
            // 
            // lblTitleGuests
            // 
            this.lblTitleGuests.AutoSize = true;
            this.lblTitleGuests.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleGuests.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleGuests.Location = new System.Drawing.Point(705, 211);
            this.lblTitleGuests.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitleGuests.Name = "lblTitleGuests";
            this.lblTitleGuests.Size = new System.Drawing.Size(134, 38);
            this.lblTitleGuests.TabIndex = 7;
            this.lblTitleGuests.Text = "Số lượng:";
            // 
            // lblValTime
            // 
            this.lblValTime.AutoSize = true;
            this.lblValTime.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.lblValTime.Location = new System.Drawing.Point(315, 206);
            this.lblValTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblValTime.Name = "lblValTime";
            this.lblValTime.Size = new System.Drawing.Size(101, 45);
            this.lblValTime.TabIndex = 6;
            this.lblValTime.Text = "19:00";
            // 
            // lblTitleTime
            // 
            this.lblTitleTime.AutoSize = true;
            this.lblTitleTime.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleTime.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleTime.Location = new System.Drawing.Point(60, 211);
            this.lblTitleTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitleTime.Name = "lblTitleTime";
            this.lblTitleTime.Size = new System.Drawing.Size(107, 38);
            this.lblTitleTime.TabIndex = 5;
            this.lblTitleTime.Text = "Giờ tới:";
            // 
            // lblValPhone
            // 
            this.lblValPhone.AutoSize = true;
            this.lblValPhone.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValPhone.ForeColor = System.Drawing.Color.Black;
            this.lblValPhone.Location = new System.Drawing.Point(315, 149);
            this.lblValPhone.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblValPhone.Name = "lblValPhone";
            this.lblValPhone.Size = new System.Drawing.Size(177, 38);
            this.lblValPhone.TabIndex = 4;
            this.lblValPhone.Text = "0987654321";
            // 
            // lblTitlePhone
            // 
            this.lblTitlePhone.AutoSize = true;
            this.lblTitlePhone.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitlePhone.ForeColor = System.Drawing.Color.Gray;
            this.lblTitlePhone.Location = new System.Drawing.Point(60, 149);
            this.lblTitlePhone.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitlePhone.Name = "lblTitlePhone";
            this.lblTitlePhone.Size = new System.Drawing.Size(186, 38);
            this.lblTitlePhone.TabIndex = 3;
            this.lblTitlePhone.Text = "Số điện thoại:";
            // 
            // lblValName
            // 
            this.lblValName.AutoSize = true;
            this.lblValName.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValName.ForeColor = System.Drawing.Color.Black;
            this.lblValName.Location = new System.Drawing.Point(315, 85);
            this.lblValName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblValName.Name = "lblValName";
            this.lblValName.Size = new System.Drawing.Size(271, 45);
            this.lblValName.TabIndex = 2;
            this.lblValName.Text = "Nguyễn Văn Anh";
            // 
            // lblTitleName
            // 
            this.lblTitleName.AutoSize = true;
            this.lblTitleName.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleName.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleName.Location = new System.Drawing.Point(60, 92);
            this.lblTitleName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitleName.Name = "lblTitleName";
            this.lblTitleName.Size = new System.Drawing.Size(168, 38);
            this.lblTitleName.TabIndex = 1;
            this.lblTitleName.Text = "Khách hàng:";
            // 
            // lblDetailTitle
            // 
            this.lblDetailTitle.AutoSize = true;
            this.lblDetailTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDetailTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.lblDetailTitle.Location = new System.Drawing.Point(30, 23);
            this.lblDetailTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDetailTitle.Name = "lblDetailTitle";
            this.lblDetailTitle.Size = new System.Drawing.Size(361, 45);
            this.lblDetailTitle.TabIndex = 0;
            this.lblDetailTitle.Text = "THÔNG TIN ĐẶT BÀN:";
            // 
            // btnCheckIn
            // 
            this.btnCheckIn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnCheckIn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCheckIn.FlatAppearance.BorderSize = 0;
            this.btnCheckIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckIn.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCheckIn.ForeColor = System.Drawing.Color.White;
            this.btnCheckIn.Location = new System.Drawing.Point(1116, 785);
            this.btnCheckIn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCheckIn.Name = "btnCheckIn";
            this.btnCheckIn.Size = new System.Drawing.Size(375, 92);
            this.btnCheckIn.TabIndex = 4;
            this.btnCheckIn.Text = "CHECK-IN";
            this.btnCheckIn.UseVisualStyleBackColor = false;
            // 
            // btnCancelBooking
            // 
            this.btnCancelBooking.BackColor = System.Drawing.Color.White;
            this.btnCancelBooking.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelBooking.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnCancelBooking.FlatAppearance.BorderSize = 2;
            this.btnCancelBooking.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelBooking.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelBooking.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnCancelBooking.Location = new System.Drawing.Point(831, 785);
            this.btnCancelBooking.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCancelBooking.Name = "btnCancelBooking";
            this.btnCancelBooking.Size = new System.Drawing.Size(240, 92);
            this.btnCancelBooking.TabIndex = 5;
            this.btnCancelBooking.Text = "HỦY ĐƠN";
            this.btnCancelBooking.UseVisualStyleBackColor = false;
            // 
            // UcOnlineBooking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.btnCancelBooking);
            this.Controls.Add(this.btnCheckIn);
            this.Controls.Add(this.pnlDetails);
            this.Controls.Add(this.pnlSearch);
            this.Controls.Add(this.pnlHeader);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "UcOnlineBooking";
            this.Size = new System.Drawing.Size(1530, 1122);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.pnlDetails.ResumeLayout(false);
            this.pnlDetails.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblSearchHint;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Panel pnlDetails;
        private System.Windows.Forms.Label lblDetailTitle;
        private System.Windows.Forms.Label lblTitleName;
        private System.Windows.Forms.Label lblValName;
        private System.Windows.Forms.Label lblTitlePhone;
        private System.Windows.Forms.Label lblValPhone;
        private System.Windows.Forms.Label lblTitleTime;
        private System.Windows.Forms.Label lblValTime;
        private System.Windows.Forms.Label lblTitleGuests;
        private System.Windows.Forms.Label lblValGuests;
        private System.Windows.Forms.Label lblTitleTable;
        private System.Windows.Forms.Label lblValTable;
        private System.Windows.Forms.Label lblTitleTableStatus;
        private System.Windows.Forms.Label lblValTableStatus;
        private System.Windows.Forms.Label lblValTimeStatus;
        private System.Windows.Forms.Button btnCheckIn;
        private System.Windows.Forms.Button btnCancelBooking;
    }
}