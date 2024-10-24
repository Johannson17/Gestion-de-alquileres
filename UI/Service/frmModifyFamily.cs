using Services.Domain;
using Services.Facade;
using System;
using System.Linq;
using System.Windows.Forms;

namespace UI.Service
{
    public partial class frmModifyFamily : Form
    {
        private Familia _selectedFamily;

        public frmModifyFamily()
        {
            InitializeComponent();
            LoadFamilies(); // Cargar todas las familias en el DataGridView

            // Vincular el evento de selección en el DataGridView
            dgvFamilies.SelectionChanged += dgvFamilies_SelectionChanged;
        }

        /// <summary>
        /// Cargar todas las familias en el DataGridView.
        /// </summary>
        private void LoadFamilies()
        {
            var familias = UserService.GetAllFamilias();

            // Crear una lista anónima para mostrar los datos de las familias con las patentes concatenadas
            var familiasData = familias.Select(f => new
            {
                f.Id,
                f.Nombre,
                Patentes = string.Join(", ", f.Accesos.OfType<Patente>().Select(p => p.Nombre)) // Concatenar los nombres de las patentes
            }).ToList();

            // Asignar la lista de familias al DataGridView
            dgvFamilies.DataSource = familiasData;

            // Ajustar las columnas del DataGridView
            dgvFamilies.Columns["Id"].Visible = false; // Ocultar la columna de Id
            dgvFamilies.Columns["Nombre"].HeaderText = "Nombre de Familia";
            dgvFamilies.Columns["Patentes"].HeaderText = "Permisos (Patentes)";
        }

        /// <summary>
        /// Al seleccionar una fila en el DataGridView, cargar los datos de la familia en los TextBox.
        /// </summary>
        private void dgvFamilies_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvFamilies.SelectedRows.Count > 0)
            {
                // Obtener el Id de la familia seleccionada
                var selectedFamilyId = (Guid)dgvFamilies.SelectedRows[0].Cells["Id"].Value;

                // Buscar la familia seleccionada en la lista cargada
                _selectedFamily = UserService.GetAllFamilias().FirstOrDefault(f => f.Id == selectedFamilyId);

                // Rellenar los campos de texto con la información de la familia seleccionada
                if (_selectedFamily != null)
                {
                    txtName.Text = _selectedFamily.Nombre;

                    // Cargar accesos (patentes y otras familias)
                    LoadAccesos(_selectedFamily);
                }
            }
        }

        /// <summary>
        /// Cargar los accesos (patentes y familias) en el CheckedListBox y marcar los accesos de la familia seleccionada.
        /// </summary>
        private void LoadAccesos(Familia family)
        {
            // Configurar DisplayMember para mostrar los nombres
            chlbAccesos.DisplayMember = "Nombre";

            // Limpiar la lista de accesos
            chlbAccesos.Items.Clear();

            // Cargar patentes
            var patentes = UserService.GetAllPatentes();
            foreach (var patente in patentes)
            {
                bool isChecked = family.Accesos.Any(a => a is Patente && a.Id == patente.Id);
                chlbAccesos.Items.Add(patente, isChecked);
            }

            // Cargar otras familias (excluyendo la misma familia)
            var familias = UserService.GetAllFamilias();
            foreach (var otraFamilia in familias)
            {
                if (otraFamilia.Id != family.Id) // Evitar añadir la misma familia
                {
                    bool isChecked = family.Accesos.Any(a => a is Familia && a.Id == otraFamilia.Id);
                    chlbAccesos.Items.Add(otraFamilia, isChecked);
                }
            }
        }

        /// <summary>
        /// Guardar los cambios realizados a la familia seleccionada.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedFamily != null)
                {
                    // Actualizar los datos de la familia seleccionada
                    _selectedFamily.Nombre = txtName.Text;

                    // Limpiar los accesos actuales y agregar los seleccionados
                    _selectedFamily.Accesos.Clear();
                    for (int i = 0; i < chlbAccesos.CheckedItems.Count; i++)
                    {
                        var acceso = chlbAccesos.CheckedItems[i] as Acceso;
                        if (acceso != null)
                        {
                            _selectedFamily.Add(acceso);
                        }
                    }

                    // Guardar los cambios en la base de datos
                    UserService.UpdateFamilia(_selectedFamily);

                    // Mostrar un mensaje de éxito
                    MessageBox.Show("Familia modificada con éxito.", "Modificación de Familia", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Recargar el DataGridView para mostrar los cambios
                    LoadFamilies();
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                MessageBox.Show($"Error al modificar la familia: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Eliminar la familia seleccionada.
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedFamily != null)
                {
                    // Confirmar la eliminación
                    var confirmResult = MessageBox.Show("¿Está seguro de que desea eliminar esta familia?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (confirmResult == DialogResult.Yes)
                    {
                        // Eliminar la familia en la base de datos
                        UserService.DeleteFamilia(_selectedFamily.Id);

                        // Mostrar un mensaje de éxito
                        MessageBox.Show("Familia eliminada con éxito.", "Eliminación de Familia", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Recargar el DataGridView para mostrar los cambios
                        LoadFamilies();
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                MessageBox.Show($"Error al eliminar la familia: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}