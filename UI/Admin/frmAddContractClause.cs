using Domain;
using Services.Facade;
using Services.Domain;
using System;
using System.Windows.Forms;

namespace UI
{
    public partial class frmAddContractClause : Form
    {
        private readonly Contract _contract;
        private readonly int _clauseIndex;
        private readonly Person _owner;
        private readonly Person _tenant;
        private readonly Property _property;
        private readonly ContractService _contractService;

        // Propiedad para devolver la cláusula creada al formulario principal
        public ContractClause NewClause { get; private set; }

        public frmAddContractClause(Contract contract, Person owner, Person tenant, Property property, int clauseIndex)
        {
            InitializeComponent();
            _contract = contract;
            _clauseIndex = clauseIndex;
            _owner = owner;
            _tenant = tenant;
            _property = property;
            _contractService = new ContractService(); // Usar ContractService en lugar de ContractLogic

            LoadClauseData();
        }

        private void LoadClauseData()
        {
            try
            {
                // Obtiene el título y descripción desde ContractService
                var (Clause1, Clause2) = _contractService.GetPredefinedClause(_clauseIndex, _owner, _tenant, _property);

                // Configura los campos según si es una cláusula predefinida o no
                if (_clauseIndex <= 1)
                {
                    txtTittle.Text = Clause1.TitleClause.ToString();
                    txtDescription.Text = Clause1.DetailClause.ToString();
                    txtTittle.ReadOnly = true;
                    txtDescription.ReadOnly = true;
                }
                else if (_clauseIndex == 2)
                {
                    txtTittle.Text = Clause2.TitleClause.ToString();
                    txtDescription.Text = Clause2.DetailClause.ToString();
                    txtTittle.ReadOnly = true;
                    txtDescription.ReadOnly = true;
                }
                else
                {
                    txtTittle.Text = string.Empty;
                    txtDescription.Text = string.Empty;
                    txtTittle.ReadOnly = false;
                    txtDescription.ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar los datos de la cláusula") + ": " + ex.Message,
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
                // Crear una nueva cláusula con los datos del formulario y almacenarla en la propiedad NewClause
                NewClause = new ContractClause
                {
                    IdContractClause = Guid.NewGuid(),
                    FkIdContract = _contract.IdContract,
                    TitleClause = txtTittle.Text,
                    DetailClause = txtDescription.Text
                };

                MessageBox.Show(
                    LanguageService.Translate("Cláusula guardada exitosamente."),
                    LanguageService.Translate("Éxito"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al guardar la cláusula") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}