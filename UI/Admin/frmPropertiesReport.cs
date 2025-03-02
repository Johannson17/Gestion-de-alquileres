using Domain;
using LOGIC.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Services.Facade;
using System.Drawing;
using System.IO;

namespace UI.Admin
{
    public partial class frmPropertiesReport : Form
    {
        private readonly PropertyService _propertyService;
        private List<Property> _properties;
        private Dictionary<Control, string> helpMessages;
        private Timer toolTipTimer;
        private Control currentControl;

        public frmPropertiesReport()
        {
            InitializeComponent();
            _propertyService = new PropertyService();

            InitializeHelpMessages(); // Inicializar mensajes de ayuda traducidos
            SubscribeToMouseEvents(); // Suscribir eventos para ToolTips

            // Configurar el Timer
            toolTipTimer = new Timer { Interval = 1000 };
            toolTipTimer.Tick += ToolTipTimer_Tick;

            LoadProperties();
            LoadComboBoxOptions();
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
                { dgvProperties, LanguageService.Translate("Muestra las propiedades registradas.") },
                { cmbStatus, LanguageService.Translate("Seleccione el estado de las propiedades que desea filtrar.") },
                { btnFilter, LanguageService.Translate("Filtra las propiedades según el estado seleccionado.") },
                { btnDownload, LanguageService.Translate("Descarga un reporte en Excel con las propiedades mostradas.") }
            };
        }

        /// <summary>
        /// Suscribe los eventos `MouseEnter` y `MouseLeave` a los controles con mensajes de ayuda.
        /// </summary>
        private void SubscribeToMouseEvents()
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

