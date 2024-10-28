using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Services.Facade; // Asegúrate de que esta ruta sea correcta
using Domain;
using Services.Domain;

namespace UI.Admin
{
    public partial class frmUsers : Form
    {
        public Guid SelectedUserId { get; private set; } // Propiedad para almacenar el ID seleccionado

        public frmUsers()
        {
            InitializeComponent();
            LoadUsers(); // Cargar usuarios al inicializar el formulario
            dgvUsers.CellDoubleClick += dgvUsers_CellDoubleClick; // Vincular el evento de doble clic
        }

        private void LoadUsers()
        {
            // Llama al método estático para obtener la lista de usuarios
            List<Usuario> users = UserService.GetAllUsers();
            dgvUsers.DataSource = users;

            // Configura las columnas a mostrar
            dgvUsers.Columns["IdUsuario"].HeaderText = "ID de Usuario";
            dgvUsers.Columns["UserName"].HeaderText = "Nombre de Usuario";
            dgvUsers.Columns["Password"].Visible = false; // Ocultar columna de contraseña si existe
        }

        private void dgvUsers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Obtener el ID del usuario seleccionado al hacer doble clic
                SelectedUserId = (Guid)dgvUsers.Rows[e.RowIndex].Cells["IdUsuario"].Value;
                this.DialogResult = DialogResult.OK; // Indica que se seleccionó un usuario
                this.Close(); // Cierra el formulario
            }
        }
    }
}
