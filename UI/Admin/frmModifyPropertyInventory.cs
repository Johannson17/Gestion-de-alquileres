using Domain;
using LOGIC.Facade;
using Services.Facade;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace UI
{
    public partial class frmModifyPropertyInventory : Form
    {
        private Property _selectedProperty;
        private readonly PropertyService _propertyService;

        private Dictionary<Control, string> helpMessages;
        private Timer toolTipTimer;
        private Control currentControl; // Control actual donde está el mouse

        /// <summary>
        /// Recorre todos los controles del formulario y traduce automáticamente su texto.
        /// </summary>
        private void TranslateControls(Control parentControl)
        {
            foreach (Control control in parentControl.Controls)
            {
                // Si el control tiene texto, lo traduce
                if (!string.IsNullOrEmpty(control.Text))
                {
                    control.Text = LanguageService.Translate(control.Text);
                }

                // Si el control tiene hijos (por ejemplo, Panel o GroupBox), recursión
                if (control.HasChildren)
                {
                    TranslateControls(control);
                }
            }
        }

        /// <summary>
        /// Traduce los encabezados de las columnas del DataGridView.
        /// </summary>
        private void TranslateDataGridViewHeaders()
        {
            if (dgvInventory.Columns.Count > 0)
            {
                dgvInventory.Columns["NameInventory"].HeaderText = LanguageService.Translate("Nombre");
                dgvInventory.Columns["DescriptionInventory"].HeaderText = LanguageService.Translate("Descripción");
            }
        }

        /// <summary>
        /// Aplica todas las traducciones en el formulario.
        /// </summary>
        private void ApplyTranslations()
        {
            this.Text = LanguageService.Translate("Modificar Inventario de Propiedad"); // Traducir el título del formulario
            TranslateControls(this); // Traduce todos los controles del formulario
            TranslateDataGridViewHeaders(); // Traduce los headers del DataGridView
            InitializeHelpMessages(); // Traduce las ayudas del usuario
        }

        /// <summary>
        /// Llama a ApplyTranslations en el constructor después de InitializeComponent.
        /// </summary>
        public frmModifyPropertyInventory(Property property)
        {
            InitializeComponent();
            _selectedProperty = property;
            _propertyService = new PropertyService();

            InitializeHelpMessages();
            SubscribeHelpMessagesEvents();

            toolTipTimer = new Timer { Interval = 1000 };
            toolTipTimer.Tick += ToolTipTimer_Tick;

            LoadInventory();

            dgvInventory.SelectionChanged += dgvInventory_SelectionChanged;
            btnSave.Click += btnSave_Click;
            btnDelete.Click += btnDelete_Click;

            ApplyTranslations(); // 🔥 Aplica la traducción automática a todo
            this.KeyPreview = true; // Permite que el formulario intercepte teclas como F1
            this.HelpRequested += FrmAddPerson_HelpRequested_f1; // <-- Asignación del evento
        }

        /// <summary>
        /// Refresca la grilla de inventario con traducción de los headers.
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
            TranslateDataGridViewHeaders(); // 🔥 Asegura que los headers del DataGridView estén traducidos
        }


        /// <summary>
        /// Inicializa los mensajes de ayuda con la traducción actual.
        /// </summary>
        private void InitializeHelpMessages()
        {
            helpMessages = new Dictionary<Control, string>
            {
                { txtName, LanguageService.Translate("Ingrese el nombre del inventario.") },
                { txtDescription, LanguageService.Translate("Ingrese la descripción del inventario.") },
                { btnSave, LanguageService.Translate("Guarda los cambios realizados o agrega un nuevo inventario.") },
                { btnDelete, LanguageService.Translate("Elimina el inventario seleccionado.") },
                { dgvInventory, LanguageService.Translate("Lista de inventarios disponibles. Seleccione uno para editar.") }
            };
        }

        /// <summary>
        /// Suscribe los eventos `MouseEnter` y `MouseLeave` a los controles con mensajes de ayuda.
        /// </summary>
        private void SubscribeHelpMessagesEvents()
        {
            if (helpMessages != null)
            {
                foreach (var control in helpMessages.Keys)
                {
                    control.MouseEnter += Control_MouseEnter;
                    control.MouseLeave += Control_MouseLeave;
                }
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
                toolTip.Show(helpMessages[currentControl], currentControl, 3000);
            }
            toolTipTimer.Stop();
        }

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

                dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{LanguageService.Translate("Error al cargar el inventario")}: {ex.Message}",
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

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
                        $"{LanguageService.Translate("Error al seleccionar el inventario")}: {ex.Message}",
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
                    $"{LanguageService.Translate("Error al guardar los cambios")}: {ex.Message}",
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Actualiza las ayudas cuando se cambia el idioma.
        /// </summary>
        public void UpdateHelpMessages()
        {
            InitializeHelpMessages();
        }

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
                    $"{LanguageService.Translate("Error al eliminar el elemento")}: {ex.Message}",
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

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

        private void FrmAddPerson_HelpRequested_f1(object sender, HelpEventArgs hlpevent)
        {
            hlpevent.Handled = true;

            // 1. Nombre de la imagen según el formulario
            string imageFileName = $"{this.Name}.png";

            // 2. Ruta de la imagen (ajusta según tu estructura de carpetas)
            string imagePath = Path.Combine(Application.StartupPath, "..", "..", "images", imageFileName);
            imagePath = Path.GetFullPath(imagePath);

            // 3. Texto de ayuda
            var helpMessage = string.Format(
                LanguageService.Translate("MÓDULO DE MODIFICACIÓN DE INVENTARIO DE PROPIEDAD").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Este formulario te permite administrar (agregar, editar o eliminar) los elementos ").ToString() +
                LanguageService.Translate("de inventario asociados a una propiedad específica. Cada elemento de inventario cuenta ").ToString() +
                LanguageService.Translate("con un nombre y una descripción que describen el objeto o artículo en cuestión.").ToString() + "\r\n\r\n" +
                LanguageService.Translate("INSTRUCCIONES PASO A PASO:").ToString() + "\r\n" +
                LanguageService.Translate("1. Observa la lista de inventario en la parte superior. Selecciona un elemento ").ToString() +
                LanguageService.Translate("   para editarlo; sus datos aparecerán en los campos 'Nombre' y 'Descripción'.").ToString() + "\r\n" +
                LanguageService.Translate("2. Si deseas modificar el elemento seleccionado, actualiza la información y haz clic en ").ToString() +
                LanguageService.Translate("   ‘Guardar cambios’ para aplicar los cambios.").ToString() + "\r\n" +
                LanguageService.Translate("3. Si deseas agregar un nuevo elemento de inventario, deja sin seleccionar cualquier ").ToString() +
                LanguageService.Translate("   fila de la lista (o haz clic en ‘No’ cuando se te pregunte si quieres modificar el elemento), ").ToString() +
                LanguageService.Translate("   rellena los campos ‘Nombre’ y ‘Descripción’ y haz clic en ‘Guardar cambios’.").ToString() + "\r\n" +
                LanguageService.Translate("4. Para eliminar un elemento existente, selecciónalo en la lista y haz clic en ").ToString() +
                LanguageService.Translate("   ‘Eliminar inventario’. Se te pedirá confirmación antes de borrarlo definitivamente.").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Con estos pasos, podrás mantener actualizado el inventario de la propiedad. ").ToString() +
                LanguageService.Translate("Si necesitas más ayuda, por favor contacta al administrador del sistema.").ToString());

            // 4. Crear el formulario de ayuda
            using (Form helpForm = new Form())
            {
                helpForm.Text = LanguageService.Translate("Ayuda del sistema");
                helpForm.StartPosition = FormStartPosition.CenterParent;
                helpForm.Size = new Size(900, 700);
                helpForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                helpForm.MaximizeBox = false;
                helpForm.MinimizeBox = false;

                // 5. Crear un TableLayoutPanel con 1 fila y 2 columnas
                TableLayoutPanel tableLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RowCount = 1,
                    ColumnCount = 2
                };

                // Columna 0: ancho fijo (p.ej. 350 px) para el texto
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 350F));
                // Columna 1: el resto del espacio para la imagen
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                // Fila única: ocupa todo el alto
                tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

                // 6. Panel de texto con scroll (por si el texto es extenso)
                Panel textPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = true,
                    Padding = new Padding(15)
                };

                // 7. Label con el texto. Usamos MaximumSize para forzar el wrap del texto
                Label lblHelp = new Label
                {
                    Text = helpMessage,
                    AutoSize = true,
                    Dock = DockStyle.Top,
                    MaximumSize = new Size(320, 0),  // Menos que 350 para dejar margen
                    Font = new Font("Segoe UI", 10, FontStyle.Regular)
                };

                textPanel.Controls.Add(lblHelp);

                // 8. PictureBox para la imagen (columna derecha)
                PictureBox pbHelpImage = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.WhiteSmoke, // Opcional, para ver contraste
                    Margin = new Padding(5)
                };

                // 9. Cargamos la imagen si existe
                if (File.Exists(imagePath))
                {
                    pbHelpImage.Image = Image.FromFile(imagePath);
                }
                else
                {
                    lblHelp.Text += "\r\n\r\n" +
                                    "$" + LanguageService.Translate("No se encontró la imagen de ayuda en la ruta: ") + imagePath;
                }

                // 10. Agregar el panel de texto (columna 0) y la imagen (columna 1) al TableLayoutPanel
                tableLayout.Controls.Add(textPanel, 0, 0);
                tableLayout.Controls.Add(pbHelpImage, 1, 0);

                // 11. Agregar el TableLayoutPanel al formulario
                helpForm.Controls.Add(tableLayout);

                // 12. Mostrar el formulario de ayuda
                helpForm.ShowDialog();
            }
        }
    }
}
