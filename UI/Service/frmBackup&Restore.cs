using Services.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace UI.Service
{
    public partial class frmBackup_Restore : Form
    {
        private readonly BackupService _backupService;

        public frmBackup_Restore()
        {
            InitializeComponent();
            _backupService = new BackupService();

            // Vincular el evento Load del formulario
            this.Load += new System.EventHandler(this.frmBackup_Restore_Load);
        }

        /// <summary>
        /// Evento Load del formulario. Llena el ComboBox con los archivos de respaldo disponibles.
        /// </summary>
        private void frmBackup_Restore_Load(object sender, EventArgs e)
        {
            try
            {
                // Obtener la lista de archivos .txt desde el servicio
                List<string> backupFiles = _backupService.GetBackupFiles();

                if (backupFiles.Any())
                {
                    cmbBackup.Items.Clear();
                    cmbBackup.Items.AddRange(backupFiles.ToArray());
                }
                else
                {
                    MessageBox.Show("No hay respaldos disponibles.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar respaldos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Evento Click del botón de restaurar respaldo. Restaura el respaldo seleccionado en el ComboBox.
        /// </summary>
        private void btnBackup_Click(object sender, EventArgs e)
        {
            if (cmbBackup.SelectedItem != null)
            {
                string selectedBackup = cmbBackup.SelectedItem.ToString();

                // Extraer la fecha del nombre del archivo (asume formato 'yyyyMMdd.txt')
                string backupDate = selectedBackup.Replace(".txt", "");

                try
                {
                    // Llamar al servicio para restaurar el respaldo
                    _backupService.RestoreBackup(backupDate);
                    MessageBox.Show("Respaldo restaurado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al restaurar el respaldo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Seleccione un respaldo antes de continuar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
