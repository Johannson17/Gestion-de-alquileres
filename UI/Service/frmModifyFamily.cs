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
            btnSave.Click += btnSave_Click;
            btnDelete.Click += btnDelete_Click;
        }

        /// <summary>
        /// Cargar todas las familias en el DataGridView.
        /// </summary>
        private void LoadFamilies()
        {
            try
            {
                var familias = UserService.GetAllFamilias();

                var familiasData = familias.Select(f => new
                {
                    f.Id,
                    f.Nombre,
                    Patentes = string.Join(", ", f.Accesos.OfType<Patente>().Select(p => p.Nombre)) // Concatenar los nombres de las patentes
                }).ToList();

                dgvFamilies.DataSource = familiasData;

                // Ajustar columnas del DataGridView
                dgvFamilies.Columns["Id"].Visible = false; // Ocultar la columna de Id
                dgvFamilies.Columns["Nombre"].HeaderText = "Nombre de Familia";
                dgvFamilies.Columns["Patentes"].HeaderText = "Permisos (Patentes)";
                dgvFamilies.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar las familias: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Al seleccionar una fila en el DataGridView, cargar los datos de la familia en los TextBox y CheckedListBox.
        /// </summary>
        private void dgvFamilies_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvFamilies.SelectedRows.Count > 0)
            {
                try
                {
                    var selectedFamilyId = (Guid)dgvFamilies.SelectedRows[0].Cells["Id"].Value;
                    _selectedFamily = UserService.GetAllFamilias().FirstOrDefault(f => f.Id == selectedFamilyId);

                    if (_selectedFamily != null)
                    {
                        txtName.Text = _selectedFamily.Nombre;
                        LoadAccesos(_selectedFamily);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al seleccionar la familia: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Cargar los accesos (patentes y familias) en el CheckedListBox y marcar los seleccionados.
        /// </summary>
        private void LoadAccesos(Familia family)
        {
            try
            {
                chlbAccesos.DisplayMember = "Nombre";
                chlbAccesos.Items.Clear();

                var patentes = UserService.GetAllPatentes();
                foreach (var patente in patentes)
                {
                    bool isChecked = family.Accesos.Any(a => a is Patente && a.Id == patente.Id);
                    chlbAccesos.Items.Add(patente, isChecked);
                }

                var familias = UserService.GetAllFamilias().Where(f => f.Id != family.Id);
                foreach (var otraFamilia in familias)
                {
                    bool isChecked = family.Accesos.Any(a => a is Familia && a.Id == otraFamilia.Id);
                    chlbAccesos.Items.Add(otraFamilia, isChecked);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los accesos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    _selectedFamily.Nombre = txtName.Text;

                    _selectedFamily.Accesos.Clear();
                    foreach (var item in chlbAccesos.CheckedItems)
                    {
                        if (item is Acceso acceso)
                        {
                            _selectedFamily.Add(acceso);
                        }
                    }

                    UserService.UpdateFamilia(_selectedFamily);

                    MessageBox.Show("Familia modificada con éxito.", "Modificación de Familia", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadFamilies();
                }
            }
            catch (Exception ex)
            {
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
                    var confirmResult = MessageBox.Show("¿Está seguro de que desea eliminar esta familia?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (confirmResult == DialogResult.Yes)
                    {
                        UserService.DeleteFamilia(_selectedFamily.Id);

                        MessageBox.Show("Familia eliminada con éxito.", "Eliminación de Familia", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadFamilies();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar la familia: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
