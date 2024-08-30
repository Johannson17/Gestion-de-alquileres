using Services.Domain;
using Services.Facade;
using System;
using System.Drawing;
using System.Windows.Forms;
using UI.Helpers;

namespace UI
{
    /// <summary>
    /// Formulario de registro de usuarios. Permite registrar un nuevo usuario en el sistema, asignándole patentes y familias.
    /// </summary>
    public partial class frmRegister : Form
    {
        /// <summary>
        /// Constructor que inicializa el formulario de registro de usuarios.
        /// </summary>
        public frmRegister()
        {
            InitializeComponent();
            btnRegister.Enabled = false; // El botón de registro comienza deshabilitado

            // Asignar eventos para la validación en tiempo real
            txtPassword.TextChanged += ValidatePasswords;
            txtConfirmPassword.TextChanged += ValidatePasswords;

            InitializeComboBoxes(); // Inicializar ComboBoxes de Familias y Patentes

            // Sincronizar las patentes con los formularios al iniciar el login
            PatenteHelper.SyncPatentesWithForms();
        }

        /// <summary>
        /// Inicializa los ComboBoxes de Familias y Patentes cargando los datos desde el backend.
        /// </summary>
        private void InitializeComboBoxes()
        {
            LoadFamilias();
            LoadPatentes();
        }

        /// <summary>
        /// Carga las familias disponibles en el ComboBox correspondiente.
        /// </summary>
        private void LoadFamilias()
        {
            // Obtén las familias desde la lógica del backend a través de la fachada
            var familias = UserService.GetAllFamilias();

            // Poblar cmbFamilias
            cmbFamilias.DataSource = familias;
            cmbFamilias.DisplayMember = "Nombre";  // Suponiendo que Familia tiene una propiedad llamada 'Nombre'
            cmbFamilias.ValueMember = "Id";        // Suponiendo que Familia tiene una propiedad llamada 'Id'
        }

        /// <summary>
        /// Carga las patentes disponibles en el ComboBox correspondiente.
        /// </summary>
        private void LoadPatentes()
        {
            // Obtén las patentes desde la lógica del backend a través de la fachada
            var patentes = UserService.GetAllPatentes();

            // Poblar cmbPatentes
            cmbPatentes.DataSource = patentes;
            cmbPatentes.DisplayMember = "Nombre";  // Suponiendo que Patente tiene una propiedad llamada 'Nombre'
            cmbPatentes.ValueMember = "Id";        // Suponiendo que Patente tiene una propiedad llamada 'Id'
        }

        /// <summary>
        /// Valida que las contraseñas ingresadas en los campos de texto coincidan.
        /// Si coinciden, habilita el botón de registro; de lo contrario, lo deshabilita y resalta los campos en rojo.
        /// </summary>
        private void ValidatePasswords(object sender, EventArgs e)
        {
            if (txtPassword.Text == txtConfirmPassword.Text)
            {
                // Si las contraseñas coinciden, establecer el color de fondo normal y habilitar el botón de registro
                txtPassword.BackColor = SystemColors.Window;
                txtConfirmPassword.BackColor = SystemColors.Window;
                btnRegister.Enabled = true;
            }
            else
            {
                // Si no coinciden, establecer el color de fondo rojo y deshabilitar el botón de registro
                txtPassword.BackColor = Color.Red;
                txtConfirmPassword.BackColor = Color.Red;
                btnRegister.Enabled = false;
            }
        }

        /// <summary>
        /// Maneja el evento de clic del botón de registro. Intenta registrar un nuevo usuario con los datos proporcionados.
        /// </summary>
        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                // Crear una nueva instancia de Usuario
                var user = new Usuario
                {
                    UserName = txtUsername.Text,
                    Password = txtPassword.Text // La contraseña se encriptará en la lógica
                };

                // Asignar Familias y Patentes seleccionadas al usuario
                var selectedFamilia = (Familia)cmbFamilias.SelectedItem;
                var selectedPatente = (Patente)cmbPatentes.SelectedItem;

                if (selectedFamilia != null)
                {
                    user.Accesos.Add(selectedFamilia);
                }

                if (selectedPatente != null)
                {
                    user.Accesos.Add(selectedPatente);
                }

                // Registrar el usuario en la base de datos usando la lógica del backend a través de la fachada
                UserService.Register(user);

                // Mostrar un mensaje de éxito
                MessageBox.Show("Usuario registrado con éxito.", "Registro", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Abrir el formulario principal de administrador
                frmMainAdmin mainForm = new frmMainAdmin();
                mainForm.Show();

                // Cerrar el formulario de registro
                this.Close();
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                MessageBox.Show($"Error al registrar el usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Maneja el evento de clic del botón para añadir una nueva Familia. 
        /// Abre un subformulario para agregar una nueva Familia.
        /// </summary>
        private void btnAddFamilia_Click(object sender, EventArgs e)
        {
            using (frmAddFamilia addFamiliaForm = new frmAddFamilia())
            {
                if (addFamiliaForm.ShowDialog() == DialogResult.OK)
                {
                    // Actualizar el ComboBox de familias
                    LoadFamilias();
                }
            }
        }
    }
}