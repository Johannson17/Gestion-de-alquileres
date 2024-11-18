using Domain;
using LOGIC.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    public partial class frmModifyProperty : Form
    {
        private readonly PropertyService _propertyService;
        private List<Property> _properties;
        private Property _currentProperty;

        public frmModifyProperty()
        {
            InitializeComponent();
            _propertyService = new PropertyService();

            LoadProperties();

            dgvProperty.SelectionChanged += dgvProperties_SelectionChanged;
            btnSave.Click += btnSave_Click;
            btnDelete.Click += btnDelete_Click;
            btnEditInventory.Click += btnEditInventory_Click;
        }

        /// <summary>
        /// Cargar todas las propiedades en el DataGridView.
        /// </summary>
        private void LoadProperties()
        {
            try
            {
                _properties = _propertyService.GetAllProperties();

                dgvProperty.Columns.Clear();

                dgvProperty.DataSource = _properties.Select(p => new
                {
                    p.IdProperty,
                    p.DescriptionProperty,
                    p.StatusProperty,
                    p.CountryProperty,
                    p.ProvinceProperty,
                    p.MunicipalityProperty,
                    p.AddressProperty
                }).ToList();

                dgvProperty.Columns["IdProperty"].Visible = false;
                dgvProperty.AutoResizeColumns();
                dgvProperty.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Ajustar columnas al ancho completo
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las propiedades: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Maneja el evento de selección en el DataGridView.
        /// </summary>
        private void dgvProperties_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProperty.SelectedRows.Count > 0)
            {
                try
                {
                    var selectedPropertyId = (Guid)dgvProperty.SelectedRows[0].Cells["IdProperty"].Value;

                    _currentProperty = _propertyService.GetProperty(selectedPropertyId);

                    if (_currentProperty != null)
                    {
                        txtAddress.Text = _currentProperty.AddressProperty;
                        txtCountry.Text = _currentProperty.CountryProperty;
                        txtMunicipality.Text = _currentProperty.MunicipalityProperty;
                        txtProvince.Text = _currentProperty.ProvinceProperty;
                        txtDescription.Text = _currentProperty.DescriptionProperty;

                        LoadStatusComboBox(_currentProperty.StatusProperty);
                    }
                    else
                    {
                        MessageBox.Show("No se encontró la propiedad seleccionada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al seleccionar la propiedad: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Cargar los estados en el ComboBox.
        /// </summary>
        private void LoadStatusComboBox(PropertyStatusEnum currentStatus)
        {
            cmbStatus.DataSource = Enum.GetValues(typeof(PropertyStatusEnum)).Cast<PropertyStatusEnum>().ToList();
            cmbStatus.SelectedItem = currentStatus;
        }

        /// <summary>
        /// Guardar los cambios en la propiedad.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _currentProperty.AddressProperty = txtAddress.Text;
                _currentProperty.CountryProperty = txtCountry.Text;
                _currentProperty.MunicipalityProperty = txtMunicipality.Text;
                _currentProperty.ProvinceProperty = txtProvince.Text;
                _currentProperty.DescriptionProperty = txtDescription.Text;
                _currentProperty.StatusProperty = (PropertyStatusEnum)cmbStatus.SelectedItem;

                _propertyService.UpdateProperty(_currentProperty);

                MessageBox.Show("Propiedad actualizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadProperties();
                ReselectCurrentProperty();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar la propiedad: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Seleccionar nuevamente la propiedad modificada.
        /// </summary>
        private void ReselectCurrentProperty()
        {
            foreach (DataGridViewRow row in dgvProperty.Rows)
            {
                if (row.Cells["IdProperty"].Value.ToString() == _currentProperty.IdProperty.ToString())
                {
                    row.Selected = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Eliminar la propiedad seleccionada.
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var confirmResult = MessageBox.Show("¿Está seguro de eliminar esta propiedad?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmResult == DialogResult.Yes)
                {
                    _propertyService.DeleteProperty(_currentProperty.IdProperty);

                    MessageBox.Show("Propiedad eliminada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadProperties();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar la propiedad: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Editar el inventario de la propiedad seleccionada.
        /// </summary>
        private void btnEditInventory_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentProperty == null)
                {
                    MessageBox.Show("Debe seleccionar una propiedad primero.", "Propiedad no seleccionada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var modifyInventoryForm = new frmModifyPropertyInventory(_currentProperty);

                if (modifyInventoryForm.ShowDialog() == DialogResult.OK)
                {
                    LoadProperties();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir el formulario de inventario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
