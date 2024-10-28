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
        private List<Property> _properties; // Lista de propiedades cargadas
        private Property _currentProperty; // Propiedad seleccionada

        public frmModifyProperty()
        {
            InitializeComponent();
            _propertyService = new PropertyService();

            // Cargar todas las propiedades al iniciar el formulario
            LoadProperties();

            // Vincular el evento SelectionChanged del DataGridView
            dgvProperty.SelectionChanged += dgvProperties_SelectionChanged;
        }

        /// <summary>
        /// Cargar todas las propiedades en el DataGridView.
        /// </summary>
        private void LoadProperties()
        {
            try
            {
                // Obtener todas las propiedades
                _properties = _propertyService.GetAllProperties();

                // Limpiar el DataGridView
                dgvProperty.Columns.Clear();

                // Asignar los datos al DataGridView directamente desde la lista de propiedades
                dgvProperty.DataSource = _properties.Select(p => new
                {
                    p.IdProperty, // Este se usa internamente, pero está oculto
                    p.DescriptionProperty,
                    p.StatusProperty,
                    p.CountryProperty,
                    p.ProvinceProperty,
                    p.MunicipalityProperty,
                    p.AddressProperty
                }).ToList();

                // Ocultar la columna IdProperty
                dgvProperty.Columns["IdProperty"].Visible = false;

                // Ajustar el ancho de las columnas automáticamente
                dgvProperty.AutoResizeColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las propiedades: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Maneja el evento de selección de una fila en el DataGridView.
        /// Llena los campos del formulario con los datos de la propiedad seleccionada.
        /// </summary>
        private void dgvProperties_SelectionChanged(object sender, EventArgs e)
        {
            // Verificar que hay filas seleccionadas en el DataGridView
            if (dgvProperty.SelectedRows.Count > 0)
            {
                try
                {
                    // Obtener el índice de la fila seleccionada
                    var selectedRowIndex = dgvProperty.SelectedRows[0].Index;

                    // Obtener la propiedad seleccionada usando el IdProperty
                    var selectedPropertyId = (Guid)dgvProperty.Rows[selectedRowIndex].Cells["IdProperty"].Value;

                    // Buscar la propiedad en la lista cargada
                    _currentProperty = _propertyService.GetProperty(selectedPropertyId); // Cargar propiedad con inventario

                    // Si la propiedad es encontrada, cargar los datos en los TextBox
                    if (_currentProperty != null)
                    {
                        txtAddress.Text = _currentProperty.AddressProperty;
                        txtCountry.Text = _currentProperty.CountryProperty;
                        txtMunicipality.Text = _currentProperty.MunicipalityProperty;
                        txtProvince.Text = _currentProperty.ProvinceProperty;
                        txtDescription.Text = _currentProperty.DescriptionProperty;

                        // Cargar el ComboBox de Estado con el enum
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
        /// Cargar el ComboBox de Estados con valores del enum PropertyStatusEnum.
        /// </summary>
        private void LoadStatusComboBox(PropertyStatusEnum currentStatus)
        {
            cmbStatus.DataSource = Enum.GetValues(typeof(PropertyStatusEnum)).Cast<PropertyStatusEnum>().ToList();
            cmbStatus.SelectedItem = currentStatus; // Seleccionar el estado actual
        }

        /// <summary>
        /// Guardar los cambios realizados en la propiedad.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Actualizar los datos de la propiedad
                _currentProperty.AddressProperty = txtAddress.Text;
                _currentProperty.CountryProperty = txtCountry.Text;
                _currentProperty.MunicipalityProperty = txtMunicipality.Text;
                _currentProperty.ProvinceProperty = txtProvince.Text;
                _currentProperty.DescriptionProperty = txtDescription.Text;
                _currentProperty.StatusProperty = (PropertyStatusEnum)cmbStatus.SelectedItem; // Convertir el valor seleccionado a enum

                // Guardar los cambios
                _propertyService.UpdateProperty(_currentProperty);

                MessageBox.Show("Propiedad actualizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Recargar el DataGridView con los cambios
                LoadProperties();

                // Reseleccionar la propiedad modificada en el DataGridView
                ReselectCurrentProperty();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar la propiedad: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Reseleccionar la propiedad modificada en el DataGridView.
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
                    // Eliminar la propiedad
                    _propertyService.DeleteProperty(_currentProperty.IdProperty);

                    MessageBox.Show("Propiedad eliminada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Recargar las propiedades después de la eliminación
                    LoadProperties();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar la propiedad: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Botón para editar el inventario de la propiedad seleccionada.
        /// </summary>
        private void btnEditInventory_Click(object sender, EventArgs e)
        {
            try
            {
                // Verificar si hay una propiedad seleccionada
                if (_currentProperty == null)
                {
                    MessageBox.Show("Debe seleccionar una propiedad primero.", "Propiedad no seleccionada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Abrir el formulario para editar el inventario (sin restricciones, aunque no tenga inventario)
                var modifyInventoryForm = new frmModifyPropertyInventory(_currentProperty);

                // Mostrar el formulario de inventario y verificar si se guardaron los cambios
                if (modifyInventoryForm.ShowDialog() == DialogResult.OK)
                {
                    // Si se guardaron los cambios en el inventario, actualiza los datos de la propiedad
                    dgvProperty.Refresh(); // Refrescar el DataGridView para reflejar los cambios
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir el formulario de inventario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}