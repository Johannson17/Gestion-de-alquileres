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

        private Timer toolTipTimer;
        private Control currentControl; // Control actual sobre el que se deja el mouse
        private Dictionary<Control, string> helpMessages; // Diccionario de mensajes de ayuda

        public frmContractsReport(Guid loggedInTenantId)
        {
            InitializeComponent();
            _contractService = new ContractService();
            _propertyService = new PropertyService();
            _personService = new PersonService();
            _loggedInTenantId = loggedInTenantId;
            this.Load += frmContractsReports_Load;

            // Inicializar el Timer
            toolTipTimer = new Timer { Interval = 1000 };
            toolTipTimer.Tick += ToolTipTimer_Tick;

            // Inicializar y suscribir eventos de ayuda
            InitializeHelpMessages();
            SubscribeHelpMessagesEvents();
        }

        private void RegisterHelpEvents(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (helpMessages.ContainsKey(control))
                {
                    control.MouseEnter += Control_MouseEnter;
                    control.MouseLeave += Control_MouseLeave;
                }

                if (control.HasChildren)
                {
                    RegisterHelpEvents(control);
                }
            }
        }

        private void Control_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Control control && helpMessages.ContainsKey(control))
            {
                currentControl = control; // Guardar el control actual
                toolTipTimer.Start(); // Iniciar el temporizador
            }
        }

        private void Control_MouseLeave(object sender, EventArgs e)
        {
            toolTipTimer.Stop(); // Detener el temporizador
            currentControl = null; // Limpiar el control actual
        }

        private void ToolTipTimer_Tick(object sender, EventArgs e)
        {
            if (currentControl != null && helpMessages.ContainsKey(currentControl))
            {
                // Mostrar el ToolTip para el control actual
                ToolTip toolTip = new ToolTip();
                toolTip.Show(helpMessages[currentControl], currentControl, 3000); // Mostrar durante 3 segundos
            }

            toolTipTimer.Stop(); // Detener el temporizador después de mostrar el mensaje
        }

        private void frmContractsReports_Load(object sender, EventArgs e)
        {
            try
            {
                LoadTenantDniMapping();
                LoadContracts();
                LoadComboBoxOptions();
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageService.Translate("Error al cargar los datos") + ": " + ex.Message, LanguageService.Translate("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTenantDniMapping()
        {
            try
            {
                _tenantDniMapping = _personService.GetAllPersonsByType(PersonTypeEnum.Tenant)
                    .ToDictionary(person => person.IdPerson, person => person.NumberDocumentPerson.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageService.Translate("Error al cargar el mapeo de arrendatarios") + ": " + ex.Message, LanguageService.Translate("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadContracts()
        {
            try
            {
                _originalContracts = _contractService.GetContractsByTenantId(_loggedInTenantId);
                var properties = _propertyService.GetActivePropertiesByTenantId(_loggedInTenantId)
                    .ToDictionary(p => p.IdProperty, p => p.AddressProperty);

                var loggedInTenantDni = _personService.GetPersonById(_loggedInTenantId)?.NumberDocumentPerson;

                var displayContracts = _originalContracts.Select(contract => new
                {
                    IdContract = contract.IdContract,
                    PropertyAddress = properties.ContainsKey(contract.FkIdProperty) ? properties[contract.FkIdProperty] : LanguageService.Translate("Dirección no encontrada"),
                    StartDate = contract.DateStartContract,
                    EndDate = contract.DateFinalContract,
                    AnnualRentPrice = contract.AnnualRentPrice,
                    IsActive = contract.StatusContract,
                    TenantDni = loggedInTenantDni
                }).ToList();

                dgvContracts.DataSource = displayContracts;

                dgvContracts.Columns["IdContract"].Visible = false;
                dgvContracts.Columns["PropertyAddress"].HeaderText = LanguageService.Translate("Propiedad");
                dgvContracts.Columns["StartDate"].HeaderText = LanguageService.Translate("Fecha de Inicio");
                dgvContracts.Columns["EndDate"].HeaderText = LanguageService.Translate("Fecha de Finalización");
                dgvContracts.Columns["AnnualRentPrice"].HeaderText = LanguageService.Translate("Precio Mensual");
                dgvContracts.Columns["IsActive"].HeaderText = LanguageService.Translate("Estado");
                dgvContracts.Columns["TenantDni"].HeaderText = LanguageService.Translate("DNI del Arrendatario");

                dgvContracts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageService.Translate("Error al cargar los contratos") + ": " + ex.Message, LanguageService.Translate("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedStatus = cmbStatus.SelectedItem?.ToString();
                var selectedProperty = cmbProperty.SelectedItem?.ToString();

                var filteredContracts = _originalContracts.Where(contract =>
                {
                    var matchesStatus = selectedStatus == LanguageService.Translate("Todos") || contract.StatusContract.ToString() == selectedStatus;
                    var matchesProperty = selectedProperty == LanguageService.Translate("Todos") ||
                                          (_propertyService.GetAllProperties()
                                              .FirstOrDefault(p => p.IdProperty == contract.FkIdProperty)?.AddressProperty == selectedProperty);

                    return matchesStatus && matchesProperty;
                }).ToList();

                dgvContracts.DataSource = filteredContracts.Select(contract => new
                {
                    IdContract = contract.IdContract,
                    PropertyAddress = _propertyService.GetAllProperties()
                        .FirstOrDefault(p => p.IdProperty == contract.FkIdProperty)?.AddressProperty ?? LanguageService.Translate("Propiedad no encontrada"),
                    StartDate = contract.DateStartContract,
                    EndDate = contract.DateFinalContract,
                    AnnualRentPrice = contract.AnnualRentPrice,
                    IsActive = contract.StatusContract
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageService.Translate("Error al filtrar los contratos") + ": " + ex.Message, LanguageService.Translate("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadComboBoxOptions()
        {
            try
            {
                var statusValues = dgvContracts.Rows
                    .Cast<DataGridViewRow>()
                    .Where(row => row.Cells["IsActive"].Value != null)
                    .Select(row => row.Cells["IsActive"].Value.ToString())
                    .Distinct()
                    .ToList();

                cmbStatus.Items.Clear();
                cmbStatus.Items.Add(LanguageService.Translate("Todos"));
                cmbStatus.Items.AddRange(statusValues.ToArray());
                cmbStatus.SelectedIndex = 0;

                var propertyValues = dgvContracts.Rows
                    .Cast<DataGridViewRow>()
                    .Where(row => row.Cells["PropertyAddress"].Value != null)
                    .Select(row => row.Cells["PropertyAddress"].Value.ToString())
                    .Distinct()
                    .ToList();

                cmbProperty.Items.Clear();
                cmbProperty.Items.Add(LanguageService.Translate("Todos"));
                cmbProperty.Items.AddRange(propertyValues.ToArray());
                cmbProperty.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageService.Translate("Error al cargar opciones en los ComboBox") + ": " + ex.Message, LanguageService.Translate("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = LanguageService.Translate("Archivos de Excel") + " (*.xlsx)|*.xlsx";
                    saveFileDialog.Title = LanguageService.Translate("Guardar Reporte de Contratos");
                    saveFileDialog.FileName = LanguageService.Translate("ReporteContratos") + ".xlsx";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Extraer los datos visibles del DataGridView
                        var loggedInTenantDni = _personService.GetPersonById(_loggedInTenantId)?.NumberDocumentPerson;

                        var contractsToExport = dgvContracts.Rows
                            .Cast<DataGridViewRow>()
                            .Where(row => !row.IsNewRow) // Excluye la última fila vacía
                            .Select(row => new Contract
                            {
                                PropertyAddres = row.Cells[LanguageService.Translate("Propiedad")].Value.ToString(),
                                DateStartContract = DateTime.Parse(row.Cells[LanguageService.Translate("Fecha de Inicio")].Value.ToString()),
                                DateFinalContract = DateTime.Parse(row.Cells[LanguageService.Translate("Fecha de Finalización")].Value.ToString()),
                                AnnualRentPrice = double.Parse(row.Cells[LanguageService.Translate("Precio Mensual")].Value.ToString()),
                                StatusContract = row.Cells[LanguageService.Translate("Estado")].Value.ToString(),
                                TenantName = loggedInTenantDni.ToString()
                            })
                            .ToList();

                        // Llamar al servicio para exportar los datos visibles del dgv
                        _contractService.ExportContractsToExcel(saveFileDialog.FileName, contractsToExport);

                        MessageBox.Show(LanguageService.Translate("El archivo se guardó exitosamente."), LanguageService.Translate("Éxito"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageService.Translate("Error al exportar el archivo") + ": " + ex.Message, LanguageService.Translate("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDownloadImage_Click(object sender, EventArgs e)
        {
            if (dgvContracts.CurrentRow == null)
            {
                MessageBox.Show(LanguageService.Translate("Seleccione un contrato para descargar su imagen."), LanguageService.Translate("Advertencia"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                dynamic selectedRow = dgvContracts.CurrentRow.DataBoundItem;
                if (selectedRow == null)
                {
                    MessageBox.Show(LanguageService.Translate("Error al obtener los datos del contrato."), LanguageService.Translate("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validar si la imagen está presente
                if (selectedRow.ContractImage == null || selectedRow.ContractImage.Length == 0)
                {
                    MessageBox.Show(LanguageService.Translate("El contrato seleccionado no tiene una imagen asociada."), LanguageService.Translate("Advertencia"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Descargar la imagen
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = LanguageService.Translate("Archivos de Imagen") + " (*.png)|*.png";
                    saveFileDialog.Title = LanguageService.Translate("Guardar Imagen del Contrato");
                    saveFileDialog.FileName = LanguageService.Translate("Contrato") + ".png";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        System.IO.File.WriteAllBytes(saveFileDialog.FileName, selectedRow.ContractImage);
                        MessageBox.Show(LanguageService.Translate("La imagen se guardó exitosamente."), LanguageService.Translate("Éxito"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageService.Translate("Error al descargar la imagen") + ": " + ex.Message, LanguageService.Translate("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImage_Click(object sender, EventArgs e)
        {
            if (dgvContracts.CurrentRow == null)
            {
                MessageBox.Show(LanguageService.Translate("Seleccione un contrato para ver su imagen."), LanguageService.Translate("Advertencia"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                dynamic selectedRow = dgvContracts.CurrentRow.DataBoundItem;
                if (selectedRow == null)
                {
                    MessageBox.Show(LanguageService.Translate("Error al obtener los datos del contrato."), LanguageService.Translate("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                byte[] contractImage = selectedRow.ContractImage;
                if (contractImage == null || contractImage.Length == 0)
                {
                    MessageBox.Show(LanguageService.Translate("El contrato seleccionado no tiene una imagen asociada."), LanguageService.Translate("Advertencia"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ShowImage(contractImage);
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageService.Translate("Error al mostrar la imagen") + ": " + ex.Message, LanguageService.Translate("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowImage(byte[] imageBytes)
        {
            using (Form imageForm = new Form())
            {
                imageForm.Text = LanguageService.Translate("Imagen del Contrato");
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

        private void FrmContractsReport_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helpMessage = string.Join(Environment.NewLine, new[]
            {
                LanguageService.Translate("Bienvenido al módulo de reportes de contratos."),
                "",
                LanguageService.Translate("Opciones disponibles:"),
                $"- {LanguageService.Translate("Filtrar contratos por estado y propiedad.")}",
                $"- {LanguageService.Translate("Descargar un reporte de los contratos en Excel.")}",
                $"- {LanguageService.Translate("Ver o descargar la imagen asociada al contrato.")}",
                "",
                LanguageService.Translate("Para más ayuda, contacte con el administrador.")
            });

            MessageBox.Show(helpMessage,
                            LanguageService.Translate("Ayuda del sistema"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        /// <summary>
        /// Inicializa los mensajes de ayuda con la traducción actual.
        /// </summary>
        private void InitializeHelpMessages()
        {
            helpMessages = new Dictionary<Control, string>
            {
                { cmbStatus, LanguageService.Translate("Seleccione el estado del contrato para filtrar los resultados.") },
                { cmbProperty, LanguageService.Translate("Seleccione una propiedad para filtrar los contratos asociados.") },
                { btnFilter, LanguageService.Translate("Haga clic para aplicar los filtros seleccionados.") },
                { btnDownload, LanguageService.Translate("Descargue un reporte de los contratos en formato Excel.") },
                { btnDownloadImage, LanguageService.Translate("Descargue la imagen asociada al contrato seleccionado.") },
                { btnImage, LanguageService.Translate("Visualice la imagen asociada al contrato seleccionado.") }
            };
        }

        /// <summary>
        /// Suscribe los eventos `MouseEnter` y `MouseLeave` a los controles con mensajes de ayuda.
        /// </summary>
        private void SubscribeHelpMessagesEvents()
        {
            foreach (var control in helpMessages.Keys)
            {
                control.MouseEnter += Control_MouseEnter;
                control.MouseLeave += Control_MouseLeave;
            }
        }
    }
}