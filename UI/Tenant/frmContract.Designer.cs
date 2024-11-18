namespace UI.Tenant
{
    partial class frmContract
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
            this.dgvContracts = new System.Windows.Forms.DataGridView();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvContracts)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvContracts
            // 
            this.dgvContracts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvContracts.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvContracts.Location = new System.Drawing.Point(0, 0);
            this.dgvContracts.Name = "dgvContracts";
            this.dgvContracts.RowHeadersWidth = 51;
            this.dgvContracts.RowTemplate.Height = 24;
            this.dgvContracts.Size = new System.Drawing.Size(873, 414);
            this.dgvContracts.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(12, 418);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(608, 25);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Guardar Firmado";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(623, 418);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(238, 25);
            this.btnDownload.TabIndex = 2;
            this.btnDownload.Text = "Descargar contrato";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // frmContract
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(873, 453);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvContracts);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmContract";
            this.Text = "Contratos";
            ((System.ComponentModel.ISupportInitialize)(this.dgvContracts)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvContracts;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDownload;
    }
}