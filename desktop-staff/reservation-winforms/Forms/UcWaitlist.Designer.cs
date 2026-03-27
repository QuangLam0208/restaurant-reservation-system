namespace reservation_winforms.Forms
{
    partial class UcWaitlist
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
            this.pnlAddWaitlist = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.nudGuests = new System.Windows.Forms.NumericUpDown();
            this.lblGuests = new System.Windows.Forms.Label();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblAddTitle = new System.Windows.Forms.Label();
            this.pnlList = new System.Windows.Forms.Panel();
            this.dgvWaitlist = new System.Windows.Forms.DataGridView();
            this.colStt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPhone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGuests = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActionSeat = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colActionSkip = new System.Windows.Forms.DataGridViewButtonColumn();
            this.lblListTitle = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlAddWaitlist.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudGuests)).BeginInit();
            this.pnlList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWaitlist)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1020, 60);
            this.pnlHeader.TabIndex = 2;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.lblTitle.Location = new System.Drawing.Point(20, 14);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(437, 41);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "DANH SÁCH CHỜ (WAITLIST)";
            // 
            // pnlAddWaitlist
            // 
            this.pnlAddWaitlist.BackColor = System.Drawing.Color.White;
            this.pnlAddWaitlist.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAddWaitlist.Controls.Add(this.btnAdd);
            this.pnlAddWaitlist.Controls.Add(this.nudGuests);
            this.pnlAddWaitlist.Controls.Add(this.lblGuests);
            this.pnlAddWaitlist.Controls.Add(this.txtPhone);
            this.pnlAddWaitlist.Controls.Add(this.lblPhone);
            this.pnlAddWaitlist.Controls.Add(this.txtName);
            this.pnlAddWaitlist.Controls.Add(this.lblName);
            this.pnlAddWaitlist.Controls.Add(this.lblAddTitle);
            this.pnlAddWaitlist.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlAddWaitlist.Location = new System.Drawing.Point(0, 60);
            this.pnlAddWaitlist.Name = "pnlAddWaitlist";
            this.pnlAddWaitlist.Padding = new System.Windows.Forms.Padding(20, 20, 20, 20);
            this.pnlAddWaitlist.Size = new System.Drawing.Size(232, 669);
            this.pnlAddWaitlist.TabIndex = 3;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(156)))), ((int)(((byte)(18)))));
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(20, 360);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(191, 50);
            this.btnAdd.TabIndex = 7;
            this.btnAdd.Text = "THÊM VÀO HÀNG ĐỢI";
            this.btnAdd.UseVisualStyleBackColor = false;
            // 
            // nudGuests
            // 
            this.nudGuests.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudGuests.Location = new System.Drawing.Point(20, 280);
            this.nudGuests.Name = "nudGuests";
            this.nudGuests.Size = new System.Drawing.Size(191, 39);
            this.nudGuests.TabIndex = 6;
            this.nudGuests.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // lblGuests
            // 
            this.lblGuests.AutoSize = true;
            this.lblGuests.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGuests.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblGuests.Location = new System.Drawing.Point(20, 250);
            this.lblGuests.Name = "lblGuests";
            this.lblGuests.Size = new System.Drawing.Size(152, 28);
            this.lblGuests.TabIndex = 5;
            this.lblGuests.Text = "Số lượng khách:";
            // 
            // txtPhone
            // 
            this.txtPhone.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPhone.Location = new System.Drawing.Point(20, 190);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(191, 39);
            this.txtPhone.TabIndex = 4;
            // 
            // lblPhone
            // 
            this.lblPhone.AutoSize = true;
            this.lblPhone.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPhone.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblPhone.Location = new System.Drawing.Point(20, 160);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(157, 28);
            this.lblPhone.TabIndex = 3;
            this.lblPhone.Text = "Số điện thoại (*):";
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(20, 100);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(191, 39);
            this.txtName.TabIndex = 2;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblName.Location = new System.Drawing.Point(20, 70);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(143, 28);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Tên gọi nhớ (*):";
            // 
            // lblAddTitle
            // 
            this.lblAddTitle.AutoSize = true;
            this.lblAddTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAddTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.lblAddTitle.Location = new System.Drawing.Point(20, 20);
            this.lblAddTitle.Name = "lblAddTitle";
            this.lblAddTitle.Size = new System.Drawing.Size(230, 32);
            this.lblAddTitle.TabIndex = 0;
            this.lblAddTitle.Text = "THÊM KHÁCH MỚI";
            // 
            // pnlList
            // 
            this.pnlList.BackColor = System.Drawing.Color.White;
            this.pnlList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlList.Controls.Add(this.dgvWaitlist);
            this.pnlList.Controls.Add(this.lblListTitle);
            this.pnlList.Location = new System.Drawing.Point(245, 80);
            this.pnlList.Name = "pnlList";
            this.pnlList.Padding = new System.Windows.Forms.Padding(20, 20, 20, 20);
            this.pnlList.Size = new System.Drawing.Size(755, 620);
            this.pnlList.TabIndex = 4;
            // 
            // dgvWaitlist
            // 
            this.dgvWaitlist.AllowUserToAddRows = false;
            this.dgvWaitlist.AllowUserToDeleteRows = false;
            this.dgvWaitlist.AllowUserToResizeRows = false;
            this.dgvWaitlist.BackgroundColor = System.Drawing.Color.White;
            this.dgvWaitlist.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvWaitlist.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvWaitlist.ColumnHeadersHeight = 40;
            this.dgvWaitlist.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colStt,
            this.colName,
            this.colPhone,
            this.colGuests,
            this.colActionSeat,
            this.colActionSkip});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(245)))), ((int)(((byte)(251)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvWaitlist.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvWaitlist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvWaitlist.EnableHeadersVisualStyles = false;
            this.dgvWaitlist.Location = new System.Drawing.Point(20, 67);
            this.dgvWaitlist.Name = "dgvWaitlist";
            this.dgvWaitlist.ReadOnly = true;
            this.dgvWaitlist.RowHeadersVisible = false;
            this.dgvWaitlist.RowHeadersWidth = 62;
            this.dgvWaitlist.RowTemplate.Height = 50;
            this.dgvWaitlist.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvWaitlist.Size = new System.Drawing.Size(713, 531);
            this.dgvWaitlist.TabIndex = 2;
            // 
            // colStt
            // 
            this.colStt.HeaderText = "STT";
            this.colStt.MinimumWidth = 8;
            this.colStt.Name = "colStt";
            this.colStt.ReadOnly = true;
            this.colStt.Width = 60;
            // 
            // colName
            // 
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colName.HeaderText = "Tên khách";
            this.colName.MinimumWidth = 8;
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            // 
            // colPhone
            // 
            this.colPhone.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colPhone.HeaderText = "SĐT";
            this.colPhone.MinimumWidth = 8;
            this.colPhone.Name = "colPhone";
            this.colPhone.ReadOnly = true;
            // 
            // colGuests
            // 
            this.colGuests.HeaderText = "Số khách";
            this.colGuests.MinimumWidth = 8;
            this.colGuests.Name = "colGuests";
            this.colGuests.ReadOnly = true;
            this.colGuests.Width = 140;
            // 
            // colActionSeat
            // 
            this.colActionSeat.HeaderText = "Xếp Bàn";
            this.colActionSeat.MinimumWidth = 8;
            this.colActionSeat.Name = "colActionSeat";
            this.colActionSeat.ReadOnly = true;
            this.colActionSeat.Text = "Xếp Bàn";
            this.colActionSeat.UseColumnTextForButtonValue = true;
            this.colActionSeat.Width = 120;
            // 
            // colActionSkip
            // 
            this.colActionSkip.HeaderText = "Bỏ qua";
            this.colActionSkip.MinimumWidth = 8;
            this.colActionSkip.Name = "colActionSkip";
            this.colActionSkip.ReadOnly = true;
            this.colActionSkip.Text = "Bỏ qua";
            this.colActionSkip.UseColumnTextForButtonValue = true;
            this.colActionSkip.Width = 120;
            // 
            // lblListTitle
            // 
            this.lblListTitle.AutoSize = true;
            this.lblListTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblListTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblListTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.lblListTitle.Location = new System.Drawing.Point(20, 20);
            this.lblListTitle.Name = "lblListTitle";
            this.lblListTitle.Padding = new System.Windows.Forms.Padding(0, 0, 0, 15);
            this.lblListTitle.Size = new System.Drawing.Size(323, 47);
            this.lblListTitle.TabIndex = 1;
            this.lblListTitle.Text = "ĐANG CHỜ (2 Khách hàng)";
            // 
            // UcWaitlist
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.pnlList);
            this.Controls.Add(this.pnlAddWaitlist);
            this.Controls.Add(this.pnlHeader);
            this.Name = "UcWaitlist";
            this.Size = new System.Drawing.Size(1020, 729);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlAddWaitlist.ResumeLayout(false);
            this.pnlAddWaitlist.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudGuests)).EndInit();
            this.pnlList.ResumeLayout(false);
            this.pnlList.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWaitlist)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlAddWaitlist;
        private System.Windows.Forms.Label lblAddTitle;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.NumericUpDown nudGuests;
        private System.Windows.Forms.Label lblGuests;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Panel pnlList;
        private System.Windows.Forms.Label lblListTitle;
        private System.Windows.Forms.DataGridView dgvWaitlist;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStt;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPhone;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGuests;
        private System.Windows.Forms.DataGridViewButtonColumn colActionSeat;
        private System.Windows.Forms.DataGridViewButtonColumn colActionSkip;
    }
}