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

        private Dictionary<Control, string> helpMessages;
        private Timer toolTipTimer;
        private Control currentControl; // Control actual donde está el mouse

        public frmModifyContract()
        {
            InitializeComponent();
            _contractService = new ContractService();
            _propertyService = new PropertyService();
            _personService = new PersonService();

            // Inicializa los mensajes de ayuda traducidos
            InitializeHelpMessages();
            // Suscribe los eventos de ToolTips
            SubscribeHelpMessagesEvents();

            // Configurar el Timer
            toolTipTimer = new Timer
            {
                Interval = 1000 // 1 segundo
            };
            toolTipTimer.Tick += ToolTipTimer_Tick;

            this.Load += frmModifyContract_Load;
            dgvContracts.CellClick += dgvContracts_CellClick;
        }

        /// <summary>
        /// Inicializa los mensajes de ayuda con la traducción actual.
        /// </summary>
        private void InitializeHelpMessages()
        {
            helpMessages = new Dictionary<Control, string>
            {
                { cmbProperty, LanguageService.Translate("Seleccione la propiedad asociada al contrato.") },
                { cmbTenant, LanguageService.Translate("Seleccione el inquilino asociado al contrato.") },
                { txtPrice, LanguageService.Translate("Ingrese el precio mensual del contrato en números.") },
                { cldStartDate, LanguageService.Translate("Seleccione la fecha de inicio del contrato.") },
                { cldFinalDate, LanguageService.Translate("Seleccione la fecha de finalización del contrato.") },
                { btnSave, LanguageService.Translate("Guarda los cambios realizados en el contrato seleccionado.") },
                { btnDelete, LanguageService.Translate("Elimina el contrato seleccionado.") },
                { dgvContracts, LanguageService.Translate("Lista de contratos disponibles. Haga clic en un contrato para editarlo.") }
            };
        }

        /// <summary>
        /// Suscribe los eventos de ayuda (ToolTips) a cada control definido en <see cref="helpMessages"/>.
        /// </summary>
        private void SubscribeHelpMessagesEvents()
        {
            if (helpMessages != null)
            {
                foreach (var control in helpMessages.Keys)
                {
                    control.MouseEnter += Control_MouseEnter;
                    control.MouseLeave += Control_MouseLeave;
                }
            }
        }

        private void Control_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Control control && helpMessages.ContainsKey(control))
            {
                currentControl = control; // Guardar el control actual
                toolTipTimer.Start();      // Iniciar el temporizador
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

        /// <summary>
        /// Actualiza las ayudas cuando se cambia el idioma.
        /// </summary>
        public void UpdateHelpMessages()
        {
            InitializeHelpMessages();
        }

        private void FrmModifyContract_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helpMessage = string.Join(Environment.NewLine, new[]
            {
                LanguageService.Translate("Bienvenido al módulo de modificación de contratos."),
                "",
                LanguageService.Translate("Opciones disponibles:"),
                $"- {LanguageService.Translate("Seleccione un contrato de la lista para editarlo o eliminarlo.")}",
                $"- {LanguageService.Translate("Modifique la propiedad, inquilino, precio y fechas del contrato.")}",
                $"- {LanguageService.Translate("Presione 'Guardar' para actualizar los cambios.")}",
                $"- {LanguageService.Translate("Presione 'Eliminar' para borrar el contrato seleccionado.")}",
                "",
                LanguageService.Translate("Para más ayuda, contacte con el administrador.")
            });

            MessageBox.Show(helpMessage,
                            LanguageService.Translate("Ayuda del sistema"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }
    }
}
