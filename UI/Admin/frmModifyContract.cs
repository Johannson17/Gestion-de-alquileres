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
        private List<Contract> _originalContracts;

        public frmModifyContract()
        {
            InitializeComponent();
            _contractService = new ContractService();
            _propertyService = new PropertyService();
            _personService = new PersonService();
            this.Load += frmModifyContract_Load;
            dgvContracts.CellClick += dgvContracts_CellClick;
        }

        private void frmModifyContract_Load(object sender, EventArgs e)
        {
            try
            {
                LoadContracts();
                LoadProperties();
                LoadTenants();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadContracts()
        {
            try
            {
                _originalContracts = _contractService.GetAllContracts();
                var properties = _propertyService.GetAllProperties()
                    .ToDictionary(p => p.IdProperty, p => p.AddressProperty);

                var displayContracts = _originalContracts.Select(contract => new
                {
                    IdContract = contract.IdContract,
                    PropertyAddress = properties.ContainsKey(contract.FkIdProperty) ? properties[contract.FkIdProperty] : "Dirección no encontrada",
                    StartDate = contract.DateStartContract,
                    EndDate = contract.DateFinalContract,
                    AnnualRentPrice = contract.AnnualRentPrice,
                    IsActive = contract.StatusContract
                }).ToList();

                dgvContracts.DataSource = displayContracts;

                dgvContracts.Columns["IdContract"].Visible = false;
                dgvContracts.Columns["PropertyAddress"].HeaderText = "Propiedad";
                dgvContracts.Columns["StartDate"].HeaderText = "Fecha de Inicio";
                dgvContracts.Columns["EndDate"].HeaderText = "Fecha de Finalización";
                dgvContracts.Columns["AnnualRentPrice"].HeaderText = "Precio Mensual";
                dgvContracts.Columns["IsActive"].HeaderText = "Estado";
                dgvContracts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Ajustar columnas al ancho completo
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los contratos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProperties()
        {
            try
            {
                var properties = _propertyService.GetPropertiesByStatus(PropertyStatusEnum.Disponible);
                cmbProperty.DataSource = properties;
                cmbProperty.DisplayMember = "AddressProperty";
                cmbProperty.ValueMember = "IdProperty";
                dgvContracts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Ajustar columnas al ancho completo
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar las propiedades: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTenants()
        {
            try
            {
                var tenants = _personService.GetAllPersonsByType(PersonTypeEnum.Tenant);
                cmbTenant.DataSource = tenants;
                cmbTenant.DisplayMember = "NumberDocumentPerson";
                cmbTenant.ValueMember = "IdPerson";
                dgvContracts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Ajustar columnas al ancho completo
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los inquilinos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvContracts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selectedContractId = (Guid)dgvContracts.Rows[e.RowIndex].Cells["IdContract"].Value;
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
            try
            {
                if (!int.TryParse(txtPrice.Text, out int price))
                {
                    MessageBox.Show("El precio debe ser numérico.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var selectedContractId = (Guid)dgvContracts.CurrentRow.Cells["IdContract"].Value;
                var contract = _originalContracts.FirstOrDefault(c => c.IdContract == selectedContractId);

                if (contract != null)
                {
                    contract.FkIdProperty = (Guid)cmbProperty.SelectedValue;
                    contract.FkIdTenant = (Guid)cmbTenant.SelectedValue;
                    contract.AnnualRentPrice = price;
                    contract.DateStartContract = cldStartDate.Value;
                    contract.DateFinalContract = cldFinalDate.Value;

                    _contractService.UpdateContract(contract);

                    var result = MessageBox.Show("¿Desea modificar las cláusulas del contrato?", "Modificar Cláusulas", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        var modifyContractClause = new frmModifyContractClause(contract)
                        {
                            MdiParent = this.MdiParent
                        };
                        modifyContractClause.FormClosed += (s, args) => this.Show();
                        modifyContractClause.Show();
                    }

                    LoadContracts();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el contrato: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvContracts.CurrentRow == null)
                {
                    MessageBox.Show("Seleccione un contrato para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedContractId = (Guid)dgvContracts.CurrentRow.Cells["IdContract"].Value;
                var result = MessageBox.Show("¿Desea eliminar el contrato y sus cláusulas asociadas?", "Confirmar", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    _contractService.DeleteContract(selectedContractId);
                    MessageBox.Show("Contrato eliminado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadContracts();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar el contrato: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
