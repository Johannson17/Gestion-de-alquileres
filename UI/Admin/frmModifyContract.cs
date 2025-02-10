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

        private readonly Dictionary<Control, string> helpMessages;
        private Timer toolTipTimer;
        private Control currentControl; // Control actual donde está el mouse

        public frmModifyContract()
        {
            InitializeComponent();
            _contractService = new ContractService();
            _propertyService = new PropertyService();
            _personService = new PersonService();
            this.Load += frmModifyContract_Load;
            dgvContracts.CellClick += dgvContracts_CellClick;

            // Inicializar mensajes de ayuda
            helpMessages = new Dictionary<Control, string>
            {
                { cmbProperty, "Seleccione la propiedad asociada al contrato." },
                { cmbTenant, "Seleccione el inquilino asociado al contrato." },
                { txtPrice, "Ingrese el precio mensual del contrato en números." },
                { cldStartDate, "Seleccione la fecha de inicio del contrato." },
                { cldFinalDate, "Seleccione la fecha de finalización del contrato." },
                { btnSave, "Guarda los cambios realizados en el contrato seleccionado." },
                { btnDelete, "Elimina el contrato seleccionado." },
                { dgvContracts, "Lista de contratos disponibles. Haga clic en un contrato para editarlo." }
            };

            // Configurar el Timer
            toolTipTimer = new Timer();
            toolTipTimer.Interval = 1000; // 2 segundos
            toolTipTimer.Tick += ToolTipTimer_Tick;

            // Suscribir eventos a los controles
            foreach (var control in helpMessages.Keys)
            {
                control.MouseEnter += Control_MouseEnter;
                control.MouseLeave += Control_MouseLeave;
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

            toolTipTimer.Stop(); // Detener el temporizador
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
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar los datos") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
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
                    PropertyAddress = properties.ContainsKey(contract.FkIdProperty) ? properties[contract.FkIdProperty] : LanguageService.Translate("Dirección no encontrada"),
                    StartDate = contract.DateStartContract,
                    EndDate = contract.DateFinalContract,
                    AnnualRentPrice = contract.AnnualRentPrice,
                    IsActive = contract.StatusContract
                }).ToList();

                dgvContracts.DataSource = displayContracts;

                dgvContracts.Columns["IdContract"].Visible = false;
                dgvContracts.Columns["PropertyAddress"].HeaderText = LanguageService.Translate("Propiedad");
                dgvContracts.Columns["StartDate"].HeaderText = LanguageService.Translate("Fecha de Inicio");
                dgvContracts.Columns["EndDate"].HeaderText = LanguageService.Translate("Fecha de Finalización");
                dgvContracts.Columns["AnnualRentPrice"].HeaderText = LanguageService.Translate("Precio Mensual");
                dgvContracts.Columns["IsActive"].HeaderText = LanguageService.Translate("Estado");
                dgvContracts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar los contratos") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
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

        private void LoadTenants()
        {
            try
            {
                var tenants = _personService.GetAllPersonsByType(PersonTypeEnum.Tenant);
                cmbTenant.DataSource = tenants;
                cmbTenant.DisplayMember = "NumberDocumentPerson";
                cmbTenant.ValueMember = "IdPerson";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar los inquilinos") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
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
                    MessageBox.Show(
                        LanguageService.Translate("El precio debe ser numérico."),
                        LanguageService.Translate("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
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

                    var result = MessageBox.Show(
                        LanguageService.Translate("¿Desea modificar las cláusulas del contrato?"),
                        LanguageService.Translate("Modificar Cláusulas"),
                        MessageBoxButtons.YesNo
                    );
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
                MessageBox.Show(
                    LanguageService.Translate("Error al guardar el contrato") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvContracts.CurrentRow == null)
                {
                    MessageBox.Show(
                        LanguageService.Translate("Seleccione un contrato para eliminar."),
                        LanguageService.Translate("Advertencia"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                var selectedContractId = (Guid)dgvContracts.CurrentRow.Cells["IdContract"].Value;
                var result = MessageBox.Show(
                    LanguageService.Translate("¿Desea eliminar el contrato y sus cláusulas asociadas?"),
                    LanguageService.Translate("Confirmar"),
                    MessageBoxButtons.YesNo
                );

                if (result == DialogResult.Yes)
                {
                    _contractService.DeleteContract(selectedContractId);
                    MessageBox.Show(
                        LanguageService.Translate("Contrato eliminado con éxito."),
                        LanguageService.Translate("Éxito"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    LoadContracts();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al eliminar el contrato") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
