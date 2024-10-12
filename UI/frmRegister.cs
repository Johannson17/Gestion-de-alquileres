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

            // Inicializar los ComboBox como vacíos
            cmbFamilias.Items.Clear();
            cmbPatentes.Items.Clear();

            // Asignar eventos para la validación en tiempo real
            txtPassword.TextChanged += ValidatePasswords;
            txtConfirmPassword.TextChanged += ValidatePasswords;

            // Asignar eventos para cargar los datos cuando se despliegan los ComboBoxes
            cmbFamilias.DropDown += cmbFamilias_DropDown;
            cmbPatentes.DropDown += cmbPatentes_DropDown;

            // Establecer los ComboBox sin ninguna selección inicial
            cmbFamilias.SelectedIndex = -1;
            cmbPatentes.SelectedIndex = -1;
        }

        /// <summary>
        /// Cargar familias cuando se despliega el ComboBox de familias.
        /// </summary>
        private void cmbFamilias_DropDown(object sender, EventArgs e)
        {
            if (cmbFamilias.Items.Count == 0)
            {
                LoadFamilias();
            }
        }

        /// <summary>
        /// Cargar patentes cuando se despliega el ComboBox de patentes.
        /// </summary>
        private void cmbPatentes_DropDown(object sender, EventArgs e)
        {
            if (cmbPatentes.Items.Count == 0)
            {
                LoadPatentes();
            }
        }

        /// <summary>
        /// Carga las familias disponibles en el ComboBox correspondiente.
        /// </summary>
        private void LoadFamilias()
        {
            if (cmbFamilias.Items.Count == 0)
            {
                // Añadir un elemento vacío para permitir no seleccionar nada
                cmbFamilias.Items.Add(new Familia { Nombre = "<Ninguna>", Id = Guid.Empty });

                var familias = UserService.GetAllFamilias();
                foreach (var familia in familias)
                {
                    cmbFamilias.Items.Add(familia);
                }

                cmbFamilias.DisplayMember = "Nombre";
                cmbFamilias.ValueMember = "Id";
                cmbFamilias.SelectedIndex = -1; // Sin selección predeterminada
            }
        }

        /// <summary>
        /// Carga las patentes disponibles en el ComboBox correspondiente.
        /// </summary>
        private void LoadPatentes()
        {
            if (cmbPatentes.Items.Count == 0)
            {
                // Añadir un elemento vacío para permitir no seleccionar nada
                cmbPatentes.Items.Add(new Patente { Nombre = "<Ninguna>", Id = Guid.Empty });

                var patentes = UserService.GetAllPatentes();
                foreach (var patente in patentes)
                {
                    cmbPatentes.Items.Add(patente);
                }

                cmbPatentes.DisplayMember = "Nombre";
                cmbPatentes.ValueMember = "Id";
                cmbPatentes.SelectedIndex = -1; // Sin selección predeterminada
            }
        }

        /// <summary>
        /// Valida que las contraseñas ingresadas en los campos de texto coincidan.
        /// Si coinciden, habilita el botón de registro; de lo contrario, lo deshabilita y resalta los campos en rojo.
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

                // Comprobar si al menos uno de los dos está seleccionado
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
                    MessageBox.Show("Debe seleccionar al menos un rol o un permiso.");
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