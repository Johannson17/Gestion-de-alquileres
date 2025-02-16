using Domain;
using LOGIC.Facade;
using Services.Domain;
using Services.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static Domain.Person;

namespace UI
{
    public partial class frmAddContract : Form
    {
        private readonly ContractService _contractFacade;
        private readonly PropertyService _propertyService;
        private readonly PersonService _personService;
        private readonly Timer toolTipTimer;
        private Control currentControl;

        public frmAddContract()
        {
            InitializeComponent();
            _contractFacade = new ContractService();
            _propertyService = new PropertyService();
            _personService = new PersonService();

            toolTipTimer = new Timer { Interval = 1000 }; // 1 segundo
            toolTipTimer.Tick += ToolTipTimer_Tick;

            InitializeHelpMessages();
            SubscribeToMouseEvents();

            this.Load += frmAddContract_Load;
        }

        /// <summary>
        /// Diccionario de mensajes de ayuda para los controles.
        /// </summary>
        private Dictionary<Control, string> helpMessages = new Dictionary<Control, string>();

        /// <summary>
        /// Inicializa los mensajes de ayuda con la traducción actual.
        /// </summary>
        private void InitializeHelpMessages()
        {
            helpMessages = new Dictionary<Control, string>
            {
                { cmbProperty, LanguageService.Translate("Seleccione la propiedad disponible para asociarla al contrato.") },
                { cmbTenant, LanguageService.Translate("Seleccione el arrendatario para este contrato.") },
                { txtPrice, LanguageService.Translate("Ingrese el precio mensual del contrato (solo números).") },
                { cldStartDate, LanguageService.Translate("Seleccione la fecha de inicio del contrato.") },
                { cldFinalDate, LanguageService.Translate("Seleccione la fecha de finalización del contrato.") },
                { btnSave, LanguageService.Translate("Presione para guardar el contrato con los datos ingresados.") }
            };
        }

        /// <summary>
        /// Suscribe eventos `MouseEnter` y `MouseLeave` a los controles con mensajes de ayuda.
        /// </summary>
        private void SubscribeToMouseEvents()
        {
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
                currentControl = control;
                toolTipTimer.Start();
            }
        }

        private void Control_MouseLeave(object sender, EventArgs e)
        {
            toolTipTimer.Stop();
            currentControl = null;
        }

        private void ToolTipTimer_Tick(object sender, EventArgs e)
        {
            if (currentControl != null && helpMessages.ContainsKey(currentControl))
            {
                ToolTip toolTip = new ToolTip();
                toolTip.Show(helpMessages[currentControl], currentControl, 3000);
            }
            toolTipTimer.Stop();
        }

        private void frmAddContract_Load(object sender, EventArgs e)
        {
            try
            {
                LoadProperties();
                LoadTenants();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar los datos iniciales") + ": " + ex.Message,
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbProperty.SelectedItem == null || cmbTenant.SelectedItem == null)
                {
                    MessageBox.Show(
                        LanguageService.Translate("Por favor, seleccione una propiedad y un arrendatario."),
                        LanguageService.Translate("Advertencia"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                if (!int.TryParse(txtPrice.Text, out int annualRentPrice))
                {
                    MessageBox.Show(
                        LanguageService.Translate("Por favor, ingrese un precio válido (solo números)."),
                        LanguageService.Translate("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                var property = (Property)cmbProperty.SelectedItem;
                var tenant = (Person)cmbTenant.SelectedItem;

                var owner = _personService.GetPersonByPropertyAndType(property.IdProperty, PersonTypeEnum.Owner);
                var contract = new Contract
                {
                    FkIdProperty = property.IdProperty,
                    FkIdTenant = tenant.IdPerson,
                    AnnualRentPrice = annualRentPrice,
                    DateStartContract = cldStartDate.Value,
                    DateFinalContract = cldFinalDate.Value,
                    Clauses = new List<ContractClause>()
                };

                OpenAddClauseForm(contract, owner, tenant, property);

                var contractId = _contractFacade.CreateContract(contract, owner, tenant, property);
                MessageBox.Show(
                    LanguageService.Translate("Contrato creado con éxito") + $". {LanguageService.Translate("ID")}: {contractId}",
                    LanguageService.Translate("Éxito"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al crear el contrato") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private Contract OpenAddClauseForm(Contract contract, Person owner, Person tenant, Property property)
        {
            var clauseIndex = 1;
            var addMore = true;

            while (addMore)
            {
                using (var frmClause = new frmAddContractClause(contract, owner, tenant, property, clauseIndex))
                {
                    var result = frmClause.ShowDialog();
                    if (result == DialogResult.OK && frmClause.NewClause != null)
                    {
                        contract.Clauses.Add(frmClause.NewClause);

                        clauseIndex++;
                        addMore = clauseIndex > 3 ? MessageBox.Show(
                            LanguageService.Translate("¿Desea añadir más cláusulas?"),
                            LanguageService.Translate("Agregar Cláusula"),
                            MessageBoxButtons.YesNo
                        ) == DialogResult.Yes : true;
                    }
                    else
                    {
                        addMore = false;
                    }
                }
            }
            return contract;
        }

        private void FrmAddContract_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helpMessage = string.Join(Environment.NewLine, new[]
            {
                LanguageService.Translate("Bienvenido al módulo de creación de contratos."),
                "",
                LanguageService.Translate("Opciones disponibles:"),
                $"- {LanguageService.Translate("Seleccionar una propiedad y un arrendatario para crear un contrato.")}",
                $"- {LanguageService.Translate("Ingresar el precio del alquiler.")}",
                $"- {LanguageService.Translate("Seleccionar la fecha de inicio y fin del contrato.")}",
                $"- {LanguageService.Translate("Añadir cláusulas al contrato.")}",
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
