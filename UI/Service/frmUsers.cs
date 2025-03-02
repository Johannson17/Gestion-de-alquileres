using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Services.Facade;
using Domain;
using Services.Domain;
using System.Drawing;
using System.IO;

namespace UI.Admin
{
    public partial class frmUsers : Form
    {
        public Guid SelectedUserId { get; private set; } // Property to store the selected User ID

        private Dictionary<Control, string> helpMessages; // Diccionario para mensajes de ayuda
        private Timer toolTipTimer; // Temporizador para mostrar ToolTips
        private Control currentControl; // Control actual donde está el mouse

        public frmUsers()
        {
            InitializeComponent();
            LoadUsers(); // Cargar usuarios al abrir el formulario
            InitializeHelpMessages(); // Inicializar mensajes de ayuda traducidos
            SubscribeHelpMessagesEvents(); // Suscribir eventos de ayuda

            // Configurar el Timer
            toolTipTimer = new Timer { Interval = 1000 }; // 1 segundo
            toolTipTimer.Tick += ToolTipTimer_Tick;

            dgvUsers.CellDoubleClick += dgvUsers_CellDoubleClick; // Vincular evento de doble clic
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
                { dgvUsers, LanguageService.Translate("Seleccione un usuario haciendo doble clic para continuar.") }
            };
        }

        /// <summary>
        /// Suscribe los eventos de ayuda (`MouseEnter` y `MouseLeave`) a los controles.
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
        /// Carga los usuarios y los muestra en el `DataGridView`.
        /// </summary>
        private void LoadUsers()
        {
            try
            {
                List<Usuario> users = UserService.GetAllUsers();
                dgvUsers.DataSource = users;

                dgvUsers.Columns["IdUsuario"].HeaderText = LanguageService.Translate("ID de Usuario");
                dgvUsers.Columns["UserName"].HeaderText = LanguageService.Translate("Nombre de Usuario");
                dgvUsers.Columns["Password"].Visible = false; // Ocultar contraseña

                dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{LanguageService.Translate("Error al cargar los usuarios")}: {ex.Message}",
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void dgvUsers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    SelectedUserId = (Guid)dgvUsers.Rows[e.RowIndex].Cells["IdUsuario"].Value;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"{LanguageService.Translate("Error al seleccionar el usuario")}: {ex.Message}",
                        LanguageService.Translate("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Actualiza los mensajes de ayuda cuando se cambia el idioma.
        /// </summary>
        public void UpdateHelpMessages()
        {
            InitializeHelpMessages();
        }

        private void FrmUsers_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helpMessage = string.Join(Environment.NewLine, new[]
            {
                LanguageService.Translate("Bienvenido al módulo de gestión de usuarios."),
                "",
                LanguageService.Translate("Opciones disponibles:"),
                $"- {LanguageService.Translate("Seleccionar un usuario haciendo doble clic en la lista.")}",
                $"- {LanguageService.Translate("Se cerrará este formulario y el usuario seleccionado será devuelto.")}",
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
                LanguageService.Translate("MÓDULO DE SELECCIÓN DE USUARIO").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Este formulario te permite elegir un usuario existente en el sistema, ").ToString() +
                LanguageService.Translate("por ejemplo, para asignarlo a una persona o a otra entidad que requiera un usuario asociado.").ToString() + "\r\n\r\n" +
                LanguageService.Translate("INSTRUCCIONES PASO A PASO:").ToString() + "\r\n" +
                LanguageService.Translate("1. Observa la lista de usuarios en la tabla.").ToString() + "\r\n" +
                LanguageService.Translate("2. Haz doble clic en el usuario que deseas seleccionar. ").ToString() +
                LanguageService.Translate("   Se cerrará este formulario y se devolverá el usuario seleccionado al formulario anterior.").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Con estos pasos, habrás elegido un usuario de la lista para utilizarlo donde sea necesario. ").ToString() +
                LanguageService.Translate("Si necesitas más ayuda, por favor contacta al administrador del sistema.").ToString());

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
                                    LanguageService.Translate("No se encontró la imagen de ayuda en la ruta: ") + imagePath;
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
