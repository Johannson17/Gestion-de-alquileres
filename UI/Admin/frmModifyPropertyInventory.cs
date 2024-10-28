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
        private readonly PropertyService _propertyService; // Servicio para aplicar cambios

        public frmModifyPropertyInventory(Property property)
        {
            InitializeComponent();
            _selectedProperty = property;
            _propertyService = new PropertyService(); // Instanciar el servicio
            LoadInventory();

            // Vincular el evento SelectionChanged del DataGridView
            dgvInventory.SelectionChanged += dgvInventory_SelectionChanged;
        }

        /// <summary>
        /// Método para cargar el inventario en el DataGridView
        /// </summary>
        private void LoadInventory()
        {
            if (_selectedProperty.InventoryProperty == null || _selectedProperty.InventoryProperty.Count == 0)
            {
                MessageBox.Show("No hay inventario para esta propiedad.", "Inventario vacío", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Asignar el inventario al DataGridView con solo las columnas NameInventory y DescriptionInventory
            dgvInventory.DataSource = _selectedProperty.InventoryProperty
                .Select(i => new
                {
                    i.NameInventory,
                    i.DescriptionInventory
                }).ToList();
        }

        /// <summary>
        /// Método manejador del evento de cambio de selección del DataGridView
        /// </summary>
        private void dgvInventory_SelectionChanged(object sender, EventArgs e)
        {
            // Verificar si hay una fila seleccionada
            if (dgvInventory.SelectedRows.Count > 0)
            {
                // Obtener el nombre del inventario seleccionado
                var selectedInventoryName = (string)dgvInventory.SelectedRows[0].Cells["NameInventory"].Value;

                // Encontrar el elemento de inventario correspondiente
                var inventoryItem = _selectedProperty.InventoryProperty.Find(i => i.NameInventory == selectedInventoryName);

                if (inventoryItem != null)
                {
                    // Rellenar los campos de texto con los valores del inventario seleccionado
                    txtName.Text = inventoryItem.NameInventory;
                    txtDescription.Text = inventoryItem.DescriptionInventory;
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // Verificar si hay alguna fila seleccionada en el DataGridView
                if (dgvInventory.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Debe seleccionar un elemento de inventario para eliminar.", "Inventario no seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Confirmar la eliminación
                var confirmResult = MessageBox.Show("¿Está seguro de que desea eliminar este elemento de inventario?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmResult == DialogResult.No)
                    return;

                // Obtener el nombre del inventario seleccionado
                var selectedInventoryName = (string)dgvInventory.SelectedRows[0].Cells["NameInventory"].Value;

                // Encontrar el elemento de inventario correspondiente
                var inventoryItem = _selectedProperty.InventoryProperty.Find(i => i.NameInventory == selectedInventoryName);

                if (inventoryItem == null)
                {
                    MessageBox.Show("No se encontró el inventario seleccionado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Eliminar el inventario de la base de datos
                _propertyService.DeleteInventory(_selectedProperty.IdProperty, inventoryItem.IdInventoryProperty); // Asegúrate de tener este método implementado en tu servicio y lógica

                // Eliminar el elemento de inventario de la lista en memoria
                _selectedProperty.InventoryProperty.Remove(inventoryItem);

                // Refrescar el DataGridView para mostrar los cambios
                dgvInventory.DataSource = null;
                dgvInventory.DataSource = _selectedProperty.InventoryProperty
                    .Select(i => new
                    {
                        i.NameInventory,
                        i.DescriptionInventory
                    }).ToList();

                MessageBox.Show("Elemento de inventario eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el elemento de inventario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Botón para guardar los cambios realizados en el inventario.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Verificar si hay alguna fila seleccionada en el DataGridView
                if (dgvInventory.SelectedRows.Count == 0)
                {
                    // No hay ningún inventario seleccionado, agregar uno nuevo
                    AddNewInventory();
                }
                else
                {
                    // Preguntar si desea modificar el seleccionado o agregar uno nuevo
                    var result = MessageBox.Show("¿Desea modificar el inventario seleccionado? Si selecciona 'No', se agregará un nuevo inventario.",
                        "Modificar o agregar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Modificar el inventario seleccionado
                        ModifySelectedInventory();
                    }
                    else
                    {
                        // Agregar un nuevo inventario
                        AddNewInventory();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar los cambios en el inventario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Método para modificar el inventario seleccionado.
        /// </summary>
        private void ModifySelectedInventory()
        {
            // Obtener el nombre del inventario seleccionado
            var selectedInventoryName = (string)dgvInventory.SelectedRows[0].Cells["NameInventory"].Value;

            // Encontrar el elemento de inventario correspondiente
            var inventoryItem = _selectedProperty.InventoryProperty.Find(i => i.NameInventory == selectedInventoryName);

            if (inventoryItem == null)
            {
                MessageBox.Show("No se encontró el inventario seleccionado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Actualizar los valores de inventario con los datos del formulario
            inventoryItem.NameInventory = txtName.Text;
            inventoryItem.DescriptionInventory = txtDescription.Text;

            // Guardar los cambios en el servicio
            _propertyService.UpdateProperty(_selectedProperty); // Guardar los cambios en la propiedad y su inventario

            MessageBox.Show("Cambios en el inventario guardados correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK; // Indicar que se guardaron los cambios
            this.Close(); // Cerrar el formulario de inventario
        }

        /// <summary>
        /// Método para agregar un nuevo inventario.
        /// </summary>
        private void AddNewInventory()
        {
            // Crear un nuevo objeto de inventario
            var newInventoryItem = new InventoryProperty
            {
                NameInventory = txtName.Text,
                DescriptionInventory = txtDescription.Text
            };

            // Agregarlo a la propiedad seleccionada
            _selectedProperty.InventoryProperty.Add(newInventoryItem);

            // Guardar los cambios en el servicio
            _propertyService.UpdateProperty(_selectedProperty); // Guardar los cambios en la propiedad y su inventario

            // Refrescar el DataGridView para mostrar el nuevo inventario
            dgvInventory.DataSource = null;
            dgvInventory.DataSource = _selectedProperty.InventoryProperty
                .Select(i => new
                {
                    i.NameInventory,
                    i.DescriptionInventory
                }).ToList();

            MessageBox.Show("Nuevo inventario añadido correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}