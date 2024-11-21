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

        public frmAddContract()
        {
            InitializeComponent();
            _contractFacade = new ContractService();
            _propertyService = new PropertyService();
            _personService = new PersonService();
            this.Load += frmAddContract_Load;
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

                // Abre el formulario para añadir las cláusulas
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
                        // Agrega la cláusula devuelta al contrato
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
    }
}
