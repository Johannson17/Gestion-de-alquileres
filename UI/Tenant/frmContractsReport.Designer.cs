namespace UI.Tenant
{
    partial class frmContractsReport
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
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbProperty = new System.Windows.Forms.ComboBox();
            this.btnFilter = new System.Windows.Forms.Button();
            this.btnDownloadImage = new System.Windows.Forms.Button();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.btnImage = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            this.dgvContracts = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvContracts)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(136, 403);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 16);
            this.label2.TabIndex = 20;
            this.label2.Text = "Propiedad:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 403);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 16);
            this.label1.TabIndex = 19;
            this.label1.Text = "Estado:";
            // 
            // cmbProperty
            // 
            this.cmbProperty.FormattingEnabled = true;
            this.cmbProperty.Location = new System.Drawing.Point(139, 422);
            this.cmbProperty.Name = "cmbProperty";
            this.cmbProperty.Size = new System.Drawing.Size(121, 24);
            this.cmbProperty.TabIndex = 17;
            // 
            // btnFilter
            // 
            this.btnFilter.Location = new System.Drawing.Point(266, 422);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(62, 25);
            this.btnFilter.TabIndex = 16;
            this.btnFilter.Text = "Filtrar";
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // btnDownloadImage
            // 
            this.btnDownloadImage.Location = new System.Drawing.Point(794, 412);
            this.btnDownloadImage.Name = "btnDownloadImage";
            this.btnDownloadImage.Size = new System.Drawing.Size(141, 34);
            this.btnDownloadImage.TabIndex = 15;
            this.btnDownloadImage.Text = "Descargar Imagen";
            this.btnDownloadImage.UseVisualStyleBackColor = true;
            this.btnDownloadImage.Click += new System.EventHandler(this.btnDownloadImage_Click);
            // 
            // cmbStatus
            // 
            this.cmbStatus.FormattingEnabled = true;
            this.cmbStatus.Location = new System.Drawing.Point(12, 422);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(121, 24);
            this.cmbStatus.TabIndex = 14;
            // 
            // btnImage
            // 
            this.btnImage.Location = new System.Drawing.Point(941, 412);
            this.btnImage.Name = "btnImage";
            this.btnImage.Size = new System.Drawing.Size(100, 34);
            this.btnImage.TabIndex = 13;
            this.btnImage.Text = "Ver Imagen";
            this.btnImage.UseVisualStyleBackColor = true;
            this.btnImage.Click += new System.EventHandler(this.btnImage_Click);
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(647, 412);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(141, 34);
            this.btnDownload.TabIndex = 12;
            this.btnDownload.Text = "Descargar Reporte";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // dgvContracts
            // 
            this.dgvContracts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvContracts.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvContracts.Location = new System.Drawing.Point(0, 0);
            this.dgvContracts.Name = "dgvContracts";
            this.dgvContracts.RowTemplate.Height = 24;
            this.dgvContracts.Size = new System.Drawing.Size(1048, 374);
            this.dgvContracts.TabIndex = 11;
            // 
            // frmContractsReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1048, 454);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbProperty);
            this.Controls.Add(this.btnFilter);
            this.Controls.Add(this.btnDownloadImage);
            this.Controls.Add(this.cmbStatus);
            this.Controls.Add(this.btnImage);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.dgvContracts);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmContractsReport";
            this.Text = "frmContractsReport";
            ((System.ComponentModel.ISupportInitialize)(this.dgvContracts)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbProperty;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Button btnDownloadImage;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.Button btnImage;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.DataGridView dgvContracts;
    }
}