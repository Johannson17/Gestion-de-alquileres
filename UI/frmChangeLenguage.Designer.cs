namespace UI
{
    partial class frmChangeLanguage
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
            this.dgvLenguages = new System.Windows.Forms.DataGridView();
            this.btnSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLenguages)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvLenguages
            // 
            this.dgvLenguages.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvLenguages.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.dgvLenguages.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLenguages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLenguages.Location = new System.Drawing.Point(0, 0);
            this.dgvLenguages.Name = "dgvLenguages";
            this.dgvLenguages.Size = new System.Drawing.Size(542, 396);
            this.dgvLenguages.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnSave.Location = new System.Drawing.Point(0, 373);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(542, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Guardar Cambios";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // frmChangeLenguage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 396);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvLenguages);
            this.Name = "frmChangeLenguage";
            this.Text = "frmChangeLenguage";
            ((System.ComponentModel.ISupportInitialize)(this.dgvLenguages)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvLenguages;
        private System.Windows.Forms.Button btnSave;
    }
}