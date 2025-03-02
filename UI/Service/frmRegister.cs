using Services.Domain;
using Services.Facade;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using UI.Helpers;

namespace UI
{
    /// <summary>
    /// User registration form. Allows registering a new user in the system, assigning roles and permissions.
    /// </summary>
    public partial class frmRegister : Form
    {
        private Dictionary<Control, string> helpMessages; // Diccionario de ayuda
        private Timer toolTipTimer; // Temporizador para mostrar ToolTips
        private Control currentControl; // Control actual donde está el mouse

        /// <summary>
        /// Constructor que inicializa el formulario de registro de usuario.
        /// </summary>
        public frmRegister()
        {
            InitializeComponent();
            btnRegister.Enabled = false; // El botón de registro inicia deshabilitado

            // Inicializar ComboBoxes vacíos
            cmbFamilias.Items.Clear();
            cmbPatentes.Items.Clear();

            // Asignar eventos para validación en tiempo real
            txtPassword.TextChanged += ValidatePasswords;
            txtConfirmPassword.TextChanged += ValidatePasswords;

            // Asignar eventos para cargar datos cuando los ComboBoxes se despliegan
            cmbFamilias.DropDown += cmbFamilias_DropDown;
            cmbPatentes.DropDown += cmbPatentes_DropDown;

            // Configuración inicial de ComboBoxes
            cmbFamilias.SelectedIndex = -1;
            cmbPatentes.SelectedIndex = -1;

            // Inicializar mensajes de ayuda y eventos
            InitializeHelpMessages();
            SubscribeHelpMessagesEvents();

            // Configurar el Timer
            toolTipTimer = new Timer { Interval = 1000 }; // 1 segundo
            toolTipTimer.Tick += ToolTipTimer_Tick;
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
                { txtUsername, LanguageService.Translate("Ingrese el nombre de usuario.") },
                { txtPassword, LanguageService.Translate("Ingrese la contraseña del usuario.") },
                { txtConfirmPassword, LanguageService.Translate("Confirme la contraseña ingresada.") },
                { cmbFamilias, LanguageService.Translate("Seleccione un rol para el usuario.") },
                { cmbPatentes, LanguageService.Translate("Seleccione un permiso para el usuario.") },
                { btnAddFamilia, LanguageService.Translate("Permite agregar un nuevo rol al sistema.") },
                { btnRegister, LanguageService.Translate("Registra al usuario con los datos proporcionados.") }
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
        /// Actualiza las ayudas cuando se cambia el idioma.
        /// </summary>
        public void UpdateHelpMessages()
        {
            InitializeHelpMessages();
        }

        /// <summary>
        /// Maneja el evento de solicitud de ayuda.
        /// </summary>
        private void FrmRegister_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helpMessage = string.Join(Environment.NewLine, new[]
            {
                LanguageService.Translate("Formulario de Registro de Usuarios."),
                "",
                LanguageService.Translate("Opciones disponibles:"),
                $"- {LanguageService.Translate("Ingresar un nombre de usuario.")}",
                $"- {LanguageService.Translate("Ingresar y confirmar una contraseña.")}",
                $"- {LanguageService.Translate("Seleccionar un rol (familia) para el usuario.")}",
                $"- {LanguageService.Translate("Seleccionar un permiso (patente) si es necesario.")}",
                $"- {LanguageService.Translate("Presionar el botón 'Registrar' para completar el registro.")}",
                "",
                LanguageService.Translate("Para más ayuda, contacte con el administrador.")
            });

            MessageBox.Show(helpMessage,
                            LanguageService.Translate("Ayuda del sistema"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        private void cmbFamilias_DropDown(object sender, EventArgs e)
        {
            if (cmbFamilias.Items.Count == 0)
            {
                LoadFamilias();
            }
        }

        private void cmbPatentes_DropDown(object sender, EventArgs e)
        {
            if (cmbPatentes.Items.Count == 0)
            {
                LoadPatentes();
            }
        }

        private void LoadFamilias()
        {
            if (cmbFamilias.Items.Count == 0)
            {
                cmbFamilias.Items.Add(new Familia { Nombre = "<None>", Id = Guid.Empty });

                var familias = UserService.GetAllFamilias();
                foreach (var familia in familias)
                {
                    cmbFamilias.Items.Add(familia);
                }

                cmbFamilias.DisplayMember = "Nombre";
                cmbFamilias.ValueMember = "Id";
                cmbFamilias.SelectedIndex = -1;
            }
        }

        private void LoadPatentes()
        {
            if (cmbPatentes.Items.Count == 0)
            {
                cmbPatentes.Items.Add(new Patente { Nombre = "<None>", Id = Guid.Empty });

                var patentes = UserService.GetAllPatentes();
                foreach (var patente in patentes)
                {
                    cmbPatentes.Items.Add(patente);
                }

                cmbPatentes.DisplayMember = "Nombre";
                cmbPatentes.ValueMember = "Id";
                cmbPatentes.SelectedIndex = -1;
            }
        }

        private void ValidatePasswords(object sender, EventArgs e)
        {
            if (txtPassword.Text == txtConfirmPassword.Text)
            {
                txtPassword.BackColor = SystemColors.Window;
                txtConfirmPassword.BackColor = SystemColors.Window;
                btnRegister.Enabled = true;
            }
            else
            {
                txtPassword.BackColor = Color.Red;
                txtConfirmPassword.BackColor = Color.Red;
                btnRegister.Enabled = false;
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                var user = new Usuario
                {
                    UserName = txtUsername.Text,
                    Password = txtPassword.Text
                };

                var selectedFamilia = (Familia)cmbFamilias.SelectedItem;
                var selectedPatente = (Patente)cmbPatentes.SelectedItem;

                if (selectedFamilia == null || selectedFamilia.Id == Guid.Empty)
                {
                    selectedFamilia = null;
                }

                if (selectedPatente == null || selectedPatente.Id == Guid.Empty)
                {
                    selectedPatente = null;
                }

                if (selectedFamilia == null && selectedPatente == null)
                {
                    MessageBox.Show(LanguageService.Translate("Debe seleccionar al menos un rol o permiso."));
                    return;
                }

                if (selectedFamilia != null) user.Accesos.Add(selectedFamilia);
                if (selectedPatente != null) user.Accesos.Add(selectedPatente);

                UserService.Register(user);

                MessageBox.Show(LanguageService.Translate("Usuario registrado con éxito."),
                                LanguageService.Translate("Registro"),
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                frmMainAdmin mainForm = new frmMainAdmin();
                mainForm.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{LanguageService.Translate("Error al registrar el usuario")}: {ex.Message}",
                                LanguageService.Translate("Error"),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the click event of the button to add a new Family.
        /// Opens a subform to add a new Family.
        /// </summary>
        private void btnAddFamilia_Click(object sender, EventArgs e)
        {
            using (frmAddFamilia addFamiliaForm = new frmAddFamilia())
            {
                if (addFamiliaForm.ShowDialog() == DialogResult.OK)
                {
                    // Update the family ComboBox
                    LoadFamilias();
                }
            }
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
                LanguageService.Translate("MÓDULO DE REGISTRO DE USUARIOS").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Este formulario te permite crear un nuevo usuario en el sistema, ").ToString() +
                LanguageService.Translate("estableciendo su nombre de usuario, contraseña y asignándole un rol (familia) ").ToString() +
                LanguageService.Translate("y permisos (patentes) específicos.").ToString() + "\r\n\r\n" +
                LanguageService.Translate("INSTRUCCIONES PASO A PASO:").ToString() + "\r\n" +
                LanguageService.Translate("1. Ingresa un nombre de usuario en el campo ‘Nombre de usuario’.").ToString() + "\r\n" +
                LanguageService.Translate("2. Escribe la contraseña en el campo ‘Contraseña’ y confírmala en ‘Confirmar contraseña’. ").ToString() +
                LanguageService.Translate("   Ambas deben coincidir para habilitar el botón de ‘Registrar’.").ToString() + "\r\n" +
                LanguageService.Translate("3. Selecciona un rol (familia) en la lista desplegable ‘Roles’. ").ToString() +
                LanguageService.Translate("   Si no encuentras el rol deseado, haz clic en ‘Nuevo Rol’ para crearlo.").ToString() + "\r\n" +
                LanguageService.Translate("4. Selecciona un permiso (patente) adicional en la lista ‘Permisos’ si el usuario ").ToString() +
                LanguageService.Translate("   requiere un acceso específico. Si no, deja la opción ‘<None>’.").ToString() + "\r\n" +
                LanguageService.Translate("5. Haz clic en ‘Registrar’ para guardar el nuevo usuario en el sistema.").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Con estos pasos, habrás creado un nuevo usuario con sus roles y permisos asignados. ").ToString() +
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