        /// <summary>
        /// Carga todas las propiedades en el DataGridView.
        /// </summary>
        private void LoadProperties()
        {
            try
            {
                _properties = _propertyService.GetAllProperties();

                dgvProperties.Columns.Clear();

                dgvProperties.DataSource = _properties.Select(p => new
                {
                    p.IdProperty,
                    p.DescriptionProperty,
                    p.StatusProperty,
                    p.CountryProperty,
                    p.ProvinceProperty,
                    p.MunicipalityProperty,
                    p.AddressProperty
                }).ToList();

                dgvProperties.Columns["IdProperty"].Visible = false;

                dgvProperties.Columns["DescriptionProperty"].HeaderText = LanguageService.Translate("Descripción");
                dgvProperties.Columns["StatusProperty"].HeaderText = LanguageService.Translate("Estado");
                dgvProperties.Columns["CountryProperty"].HeaderText = LanguageService.Translate("País");
                dgvProperties.Columns["ProvinceProperty"].HeaderText = LanguageService.Translate("Provincia");
                dgvProperties.Columns["MunicipalityProperty"].HeaderText = LanguageService.Translate("Municipio");
                dgvProperties.Columns["AddressProperty"].HeaderText = LanguageService.Translate("Dirección");

                dgvProperties.AutoResizeColumns();
                dgvProperties.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{LanguageService.Translate("Error al cargar las propiedades")}: {ex.Message}",
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Cargar opciones únicas en el ComboBox de Estado desde las propiedades.
        /// </summary>
        private void LoadComboBoxOptions()
        {
            try
            {
                var statusValues = _properties
                    .Select(p => p.StatusProperty.ToString())
                    .Distinct()
                    .ToList();

                cmbStatus.Items.Clear();
                cmbStatus.Items.Add(LanguageService.Translate("Todos"));
                cmbStatus.Items.AddRange(statusValues.ToArray());
                cmbStatus.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{LanguageService.Translate("Error al cargar opciones en los ComboBox:")} {ex.Message}",
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Filtrar las propiedades en el DataGridView en función del estado seleccionado.
        /// </summary>
        private void btnFilter_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedStatus = cmbStatus.SelectedItem?.ToString();

                List<Property> filteredProperties = selectedStatus == LanguageService.Translate("Todos")
                    ? _propertyService.GetAllProperties()
                    : _propertyService.GetPropertiesByStatus((PropertyStatusEnum)Enum.Parse(typeof(PropertyStatusEnum), selectedStatus));

                dgvProperties.DataSource = filteredProperties.Select(p => new
                {
                    p.IdProperty,
                    p.DescriptionProperty,
                    p.StatusProperty,
                    p.CountryProperty,
                    p.ProvinceProperty,
                    p.MunicipalityProperty,
                    p.AddressProperty
                }).ToList();

                dgvProperties.Columns["IdProperty"].Visible = false;
                dgvProperties.AutoResizeColumns();
                dgvProperties.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{LanguageService.Translate("Error al filtrar las propiedades")}: {ex.Message}",
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Descargar los datos visibles en el DataGridView a un archivo Excel.
        /// </summary>
        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = LanguageService.Translate("Archivos de Excel (*.xlsx)|*.xlsx");
                    saveFileDialog.Title = LanguageService.Translate("Guardar Reporte de Propiedades");
                    saveFileDialog.FileName = "ReportePropiedades.xlsx";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        var propertiesToExport = dgvProperties.Rows
                            .Cast<DataGridViewRow>()
                            .Where(row => !row.IsNewRow)
                            .Select(row => new Property
                            {
                                DescriptionProperty = row.Cells["DescriptionProperty"].Value?.ToString(),
                                StatusProperty = Enum.TryParse<PropertyStatusEnum>(row.Cells["StatusProperty"].Value?.ToString(), out var status) ? status : PropertyStatusEnum.Disponible,
                                CountryProperty = row.Cells["CountryProperty"].Value?.ToString(),
                                ProvinceProperty = row.Cells["ProvinceProperty"].Value?.ToString(),
                                MunicipalityProperty = row.Cells["MunicipalityProperty"].Value?.ToString(),
                                AddressProperty = row.Cells["AddressProperty"].Value?.ToString()
                            })
                            .ToList();

                        _propertyService.ExportPropertiesToExcel(saveFileDialog.FileName, propertiesToExport);

                        MessageBox.Show(
                            LanguageService.Translate("El archivo se guardó exitosamente"),
                            LanguageService.Translate("Éxito"),
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{LanguageService.Translate("Error al exportar el archivo")}: {ex.Message}",
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Actualiza las ayudas cuando se cambia el idioma.
        /// </summary>
        public void UpdateHelpMessages()
        {
            InitializeHelpMessages();
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
                LanguageService.Translate("MÓDULO DE REPORTE DE PROPIEDADES").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Este formulario te permite visualizar, filtrar y generar un reporte de las propiedades ").ToString() +
                LanguageService.Translate("registradas en el sistema. Podrás ver información como la descripción, estado, país, provincia, ").ToString() +
                LanguageService.Translate("municipio y dirección de cada propiedad.").ToString() + "\r\n\r\n" +
                LanguageService.Translate("INSTRUCCIONES PASO A PASO:").ToString() + "\r\n" +
                LanguageService.Translate("1. Observa la lista de propiedades en la parte superior. Aparecerán todas las propiedades registradas.").ToString() + "\r\n" +
                LanguageService.Translate("2. Para filtrar las propiedades según su estado (por ejemplo, Disponible, Ocupada, etc.), ").ToString() +
                LanguageService.Translate("   selecciona el estado en la lista desplegable y haz clic en ‘Filtrar’.").ToString() + "\r\n" +
                LanguageService.Translate("3. Para volver a mostrar todas las propiedades, selecciona ‘Todos’ en la lista de estados y ").ToString() +
                LanguageService.Translate("   haz clic en ‘Filtrar’ nuevamente.").ToString() + "\r\n" +
                LanguageService.Translate("4. Si deseas generar un reporte en formato Excel con las propiedades mostradas, ").ToString() +
                LanguageService.Translate("   haz clic en ‘Descargar Reporte’. Se te pedirá la ubicación donde deseas guardar el archivo.").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Con estos pasos, podrás obtener fácilmente un reporte de las propiedades filtradas o ").ToString() +
                LanguageService.Translate("todas las propiedades del sistema. Si necesitas más ayuda, por favor contacta al administrador del sistema.").ToString());

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
