namespace reservation_winforms.Forms
{
    partial class UcCheckOut
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

        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.flpActiveTables = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlDetails = new System.Windows.Forms.Panel();
            this.lblDetailTitle = new System.Windows.Forms.Label();
            this.lblTitleTable = new System.Windows.Forms.Label();
            this.lblValTable = new System.Windows.Forms.Label();
            this.lblTitleName = new System.Windows.Forms.Label();
            this.lblValName = new System.Windows.Forms.Label();
            this.lblTitleTime = new System.Windows.Forms.Label();
            this.lblValTime = new System.Windows.Forms.Label();
            this.lblTitleDuration = new System.Windows.Forms.Label();
            this.lblValDuration = new System.Windows.Forms.Label();
            this.lblTitleDeposit = new System.Windows.Forms.Label();
            this.lblValDeposit = new System.Windows.Forms.Label();
            this.btnCheckOut = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            this.pnlDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.Controls.Add(this.btnRefresh);
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
            this.lblTitle.Text = "THANH TOÁN & TRẢ BÀN";
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(41, 128, 185);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.Location = new System.Drawing.Point(1350, 20);
            this.btnRefresh.Size = new System.Drawing.Size(150, 50);
            this.btnRefresh.Text = "LÀM MỚI";
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            // 
            // flpActiveTables
            // 
            this.flpActiveTables.AutoScroll = true;
            this.flpActiveTables.Location = new System.Drawing.Point(30, 120);
            this.flpActiveTables.Size = new System.Drawing.Size(850, 950);
            // 
            // pnlDetails
            // 
            this.pnlDetails.BackColor = System.Drawing.Color.White;
            this.pnlDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDetails.Location = new System.Drawing.Point(920, 120);
            this.pnlDetails.Size = new System.Drawing.Size(580, 550);
            this.pnlDetails.Controls.Add(this.lblDetailTitle);
            this.pnlDetails.Controls.Add(this.lblTitleTable);
            this.pnlDetails.Controls.Add(this.lblValTable);
            this.pnlDetails.Controls.Add(this.lblTitleName);
            this.pnlDetails.Controls.Add(this.lblValName);
            this.pnlDetails.Controls.Add(this.lblTitleTime);
            this.pnlDetails.Controls.Add(this.lblValTime);
            this.pnlDetails.Controls.Add(this.lblTitleDuration);
            this.pnlDetails.Controls.Add(this.lblValDuration);
            this.pnlDetails.Controls.Add(this.lblTitleDeposit);
            this.pnlDetails.Controls.Add(this.lblValDeposit);
            // 
            // lblDetailTitle
            // 
            this.lblDetailTitle.AutoSize = true;
            this.lblDetailTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblDetailTitle.ForeColor = System.Drawing.Color.FromArgb(44, 62, 80);
            this.lblDetailTitle.Location = new System.Drawing.Point(30, 30);
            this.lblDetailTitle.Text = "THÔNG TIN BÀN:";
            // 
            // lblTitleTable
            // 
            this.lblTitleTable.AutoSize = true;
            this.lblTitleTable.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblTitleTable.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleTable.Location = new System.Drawing.Point(30, 110);
            this.lblTitleTable.Text = "Bàn số:";
            // 
            // lblValTable
            // 
            this.lblValTable.AutoSize = true;
            this.lblValTable.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblValTable.ForeColor = System.Drawing.Color.FromArgb(41, 128, 185);
            this.lblValTable.Location = new System.Drawing.Point(220, 105);
            this.lblValTable.Text = "5, 6";
            // 
            // lblTitleName
            // 
            this.lblTitleName.AutoSize = true;
            this.lblTitleName.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblTitleName.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleName.Location = new System.Drawing.Point(30, 180);
            this.lblTitleName.Text = "Khách hàng:";
            // 
            // lblValName
            // 
            this.lblValName.AutoSize = true;
            this.lblValName.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblValName.Location = new System.Drawing.Point(220, 180);
            this.lblValName.Text = "Nguyễn Văn A";
            // 
            // lblTitleTime
            // 
            this.lblTitleTime.AutoSize = true;
            this.lblTitleTime.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblTitleTime.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleTime.Location = new System.Drawing.Point(30, 250);
            this.lblTitleTime.Text = "Giờ vào:";
            // 
            // lblValTime
            // 
            this.lblValTime.AutoSize = true;
            this.lblValTime.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblValTime.Location = new System.Drawing.Point(220, 250);
            this.lblValTime.Text = "18:00";
            // 
            // lblTitleDuration
            // 
            this.lblTitleDuration.AutoSize = true;
            this.lblTitleDuration.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblTitleDuration.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleDuration.Location = new System.Drawing.Point(30, 320);
            this.lblTitleDuration.Text = "Đã ngồi:";
            // 
            // lblValDuration
            // 
            this.lblValDuration.AutoSize = true;
            this.lblValDuration.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblValDuration.ForeColor = System.Drawing.Color.FromArgb(243, 156, 18);
            this.lblValDuration.Location = new System.Drawing.Point(220, 320);
            this.lblValDuration.Text = "45 phút";
            // 
            // lblTitleDeposit
            // 
            this.lblTitleDeposit.AutoSize = true;
            this.lblTitleDeposit.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblTitleDeposit.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleDeposit.Location = new System.Drawing.Point(30, 390);
            this.lblTitleDeposit.Text = "Tiền cọc:";
            // 
            // lblValDeposit
            // 
            this.lblValDeposit.AutoSize = true;
            this.lblValDeposit.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblValDeposit.ForeColor = System.Drawing.Color.FromArgb(231, 76, 60);
            this.lblValDeposit.Location = new System.Drawing.Point(220, 385);
            this.lblValDeposit.Text = "200,000đ";
            // 
            // btnCheckOut
            // 
            this.btnCheckOut.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            this.btnCheckOut.ForeColor = System.Drawing.Color.White;
            this.btnCheckOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckOut.FlatAppearance.BorderSize = 0;
            this.btnCheckOut.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.btnCheckOut.Location = new System.Drawing.Point(920, 700);
            this.btnCheckOut.Size = new System.Drawing.Size(580, 100);
            this.btnCheckOut.Text = "XÁC NHẬN TRẢ BÀN";
            this.btnCheckOut.Cursor = System.Windows.Forms.Cursors.Hand;
            // 
            // UcCheckOut
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(245, 246, 250);
            this.Controls.Add(this.btnCheckOut);
            this.Controls.Add(this.pnlDetails);
            this.Controls.Add(this.flpActiveTables);
            this.Controls.Add(this.pnlHeader);
            this.Name = "UcCheckOut";
            this.Size = new System.Drawing.Size(1530, 1122);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlDetails.ResumeLayout(false);
            this.pnlDetails.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.FlowLayoutPanel flpActiveTables;
        private System.Windows.Forms.Panel pnlDetails;
        private System.Windows.Forms.Label lblDetailTitle;
        private System.Windows.Forms.Label lblTitleTable;
        private System.Windows.Forms.Label lblValTable;
        private System.Windows.Forms.Label lblTitleName;
        private System.Windows.Forms.Label lblValName;
        private System.Windows.Forms.Label lblTitleTime;
        private System.Windows.Forms.Label lblValTime;
        private System.Windows.Forms.Label lblTitleDuration;
        private System.Windows.Forms.Label lblValDuration;
        private System.Windows.Forms.Label lblTitleDeposit;
        private System.Windows.Forms.Label lblValDeposit;
        private System.Windows.Forms.Button btnCheckOut;
    }
}