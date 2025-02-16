using Services.Domain;
using Services.Facade;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UI.Service
{
    public partial class frmModifyUser : Form
    {
        private Usuario _selectedUser;
        private Dictionary<Control, string> helpMessages;
        private Timer toolTipTimer;
        private Control currentControl;

        public frmModifyUser()
        {
            InitializeComponent();
            btnSave.Enabled = false; // El botón de guardar empieza deshabilitado
            btnDelete.Enabled = false; // El botón de eliminar también empieza deshabilitado

            InitializeHelpMessages();
            SubscribeHelpMessagesEvents();

            // Configurar el Timer para el tooltip
            toolTipTimer = new Timer { Interval = 1000 }; // 2 segundos
            toolTipTimer.Tick += ToolTipTimer_Tick;

            // Asignar eventos MouseEnter y MouseLeave a los controles
            foreach (var control in helpMessages.Keys)
            {
                control.MouseEnter += Control_MouseEnter;
                control.MouseLeave += Control_MouseLeave;
            }

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
        /// Inicializa los mensajes de ayuda con traducción dinámica.
        /// </summary>
        private void InitializeHelpMessages()
        {
            helpMessages = new Dictionary<Control, string>
            {
                { txtUsername, LanguageService.Translate("Ingrese el nombre de usuario.") },
                { txtPassword, LanguageService.Translate("Ingrese la contraseña del usuario.") },
                { txtConfirmPassword, LanguageService.Translate("Confirme la contraseña ingresada.") },
                { cmbFamily, LanguageService.Translate("Seleccione un rol para el usuario.") },
                { cmbPermissions, LanguageService.Translate("Seleccione los permisos del usuario.") },
                { btnSave, LanguageService.Translate("Guarde los cambios realizados en el usuario.") },
                { btnDelete, LanguageService.Translate("Elimine el usuario seleccionado.") },
                { btnAddFamilia, LanguageService.Translate("Agregue un nuevo rol al sistema.") }
            };
        }

        /// <summary>
        /// Suscribe los eventos `MouseEnter` y `MouseLeave` a los controles con mensajes de ayuda.
        /// </summary>
        private void SubscribeHelpMessagesEvents()
        {
            if (helpMessages != null)
            {
                foreach (var control in helpMessages.Keys)
                {
                    control.MouseEnter += Control_MouseEnter;
                    control.MouseLeave += Control_MouseLeave;
                }
            }
        }

        private void Control_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Control control && helpMessages.ContainsKey(control))
            {
                currentControl = control;
                toolTipTimer.Start();
            }
        }

        private void Control_MouseLeave(object sender, EventArgs e)
        {
            toolTipTimer.Stop();
            currentControl = null;
        }

        private void ToolTipTimer_Tick(object sender, EventArgs e)
        {
            if (currentControl != null && helpMessages.ContainsKey(currentControl))
            {
                ToolTip toolTip = new ToolTip();
                toolTip.Show(helpMessages[currentControl], currentControl, 3000);
            }
            toolTipTimer.Stop();
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
                dgvUsers.Columns["UserName"].HeaderText = LanguageService.Translate("Nombre de usuario");
                dgvUsers.Columns["Password"].HeaderText = LanguageService.Translate("Contraseña");
                dgvUsers.Columns["Roles"].HeaderText = LanguageService.Translate("Roles");
                dgvUsers.Columns["Permisos"].HeaderText = LanguageService.Translate("Permisos");
                dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar usuarios:") + " " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
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

                MessageBox.Show(
                    LanguageService.Translate("Usuario actualizado con éxito."),
                    LanguageService.Translate("Modificación"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // Recargar el DataGridView para mostrar los cambios
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al actualizar el usuario:") + " " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
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
                var confirmResult = MessageBox.Show(
                    LanguageService.Translate("¿Está seguro de que desea eliminar este usuario?"),
                    LanguageService.Translate("Confirmar eliminación"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (confirmResult == DialogResult.Yes)
                {
                    // Eliminar el usuario
                    UserService.Delete(_selectedUser.IdUsuario);

                    MessageBox.Show(
                        LanguageService.Translate("Usuario eliminado con éxito."),
                        LanguageService.Translate("Eliminación"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

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
                MessageBox.Show(
                    LanguageService.Translate("Error al eliminar el usuario:") + " " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Actualiza las ayudas cuando se cambia el idioma.
        /// </summary>
        public void UpdateHelpMessages()
        {
            InitializeHelpMessages();
        }

        private void FrmModifyUser_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helpMessage = string.Join(Environment.NewLine, new[]
            {
                LanguageService.Translate("Bienvenido al módulo de gestión de usuarios."),
                "",
                LanguageService.Translate("Opciones disponibles:"),
                $"- {LanguageService.Translate("Seleccionar un usuario de la lista para modificar o eliminar.")}",
                $"- {LanguageService.Translate("Modificar el nombre de usuario y su contraseña.")}",
                $"- {LanguageService.Translate("Asignar roles y permisos al usuario.")}",
                $"- {LanguageService.Translate("Presionar el botón 'Guardar' para actualizar los cambios.")}",
                $"- {LanguageService.Translate("Presionar el botón 'Eliminar' para eliminar el usuario seleccionado.")}",
                "",
                LanguageService.Translate("Para más ayuda, contacte con el administrador.")
            });

            MessageBox.Show(helpMessage,
                            LanguageService.Translate("Ayuda del sistema"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
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