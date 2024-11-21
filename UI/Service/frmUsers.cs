using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Services.Facade;
using Domain;
using Services.Domain;

namespace UI.Admin
{
    public partial class frmUsers : Form
    {
        public Guid SelectedUserId { get; private set; } // Property to store the selected User ID

        public frmUsers()
        {
            InitializeComponent();
            LoadUsers(); // Load users when the form is initialized
            dgvUsers.CellDoubleClick += dgvUsers_CellDoubleClick; // Link the double-click event
        }

        private void LoadUsers()
        {
            try
            {
                // Call the static method to get the list of users
                List<Usuario> users = UserService.GetAllUsers();
                dgvUsers.DataSource = users;

                // Configure the columns to display
                dgvUsers.Columns["IdUsuario"].HeaderText = LanguageService.Translate("ID de Usuario");
                dgvUsers.Columns["UserName"].HeaderText = LanguageService.Translate("Nombre de Usuario");
                dgvUsers.Columns["Password"].Visible = false; // Hide password column if it exists

                dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Adjust column sizes
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar los usuarios") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void dgvUsers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    // Get the selected user's ID on double-click
                    SelectedUserId = (Guid)dgvUsers.Rows[e.RowIndex].Cells["IdUsuario"].Value;
                    this.DialogResult = DialogResult.OK; // Indicate a user was selected
                    this.Close(); // Close the form
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        LanguageService.Translate("Error al seleccionar el usuario") + ": " + ex.Message,
                        LanguageService.Translate("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }
    }
}