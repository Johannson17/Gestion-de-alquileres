using Services.Domain;
using Services.Facade;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using UI.Helpers;
using UI.Admin;
using UI.Tenant;
using System.IO;

namespace UI
{
    /// <summary>
    /// Formulario de inicio de sesión. Permite la autenticación de usuarios y la apertura del formulario correspondiente según los permisos del usuario.
    /// </summary>
    public partial class frmLogin : Form
    {

        private readonly Dictionary<Control, string> helpMessages;
        private Timer toolTipTimer;
        private Control currentControl;

        /// <summary>
        /// Inicializa una nueva instancia del formulario de inicio de sesión.
        /// </summary>

        public frmLogin()
        {
            InitializeComponent();

            // Vincular el evento HelpRequested al formulario
            this.HelpRequested += FrmLogin_HelpRequested;

            // Inicializar mensajes de ayuda
            helpMessages = new Dictionary<Control, string>
            {
                { txtUsername, "Ingrese su nombre de usuario en este campo." },
                { txtPassword, "Ingrese su contraseña en este campo." },
                { btnLogin, "Presione este botón para iniciar sesión con las credenciales proporcionadas." }
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

            // Sincronizar las patentes con los formularios al iniciar el login
            PatenteHelper.SyncPatentesWithForms();

            // Establecer el foco en el TextBox del usuario
            txtUsername.Focus();
        }

        private void FrmLogin_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            // Mostrar una ventana emergente con explicaciones
            MessageBox.Show(
                "Instrucciones para iniciar sesión:\n\n" +
                "1. En el campo 'Usuario', escribe tu nombre de usuario.\n" +
                "2. En el campo 'Contraseña', introduce tu contraseña.\n" +
                "3. Haz clic en el botón 'Iniciar Sesión' para acceder al sistema.\n\n" +
                "Si tienes problemas, contacta al administrador del sistema.",
                "Ayuda - Inicio de Sesión",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void Control_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Control control && helpMessages.ContainsKey(control))
            {
                currentControl = control; // Guardar el control actual
                toolTipTimer.Start(); // Iniciar el temporizador
            }
        }

        private void Control_MouseLeave(object sender, EventArgs e)
        {
            toolTipTimer.Stop(); // Detener el temporizador
            currentControl = null; // Limpiar el control actual
        }

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
        /// Maneja el evento de clic del botón de inicio de sesión.
        /// </summary>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Obtener los datos del formulario
            string userName = txtUsername.Text;
            string password = txtPassword.Text;

            // Validar que los campos no estén vacíos
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Por favor, ingrese nombre de usuario y contraseña.", "Error de autenticación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Crear una instancia de Usuario
            Usuario user = new Usuario
            {
                UserName = userName,
                Password = password // La contraseña será encriptada en la lógica de negocio
            };

            // Intentar validar al usuario llamando a UserService.Validate
            user = UserService.Validate(user);

            // Manejar el resultado de la autenticación
            if (user != null && user.IdUsuario != Guid.Empty)
            {
                // Ocultar el formulario de login
                this.Hide();

                // Obtener los accesos del usuario (patentes, familias) usando la lógica del composite a través de la fachada
                Usuario usuarioAutenticado = UserService.GetById(user.IdUsuario);
                var accesos = usuarioAutenticado.Accesos;

                Form mainForm = null;

                // Verificar los permisos y mostrar el formulario correspondiente
                if (TienePermisoAdmin(accesos))
                {
                    mainForm = new frmMainAdmin();
                }
                else
                {
                    mainForm = new frmMainTenant(Guid.Parse(user.IdUsuario.ToString().ToUpper()));
                }

                try
                {
                    // Mostrar el formulario principal
                    if (mainForm != null)
                    {
                        mainForm.ShowDialog();
                    }
                }
                catch 
                {

                }
                finally
                {
                    txtUsername.Text = string.Empty;
                    txtPassword.Text = string.Empty;
                    // Mostrar nuevamente el formulario de login cuando el formulario principal se cierre
                    this.Show();
                }
            }
            else
            {
                MessageBox.Show("Nombre de usuario o contraseña incorrectos.", "Error de autenticación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Verifica si el usuario tiene permisos de administrador.
        /// </summary>
        /// <param name="accesos">La lista de accesos (patentes y familias) del usuario.</param>
        /// <returns>True si el usuario tiene permisos de administrador; de lo contrario, false.</returns>
        private bool TienePermisoAdmin(List<Acceso> accesos)
        {
            foreach (var acceso in accesos)
            {
                if (acceso is Patente patente && patente.Nombre == "frmMainAdmin")
                {
                    return true;
                }
                else if (acceso is Familia familiaAdmin && familiaAdmin.Nombre == "Admin")
                {
                    return true;
                }
                else if (acceso is Familia familiaTenant && familiaTenant.Nombre == "Tenant")
                {
                    return false;
                }
                else if (acceso is Patente patenteTenant && patenteTenant.Nombre == "frmMainTenant")
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Manejar el evento de tecla presionada en los TextBox para simular el clic del botón de inicio de sesión al presionar Enter.
        /// </summary>
        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Evitar que se reproduzca el sonido de "Enter"
                txtPassword.Focus(); // Enfocar el siguiente TextBox (contraseña)
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e) 
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Evitar que se reproduzca el sonido de "Enter"
                btnLogin.PerformClick(); // Simular el clic en el botón de inicio de sesión
            }
        }
    }
}
