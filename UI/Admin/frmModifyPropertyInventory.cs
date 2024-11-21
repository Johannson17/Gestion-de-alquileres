using Domain;
using LOGIC.Facade;
using Services.Facade;
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

            // Bind events
            dgvInventory.SelectionChanged += dgvInventory_SelectionChanged;
            btnSave.Click += btnSave_Click;
            btnDelete.Click += btnDelete_Click;
        }

        /// <summary>
        /// Method to load inventory into the DataGridView.
        /// </summary>
        private void LoadInventory()
        {
            try
            {
                if (_selectedProperty.InventoryProperty == null || !_selectedProperty.InventoryProperty.Any())
                {
                    MessageBox.Show(
                        LanguageService.Translate("No hay inventario para esta propiedad."),
                        LanguageService.Translate("Inventario vacío"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                dgvInventory.DataSource = _selectedProperty.InventoryProperty
                    .Select(i => new
                    {
                        i.NameInventory,
                        i.DescriptionInventory
                    }).ToList();

                dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Adjust columns to DataGridView width
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar el inventario") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Handles selection change event in the DataGridView.
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
                    MessageBox.Show(
                        LanguageService.Translate("Error al seleccionar el inventario") + ": " + ex.Message,
                        LanguageService.Translate("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        /// <summary>
        /// Button to save changes or add new inventory.
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
                    var result = MessageBox.Show(
                        LanguageService.Translate("¿Desea modificar el inventario seleccionado? Si elige 'No', se agregará un nuevo inventario."),
                        LanguageService.Translate("Modificar o agregar"),
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (result == DialogResult.Yes)
                        ModifySelectedInventory();
                    else
                        AddNewInventory();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al guardar los cambios") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Button to delete an inventory item.
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvInventory.SelectedRows.Count == 0)
                {
                    MessageBox.Show(
                        LanguageService.Translate("Debe seleccionar un elemento de inventario para eliminar."),
                        LanguageService.Translate("Advertencia"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                var confirmResult = MessageBox.Show(
                    LanguageService.Translate("¿Está seguro de que desea eliminar este elemento de inventario?"),
                    LanguageService.Translate("Confirmar eliminación"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );
                if (confirmResult == DialogResult.No) return;

                var selectedInventoryName = dgvInventory.SelectedRows[0].Cells["NameInventory"].Value.ToString();
                var inventoryItem = _selectedProperty.InventoryProperty.FirstOrDefault(i => i.NameInventory == selectedInventoryName);

                if (inventoryItem != null)
                {
                    _propertyService.DeleteInventory(_selectedProperty.IdProperty, inventoryItem.IdInventoryProperty);
                    _selectedProperty.InventoryProperty.Remove(inventoryItem);

                    RefreshInventoryGrid();
                    MessageBox.Show(
                        LanguageService.Translate("Elemento eliminado correctamente."),
                        LanguageService.Translate("Éxito"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al eliminar el elemento") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Modify the selected inventory.
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

                    MessageBox.Show(
                        LanguageService.Translate("Cambios guardados correctamente."),
                        LanguageService.Translate("Éxito"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    RefreshInventoryGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al modificar el inventario") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Add new inventory to the property.
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
                MessageBox.Show(
                    LanguageService.Translate("Nuevo inventario añadido correctamente."),
                    LanguageService.Translate("Éxito"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al agregar el inventario") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Refresh the DataGridView content with updated inventory.
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
