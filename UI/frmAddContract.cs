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
            LoadProperties();
            LoadTenants();
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbProperty.SelectedItem == null || cmbTenant.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccione una propiedad y un arrendatario.");
                return;
            }

            if (!int.TryParse(txtPrice.Text, out int annualRentPrice))
            {
                MessageBox.Show("Por favor, ingrese un precio válido (solo números).");
                return;
            }

            var property = (Property)cmbProperty.SelectedItem;
            var tenant = (Person)cmbTenant.SelectedItem;

            try
            {
                var owner = _personService.GetPersonByPropertyAndType(property.IdProperty, PersonTypeEnum.Owner);
                var contract = new Contract
                {
                    FkIdProperty = property.IdProperty,
                    FkIdTenant = tenant.IdPerson,
                    AnnualRentPrice = annualRentPrice,
                    DateStartContract = cldStartDate.Value,
                    DateFinalContract = cldFinalDate.Value
                };

                // Abre el formulario para añadir las cláusulas
                OpenAddClauseForm(contract, owner, tenant, property);

                var contractId = _contractFacade.CreateContract(contract, owner, tenant, property);
                MessageBox.Show($"Contrato creado con éxito. ID: {contractId}");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear el contrato: {ex.Message}");
            }
        }

        private Contract OpenAddClauseForm(Contract contract, Person owner, Person tenant, Property property)
        {
            var clauseIndex = 1;
            var addMore = true;

            // Abre el formulario recursivamente
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
                        addMore = clauseIndex > 3 ? MessageBox.Show("¿Desea añadir más cláusulas?", "Agregar Cláusula", MessageBoxButtons.YesNo) == DialogResult.Yes : true;
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
