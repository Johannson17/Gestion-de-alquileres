using Domain;
using Services.Facade;
using Services.Domain;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace UI
{
    public partial class frmModifyContractClause : Form
    {
        private readonly ContractService _contractService;
        private readonly Contract _contract;
        private Guid _selectedClauseId;

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
        }

        /// <summary>
        /// Carga inicial de las cláusulas del contrato.
        /// </summary>
        private void frmModifyContractClause_Load(object sender, EventArgs e)
        {
            LoadClauses();
        }

        /// <summary>
        /// Cargar las cláusulas del contrato actual en el DataGridView.
        /// </summary>
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
                dgvContractClauses.Columns["TitleClause"].HeaderText = "Título de la Cláusula";
                dgvContractClauses.Columns["DetailClause"].HeaderText = "Descripción de la Cláusula";

                dgvContractClauses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar las cláusulas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Maneja el clic en una celda del DataGridView para mostrar los datos de la cláusula seleccionada.
        /// </summary>
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
                    MessageBox.Show($"Error al seleccionar la cláusula: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Guarda los cambios realizados en la cláusula o agrega una nueva.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTittle.Text) || string.IsNullOrWhiteSpace(txtDescription.Text))
                {
                    MessageBox.Show("Por favor, complete ambos campos antes de guardar la cláusula.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_selectedClauseId != Guid.Empty)
                {
                    var result = MessageBox.Show("¿Desea modificar la cláusula seleccionada o crear una nueva?", "Guardar cláusula",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        var clause = new ContractClause
                        {
                            IdContractClause = _selectedClauseId,
                            FkIdContract = _contract.IdContract,
                            TitleClause = txtTittle.Text,
                            DetailClause = txtDescription.Text
                        };

                        _contractService.UpdateContractClause(clause);
                        MessageBox.Show("Cláusula modificada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (result == DialogResult.No)
                    {
                        AddNewClause();
                    }
                }
                else
                {
                    AddNewClause();
                }

                ClearForm();
                LoadClauses();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la cláusula: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Agrega una nueva cláusula al contrato.
        /// </summary>
        private void AddNewClause()
        {
            var newClause = new ContractClause
            {
                IdContractClause = Guid.NewGuid(),
                FkIdContract = _contract.IdContract,
                TitleClause = txtTittle.Text,
                DetailClause = txtDescription.Text
            };

            _contractService.AddContractClause(newClause);
            MessageBox.Show("Cláusula agregada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Elimina la cláusula seleccionada.
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedClauseId == Guid.Empty)
                {
                    MessageBox.Show("Por favor, seleccione una cláusula para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show("¿Está seguro de que desea eliminar la cláusula seleccionada?", "Confirmar eliminación",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _contractService.DeleteContractClause(_selectedClauseId);
                    MessageBox.Show("Cláusula eliminada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ClearForm();
                    LoadClauses();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar la cláusula: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Limpia los campos del formulario.
        /// </summary>
        private void ClearForm()
        {
            _selectedClauseId = Guid.Empty;
            txtTittle.Clear();
            txtDescription.Clear();
        }
    }
}
