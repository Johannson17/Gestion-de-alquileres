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
        private readonly Timer toolTipTimer;
        private Control currentControl;

        // Diccionario para almacenar los mensajes de ayuda
        private readonly Dictionary<Control, string> helpMessages;

        public frmAddPropertyInventory(Property property)
        {
            InitializeComponent();
            _property = property;

            // Inicializar el temporizador para ToolTips
            toolTipTimer = new Timer
            {
                Interval = 1000 // 2 segundos
            };
            toolTipTimer.Tick += ToolTipTimer_Tick;

            // Inicializar mensajes de ayuda
            helpMessages = new Dictionary<Control, string>
            {
                { txtName, "Ingrese el nombre del elemento del inventario. Este campo es obligatorio." },
                { txtDescription, "Ingrese una descripción detallada del elemento del inventario. Este campo es obligatorio." },
                { btnSave, "Presione este botón para guardar el elemento del inventario en la propiedad actual." }
            };

            SubscribeToMouseEvents();
        }

        /// <summary>
        /// Suscribe los eventos MouseEnter y MouseLeave a los controles.
        /// </summary>
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

                // Verificar si ya existe el inventario antes de agregarlo
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
