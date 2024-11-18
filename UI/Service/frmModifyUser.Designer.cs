namespace UI.Service
{
    partial class frmModifyUser
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAddFamilia = new System.Windows.Forms.Button();
            this.cmbFamily = new System.Windows.Forms.ComboBox();
            this.cmbPermissions = new System.Windows.Forms.ComboBox();
            this.txtConfirmPassword = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.dgvUsers = new System.Windows.Forms.DataGridView();
            this.btnDelete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).BeginInit();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(572, 252);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 33;
            this.label5.Text = "Permisos:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(397, 252);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 32;
            this.label4.Text = "Roles:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 366);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "Confirmar contraseña: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 306);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 30;
            this.label2.Text = "Contraseña: ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 244);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "Nombre de usuario:";
            // 
            // btnAddFamilia
            // 
            this.btnAddFamilia.Location = new System.Drawing.Point(399, 391);
            this.btnAddFamilia.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnAddFamilia.Name = "btnAddFamilia";
            this.btnAddFamilia.Size = new System.Drawing.Size(140, 19);
            this.btnAddFamilia.TabIndex = 28;
            this.btnAddFamilia.Text = "Nuevo Rol";
            this.btnAddFamilia.UseVisualStyleBackColor = true;
            this.btnAddFamilia.Click += new System.EventHandler(this.btnAddFamilia_Click);
            // 
            // cmbFamily
            // 
            this.cmbFamily.FormattingEnabled = true;
            this.cmbFamily.Location = new System.Drawing.Point(399, 267);
            this.cmbFamily.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmbFamily.Name = "cmbFamily";
            this.cmbFamily.Size = new System.Drawing.Size(140, 21);
            this.cmbFamily.TabIndex = 27;
            // 
            // cmbPermissions
            // 
            this.cmbPermissions.FormattingEnabled = true;
            this.cmbPermissions.Location = new System.Drawing.Point(574, 267);
            this.cmbPermissions.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmbPermissions.Name = "cmbPermissions";
            this.cmbPermissions.Size = new System.Drawing.Size(140, 21);
            this.cmbPermissions.TabIndex = 26;
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.Location = new System.Drawing.Point(10, 383);
            this.txtConfirmPassword.Multiline = true;
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.Size = new System.Drawing.Size(307, 28);
            this.txtConfirmPassword.TabIndex = 25;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(10, 323);
            this.txtPassword.Multiline = true;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(307, 25);
            this.txtPassword.TabIndex = 24;
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(10, 260);
            this.txtUsername.Multiline = true;
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(307, 28);
            this.txtUsername.TabIndex = 23;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(9, 428);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(704, 21);
            this.btnSave.TabIndex = 22;
            this.btnSave.Text = "Guardar Cambios";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dgvUsers
            // 
            this.dgvUsers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsers.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvUsers.Location = new System.Drawing.Point(0, 0);
            this.dgvUsers.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dgvUsers.Name = "dgvUsers";
            this.dgvUsers.RowHeadersWidth = 51;
            this.dgvUsers.RowTemplate.Height = 24;
            this.dgvUsers.Size = new System.Drawing.Size(723, 222);
            this.dgvUsers.TabIndex = 48;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(9, 456);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(704, 19);
            this.btnDelete.TabIndex = 49;
            this.btnDelete.Text = "Eliminar usuario";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // frmModifyUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(723, 483);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.dgvUsers);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnAddFamilia);
            this.Controls.Add(this.cmbFamily);
            this.Controls.Add(this.cmbPermissions);
            this.Controls.Add(this.txtConfirmPassword);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "frmModifyUser";
            this.Text = "Modificar usuario";
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAddFamilia;
        private System.Windows.Forms.ComboBox cmbFamily;
        private System.Windows.Forms.ComboBox cmbPermissions;
        private System.Windows.Forms.TextBox txtConfirmPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DataGridView dgvUsers;
        private System.Windows.Forms.Button btnDelete;
    }
}