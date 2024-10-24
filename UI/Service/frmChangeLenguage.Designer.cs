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
            this.dgvLanguages = new System.Windows.Forms.DataGridView();
            this.btnSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLanguages)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvLanguages
            // 
            this.dgvLanguages.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvLanguages.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.dgvLanguages.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLanguages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLanguages.Location = new System.Drawing.Point(0, 0);
            this.dgvLanguages.Margin = new System.Windows.Forms.Padding(4);
            this.dgvLanguages.Name = "dgvLanguages";
            this.dgvLanguages.RowHeadersWidth = 51;
            this.dgvLanguages.Size = new System.Drawing.Size(723, 487);
            this.dgvLanguages.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnSave.Location = new System.Drawing.Point(0, 459);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(723, 28);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Guardar Cambios";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // frmChangeLanguage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(723, 487);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvLanguages);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmChangeLanguage";
            this.Text = "Agregar Lenguaje";
            ((System.ComponentModel.ISupportInitialize)(this.dgvLanguages)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvLanguages;
        private System.Windows.Forms.Button btnSave;
    }
}