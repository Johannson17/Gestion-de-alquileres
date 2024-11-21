using Domain;
using System;
using Services.Facade;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    public partial class frmAddPropertyInventory : Form
    {
        private Property _property;  // Referencia al objeto Property existente

        public frmAddPropertyInventory(Property property)
        {
            InitializeComponent();
            _property = property;

            // Opción 1: Limpiar lista de inventario si es necesario
            // _property.InventoryProperty = new List<InventoryProperty>(); 
        }

        /// <summary>
        /// Maneja el evento de clic en el botón de guardar, donde se agregan los datos del inventario a la propiedad.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtDescription.Text))
                {
                    MessageBox.Show(
                        LanguageService.Translate("Por favor, complete todos los campos del inventario."),
                        LanguageService.Translate("Campos incompletos"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                // Crear un nuevo objeto InventoryProperty con los datos del formulario
                var newInventoryItem = new InventoryProperty
                {
                    IdInventoryProperty = Guid.NewGuid(), // Generar un nuevo ID para el inventario
                    NameInventory = txtName.Text,         // Nombre del inventario
                    DescriptionInventory = txtDescription.Text // Descripción del inventario
                };

                // Opción 2: Verificar si ya existe el inventario antes de agregarlo
                if (_property.InventoryProperty == null)
                {
                    _property.InventoryProperty = new List<InventoryProperty>();
                }

                if (!_property.InventoryProperty.Any(i => i.NameInventory == newInventoryItem.NameInventory && i.DescriptionInventory == newInventoryItem.DescriptionInventory))
                {
                    _property.InventoryProperty.Add(newInventoryItem);
                }

                // Preguntar si desea agregar más
                var result = MessageBox.Show(
                    LanguageService.Translate("¿Desea agregar más elementos de inventario?"),
                    LanguageService.Translate("Agregar más"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    // Limpiar el formulario para agregar otro elemento de inventario
                    ClearForm();
                }
                else
                {
                    // Cerrar el formulario si no se desean agregar más elementos
                    this.Close();
                }
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
        /// Limpia los campos del formulario.
        /// </summary>
        private void ClearForm()
        {
            txtName.Clear();
            txtDescription.Clear();
            txtName.Focus(); // Poner foco en el campo de nombre
        }
    }
}
