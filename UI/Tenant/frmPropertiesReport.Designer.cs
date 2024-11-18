namespace UI.Tenant
{
    partial class frmPropertiesReport
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
            this.btnFilter = new System.Windows.Forms.Button();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.btnDownload = new System.Windows.Forms.Button();
            this.dgvProperties = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProperties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnFilter
            // 
            this.btnFilter.Location = new System.Drawing.Point(211, 403);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(60, 25);
            this.btnFilter.TabIndex = 24;
            this.btnFilter.Text = "Filtrar";
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // cmbStatus
            // 
            this.cmbStatus.FormattingEnabled = true;
            this.cmbStatus.Location = new System.Drawing.Point(12, 404);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(193, 24);
            this.cmbStatus.TabIndex = 23;
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(12, 450);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(792, 26);
            this.btnDownload.TabIndex = 22;
            this.btnDownload.Text = "Descargar Reporte";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // dgvProperties
            // 
            this.dgvProperties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProperties.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvProperties.Location = new System.Drawing.Point(0, 0);
            this.dgvProperties.Name = "dgvProperties";
            this.dgvProperties.RowTemplate.Height = 24;
            this.dgvProperties.Size = new System.Drawing.Size(814, 374);
            this.dgvProperties.TabIndex = 21;
            // 
            // frmPropertiesReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 483);
            this.Controls.Add(this.btnFilter);
            this.Controls.Add(this.cmbStatus);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.dgvProperties);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmPropertiesReport";
            this.Text = "frmPropertiesReport";
            ((System.ComponentModel.ISupportInitialize)(this.dgvProperties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.DataGridView dgvProperties;
    }
}