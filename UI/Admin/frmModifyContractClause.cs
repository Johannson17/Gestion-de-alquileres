using Domain;
using Services.Facade;
using Services.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    public partial class frmModifyContractClause : Form
    {
        private readonly ContractService _contractService;
        private readonly Contract _contract;
        private Guid _selectedClauseId;

        private readonly Dictionary<Control, string> helpMessages;
        private Timer toolTipTimer;
        private Control currentControl; // Control actual donde está el mouse

        public frmModifyContractClause(Contract contract)
        {
            InitializeComponent();
            _contractService = new ContractService();
            _contract = contract;
            _selectedClauseId = Guid.Empty;

            this.Load += frmModifyContractClause_Load;
            dgvContractClauses.CellClick += dgvContractClauses_CellClick;
            btnSave.Click += btnSave_Click;
            btnDelete.Click += btnDelete_Click;

            // Inicializar mensajes de ayuda
            helpMessages = new Dictionary<Control, string>
            {
                { txtTittle, "Ingrese el título de la cláusula." },
                { txtDescription, "Ingrese la descripción de la cláusula." },
                { btnSave, "Guarda los cambios realizados en la cláusula o agrega una nueva." },
                { btnDelete, "Elimina la cláusula seleccionada del contrato." },
                { dgvContractClauses, "Lista de cláusulas del contrato. Seleccione una para editarla." }
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

        private void frmModifyContractClause_Load(object sender, EventArgs e)
        {
            LoadClauses();
        }

        private void LoadClauses()
        {
            try
            {
                var clauses = _contractService.GetContractClauses(_contract.IdContract);

                var displayClauses = clauses.Select(clause => new
                {
                    clause.IdContractClause,
                    clause.TitleClause,
                    clause.DetailClause
                }).ToList();

                dgvContractClauses.DataSource = displayClauses;

                dgvContractClauses.Columns["IdContractClause"].Visible = false;
                dgvContractClauses.Columns["TitleClause"].HeaderText = LanguageService.Translate("Título de la Cláusula");
                dgvContractClauses.Columns["DetailClause"].HeaderText = LanguageService.Translate("Descripción de la Cláusula");

                dgvContractClauses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar las cláusulas") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void dgvContractClauses_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    var selectedClause = _contractService.GetContractClauses(_contract.IdContract)[e.RowIndex];

                    if (selectedClause != null)
                    {
                        _selectedClauseId = selectedClause.IdContractClause;
                        txtTittle.Text = selectedClause.TitleClause;
                        txtDescription.Text = selectedClause.DetailClause;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        LanguageService.Translate("Error al seleccionar la cláusula") + ": " + ex.Message,
                        LanguageService.Translate("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTittle.Text) || string.IsNullOrWhiteSpace(txtDescription.Text))
                {
                    MessageBox.Show(
                        LanguageService.Translate("Por favor, complete ambos campos antes de guardar la cláusula."),
                        LanguageService.Translate("Advertencia"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                if (_selectedClauseId != Guid.Empty)
                {
                    var clause = new ContractClause
                    {
                        IdContractClause = _selectedClauseId,
                        FkIdContract = _contract.IdContract,
                        TitleClause = txtTittle.Text,
                        DetailClause = txtDescription.Text
                    };

                    _contractService.UpdateContractClause(clause);
                    MessageBox.Show(
                        LanguageService.Translate("Cláusula modificada con éxito."),
                        LanguageService.Translate("Éxito"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    var newClause = new ContractClause
                    {
                        IdContractClause = Guid.NewGuid(),
                        FkIdContract = _contract.IdContract,
                        TitleClause = txtTittle.Text,
                        DetailClause = txtDescription.Text
                    };

                    _contractService.AddContractClause(newClause);
                    MessageBox.Show(
                        LanguageService.Translate("Cláusula agregada con éxito."),
                        LanguageService.Translate("Éxito"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }

                ClearForm();
                LoadClauses();
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedClauseId == Guid.Empty)
                {
                    MessageBox.Show(
                        LanguageService.Translate("Por favor, seleccione una cláusula para eliminar."),
                        LanguageService.Translate("Advertencia"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                var result = MessageBox.Show(
                    LanguageService.Translate("¿Está seguro de que desea eliminar la cláusula seleccionada?"),
                    LanguageService.Translate("Confirmar eliminación"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    _contractService.DeleteContractClause(_selectedClauseId);
                    MessageBox.Show(
                        LanguageService.Translate("Cláusula eliminada con éxito."),
                        LanguageService.Translate("Éxito"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    ClearForm();
                    LoadClauses();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al eliminar la cláusula") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void ClearForm()
        {
            _selectedClauseId = Guid.Empty;
            txtTittle.Clear();
            txtDescription.Clear();
        }
    }
}
