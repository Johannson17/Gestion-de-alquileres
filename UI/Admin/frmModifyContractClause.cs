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
        private Guid _selectedClauseId; // Variable para almacenar el ID de la cláusula seleccionada

        public frmModifyContractClause(Contract contract)
        {
            InitializeComponent();
            _contractService = new ContractService();
            _contract = contract;
            this.Load += frmModifyContractClause_Load;
            dgvContractClauses.CellClick += dgvContractClauses_CellClick; // Vincula el evento CellClick
            _selectedClauseId = Guid.Empty; // Inicializa sin selección
        }

        private void frmModifyContractClause_Load(object sender, EventArgs e)
        {
            LoadClauses();
        }

        private void LoadClauses()
        {
            // Obtiene las cláusulas del contrato actual usando el ID del contrato
            var clauses = _contractService.GetContractClauses(_contract.IdContract);

            // Configurar solo las columnas que deseas mostrar
            var displayClauses = clauses.Select(clause => new
            {
                clause.IdContractClause, // Incluye el ID de la cláusula para uso interno
                clause.TitleClause,
                clause.DetailClause
            }).ToList();

            dgvContractClauses.DataSource = displayClauses;

            // Configurar los encabezados de las columnas
            dgvContractClauses.Columns["IdContractClause"].Visible = false; // Ocultar la columna de ID
            dgvContractClauses.Columns["TitleClause"].HeaderText = "Título de la Cláusula";
            dgvContractClauses.Columns["DetailClause"].HeaderText = "Descripción de la Cláusula";
        }

        private void dgvContractClauses_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Obtener la cláusula seleccionada
                var selectedClause = _contractService.GetContractClauses(_contract.IdContract)[e.RowIndex];
                if (selectedClause != null)
                {
                    // Guardar el ID de la cláusula seleccionada
                    _selectedClauseId = selectedClause.IdContractClause;

                    // Mostrar los datos en los campos de texto
                    txtTittle.Text = selectedClause.TitleClause;
                    txtDescription.Text = selectedClause.DetailClause;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTittle.Text) || string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Por favor, complete ambos campos antes de guardar la cláusula.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Si se ha seleccionado una cláusula, preguntar si desea modificarla o crear una nueva
            if (_selectedClauseId != Guid.Empty)
            {
                var result = MessageBox.Show("¿Desea modificar la cláusula seleccionada o crear una nueva?", "Guardar cláusula",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                if (result == DialogResult.Yes)
                {
                    // Modificar la cláusula seleccionada
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
                    // Crear una nueva cláusula
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
                // Si el resultado es Cancel, no se realiza ninguna acción.
            }
            else
            {
                // Si no se ha seleccionado una cláusula, crear una nueva
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

            // Limpiar el formulario y actualizar el DataGridView
            _selectedClauseId = Guid.Empty; // Restablecer la selección
            txtTittle.Clear();
            txtDescription.Clear();
            LoadClauses();
        }

        private void btnDelete_Click(object sender, EventArgs e)
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
                // Eliminar la cláusula seleccionada
                _contractService.DeleteContractClause(_selectedClauseId);
                MessageBox.Show("Cláusula eliminada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Restablecer la selección y actualizar el DataGridView
                _selectedClauseId = Guid.Empty;
                txtTittle.Clear();
                txtDescription.Clear();
                LoadClauses();
            }
        }
    }
}
