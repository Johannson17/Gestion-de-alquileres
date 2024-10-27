using Domain;
using Services.Facade;
using Services.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LOGIC.Facade;
using static Domain.Person;

namespace UI
{
    public partial class frmModifyContract : Form
    {
        private readonly ContractService _contractService;
        private readonly PropertyService _propertyService;
        private readonly PersonService _personService;
        private List<Contract> _originalContracts; // Almacena los contratos originales

        public frmModifyContract()
        {
            InitializeComponent();
            _contractService = new ContractService();
            _propertyService = new PropertyService();
            _personService = new PersonService();
            this.Load += frmModifyContract_Load;
            dgvContracts.CellClick += dgvContracts_CellClick; // Vincula el evento CellClick
        }

        private void frmModifyContract_Load(object sender, EventArgs e)
        {
            LoadContracts();
            LoadProperties();
            LoadTenants();
        }

        private void LoadContracts()
        {
            // Obtener todos los contratos que no están activos
            _originalContracts = _contractService.GetContractsByStatus("Inactivo");

            // Obtener las propiedades para extraer la dirección
            var properties = _propertyService.GetAllProperties().ToDictionary(p => p.IdProperty, p => p.AddressProperty);

            // Crear una lista para mostrar solo la información deseada
            var displayContracts = _originalContracts.Select(contract => new
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

        private void LoadProperties()
        {
            var properties = _propertyService.GetPropertiesByStatus(PropertyStatusEnum.Disponible);
            cmbProperty.DataSource = properties;
            cmbProperty.DisplayMember = "AddressProperty";
            cmbProperty.ValueMember = "IdProperty";
        }

        private void LoadTenants()
        {
            var tenants = _personService.GetAllPersonsByType(PersonTypeEnum.Tenant);
            cmbTenant.DataSource = tenants;
            cmbTenant.DisplayMember = "NumberDocumentPerson";
            cmbTenant.ValueMember = "IdPerson";
        }

        private void dgvContracts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Obtener el ID del contrato seleccionado
                var selectedContractId = (Guid)dgvContracts.Rows[e.RowIndex].Cells["IdContract"].Value;

                // Buscar el contrato en la lista original usando el ID
                var selectedContract = _originalContracts.FirstOrDefault(c => c.IdContract == selectedContractId);

                if (selectedContract != null)
                {
                    LoadContractDetails(selectedContract);
                }
            }
        }

        private void LoadContractDetails(Contract contract)
        {
            cmbProperty.SelectedValue = contract.FkIdProperty;
            cmbTenant.SelectedValue = contract.FkIdTenant;
            txtPrice.Text = contract.AnnualRentPrice.ToString();
            cldStartDate.Value = contract.DateStartContract;
            cldFinalDate.Value = contract.DateFinalContract;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtPrice.Text, out int price))
            {
                MessageBox.Show("El precio mensual debe ser un valor numérico.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Obtener el ID del contrato seleccionado desde el DataGridView
            var selectedContractId = (Guid)dgvContracts.CurrentRow.Cells["IdContract"].Value;

            // Buscar el contrato en la lista original usando el ID
            var contract = _originalContracts.FirstOrDefault(c => c.IdContract == selectedContractId);
            if (contract == null)
            {
                MessageBox.Show("No se encontró el contrato seleccionado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Actualizar los datos del contrato seleccionado
            contract.FkIdProperty = (Guid)cmbProperty.SelectedValue;
            contract.FkIdTenant = (Guid)cmbTenant.SelectedValue;
            contract.AnnualRentPrice = price;
            contract.DateStartContract = cldStartDate.Value;
            contract.DateFinalContract = cldFinalDate.Value;
            contract.StatusContract = "Inactivo"; // Puedes establecer el estado que desees aquí

            _contractService.UpdateContract(contract);

            var result = MessageBox.Show("¿Desea modificar las cláusulas del contrato?", "Modificar Cláusulas", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                // Crear una nueva instancia del formulario frmModifyContractClause
                frmModifyContractClause ModifyContractClause = new frmModifyContractClause(contract);

                // Establecer el formulario frmModifyContractClause como el padre MDI
                ModifyContractClause.MdiParent = this.MdiParent;

                // Manejar el evento FormClosed para volver a mostrar el formulario actual cuando se cierre el formulario hijo
                ModifyContractClause.FormClosed += (s, args) => this.Show();

                // Mostrar el formulario hijo
                ModifyContractClause.Show();
            }

            LoadContracts(); // Recargar contratos
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvContracts.CurrentRow == null)
            {
                MessageBox.Show("Por favor, seleccione un contrato para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener el ID del contrato seleccionado
            var selectedContractId = (Guid)dgvContracts.CurrentRow.Cells["IdContract"].Value;

            var result = MessageBox.Show("¿Está seguro de que desea eliminar el contrato seleccionado y todas sus cláusulas asociadas?",
                                         "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Eliminar el contrato seleccionado y sus cláusulas
                _contractService.DeleteContract(selectedContractId);

                MessageBox.Show("Contrato eliminado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Actualizar el DataGridView para reflejar la eliminación
                LoadContracts();
            }
        }
    }
}
