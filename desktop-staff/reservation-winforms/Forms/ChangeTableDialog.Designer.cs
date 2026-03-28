namespace reservation_winforms.Forms
{
    partial class ChangeTableDialog
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.flpAvailableTables = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAltTable1 = new System.Windows.Forms.Button();
            this.btnAltTable2 = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            this.flpAvailableTables.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(600, 50);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(15, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(268, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "🔄 ĐỔI BÀN (NHẬN KHÁCH)";
            // 
            // lblInfo
            // 
            this.lblInfo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.lblInfo.Location = new System.Drawing.Point(20, 70);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(560, 50);
            this.lblInfo.TabIndex = 1;
            this.lblInfo.Text = "Bàn 5 hiện tại chưa trống. Vui lòng chọn một bàn trống dưới đây có sức chứa phù h" +
    "ợp (Yêu cầu: 4 người) để nhận khách.";
            // 
            // flpAvailableTables
            // 
            this.flpAvailableTables.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            this.flpAvailableTables.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flpAvailableTables.Controls.Add(this.btnAltTable1);
            this.flpAvailableTables.Controls.Add(this.btnAltTable2);
            this.flpAvailableTables.Location = new System.Drawing.Point(24, 130);
            this.flpAvailableTables.Name = "flpAvailableTables";
            this.flpAvailableTables.Padding = new System.Windows.Forms.Padding(10);
            this.flpAvailableTables.Size = new System.Drawing.Size(556, 180);
            this.flpAvailableTables.TabIndex = 2;
            // 
            // btnAltTable1
            // 
            this.btnAltTable1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnAltTable1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAltTable1.FlatAppearance.BorderSize = 0;
            this.btnAltTable1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAltTable1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAltTable1.ForeColor = System.Drawing.Color.White;
            this.btnAltTable1.Location = new System.Drawing.Point(15, 15);
            this.btnAltTable1.Margin = new System.Windows.Forms.Padding(5);
            this.btnAltTable1.Name = "btnAltTable1";
            this.btnAltTable1.Size = new System.Drawing.Size(120, 100);
            this.btnAltTable1.TabIndex = 0;
            this.btnAltTable1.Text = "Bàn 8\r\n(Trống)";
            this.btnAltTable1.UseVisualStyleBackColor = false;
            // 
            // btnAltTable2
            // 
            this.btnAltTable2.BackColor = System.Drawing.Color.White;
            this.btnAltTable2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAltTable2.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnAltTable2.FlatAppearance.BorderSize = 2;
            this.btnAltTable2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAltTable2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAltTable2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnAltTable2.Location = new System.Drawing.Point(145, 15);
            this.btnAltTable2.Margin = new System.Windows.Forms.Padding(5);
            this.btnAltTable2.Name = "btnAltTable2";
            this.btnAltTable2.Size = new System.Drawing.Size(120, 100);
            this.btnAltTable2.TabIndex = 1;
            this.btnAltTable2.Text = "Bàn 12\r\n(Trống)";
            this.btnAltTable2.UseVisualStyleBackColor = false;
            // 
            // btnConfirm
            // 
            this.btnConfirm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirm.FlatAppearance.BorderSize = 0;
            this.btnConfirm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirm.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirm.ForeColor = System.Drawing.Color.White;
            this.btnConfirm.Location = new System.Drawing.Point(380, 330);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(200, 50);
            this.btnConfirm.TabIndex = 3;
            this.btnConfirm.Text = "ĐỔI & CHECK-IN";
            this.btnConfirm.UseVisualStyleBackColor = false;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnCancel.Location = new System.Drawing.Point(250, 330);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 50);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Hủy bỏ";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // ChangeTableDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(600, 400);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.flpAvailableTables);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangeTableDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Thay đổi bàn";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.flpAvailableTables.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.FlowLayoutPanel flpAvailableTables;
        private System.Windows.Forms.Button btnAltTable1;
        private System.Windows.Forms.Button btnAltTable2;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnCancel;
    }
}