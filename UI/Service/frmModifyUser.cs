using Services.Domain;
using Services.Facade;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UI.Service
{
    public partial class frmModifyUser : Form
    {
        private Usuario _selectedUser;

        public frmModifyUser()
        {
            InitializeComponent();
            btnSave.Enabled = false; // El botón de guardar empieza deshabilitado
            btnDelete.Enabled = false; // El botón de eliminar también empieza deshabilitado

            // Inicializar los ComboBox
            cmbFamily.Items.Clear();
            cmbPermissions.Items.Clear();

            // Asignar eventos para la validación de contraseñas
            txtPassword.TextChanged += ValidatePasswords;
            txtConfirmPassword.TextChanged += ValidatePasswords;

            // Asignar eventos para cargar datos en los ComboBox cuando se despliegan
            cmbFamily.DropDown += cmbFamily_DropDown;
            cmbPermissions.DropDown += cmbPermissions_DropDown;

            // Sin selección predeterminada
            cmbFamily.SelectedIndex = -1;
            cmbPermissions.SelectedIndex = -1;

            // Cargar usuarios en el DataGridView
            LoadUsers();

            // Vincular el evento de selección del DataGridView
            dgvUsers.SelectionChanged += dgvUsers_SelectionChanged;
        }

        /// <summary>
        /// Cargar usuarios en el DataGridView para su modificación, mostrando los nombres de roles y permisos.
        /// </summary>
        private void LoadUsers()
        {
            try
            {
                var users = UserService.GetAllUsers();

                var usersData = users.Select(user => new
                {
                    user.IdUsuario,
                    user.UserName,
                    Password = user.Password, // Mostrar la contraseña si es necesario
                    Roles = string.Join(", ", user.Accesos.OfType<Familia>().Select(f => f.Nombre)),
                    Permisos = string.Join(", ", user.Accesos.OfType<Patente>().Select(p => p.Nombre))
                }).ToList();

                dgvUsers.DataSource = usersData;

                dgvUsers.Columns["IdUsuario"].Visible = false;
                dgvUsers.Columns["UserName"].HeaderText = "Nombre de usuario";
                dgvUsers.Columns["Password"].HeaderText = "Contraseña";
                dgvUsers.Columns["Roles"].HeaderText = "Roles";
                dgvUsers.Columns["Permisos"].HeaderText = "Permisos";
                dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar usuarios: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Al seleccionar un usuario en el DataGridView, cargar sus datos en los campos de texto.
        /// </summary>
        private void dgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count > 0)
            {
                // Obtener el Id del usuario seleccionado
                var selectedUserId = (Guid)dgvUsers.SelectedRows[0].Cells["IdUsuario"].Value;

                // Obtener el usuario completo desde el servicio
                _selectedUser = UserService.GetById(selectedUserId);

                // Rellenar los campos con la información del usuario seleccionado
                txtUsername.Text = _selectedUser.UserName;
                txtPassword.Text = _selectedUser.Password;
                txtConfirmPassword.Text = _selectedUser.Password;

                // Llenar los roles y permisos actuales en los ComboBox
                LoadRolesAndPermissions(_selectedUser);

                // Habilitar el botón de guardar y eliminar
                btnSave.Enabled = true;
                btnDelete.Enabled = true;
            }
        }

        /// <summary>
        /// Cargar los roles y permisos del usuario seleccionado y mostrar los nombres en los ComboBox.
        /// </summary>
        private void LoadRolesAndPermissions(Usuario user)
        {
            // Cargar roles (familias)
            cmbFamily.Items.Clear();
            cmbFamily.Items.Add(new Familia { Nombre = "<Ninguna>", Id = Guid.Empty });

            var familias = UserService.GetAllFamilias();
            foreach (var familia in familias)
            {
                cmbFamily.Items.Add(familia);
            }

            // Configurar las propiedades DisplayMember y ValueMember para mostrar correctamente los nombres en el ComboBox
            cmbFamily.DisplayMember = "Nombre";
            cmbFamily.ValueMember = "Id";

            // Seleccionar el rol del usuario en el ComboBox
            if (user.Accesos.Exists(a => a is Familia))
            {
                var userFamily = (Familia)user.Accesos.Find(a => a is Familia);
                cmbFamily.SelectedItem = familias.FirstOrDefault(f => f.Id == userFamily.Id);
            }

            // Cargar permisos (patentes)
            cmbPermissions.Items.Clear();
            cmbPermissions.Items.Add(new Patente { Nombre = "<Ninguna>", Id = Guid.Empty });

            var patentes = UserService.GetAllPatentes();
            foreach (var patente in patentes)
            {
                cmbPermissions.Items.Add(patente);
            }

            // Configurar las propiedades DisplayMember y ValueMember para mostrar correctamente los nombres en el ComboBox
            cmbPermissions.DisplayMember = "Nombre";
            cmbPermissions.ValueMember = "Id";

            // Seleccionar el permiso del usuario en el ComboBox
            if (user.Accesos.Exists(a => a is Patente))
            {
                var userPatente = (Patente)user.Accesos.Find(a => a is Patente);
                cmbPermissions.SelectedItem = patentes.FirstOrDefault(p => p.Id == userPatente.Id);
            }
        }

        /// <summary>
        /// Cargar roles (familias) cuando se despliega el ComboBox.
        /// </summary>
        private void cmbFamily_DropDown(object sender, EventArgs e)
        {
            if (cmbFamily.Items.Count == 0)
            {
                LoadFamilias();
            }
        }

        /// <summary>
        /// Cargar permisos (patentes) cuando se despliega el ComboBox.
        /// </summary>
        private void cmbPermissions_DropDown(object sender, EventArgs e)
        {
            if (cmbPermissions.Items.Count == 0)
            {
                LoadPermisos();
            }
        }

        /// <summary>
        /// Cargar permisos disponibles en el ComboBox.
        /// </summary>
        private void LoadPermisos()
        {
            cmbPermissions.Items.Clear();
            cmbPermissions.Items.Add(new Patente { Nombre = "<Ninguna>", Id = Guid.Empty });

            var patentes = UserService.GetAllPatentes();
            foreach (var patente in patentes)
            {
                cmbPermissions.Items.Add(patente);
            }

            cmbPermissions.DisplayMember = "Nombre"; // Mostrar el nombre en el ComboBox
            cmbPermissions.ValueMember = "Id"; // Usar el ID para la selección
        }

        /// <summary>
        /// Validar que las contraseñas ingresadas coincidan.
        /// </summary>
        private void ValidatePasswords(object sender, EventArgs e)
        {
            if (txtPassword.Text == txtConfirmPassword.Text)
            {
                txtPassword.BackColor = SystemColors.Window;
                txtConfirmPassword.BackColor = SystemColors.Window;
                btnSave.Enabled = true;
            }
            else
            {
                txtPassword.BackColor = Color.Red;
                txtConfirmPassword.BackColor = Color.Red;
                btnSave.Enabled = false;
            }
        }

        /// <summary>
        /// Guardar los cambios realizados al usuario.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Actualizar los datos del usuario seleccionado
                _selectedUser.UserName = txtUsername.Text;
                _selectedUser.Password = txtPassword.Text; // Actualiza la contraseña si es necesario

                // Asignar roles y permisos seleccionados
                var selectedFamilia = (Familia)cmbFamily.SelectedItem;
                var selectedPatente = (Patente)cmbPermissions.SelectedItem;

                _selectedUser.Accesos.Clear();
                if (selectedFamilia != null && selectedFamilia.Id != Guid.Empty)
                {
                    _selectedUser.Accesos.Add(selectedFamilia);
                }
                if (selectedPatente != null && selectedPatente.Id != Guid.Empty)
                {
                    _selectedUser.Accesos.Add(selectedPatente);
                }

                // Guardar los cambios en la base de datos
                UserService.Update(_selectedUser);

                MessageBox.Show("Usuario actualizado con éxito.", "Modificación", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Recargar el DataGridView para mostrar los cambios
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar el usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Eliminar el usuario seleccionado.
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // Confirmar eliminación
                var confirmResult = MessageBox.Show("¿Está seguro de que desea eliminar este usuario?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmResult == DialogResult.Yes)
                {
                    // Eliminar el usuario
                    UserService.Delete(_selectedUser.IdUsuario);

                    MessageBox.Show("Usuario eliminado con éxito.", "Eliminación", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Recargar el DataGridView para reflejar los cambios
                    LoadUsers();

                    // Limpiar los campos y deshabilitar botones
                    txtUsername.Clear();
                    txtPassword.Clear();
                    txtConfirmPassword.Clear();
                    btnSave.Enabled = false;
                    btnDelete.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar el usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        /// <summary>
        /// Carga las familias disponibles en el ComboBox correspondiente.
        /// </summary>
        private void LoadFamilias()
        {
            if (cmbFamily.Items.Count == 0)
            {
                // Añadir un elemento vacío para permitir no seleccionar nada
                cmbFamily.Items.Add(new Familia { Nombre = "<Ninguna>", Id = Guid.Empty });

                var familias = UserService.GetAllFamilias();
                foreach (var familia in familias)
                {
                    cmbFamily.Items.Add(familia);
                }

                cmbFamily.DisplayMember = "Nombre";
                cmbFamily.ValueMember = "Id";
                cmbFamily.SelectedIndex = -1; // Sin selección predeterminada
            }
        }
    }
}