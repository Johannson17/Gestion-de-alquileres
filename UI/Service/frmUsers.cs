using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Services.Facade;
using Domain;
using Services.Domain;

namespace UI.Admin
{
    public partial class frmUsers : Form
    {
        public Guid SelectedUserId { get; private set; } // Property to store the selected User ID

        private readonly Dictionary<Control, string> helpMessages; // Diccionario para mensajes de ayuda
        private Timer toolTipTimer; // Temporizador para mostrar ToolTips
        private Control currentControl; // Control actual donde está el mouse

        public frmUsers()
        {
            InitializeComponent();
            LoadUsers(); // Load users when the form is initialized

            dgvUsers.CellDoubleClick += dgvUsers_CellDoubleClick; // Link the double-click event

            // Inicializar mensajes de ayuda
            helpMessages = new Dictionary<Control, string>
            {
                { dgvUsers, "Seleccione un usuario haciendo doble clic para continuar." }
            };

            // Configurar el Timer
            toolTipTimer = new Timer();
            toolTipTimer.Interval = 1000; // 2 segundos
            toolTipTimer.Tick += ToolTipTimer_Tick;

            // Asignar eventos de ayuda a los controles
            foreach (var control in helpMessages.Keys)
            {
                control.MouseEnter += Control_MouseEnter;
                control.MouseLeave += Control_MouseLeave;
            }
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
                ToolTip toolTip = new ToolTip();
                toolTip.Show(helpMessages[currentControl], currentControl, 3000); // Mostrar durante 3 segundos
            }

            toolTipTimer.Stop(); // Detener el temporizador después de mostrar el ToolTip
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

        private void dgvUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Método generado automáticamente, sin lógica adicional.
        }
    }
}
