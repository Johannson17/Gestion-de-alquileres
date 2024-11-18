using Domain;
using LOGIC.Facade;
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
        private readonly Guid _loggedInTenantId; // ID del arrendatario logueado

        public frmPropertiesReport(Guid loggedInTenantId)
        {
            InitializeComponent();
            _propertyService = new PropertyService();
            _loggedInTenantId = loggedInTenantId;

            LoadProperties();
            LoadComboBoxOptions();
        }

        /// <summary>
        /// Cargar todas las propiedades en el DataGridView.
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

                // Asignar nombres en español a las columnas
                dgvProperties.Columns["DescriptionProperty"].HeaderText = "Descripción";
                dgvProperties.Columns["StatusProperty"].HeaderText = "Estado";
                dgvProperties.Columns["CountryProperty"].HeaderText = "País";
                dgvProperties.Columns["ProvinceProperty"].HeaderText = "Provincia";
                dgvProperties.Columns["MunicipalityProperty"].HeaderText = "Municipio";
                dgvProperties.Columns["AddressProperty"].HeaderText = "Dirección";

                dgvProperties.AutoResizeColumns();
                dgvProperties.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Ajustar columnas al ancho completo
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las propiedades: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Cargar opciones únicas en el ComboBox de Estado desde las propiedades.
        /// </summary>
        private void LoadComboBoxOptions()
        {
            try
            {
                // Convertir los valores de StatusProperty a cadenas
                var statusValues = _properties
                    .Select(p => p.StatusProperty.ToString())
                    .Distinct()
                    .ToList();

                cmbStatus.Items.Clear();
                cmbStatus.Items.Add("Todos"); // Agregar la opción "Todos"
                cmbStatus.Items.AddRange(statusValues.ToArray()); // Agregar los valores convertidos a string
                cmbStatus.SelectedIndex = 0; // Seleccionar el primer elemento por defecto
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar opciones en los ComboBox: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Filtrar las propiedades en el DataGridView en función del estado seleccionado.
        /// </summary>
        private void btnFilter_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedStatus = cmbStatus.SelectedItem?.ToString();

                // Si el filtro es "Todos", obtener todas las propiedades
                List<Property> filteredProperties = selectedStatus == "Todos"
                    ? _propertyService.GetAllProperties() // Llama a GetAllProperties cuando no hay filtro
                    : _propertyService.GetPropertiesByStatus((PropertyStatusEnum)Enum.Parse(typeof(PropertyStatusEnum), selectedStatus));

                // Actualiza el DataGridView con las propiedades filtradas
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
                MessageBox.Show($"Error al filtrar las propiedades: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Descargar los datos visibles en el DataGridView a un archivo Excel.
        /// </summary>
        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Archivos de Excel (*.xlsx)|*.xlsx";
                    saveFileDialog.Title = "Guardar Reporte de Propiedades";
                    saveFileDialog.FileName = "ReportePropiedades.xlsx";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Extraer las propiedades visibles en el DataGridView
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

                        // Llamar al servicio para exportar las propiedades
                        _propertyService.ExportPropertiesToExcel(saveFileDialog.FileName, propertiesToExport);

                        MessageBox.Show("El archivo se guardó exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar el archivo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}