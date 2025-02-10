using Domain;
using Services.Facade;
using Services.Domain;
using System;
using System.Collections.Generic;
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

        private readonly Timer toolTipTimer;
        private Control currentControl;

        // Diccionario para almacenar los mensajes de ayuda
        private readonly Dictionary<Control, string> helpMessages;

        public frmAddContractClause(Contract contract, Person owner, Person tenant, Property property, int clauseIndex)
        {
            InitializeComponent();
            _contract = contract;
            _clauseIndex = clauseIndex;
            _owner = owner;
            _tenant = tenant;
            _property = property;
            _contractService = new ContractService();

            toolTipTimer = new Timer
            {
                Interval = 1000 // 2 segundos
            };
            toolTipTimer.Tick += ToolTipTimer_Tick;

            // Inicializar mensajes de ayuda
            helpMessages = new Dictionary<Control, string>
            {
                { txtTittle, "Ingrese el título de la cláusula. Si es una cláusula predefinida, este campo estará bloqueado." },
                { txtDescription, "Ingrese la descripción detallada de la cláusula. Si es una cláusula predefinida, este campo estará bloqueado." },
                { btnSave, "Presione para guardar la cláusula actual en el contrato." }
            };

            SubscribeToMouseEvents();
            LoadClauseData();
        }

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
                toolTip.Show(helpMessages[currentControl], currentControl, 3000); // Mostrar durante 3 segundos
            }
            toolTipTimer.Stop();
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
