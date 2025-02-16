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
        private Dictionary<Control, string> helpMessages;
        private Timer toolTipTimer;
        private Control currentControl;

        public frmBackup_Restore()
        {
            InitializeComponent();
            _backupService = new BackupService();

            InitializeHelpMessages(); // Inicializa los mensajes de ayuda traducidos
            SubscribeHelpMessagesEvents(); // Suscribe los eventos de ToolTips

            // Configurar el Timer
            toolTipTimer = new Timer { Interval = 1000 }; // 1 segundo
            toolTipTimer.Tick += ToolTipTimer_Tick;

            // Vincular el evento Load del formulario
            this.Load += frmBackup_Restore_Load;
            this.HelpRequested += FrmBackup_Restore_HelpRequested;
        }

        /// <summary>
        /// Inicializa los mensajes de ayuda con la traducción actual.
        /// </summary>
        private void InitializeHelpMessages()
        {
            helpMessages = new Dictionary<Control, string>
            {
                { cmbBackup, LanguageService.Translate("Seleccione una copia de seguridad disponible para restaurar.") },
                { btnBackup, LanguageService.Translate("Haga clic para restaurar la copia de seguridad seleccionada.") }
            };
        }

        /// <summary>
        /// Suscribe los eventos `MouseEnter` y `MouseLeave` a los controles con mensajes de ayuda.
        /// </summary>
        private void SubscribeHelpMessagesEvents()
        {
            if (helpMessages != null)
            {
                foreach (var control in helpMessages.Keys)
                {
                    control.MouseEnter += Control_MouseEnter;
                    control.MouseLeave += Control_MouseLeave;
                }
            }
        }

        private void Control_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Control control && helpMessages.ContainsKey(control))
            {
                currentControl = control;
                toolTipTimer.Start();
            }
        }

        private void Control_MouseLeave(object sender, EventArgs e)
        {
            toolTipTimer.Stop();
            currentControl = null;
        }

        private void ToolTipTimer_Tick(object sender, EventArgs e)
        {
            if (currentControl != null && helpMessages.ContainsKey(currentControl))
            {
                ToolTip toolTip = new ToolTip();
                toolTip.Show(helpMessages[currentControl], currentControl, 3000);
            }
            toolTipTimer.Stop();
        }

        /// <summary>
        /// Evento Load del formulario. Llena el ComboBox con los archivos de respaldo disponibles.
        /// </summary>
        private void frmBackup_Restore_Load(object sender, EventArgs e)
        {
            try
            {
                List<string> backupFiles = _backupService.GetBackupFiles();

                if (backupFiles.Any())
                {
                    cmbBackup.Items.Clear();
                    cmbBackup.Items.AddRange(backupFiles.ToArray());
                }
                else
                {
                    MessageBox.Show(
                        LanguageService.Translate("No hay respaldos disponibles."),
                        LanguageService.Translate("Información"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{LanguageService.Translate("Error al cargar respaldos")}: {ex.Message}",
                    LanguageService.Translate("Error"),
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
                    _backupService.RestoreBackup(backupDate);
                    MessageBox.Show(
                        LanguageService.Translate("Respaldo restaurado con éxito."),
                        LanguageService.Translate("Éxito"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"{LanguageService.Translate("Error al restaurar el respaldo")}: {ex.Message}",
                        LanguageService.Translate("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            else
            {
                MessageBox.Show(
                    LanguageService.Translate("Seleccione un respaldo antes de continuar."),
                    LanguageService.Translate("Advertencia"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        /// <summary>
        /// Actualiza las ayudas cuando se cambia el idioma.
        /// </summary>
        public void UpdateHelpMessages()
        {
            InitializeHelpMessages();
        }

        private void FrmBackup_Restore_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helpMessage = string.Join(Environment.NewLine, new[]
            {
                LanguageService.Translate("Bienvenido al módulo de gestión de copias de seguridad."),
                "",
                LanguageService.Translate("Opciones disponibles:"),
                $"- {LanguageService.Translate("Seleccionar una copia de seguridad de la lista disponible.")}",
                $"- {LanguageService.Translate("Presionar el botón 'Restaurar' para recuperar los datos desde la copia de seguridad seleccionada.")}",
                "",
                LanguageService.Translate("Para más ayuda, contacte con el administrador.")
            });

            MessageBox.Show(helpMessage,
                            LanguageService.Translate("Ayuda del sistema"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }
    }
}
