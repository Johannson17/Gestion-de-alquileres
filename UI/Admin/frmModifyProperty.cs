using Domain;
using LOGIC.Facade;
using Services.Facade;
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
        /// Load all properties into the DataGridView.
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
                dgvProperty.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Adjust columns to full width
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar las propiedades") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Handle selection change event in the DataGridView.
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
                        MessageBox.Show(
                            LanguageService.Translate("No se encontró la propiedad seleccionada"),
                            LanguageService.Translate("Error"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        LanguageService.Translate("Error al seleccionar la propiedad") + ": " + ex.Message,
                        LanguageService.Translate("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        /// <summary>
        /// Load statuses into the ComboBox.
        /// </summary>
        private void LoadStatusComboBox(PropertyStatusEnum currentStatus)
        {
            cmbStatus.DataSource = Enum.GetValues(typeof(PropertyStatusEnum)).Cast<PropertyStatusEnum>().ToList();
            cmbStatus.SelectedItem = currentStatus;
        }

        /// <summary>
        /// Save changes to the selected property.
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

                MessageBox.Show(
                    LanguageService.Translate("Propiedad actualizada correctamente"),
                    LanguageService.Translate("Éxito"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LoadProperties();
                ReselectCurrentProperty();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al actualizar la propiedad") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Reselect the modified property.
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
        /// Delete the selected property.
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var confirmResult = MessageBox.Show(
                    LanguageService.Translate("¿Está seguro de eliminar esta propiedad?"),
                    LanguageService.Translate("Confirmar eliminación"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (confirmResult == DialogResult.Yes)
                {
                    _propertyService.DeleteProperty(_currentProperty.IdProperty);

                    MessageBox.Show(
                        LanguageService.Translate("Propiedad eliminada correctamente"),
                        LanguageService.Translate("Éxito"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    LoadProperties();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al eliminar la propiedad") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Edit inventory of the selected property.
        /// </summary>
        private void btnEditInventory_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentProperty == null)
                {
                    MessageBox.Show(
                        LanguageService.Translate("Debe seleccionar una propiedad primero"),
                        LanguageService.Translate("Propiedad no seleccionada"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
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
                MessageBox.Show(
                    LanguageService.Translate("Error al abrir el formulario de inventario") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}