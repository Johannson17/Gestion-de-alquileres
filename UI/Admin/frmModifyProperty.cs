using Domain;
using LOGIC.Facade;
using Services.Facade;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    public partial class frmModifyProperty : Form
    {
        private readonly PropertyService _propertyService;
        private List<Property> _properties;
        private Property _currentProperty;
        private Timer toolTipTimer;
        private Control currentControl;
        private Dictionary<Control, string> helpMessages;

        public frmModifyProperty()
        {
            InitializeComponent();
            _propertyService = new PropertyService();
            LoadProperties();
            InitializeHelpMessages();
            SubscribeHelpMessagesEvents();

            dgvProperty.SelectionChanged += dgvProperties_SelectionChanged;
            // Evitar que el evento se agregue múltiples veces
            btnDelete.Click -= btnDelete_Click;
            btnDelete.Click += btnDelete_Click;
            btnEditInventory.Click += btnEditInventory_Click;
            this.KeyPreview = true; // Permite que el formulario intercepte teclas como F1
            this.HelpRequested += FrmAddProperty_HelpRequested_f1; // <-- Asignación del evento
        }

        /// <summary>
        /// Inicializa los mensajes de ayuda con la traducción actual.
        /// </summary>
        private void InitializeHelpMessages()
        {
            helpMessages = new Dictionary<Control, string>
            {
                { dgvProperty, LanguageService.Translate("Lista de propiedades registradas. Seleccione una fila para editar o eliminar una propiedad.") },
                { txtAddress, LanguageService.Translate("Ingrese la dirección de la propiedad.") },
                { txtCountry, LanguageService.Translate("Ingrese el país donde se encuentra la propiedad.") },
                { txtMunicipality, LanguageService.Translate("Ingrese el municipio de la propiedad.") },
                { txtProvince, LanguageService.Translate("Ingrese la provincia de la propiedad.") },
                { txtDescription, LanguageService.Translate("Ingrese una descripción detallada de la propiedad.") },
                { cmbStatus, LanguageService.Translate("Seleccione el estado actual de la propiedad.") },
                { btnSave, LanguageService.Translate("Guarde los cambios realizados en la propiedad seleccionada.") },
                { btnDelete, LanguageService.Translate("Elimine la propiedad seleccionada.") },
                { btnEditInventory, LanguageService.Translate("Edite el inventario asociado a la propiedad seleccionada.") }
            };
        }

        /// <summary>
        /// Suscribe los eventos `MouseEnter` y `MouseLeave` a los controles con mensajes de ayuda.
        /// </summary>
        private void SubscribeHelpMessagesEvents()
        {
            toolTipTimer = new Timer { Interval = 1000 };
            toolTipTimer.Tick += ToolTipTimer_Tick;

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
                toolTip.Show(helpMessages[currentControl], currentControl, currentControl.Width / 2, currentControl.Height, 3000);
            }
            toolTipTimer.Stop();
        }

        /// <summary>
        /// Carga las propiedades en el `DataGridView` y traduce los encabezados.
        /// </summary>
        private void LoadProperties()
        {
            try
            {
                _properties = _propertyService.GetAllProperties();

                dgvProperty.Columns.Clear();
                dgvProperty.DataSource = _properties.Select(p => new
                {
                    p.IdProperty,
                    p.DescriptionProperty,
                    p.StatusProperty,
                    p.CountryProperty,
                    p.ProvinceProperty,
                    p.MunicipalityProperty,
                    p.AddressProperty
                }).ToList();

                dgvProperty.Columns["IdProperty"].Visible = false;
                TranslateHeaders(); // Llamar a la función de traducción

                dgvProperty.AutoResizeColumns();
                dgvProperty.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar las propiedades") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Traduce los encabezados del DataGridView.
        /// </summary>
        private void TranslateHeaders()
        {
            dgvProperty.Columns["DescriptionProperty"].HeaderText = LanguageService.Translate("Descripción");
            dgvProperty.Columns["StatusProperty"].HeaderText = LanguageService.Translate("Estado");
            dgvProperty.Columns["CountryProperty"].HeaderText = LanguageService.Translate("País");
            dgvProperty.Columns["ProvinceProperty"].HeaderText = LanguageService.Translate("Provincia");
            dgvProperty.Columns["MunicipalityProperty"].HeaderText = LanguageService.Translate("Municipio");
            dgvProperty.Columns["AddressProperty"].HeaderText = LanguageService.Translate("Dirección");
        }

        private void dgvProperties_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProperty.SelectedRows.Count > 0)
            {
                try
                {
                    var selectedPropertyId = (Guid)dgvProperty.SelectedRows[0].Cells["IdProperty"].Value;
                    _currentProperty = _propertyService.GetProperty(selectedPropertyId);

                    if (_currentProperty != null)
                    {
                        txtAddress.Text = _currentProperty.AddressProperty;
                        txtCountry.Text = _currentProperty.CountryProperty;
                        txtMunicipality.Text = _currentProperty.MunicipalityProperty;
                        txtProvince.Text = _currentProperty.ProvinceProperty;
                        txtDescription.Text = _currentProperty.DescriptionProperty;

                        LoadStatusComboBox(_currentProperty.StatusProperty);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(LanguageService.Translate("Error al seleccionar la propiedad") + ": " + ex.Message,
                        LanguageService.Translate("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void LoadStatusComboBox(PropertyStatusEnum currentStatus)
        {
            cmbStatus.DataSource = Enum.GetValues(typeof(PropertyStatusEnum)).Cast<PropertyStatusEnum>().ToList();
            cmbStatus.SelectedItem = currentStatus;
        }

        public void RefreshUI()
        {
            LoadProperties(); // Recargar propiedades con traducciones
            InitializeHelpMessages(); // Recargar mensajes de ayuda con el idioma actual
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _currentProperty.AddressProperty = txtAddress.Text;
                _currentProperty.CountryProperty = txtCountry.Text;
                _currentProperty.MunicipalityProperty = txtMunicipality.Text;
                _currentProperty.ProvinceProperty = txtProvince.Text;
                _currentProperty.DescriptionProperty = txtDescription.Text;
                _currentProperty.StatusProperty = (PropertyStatusEnum)cmbStatus.SelectedItem;

                _propertyService.UpdateProperty(_currentProperty);

                MessageBox.Show(LanguageService.Translate("Propiedad actualizada correctamente"),
                    LanguageService.Translate("Éxito"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                RefreshUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageService.Translate("Error al actualizar la propiedad") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var confirmResult = MessageBox.Show(LanguageService.Translate("¿Está seguro de eliminar esta propiedad?"),
                    LanguageService.Translate("Confirmar eliminación"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirmResult == DialogResult.Yes)
                {
                    _propertyService.DeleteProperty(_currentProperty.IdProperty);

                    MessageBox.Show(LanguageService.Translate("Propiedad eliminada correctamente"),
                        LanguageService.Translate("Éxito"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadProperties();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageService.Translate("Error al eliminar la propiedad") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            { RefreshUI(); }
        }

        private void btnEditInventory_Click(object sender, EventArgs e)
        {
            try
            {
                var modifyInventoryForm = new frmModifyPropertyInventory(_currentProperty);

                if (modifyInventoryForm.ShowDialog() == DialogResult.OK)
                {
                    LoadProperties();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al abrir el formulario de inventario") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void FrmAddProperty_HelpRequested_f1(object sender, HelpEventArgs hlpevent)
        {
            hlpevent.Handled = true;

            // 1. Construimos el nombre del archivo de imagen según el formulario
            string imageFileName = $"{this.Name}.png";
            // 2. Ruta completa de la imagen (ajusta la carpeta si difiere)
            string imagePath = Path.Combine(Application.StartupPath, "..", "..", "images", imageFileName);
            imagePath = Path.GetFullPath(imagePath);

            // 3. Texto de ayuda específico para "Agregar propiedad"
            var helpMessage = string.Format(
                LanguageService.Translate("MÓDULO DE MODIFICACIÓN DE PROPIEDADES").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Este formulario te permite editar y/o eliminar propiedades existentes en el sistema, ") +
                LanguageService.Translate("así como gestionar su inventario. Para modificar una propiedad, primero debes ") +
                LanguageService.Translate("seleccionarla en la lista superior (DataGridView) y, a continuación, actualizar sus datos ") +
                LanguageService.Translate("en los campos inferiores.") + "\r\n\r\n" +
                LanguageService.Translate("INSTRUCCIONES PASO A PASO:") + "\r\n" +
                LanguageService.Translate("1. Selecciona una propiedad de la lista superior. Sus datos se mostrarán en los campos de edición (dirección, país, provincia, municipio, etc.).") + "\r\n" +
                LanguageService.Translate("2. Ajusta la información que necesites, incluyendo el estado de la propiedad (Disponible, Ocupada, etc.).") + "\r\n" +
                LanguageService.Translate("3. Haz clic en ‘Guardar cambios’ para actualizar los datos de la propiedad en la base de datos.") + "\r\n" +
                LanguageService.Translate("4. Para editar el inventario asociado a la propiedad, haz clic en ‘Editar inventario’. ") +
                LanguageService.Translate("   Se abrirá una ventana donde podrás añadir, modificar o eliminar elementos de inventario.") + "\r\n" +
                LanguageService.Translate("5. Si deseas eliminar la propiedad seleccionada por completo, haz clic en ‘Eliminar propiedad’. ") +
                LanguageService.Translate("   Se te pedirá confirmación antes de borrarla definitivamente.") + "\r\n\r\n" +
                LanguageService.Translate("Con estos pasos, podrás mantener actualizada la información de cada propiedad en el sistema. ") +
                LanguageService.Translate("Si necesitas más ayuda, por favor contacta al administrador del sistema."));

            // 4. Creamos el formulario de ayuda
            using (Form helpForm = new Form())
            {
                // El formulario se autoajusta a su contenido
                helpForm.AutoSize = true;
                helpForm.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                helpForm.StartPosition = FormStartPosition.CenterScreen;
                helpForm.Text = LanguageService.Translate("Ayuda del sistema");
                helpForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                helpForm.MaximizeBox = false;
                helpForm.MinimizeBox = false;

                // (Opcional) Limitamos el tamaño máximo para no salirnos de la pantalla
                // Esto hace que aparezca scroll si el contenido excede este tamaño
                helpForm.MaximumSize = new Size(
                    (int)(Screen.PrimaryScreen.WorkingArea.Width * 0.9),
                    (int)(Screen.PrimaryScreen.WorkingArea.Height * 0.9)
                );

                // Para permitir scroll si el contenido excede el tamaño máximo
                helpForm.AutoScroll = true;

                // 5. Creamos un FlowLayoutPanel que también se autoajuste
                FlowLayoutPanel flowPanel = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.TopDown,   // Apilar texto e imagen verticalmente
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    WrapContents = false,                    // No “romper” en más columnas
                    Padding = new Padding(10)
                };

                // 6. Label para el texto de ayuda
                Label lblHelp = new Label
                {
                    Text = helpMessage,
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    Margin = new Padding(5)
                };

                // (Opcional) Si deseas forzar que el texto no exceda cierto ancho y haga wrap:
                lblHelp.MaximumSize = new Size(800, 0);

                flowPanel.Controls.Add(lblHelp);

                // 7. PictureBox para la imagen
                PictureBox pbHelpImage = new PictureBox
                {
                    Margin = new Padding(5),
                    // Si quieres mostrar la imagen a tamaño real:
                    SizeMode = PictureBoxSizeMode.AutoSize,
                    // O si prefieres que se ajuste pero mantenga proporción:
                    // SizeMode = PictureBoxSizeMode.Zoom,
                };

                if (File.Exists(imagePath))
                {
                    pbHelpImage.Image = Image.FromFile(imagePath);
                }
                else
                {
                    lblHelp.Text += "\r\n\r\n" +
                                    "$" + LanguageService.Translate("No se encontró la imagen de ayuda en la ruta: ") + imagePath;
                }

                // (Opcional) Si usas Zoom, el PictureBox por defecto no hace auto-size, 
                // puedes darle un tamaño inicial y dejar que el form se ajuste
                // pbHelpImage.Size = new Size(600, 400);

                flowPanel.Controls.Add(pbHelpImage);

                // 8. Agregamos el FlowLayoutPanel al formulario
                helpForm.Controls.Add(flowPanel);

                // 9. Mostramos el formulario
                helpForm.ShowDialog();
            }
        }
    }
}
