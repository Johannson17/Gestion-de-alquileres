using Services.Facade;
using Services.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LOGIC.Facade;
using Domain;
using static Domain.Person;
using System.Drawing;

namespace UI.Tenant
{
    public partial class frmContractsReport : Form
    {
        private readonly ContractService _contractService;
        private readonly PropertyService _propertyService;
        private readonly PersonService _personService;
        private List<Contract> _originalContracts;
        private Dictionary<Guid, string> _tenantDniMapping;
        private readonly Guid _loggedInTenantId; // ID del arrendatario logueado

        public frmContractsReport(Guid loggedInTenantId)
        {
            InitializeComponent();
            _contractService = new ContractService();
            _propertyService = new PropertyService();
            _personService = new PersonService();
            _loggedInTenantId = loggedInTenantId;
            this.Load += frmContractsReports_Load;
        }

        private void frmContractsReports_Load(object sender, EventArgs e)
        {
            try
            {
                LoadTenantDniMapping(); // Cargar mapeo de IDs a DNIs
                LoadContracts();
                LoadComboBoxOptions();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTenantDniMapping()
        {
            try
            {
                // Crear un diccionario que mapea el ID del arrendatario (Guid) al DNI
                _tenantDniMapping = _personService.GetAllPersonsByType(PersonTypeEnum.Tenant)
                    .ToDictionary(person => person.IdPerson, person => person.NumberDocumentPerson.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el mapeo de arrendatarios: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadContracts()
        {
            try
            {
                // Obtener contratos únicamente del arrendatario logueado
                _originalContracts = _contractService.GetContractsByTenantId(_loggedInTenantId);
                var properties = _propertyService.GetActivePropertiesByTenantId(_loggedInTenantId)
                    .ToDictionary(p => p.IdProperty, p => p.AddressProperty);

                // Obtener el DNI del arrendatario logueado
                var loggedInTenantDni = _personService.GetPersonById(_loggedInTenantId)?.NumberDocumentPerson;

                var displayContracts = _originalContracts.Select(contract => new
                {
                    IdContract = contract.IdContract,
                    PropertyAddress = properties.ContainsKey(contract.FkIdProperty) ? properties[contract.FkIdProperty] : "Dirección no encontrada",
                    StartDate = contract.DateStartContract,
                    EndDate = contract.DateFinalContract,
                    AnnualRentPrice = contract.AnnualRentPrice,
                    IsActive = contract.StatusContract,
                    TenantDni = loggedInTenantDni // Agregar columna con el DNI del arrendatario logueado
                }).ToList();

                dgvContracts.DataSource = displayContracts;

                // Configurar columnas visibles
                dgvContracts.Columns["IdContract"].Visible = false;
                dgvContracts.Columns["PropertyAddress"].HeaderText = "Propiedad";
                dgvContracts.Columns["StartDate"].HeaderText = "Fecha de Inicio";
                dgvContracts.Columns["EndDate"].HeaderText = "Fecha de Finalización";
                dgvContracts.Columns["AnnualRentPrice"].HeaderText = "Precio Mensual";
                dgvContracts.Columns["IsActive"].HeaderText = "Estado";
                dgvContracts.Columns["TenantDni"].HeaderText = "DNI del Arrendatario";

                dgvContracts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Ajustar columnas al ancho completo
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los contratos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            try
            {
                // Obtener los valores seleccionados en los ComboBox
                var selectedStatus = cmbStatus.SelectedItem?.ToString();
                var selectedProperty = cmbProperty.SelectedItem?.ToString();

                // Filtrar los contratos del arrendatario logueado
                var filteredContracts = _originalContracts.Where(contract =>
                {
                    var matchesStatus = selectedStatus == "Todos" || contract.StatusContract.ToString() == selectedStatus;
                    var matchesProperty = selectedProperty == "Todos" ||
                                          (_propertyService.GetAllProperties()
                                              .FirstOrDefault(p => p.IdProperty == contract.FkIdProperty)?.AddressProperty == selectedProperty);

                    return matchesStatus && matchesProperty;
                }).ToList();

                // Actualizar el DataGridView
                dgvContracts.DataSource = filteredContracts.Select(contract => new
                {
                    IdContract = contract.IdContract,
                    PropertyAddress = _propertyService.GetAllProperties()
                        .FirstOrDefault(p => p.IdProperty == contract.FkIdProperty)?.AddressProperty ?? "Propiedad no encontrada",
                    StartDate = contract.DateStartContract,
                    EndDate = contract.DateFinalContract,
                    AnnualRentPrice = contract.AnnualRentPrice,
                    IsActive = contract.StatusContract
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al filtrar los contratos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadComboBoxOptions()
        {
            try
            {
                // Cargar valores únicos del DataGridView para el ComboBox de Estado
                var statusValues = dgvContracts.Rows
                    .Cast<DataGridViewRow>()
                    .Where(row => row.Cells["IsActive"].Value != null)
                    .Select(row => row.Cells["IsActive"].Value.ToString())
                    .Distinct()
                    .ToList();

                cmbStatus.Items.Clear();
                cmbStatus.Items.Add("Todos");
                cmbStatus.Items.AddRange(statusValues.ToArray());
                cmbStatus.SelectedIndex = 0;

                // Cargar valores únicos del DataGridView para el ComboBox de Propiedad
                var propertyValues = dgvContracts.Rows
                    .Cast<DataGridViewRow>()
                    .Where(row => row.Cells["PropertyAddress"].Value != null)
                    .Select(row => row.Cells["PropertyAddress"].Value.ToString())
                    .Distinct()
                    .ToList();

                cmbProperty.Items.Clear();
                cmbProperty.Items.Add("Todos");
                cmbProperty.Items.AddRange(propertyValues.ToArray());
                cmbProperty.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar opciones en los ComboBox: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Archivos de Excel (*.xlsx)|*.xlsx";
                    saveFileDialog.Title = "Guardar Reporte de Contratos";
                    saveFileDialog.FileName = "ReporteContratos.xlsx";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Extraer los datos visibles del DataGridView
                        var loggedInTenantDni = _personService.GetPersonById(_loggedInTenantId)?.NumberDocumentPerson;

                        var contractsToExport = dgvContracts.Rows
                            .Cast<DataGridViewRow>()
                            .Where(row => !row.IsNewRow) // Excluye la última fila vacía
                            .Select(row => new Contract
                            {
                                PropertyAddres = row.Cells["PropertyAddress"].Value.ToString(),
                                DateStartContract = DateTime.Parse(row.Cells["StartDate"].Value.ToString()),
                                DateFinalContract = DateTime.Parse(row.Cells["EndDate"].Value.ToString()),
                                AnnualRentPrice = double.Parse(row.Cells["AnnualRentPrice"].Value.ToString()),
                                StatusContract = row.Cells["IsActive"].Value.ToString(),
                                TenantName = loggedInTenantDni.ToString()
                            })
                            .ToList();

                        // Llamar al servicio para exportar los datos visibles del dgv
                        _contractService.ExportContractsToExcel(saveFileDialog.FileName, contractsToExport);

                        MessageBox.Show("El archivo se guardó exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar el archivo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDownloadImage_Click(object sender, EventArgs e)
        {
            if (dgvContracts.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un contrato para descargar su imagen.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                dynamic selectedRow = dgvContracts.CurrentRow.DataBoundItem;
                if (selectedRow == null)
                {
                    MessageBox.Show("Error al obtener los datos del contrato.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validar si la imagen está presente
                if (selectedRow.ContractImage == null || selectedRow.ContractImage.Length == 0)
                {
                    MessageBox.Show("El contrato seleccionado no tiene una imagen asociada.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Descargar la imagen
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Archivos de Imagen (*.png)|*.png";
                    saveFileDialog.Title = "Guardar Imagen del Contrato";
                    saveFileDialog.FileName = "Contrato.png";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        System.IO.File.WriteAllBytes(saveFileDialog.FileName, selectedRow.ContractImage);
                        MessageBox.Show("La imagen se guardó exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al descargar la imagen: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImage_Click(object sender, EventArgs e)
        {
            if (dgvContracts.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un contrato para ver su imagen.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                dynamic selectedRow = dgvContracts.CurrentRow.DataBoundItem;
                if (selectedRow == null)
                {
                    MessageBox.Show("Error al obtener los datos del contrato.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                byte[] contractImage = selectedRow.ContractImage;
                if (contractImage == null || contractImage.Length == 0)
                {
                    MessageBox.Show("El contrato seleccionado no tiene una imagen asociada.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ShowImage(contractImage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al mostrar la imagen: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowImage(byte[] imageBytes)
        {
            using (Form imageForm = new Form())
            {
                imageForm.Text = "Imagen del Contrato";
                imageForm.Size = new Size(600, 400);
                imageForm.StartPosition = FormStartPosition.CenterScreen;

                PictureBox pictureBox = new PictureBox
                {
                    Image = Image.FromStream(new System.IO.MemoryStream(imageBytes)),
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom
                };

                imageForm.Controls.Add(pictureBox);
                imageForm.ShowDialog();
            }
        }
    }
}