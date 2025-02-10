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
        private readonly Dictionary<Control, string> helpMessages;
        private Timer toolTipTimer;
        private Control currentControl;

        public frmBackup_Restore()
        {
            InitializeComponent();
            _backupService = new BackupService();

            // Inicializar mensajes de ayuda
            helpMessages = new Dictionary<Control, string>
            {
                { cmbBackup, "Seleccione una copia de seguridad disponible para restaurar." },
                { btnBackup, "Haga clic para restaurar la copia de seguridad seleccionada." }
            };

            // Configurar el Timer
            toolTipTimer = new Timer();
            toolTipTimer.Interval = 1000; // 2 segundos
            toolTipTimer.Tick += ToolTipTimer_Tick;

            // Asociar eventos a los controles
            foreach (var control in helpMessages.Keys)
            {
                control.MouseEnter += Control_MouseEnter;
                control.MouseLeave += Control_MouseLeave;
            }

            // Vincular el evento Load del formulario
            this.Load += frmBackup_Restore_Load;
        }

        /// <summary>
        /// Maneja la entrada del ratón en un control para iniciar el temporizador del mensaje de ayuda.
        /// </summary>
        private void Control_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Control control && helpMessages.ContainsKey(control))
            {
                currentControl = control; // Guardar el control actual
                toolTipTimer.Start(); // Iniciar el temporizador
            }
        }

        /// <summary>
        /// Maneja la salida del ratón de un control para detener el temporizador.
        /// </summary>
        private void Control_MouseLeave(object sender, EventArgs e)
        {
            toolTipTimer.Stop(); // Detener el temporizador
            currentControl = null; // Limpiar el control actual
        }

        /// <summary>
        /// Muestra un mensaje de ayuda cuando el temporizador alcanza el tiempo establecido.
        /// </summary>
        private void ToolTipTimer_Tick(object sender, EventArgs e)
        {
            if (currentControl != null && helpMessages.ContainsKey(currentControl))
            {
                // Mostrar el ToolTip para el control actual
                ToolTip toolTip = new ToolTip();
                toolTip.Show(helpMessages[currentControl], currentControl, 3000); // Mostrar durante 3 segundos
            }

            toolTipTimer.Stop(); // Detener el temporizador
        }

        /// <summary>
        /// Evento Load del formulario. Llena el ComboBox con los archivos de respaldo disponibles.
        /// </summary>
        private void frmBackup_Restore_Load(object sender, EventArgs e)
        {
            try
            {
                // Obtener la lista de archivos de respaldo desde el servicio
                List<string> backupFiles = _backupService.GetBackupFiles();

                if (backupFiles.Any())
                {
                    cmbBackup.Items.Clear();
                    cmbBackup.Items.AddRange(backupFiles.ToArray());
                }
                else
                {
                    MessageBox.Show(
                        "No hay respaldos disponibles.",
                        "Información",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al cargar respaldos: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
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
                    MessageBox.Show(
                        "Respaldo restaurado con éxito.",
                        "Éxito",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Error al restaurar el respaldo: {ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            else
            {
                MessageBox.Show(
                    "Seleccione un respaldo antes de continuar.",
                    "Advertencia",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }
    }
}
