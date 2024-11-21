using Domain;
using LOGIC.Facade;
using Services.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace UI.Tenant
{
    public partial class frmPropertiesReport : Form
    {
        private readonly PropertyService _propertyService;
        private List<Property> _properties;
        private readonly Guid _loggedInTenantId; // ID of the logged-in tenant

        public frmPropertiesReport(Guid loggedInTenantId)
        {
            InitializeComponent();
            _propertyService = new PropertyService();
            _loggedInTenantId = loggedInTenantId;

            LoadProperties();
            LoadComboBoxOptions();
        }

        /// <summary>
        /// Load all properties into the DataGridView.
        /// </summary>
        private void LoadProperties()
        {
            try
            {
                _properties = _propertyService.GetAllProperties();

                dgvProperties.Columns.Clear();

                dgvProperties.DataSource = _properties.Select(p => new
                {
                    p.IdProperty,
                    p.DescriptionProperty,
                    p.StatusProperty,
                    p.CountryProperty,
                    p.ProvinceProperty,
                    p.MunicipalityProperty,
                    p.AddressProperty
                }).ToList();

                dgvProperties.Columns["IdProperty"].Visible = false;

                // Assign English column headers
                dgvProperties.Columns["DescriptionProperty"].HeaderText = LanguageService.Translate("Descripción");
                dgvProperties.Columns["StatusProperty"].HeaderText = LanguageService.Translate("Estado");
                dgvProperties.Columns["CountryProperty"].HeaderText = LanguageService.Translate("País");
                dgvProperties.Columns["ProvinceProperty"].HeaderText = LanguageService.Translate("Provincia");
                dgvProperties.Columns["MunicipalityProperty"].HeaderText = LanguageService.Translate("Municipio");
                dgvProperties.Columns["AddressProperty"].HeaderText = LanguageService.Translate("Dirección");

                dgvProperties.AutoResizeColumns();
                dgvProperties.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Adjust columns to fill width
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
        /// Load unique options into the Status ComboBox from properties.
        /// </summary>
        private void LoadComboBoxOptions()
        {
            try
            {
                var statusValues = _properties
                    .Select(p => p.StatusProperty.ToString())
                    .Distinct()
                    .ToList();

                cmbStatus.Items.Clear();
                cmbStatus.Items.Add(LanguageService.Translate("Todos")); // Add "Todos" option
                cmbStatus.Items.AddRange(statusValues.ToArray());
                cmbStatus.SelectedIndex = 0; // Default to first option
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar opciones en los ComboBox") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Filter properties in the DataGridView based on the selected status.
        /// </summary>
        private void btnFilter_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedStatus = cmbStatus.SelectedItem?.ToString();

                var filteredProperties = selectedStatus == LanguageService.Translate("Todos")
                    ? _propertyService.GetAllProperties()
                    : _propertyService.GetPropertiesByStatus((PropertyStatusEnum)Enum.Parse(typeof(PropertyStatusEnum), selectedStatus));

                dgvProperties.DataSource = filteredProperties.Select(p => new
                {
                    p.IdProperty,
                    p.DescriptionProperty,
                    p.StatusProperty,
                    p.CountryProperty,
                    p.ProvinceProperty,
                    p.MunicipalityProperty,
                    p.AddressProperty
                }).ToList();

                dgvProperties.Columns["IdProperty"].Visible = false;
                dgvProperties.AutoResizeColumns();
                dgvProperties.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al filtrar las propiedades") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Download visible data in the DataGridView to an Excel file.
        /// </summary>
        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = LanguageService.Translate("Archivos de Excel") + " (*.xlsx)|*.xlsx";
                    saveFileDialog.Title = LanguageService.Translate("Guardar Reporte de Propiedades");
                    saveFileDialog.FileName = LanguageService.Translate("ReportePropiedades") + ".xlsx";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        var propertiesToExport = dgvProperties.Rows
                            .Cast<DataGridViewRow>()
                            .Where(row => !row.IsNewRow)
                            .Select(row => new Property
                            {
                                DescriptionProperty = row.Cells["DescriptionProperty"].Value?.ToString(),
                                StatusProperty = Enum.TryParse<PropertyStatusEnum>(row.Cells["StatusProperty"].Value?.ToString(), out var status) ? status : PropertyStatusEnum.Disponible,
                                CountryProperty = row.Cells["CountryProperty"].Value?.ToString(),
                                ProvinceProperty = row.Cells["ProvinceProperty"].Value?.ToString(),
                                MunicipalityProperty = row.Cells["MunicipalityProperty"].Value?.ToString(),
                                AddressProperty = row.Cells["AddressProperty"].Value?.ToString()
                            })
                            .ToList();

                        _propertyService.ExportPropertiesToExcel(saveFileDialog.FileName, propertiesToExport);

                        MessageBox.Show(
                            LanguageService.Translate("El archivo se guardó exitosamente."),
                            LanguageService.Translate("Éxito"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al exportar el archivo") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}