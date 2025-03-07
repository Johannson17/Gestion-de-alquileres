﻿namespace UI.Tenant
{
    partial class frmMainTenant
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.contratosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ticketsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configuraciónToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmbLanguage = new System.Windows.Forms.ToolStripComboBox();
            this.reportesToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.propiedadesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.contratosToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.menuStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contratosToolStripMenuItem,
            this.ticketsToolStripMenuItem,
            this.configuraciónToolStripMenuItem,
            this.reportesToolStripMenuItem3});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(11, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(600, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // contratosToolStripMenuItem
            // 
            this.contratosToolStripMenuItem.Name = "contratosToolStripMenuItem";
            this.contratosToolStripMenuItem.Size = new System.Drawing.Size(132, 20);
            this.contratosToolStripMenuItem.Text = "Contratos pendientes";
            this.contratosToolStripMenuItem.Click += new System.EventHandler(this.contratosToolStripMenuItem_Click);
            // 
            // ticketsToolStripMenuItem
            // 
            this.ticketsToolStripMenuItem.Name = "ticketsToolStripMenuItem";
            this.ticketsToolStripMenuItem.Size = new System.Drawing.Size(78, 20);
            this.ticketsToolStripMenuItem.Text = "Incidencias";
            this.ticketsToolStripMenuItem.Click += new System.EventHandler(this.ticketsToolStripMenuItem_Click);
            // 
            // configuraciónToolStripMenuItem
            // 
            this.configuraciónToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmbLanguage});
            this.configuraciónToolStripMenuItem.Name = "configuraciónToolStripMenuItem";
            this.configuraciónToolStripMenuItem.Size = new System.Drawing.Size(95, 20);
            this.configuraciónToolStripMenuItem.Text = "Configuración";
            // 
            // cmbLanguage
            // 
            this.cmbLanguage.Name = "cmbLanguage";
            this.cmbLanguage.Size = new System.Drawing.Size(121, 23);
            this.cmbLanguage.SelectedIndexChanged += new System.EventHandler(this.btnTranslate_Click);
            // 
            // reportesToolStripMenuItem3
            // 
            this.reportesToolStripMenuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.propiedadesToolStripMenuItem1,
            this.contratosToolStripMenuItem1});
            this.reportesToolStripMenuItem3.Name = "reportesToolStripMenuItem3";
            this.reportesToolStripMenuItem3.Size = new System.Drawing.Size(65, 20);
            this.reportesToolStripMenuItem3.Text = "Reportes";
            // 
            // propiedadesToolStripMenuItem1
            // 
            this.propiedadesToolStripMenuItem1.Name = "propiedadesToolStripMenuItem1";
            this.propiedadesToolStripMenuItem1.Size = new System.Drawing.Size(139, 22);
            this.propiedadesToolStripMenuItem1.Text = "Propiedades";
            this.propiedadesToolStripMenuItem1.Click += new System.EventHandler(this.propiedadesToolStripMenuItem1_Click);
            // 
            // contratosToolStripMenuItem1
            // 
            this.contratosToolStripMenuItem1.Name = "contratosToolStripMenuItem1";
            this.contratosToolStripMenuItem1.Size = new System.Drawing.Size(139, 22);
            this.contratosToolStripMenuItem1.Text = "Contratos";
            this.contratosToolStripMenuItem1.Click += new System.EventHandler(this.contratosToolStripMenuItem1_Click);
            // 
            // frmMainTenant
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 366);
            this.Controls.Add(this.menuStrip1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.IsMdiContainer = true;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "frmMainTenant";
            this.Text = "Inicio";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem contratosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ticketsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportesToolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem propiedadesToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem contratosToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem configuraciónToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox cmbLanguage;
    }
}