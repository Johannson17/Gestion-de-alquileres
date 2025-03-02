using Services.Facade;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
            this.KeyPreview = true; // Permite que el formulario intercepte teclas como F1
            this.HelpRequested += FrmAddProperty_HelpRequested_f1; // <-- Asignación del evento
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

        private void FrmAddProperty_HelpRequested_f1(object sender, HelpEventArgs hlpevent)
        {
            hlpevent.Handled = true;

            // 1. Construimos el nombre del archivo de imagen según el formulario
            string imageFileName = $"{this.Name}.png";
            // 2. Ruta completa de la imagen (ajusta la carpeta si difiere)
            string imagePath = Path.Combine(Application.StartupPath, "..", "..", "images", imageFileName);
            imagePath = Path.GetFullPath(imagePath);

            // 3. Texto de ayuda específico para "Agregar propiedad"
            var helpMessage = string.Format(
                LanguageService.Translate("MÓDULO DE RESTAURACIÓN DE COPIAS DE SEGURIDAD").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Este formulario te permite restaurar una copia de seguridad del sistema, ").ToString() +
                LanguageService.Translate("revirtiendo la base de datos al estado que tenía en la fecha del respaldo seleccionado. ").ToString() +
                LanguageService.Translate("Ten en cuenta que esta acción sobrescribirá los datos actuales con la información ").ToString() +
                LanguageService.Translate("almacenada en la copia de seguridad.").ToString() + "\r\n\r\n" +
                LanguageService.Translate("INSTRUCCIONES PASO A PASO:").ToString() + "\r\n" +
                LanguageService.Translate("1. Selecciona la copia de seguridad que deseas restaurar en la lista desplegable. ").ToString() +
                LanguageService.Translate("   Verás las opciones disponibles, normalmente identificadas por fecha u otro criterio.").ToString() + "\r\n" +
                LanguageService.Translate("2. Haz clic en ‘Restaurar copia de seguridad’ para iniciar el proceso de restauración.").ToString() + "\r\n" +
                LanguageService.Translate("3. Espera a que el sistema complete la restauración. ").ToString() +
                LanguageService.Translate("   Se mostrará un mensaje indicando si la operación fue exitosa o si ocurrió algún error.").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Con estos pasos, revertirás el sistema al estado guardado en la copia de seguridad elegida. ").ToString() +
                LanguageService.Translate("Si necesitas más ayuda o si no estás seguro de qué respaldo restaurar, contacta al administrador del sistema.").ToString());

            // 4. Creamos el formulario de ayuda
            using (Form helpForm = new Form())
            {
                // El formulario se autoajusta a su contenido
                helpForm.AutoSize = true;
                helpForm.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                helpForm.StartPosition = FormStartPosition.CenterScreen;
                helpForm.Text = LanguageService.Translate("Ayuda del sistema");
                helpForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                helpForm.MaximizeBox = false;
                helpForm.MinimizeBox = false;

                // (Opcional) Limitamos el tamaño máximo para no salirnos de la pantalla
                // Esto hace que aparezca scroll si el contenido excede este tamaño
                helpForm.MaximumSize = new Size(
                    (int)(Screen.PrimaryScreen.WorkingArea.Width * 0.9),
                    (int)(Screen.PrimaryScreen.WorkingArea.Height * 0.9)
                );

                // Para permitir scroll si el contenido excede el tamaño máximo
                helpForm.AutoScroll = true;

                // 5. Creamos un FlowLayoutPanel que también se autoajuste
                FlowLayoutPanel flowPanel = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.TopDown,   // Apilar texto e imagen verticalmente
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    WrapContents = false,                    // No “romper” en más columnas
                    Padding = new Padding(10)
                };

                // 6. Label para el texto de ayuda
                Label lblHelp = new Label
                {
                    Text = helpMessage,
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    Margin = new Padding(5)
                };

                // (Opcional) Si deseas forzar que el texto no exceda cierto ancho y haga wrap:
                lblHelp.MaximumSize = new Size(800, 0);

                flowPanel.Controls.Add(lblHelp);

                // 7. PictureBox para la imagen
                PictureBox pbHelpImage = new PictureBox
                {
                    Margin = new Padding(5),
                    // Si quieres mostrar la imagen a tamaño real:
                    SizeMode = PictureBoxSizeMode.AutoSize,
                    // O si prefieres que se ajuste pero mantenga proporción:
                    // SizeMode = PictureBoxSizeMode.Zoom,
                };

                if (File.Exists(imagePath))
                {
                    pbHelpImage.Image = Image.FromFile(imagePath);
                }
                else
                {
                    lblHelp.Text += "\r\n\r\n" +
                                    "$" + LanguageService.Translate("No se encontró la imagen de ayuda en la ruta: ") + imagePath;
                }

                // (Opcional) Si usas Zoom, el PictureBox por defecto no hace auto-size, 
                // puedes darle un tamaño inicial y dejar que el form se ajuste
                // pbHelpImage.Size = new Size(600, 400);

                flowPanel.Controls.Add(pbHelpImage);

                // 8. Agregamos el FlowLayoutPanel al formulario
                helpForm.Controls.Add(flowPanel);

                // 9. Mostramos el formulario
                helpForm.ShowDialog();
            }
        }
    }
}
