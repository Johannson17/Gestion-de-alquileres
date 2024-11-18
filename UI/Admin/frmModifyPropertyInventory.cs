using Domain;
using LOGIC.Facade;
using System;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    public partial class frmModifyPropertyInventory : Form
    {
        private Property _selectedProperty;
        private readonly PropertyService _propertyService;

        public frmModifyPropertyInventory(Property property)
        {
            InitializeComponent();
            _selectedProperty = property;
            _propertyService = new PropertyService();
            LoadInventory();

            // Vincular eventos
            dgvInventory.SelectionChanged += dgvInventory_SelectionChanged;
            btnSave.Click += btnSave_Click;
            btnDelete.Click += btnDelete_Click;
        }

        /// <summary>
        /// Método para cargar el inventario en el DataGridView.
        /// </summary>
        private void LoadInventory()
        {
            try
            {
                if (_selectedProperty.InventoryProperty == null || !_selectedProperty.InventoryProperty.Any())
                {
                    MessageBox.Show("No hay inventario para esta propiedad.", "Inventario vacío", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                dgvInventory.DataSource = _selectedProperty.InventoryProperty
                    .Select(i => new
                    {
                        i.NameInventory,
                        i.DescriptionInventory
                    }).ToList();

                dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Ajustar columnas al ancho del DataGridView
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el inventario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Evento que maneja el cambio de selección en el DataGridView.
        /// </summary>
        private void dgvInventory_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvInventory.SelectedRows.Count > 0)
            {
                try
                {
                    var selectedInventoryName = dgvInventory.SelectedRows[0].Cells["NameInventory"].Value.ToString();
                    var inventoryItem = _selectedProperty.InventoryProperty.FirstOrDefault(i => i.NameInventory == selectedInventoryName);

                    if (inventoryItem != null)
                    {
                        txtName.Text = inventoryItem.NameInventory;
                        txtDescription.Text = inventoryItem.DescriptionInventory;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al seleccionar el inventario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Botón para guardar cambios o agregar nuevo inventario.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvInventory.SelectedRows.Count == 0)
                {
                    AddNewInventory();
                }
                else
                {
                    var result = MessageBox.Show("¿Desea modificar el inventario seleccionado? Si elige 'No', se agregará un nuevo inventario.",
                        "Modificar o agregar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                        ModifySelectedInventory();
                    else
                        AddNewInventory();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar los cambios: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Botón para eliminar un elemento del inventario.
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvInventory.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Debe seleccionar un elemento de inventario para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmResult = MessageBox.Show("¿Está seguro de que desea eliminar este elemento de inventario?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirmResult == DialogResult.No) return;

                var selectedInventoryName = dgvInventory.SelectedRows[0].Cells["NameInventory"].Value.ToString();
                var inventoryItem = _selectedProperty.InventoryProperty.FirstOrDefault(i => i.NameInventory == selectedInventoryName);

                if (inventoryItem != null)
                {
                    _propertyService.DeleteInventory(_selectedProperty.IdProperty, inventoryItem.IdInventoryProperty);
                    _selectedProperty.InventoryProperty.Remove(inventoryItem);

                    RefreshInventoryGrid();
                    MessageBox.Show("Elemento eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar el elemento: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Modifica el inventario seleccionado.
        /// </summary>
        private void ModifySelectedInventory()
        {
            try
            {
                var selectedInventoryName = dgvInventory.SelectedRows[0].Cells["NameInventory"].Value.ToString();
                var inventoryItem = _selectedProperty.InventoryProperty.FirstOrDefault(i => i.NameInventory == selectedInventoryName);

                if (inventoryItem != null)
                {
                    inventoryItem.NameInventory = txtName.Text;
                    inventoryItem.DescriptionInventory = txtDescription.Text;

                    _propertyService.UpdateProperty(_selectedProperty);

                    MessageBox.Show("Cambios guardados correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshInventoryGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al modificar el inventario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Agrega un nuevo inventario a la propiedad.
        /// </summary>
        private void AddNewInventory()
        {
            try
            {
                var newInventory = new InventoryProperty
                {
                    NameInventory = txtName.Text,
                    DescriptionInventory = txtDescription.Text
                };

                _selectedProperty.InventoryProperty.Add(newInventory);
                _propertyService.UpdateProperty(_selectedProperty);

                RefreshInventoryGrid();
                MessageBox.Show("Nuevo inventario añadido correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar el inventario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Refresca el contenido del DataGridView con el inventario actualizado.
        /// </summary>
        private void RefreshInventoryGrid()
        {
            dgvInventory.DataSource = null;
            dgvInventory.DataSource = _selectedProperty.InventoryProperty
                .Select(i => new
                {
                    i.NameInventory,
                    i.DescriptionInventory
                }).ToList();

            dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }
}
