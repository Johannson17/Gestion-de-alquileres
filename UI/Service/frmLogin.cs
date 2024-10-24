using Services.Domain;
using Services.Facade;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using UI.Helpers;

namespace UI
{
    /// <summary>
    /// Formulario de inicio de sesión. Permite la autenticación de usuarios y la apertura del formulario correspondiente según los permisos del usuario.
    /// </summary>
    public partial class frmLogin : Form
    {
        /// <summary>
        /// Inicializa una nueva instancia del formulario de inicio de sesión.
        /// </summary>
        public frmLogin()
        {
            InitializeComponent();
            // Sincronizar las patentes con los formularios al iniciar el login
            PatenteHelper.SyncPatentesWithForms();
            
            // Establecer el foco en el TextBox del usuario
            txtUsername.Focus();
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
                    mainForm = new frmMainAdmin(); // Aquí deberías abrir el formulario para usuarios estándar
                }

                // Mostrar el formulario principal
                if (mainForm != null)
                {
                    mainForm.ShowDialog();
                }

                // Mostrar nuevamente el formulario de login cuando el formulario principal se cierre
                this.Show();
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
                if (acceso is Patente patente && patente.Nombre == "Admin")
                {
                    return true;
                }
                else if (acceso is Familia familia && familia.Nombre == "Admin")
                {
                    return true;
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
