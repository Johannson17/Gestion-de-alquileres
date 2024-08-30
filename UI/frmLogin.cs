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
            bool isValid = UserService.Validate(user);

            // Manejar el resultado de la autenticación
            if (isValid)
            {
                MessageBox.Show("Inicio de sesión exitoso.", "Bienvenido", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Obtener los accesos del usuario (patentes, familias) usando la lógica del composite a través de la fachada
                Usuario usuarioAutenticado = UserService.GetByUserName(userName);
                var accesos = usuarioAutenticado.Accesos;

                // Verificar los permisos y mostrar el formulario correspondiente
                if (TienePermisoAdmin(accesos))
                {
                    // Mostrar formulario de administración
                    var adminForm = new frmMainAdmin();
                    adminForm.Show();
                }
                else if (TienePermisoUsuarioEstandar(accesos))
                {
                    // Mostrar formulario de usuario estándar
                    var standardForm = new frmMainAdmin();
                    standardForm.Show(); // Puedes reemplazar esto con un formulario específico para usuarios estándar
                }
                else
                {
                    // Si el usuario no tiene permisos, mostrar un mensaje
                    MessageBox.Show("No tiene permisos para acceder a esta aplicación.", "Permisos insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                this.Hide(); // Ocultar el formulario de login
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
            }
            return false;
        }

        /// <summary>
        /// Verifica si el usuario tiene permisos de usuario estándar.
        /// </summary>
        /// <param name="accesos">La lista de accesos (patentes y familias) del usuario.</param>
        /// <returns>True si el usuario tiene permisos de usuario estándar; de lo contrario, false.</returns>
        private bool TienePermisoUsuarioEstandar(List<Acceso> accesos)
        {
            foreach (var acceso in accesos)
            {
                if (acceso is Familia familia && familia.Nombre == "UsuarioEstandar")
                {
                    return true;
                }
            }
            return false;
        }
    }
}