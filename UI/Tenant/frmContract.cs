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
            List<Contract> contracts = _contractService.GetContractsByTenantId(_loggedInTenantId);

            // Obtener las propiedades para extraer la dirección
            var properties = _propertyService.GetAllProperties().ToDictionary(p => p.IdProperty, p => p.AddressProperty);

            // Crear una lista para mostrar solo la información deseada
            var displayContracts = contracts.Select(contract => new
            {
                IdContract = contract.IdContract, // Agregar ID para referencia
                PropertyAddress = properties.ContainsKey(contract.FkIdProperty) ? properties[contract.FkIdProperty] : "Dirección no encontrada",
                StartDate = contract.DateStartContract,
                EndDate = contract.DateFinalContract,
                AnnualRentPrice = contract.AnnualRentPrice,
                IsActive = contract.StatusContract
            }).ToList();

            dgvContracts.DataSource = displayContracts;

            // Ajustar encabezados de columna
            dgvContracts.Columns["IdContract"].Visible = false; // Ocultar ID del contrato
            dgvContracts.Columns["PropertyAddress"].HeaderText = "Propiedad";
            dgvContracts.Columns["StartDate"].HeaderText = "Fecha de Inicio";
            dgvContracts.Columns["EndDate"].HeaderText = "Fecha de Finalización";
            dgvContracts.Columns["AnnualRentPrice"].HeaderText = "Precio Mensual";
            dgvContracts.Columns["IsActive"].HeaderText = "Estado ";
        }


    private void btnSave_Click(object sender, EventArgs e)
        {
            if (dgvContracts.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un contrato para guardar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedContract = (Contract)dgvContracts.CurrentRow.DataBoundItem;

            // Abre el diálogo para seleccionar la imagen
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                openFileDialog.Title = "Seleccione la imagen del contrato firmado";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Lee la imagen seleccionada y convierte en byte[]
                    byte[] contractImage = File.ReadAllBytes(openFileDialog.FileName);

                    // Llama al servicio para guardar la imagen del contrato en la base de datos
                    _contractService.SaveContractImage(selectedContract.IdContract, contractImage);

                    MessageBox.Show("Contrato e imagen guardados exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No se seleccionó ninguna imagen. No se guardarán los cambios.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }


        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (dgvContracts.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un contrato para descargar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedContract = (Contract)dgvContracts.CurrentRow.DataBoundItem;

            // Usar FolderBrowserDialog para seleccionar la carpeta de destino
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Seleccione la carpeta de destino para el contrato";

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    // Ruta del archivo PDF en la carpeta seleccionada
                    string outputPath = Path.Combine(folderDialog.SelectedPath, $"Contrato_{selectedContract.IdContract}.pdf");

                    // Generar el PDF
                    _contractService.GenerateContractPDF(selectedContract.IdContract, outputPath);

                    MessageBox.Show($"Contrato descargado en: {outputPath}", "Descarga Completa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
