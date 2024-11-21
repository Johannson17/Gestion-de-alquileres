using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Services.Facade; // Asegúrate de que esta ruta sea correcta
using Domain;
using Services.Domain;
using System.IO;
using LOGIC.Facade;
using System.Linq;

namespace UI.Tenant
{
    public partial class frmContract : Form
    {
        private readonly ContractService _contractService;
        private readonly PropertyService _propertyService;
        private readonly Guid _loggedInTenantId; // ID del arrendatario logueado

        public frmContract(Guid loggedInTenantId)
        {
            InitializeComponent();
            _contractService = new ContractService();
            _propertyService = new PropertyService();
            _loggedInTenantId = loggedInTenantId;

            LoadContracts(); // Cargar contratos al abrir el formulario
        }

        private void LoadContracts()
        {
            // Obtener todos los contratos que no están activos
            List<Contract> contracts = _contractService.GetContractsByTenantIdAndStatus(_loggedInTenantId, "Inactivo");

            // Obtener las propiedades para extraer la dirección
            var properties = _propertyService.GetAllProperties().ToDictionary(p => p.IdProperty, p => p.AddressProperty);

            // Crear una lista para mostrar solo la información deseada
            var displayContracts = contracts.Select(contract => new
            {
                IdContract = contract.IdContract, // Agregar ID para referencia
                PropertyAddress = properties.ContainsKey(contract.FkIdProperty) ? properties[contract.FkIdProperty] : LanguageService.Translate("Dirección no encontrada"),
                StartDate = contract.DateStartContract,
                EndDate = contract.DateFinalContract,
                AnnualRentPrice = contract.AnnualRentPrice,
                IsActive = contract.StatusContract
            }).ToList();

            dgvContracts.DataSource = displayContracts;

            // Ajustar encabezados de columna
            dgvContracts.Columns["IdContract"].Visible = false; // Ocultar ID del contrato
            dgvContracts.Columns["PropertyAddress"].HeaderText = LanguageService.Translate("Propiedad");
            dgvContracts.Columns["StartDate"].HeaderText = LanguageService.Translate("Fecha de Inicio");
            dgvContracts.Columns["EndDate"].HeaderText = LanguageService.Translate("Fecha de Finalización");
            dgvContracts.Columns["AnnualRentPrice"].HeaderText = LanguageService.Translate("Precio Mensual");
            dgvContracts.Columns["IsActive"].HeaderText = LanguageService.Translate("Estado");
        }


    private void btnSave_Click(object sender, EventArgs e)
        {
            if (dgvContracts.CurrentRow == null)
            {
                MessageBox.Show(LanguageService.Translate("Seleccione un contrato para guardar."), LanguageService.Translate("Advertencia"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = dgvContracts.CurrentRow.DataBoundItem;
            var idContract = (Guid)selectedRow.GetType().GetProperty("IdContract").GetValue(selectedRow);

            // Abre el diálogo para seleccionar la imagen
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = LanguageService.Translate("Archivos de Imagen") + "|*.jpg;*.jpeg;*.png;*.bmp";
                openFileDialog.Title = LanguageService.Translate("Seleccione la imagen del contrato firmado");

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Lee la imagen seleccionada y convierte en byte[]
                    byte[] contractImage = File.ReadAllBytes(openFileDialog.FileName);

                    // Llama al servicio para guardar la imagen del contrato en la base de datos
                    _contractService.SaveContractImage(idContract, contractImage);

                    MessageBox.Show(LanguageService.Translate("Contrato e imagen guardados exitosamente."), LanguageService.Translate("Éxito"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(LanguageService.Translate("No se seleccionó ninguna imagen. No se guardarán los cambios."), LanguageService.Translate("Advertencia"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }


        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (dgvContracts.CurrentRow == null)
            {
                MessageBox.Show(LanguageService.Translate("Seleccione un contrato para descargar."), LanguageService.Translate("Advertencia"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener el DataBoundItem del tipo anónimo y extraer el IdContract
            var selectedRow = dgvContracts.CurrentRow.DataBoundItem;
            var idContract = (Guid)selectedRow.GetType().GetProperty("IdContract").GetValue(selectedRow);

            // Usar FolderBrowserDialog para seleccionar la carpeta de destino
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = LanguageService.Translate("Seleccione la carpeta de destino para el contrato");

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    // Ruta del archivo PDF en la carpeta seleccionada
                    string outputPath = Path.Combine(folderDialog.SelectedPath, $"Contrato_{idContract}.pdf");

                    // Generar el PDF
                    _contractService.GenerateContractPDF(idContract, outputPath);

                    MessageBox.Show(LanguageService.Translate("Contrato descargado en") + $": {outputPath}", LanguageService.Translate("Descarga Completa"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

    }
}
