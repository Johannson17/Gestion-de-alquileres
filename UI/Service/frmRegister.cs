using Services.Domain;
using Services.Facade;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using UI.Helpers;

namespace UI
{
    /// <summary>
    /// User registration form. Allows registering a new user in the system, assigning roles and permissions.
    /// </summary>
    public partial class frmRegister : Form
    {
        private readonly Dictionary<Control, string> helpMessages; // Diccionario de ayuda
        private Timer toolTipTimer; // Temporizador para mostrar ToolTip
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

            // Inicializar mensajes de ayuda
            helpMessages = new Dictionary<Control, string>
            {
                { txtUsername, "Ingrese el nombre de usuario." },
                { txtPassword, "Ingrese la contraseña del usuario." },
                { txtConfirmPassword, "Confirme la contraseña ingresada." },
                { cmbFamilias, "Seleccione un rol para el usuario." },
                { cmbPatentes, "Seleccione un permiso para el usuario." },
                { btnAddFamilia, "Permite agregar un nuevo rol al sistema." },
                { btnRegister, "Registra al usuario con los datos proporcionados." }
            };

            // Configurar el Timer
            toolTipTimer = new Timer();
            toolTipTimer.Interval = 1000; // 2 segundos
            toolTipTimer.Tick += ToolTipTimer_Tick;

            // Asignar eventos a los controles
            foreach (var control in helpMessages.Keys)
            {
                control.MouseEnter += Control_MouseEnter;
                control.MouseLeave += Control_MouseLeave;
            }
        }

        /// <summary>
        /// Maneja el evento de MouseEnter en los controles para iniciar el temporizador.
        /// </summary>
        private void Control_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Control control && helpMessages.ContainsKey(control))
            {
                currentControl = control; // Guardar el control actual
                toolTipTimer.Start(); // Iniciar el temporizador
            }
        }

        /// <summary>
        /// Maneja el evento de MouseLeave para detener el temporizador.
        /// </summary>
        private void Control_MouseLeave(object sender, EventArgs e)
        {
            toolTipTimer.Stop(); // Detener el temporizador
            currentControl = null; // Limpiar el control actual
        }

        /// <summary>
        /// Muestra el ToolTip cuando se cumple el tiempo del temporizador.
        /// </summary>
        private void ToolTipTimer_Tick(object sender, EventArgs e)
        {
            if (currentControl != null && helpMessages.ContainsKey(currentControl))
            {
                ToolTip toolTip = new ToolTip();
                toolTip.Show(helpMessages[currentControl], currentControl, 3000); // Mostrar durante 3 segundos
            }

            toolTipTimer.Stop(); // Detener el temporizador después de mostrar el ToolTip
        }

        /// <summary>
        /// Load families when the family ComboBox is dropped down.
        /// </summary>
        private void cmbFamilias_DropDown(object sender, EventArgs e)
        {
            if (cmbFamilias.Items.Count == 0)
            {
                LoadFamilias();
            }
        }

        /// <summary>
        /// Load permissions when the permissions ComboBox is dropped down.
        /// </summary>
        private void cmbPatentes_DropDown(object sender, EventArgs e)
        {
            if (cmbPatentes.Items.Count == 0)
            {
                LoadPatentes();
            }
        }

        /// <summary>
        /// Load available families into the corresponding ComboBox.
        /// </summary>
        private void LoadFamilias()
        {
            if (cmbFamilias.Items.Count == 0)
            {
                // Add an empty item to allow no selection
                cmbFamilias.Items.Add(new Familia { Nombre = "<None>", Id = Guid.Empty });

                var familias = UserService.GetAllFamilias();
                foreach (var familia in familias)
                {
                    cmbFamilias.Items.Add(familia);
                }

                cmbFamilias.DisplayMember = "Nombre";
                cmbFamilias.ValueMember = "Id";
                cmbFamilias.SelectedIndex = -1; // No default selection
            }
        }

        /// <summary>
        /// Load available permissions into the corresponding ComboBox.
        /// </summary>
        private void LoadPatentes()
        {
            if (cmbPatentes.Items.Count == 0)
            {
                // Add an empty item to allow no selection
                cmbPatentes.Items.Add(new Patente { Nombre = "<None>", Id = Guid.Empty });

                var patentes = UserService.GetAllPatentes();
                foreach (var patente in patentes)
                {
                    cmbPatentes.Items.Add(patente);
                }

                cmbPatentes.DisplayMember = "Nombre";
                cmbPatentes.ValueMember = "Id";
                cmbPatentes.SelectedIndex = -1; // No default selection
            }
        }

        /// <summary>
        /// Validate that the passwords entered in the text fields match.
        /// If they match, enable the register button; otherwise, disable it and highlight the fields in red.
        /// </summary>
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

        /// <summary>
        /// Handles the click event of the register button. Attempts to register a new user with the provided data.
        /// </summary>
        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                // Create a new User instance
                var user = new Usuario
                {
                    UserName = txtUsername.Text,
                    Password = txtPassword.Text // Password will be encrypted in logic
                };

                // Assign selected roles and permissions to the user
                var selectedFamilia = (Familia)cmbFamilias.SelectedItem;
                var selectedPatente = (Patente)cmbPatentes.SelectedItem;

                // Check if at least one of the two is selected
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
                    MessageBox.Show("You must select at least one role or permission.");
                    return;
                }

                if (selectedFamilia != null)
                {
                    user.Accesos.Add(selectedFamilia);
                }

                if (selectedPatente != null)
                {
                    user.Accesos.Add(selectedPatente);
                }

                // Register the user in the database using backend logic through the facade
                UserService.Register(user);

                // Show a success message
                MessageBox.Show("User registered successfully.", "Registration", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Open the main administrator form
                frmMainAdmin mainForm = new frmMainAdmin();
                mainForm.Show();

                // Close the registration form
                this.Close();
            }
            catch (Exception ex)
            {
                // Exception handling
                MessageBox.Show($"Error registering the user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
