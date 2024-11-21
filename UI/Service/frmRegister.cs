using Services.Domain;
using Services.Facade;
using System;
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
        /// <summary>
        /// Constructor that initializes the user registration form.
        /// </summary>
        public frmRegister()
        {
            InitializeComponent();
            btnRegister.Enabled = false; // The register button starts disabled

            // Initialize ComboBoxes as empty
            cmbFamilias.Items.Clear();
            cmbPatentes.Items.Clear();

            // Assign events for real-time validation
            txtPassword.TextChanged += ValidatePasswords;
            txtConfirmPassword.TextChanged += ValidatePasswords;

            // Assign events to load data when ComboBoxes are dropped down
            cmbFamilias.DropDown += cmbFamilias_DropDown;
            cmbPatentes.DropDown += cmbPatentes_DropDown;

            // Set ComboBoxes with no initial selection
            cmbFamilias.SelectedIndex = -1;
            cmbPatentes.SelectedIndex = -1;
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
