using LOGIC.Facade;
using System;
using System.Linq;
using System.Windows.Forms;
using Domain;
using Services.Facade;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace UI.Admin
{
    public partial class frmTenantsReport : Form
    {
        private readonly PersonService _personService;
        private Dictionary<Control, string> helpMessages;
        private Timer toolTipTimer;
        private Control currentControl;

        public frmTenantsReport()
        {
            InitializeComponent();
            _personService = new PersonService();

            InitializeHelpMessages(); // Inicializar mensajes de ayuda traducidos
            SubscribeHelpMessagesEvents(); // Suscribir eventos a los controles

            // Configurar el Timer
            toolTipTimer = new Timer { Interval = 1000 }; // 1 segundo
            toolTipTimer.Tick += ToolTipTimer_Tick;

            LoadPersonData();
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
                { dgvPersons, LanguageService.Translate("Lista de inquilinos con su información detallada.") },
                { btnDownload, LanguageService.Translate("Descargar el reporte en formato Excel.") }
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

        private void LoadPersonData()
        {
            try
            {
                var persons = _personService.GetAllPersonsByType(Person.PersonTypeEnum.Tenant);

                dgvPersons.DataSource = persons.Select(p => new
                {
                    p.IdPerson,
                    Nombre = p.NamePerson,
                    Apellido = p.LastNamePerson,
                    Domicilio = p.DomicilePerson,
                    DomicilioElectronico = p.ElectronicDomicilePerson,
                    Telefono = p.PhoneNumberPerson,
                    TipoDocumento = p.TypeDocumentPerson,
                    NumeroDocumento = p.NumberDocumentPerson
                }).ToList();

                dgvPersons.Columns["IdPerson"].Visible = false;

                UpdateColumnHeaders(); // Traducir los encabezados de las columnas
                dgvPersons.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar los datos") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Actualiza los encabezados de las columnas con la traducción actual.
        /// </summary>
        private void UpdateColumnHeaders()
        {
            dgvPersons.Columns["Nombre"].HeaderText = LanguageService.Translate("Nombre");
            dgvPersons.Columns["Apellido"].HeaderText = LanguageService.Translate("Apellido");
            dgvPersons.Columns["Domicilio"].HeaderText = LanguageService.Translate("Domicilio");
            dgvPersons.Columns["DomicilioElectronico"].HeaderText = LanguageService.Translate("Domicilio Electrónico");
            dgvPersons.Columns["Telefono"].HeaderText = LanguageService.Translate("Teléfono");
            dgvPersons.Columns["TipoDocumento"].HeaderText = LanguageService.Translate("Tipo de Documento");
            dgvPersons.Columns["NumeroDocumento"].HeaderText = LanguageService.Translate("Número de Documento");
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = LanguageService.Translate("Archivos de Excel") + " (*.xlsx)|*.xlsx";
                    saveFileDialog.Title = LanguageService.Translate("Guardar Reporte de Inquilinos");
                    saveFileDialog.FileName = LanguageService.Translate("Reporte de Inquilinos") + ".xlsx";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        var personsToExport = dgvPersons.Rows
                            .Cast<DataGridViewRow>()
                            .Where(row => !row.IsNewRow)
                            .Select(row => new Person
                            {
                                IdPerson = Guid.Parse(row.Cells["IdPerson"].Value?.ToString() ?? Guid.Empty.ToString()),
                                NamePerson = row.Cells["Nombre"].Value?.ToString(),
                                LastNamePerson = row.Cells["Apellido"].Value?.ToString(),
                                DomicilePerson = row.Cells["Domicilio"].Value?.ToString(),
                                ElectronicDomicilePerson = row.Cells["DomicilioElectronico"].Value?.ToString(),
                                PhoneNumberPerson = int.Parse(row.Cells["Telefono"].Value?.ToString()),
                                TypeDocumentPerson = row.Cells["TipoDocumento"].Value?.ToString(),
                                NumberDocumentPerson = int.Parse(row.Cells["NumeroDocumento"].Value?.ToString())
                            })
                            .ToList();

                        _personService.ExportPersonsToExcel(saveFileDialog.FileName, personsToExport);

                        MessageBox.Show(
                            LanguageService.Translate("El archivo se guardó exitosamente."),
                            LanguageService.Translate("Éxito"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al exportar el archivo") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Actualiza los mensajes de ayuda cuando se cambia el idioma.
        /// </summary>
        public void UpdateHelpMessages()
        {
            InitializeHelpMessages();
        }

        private void FrmTenantsReport_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helpMessage = string.Join(Environment.NewLine, new[]
            {
                LanguageService.Translate("Bienvenido al reporte de inquilinos."),
                "",
                LanguageService.Translate("Opciones disponibles:"),
                $"- {LanguageService.Translate("Consultar la lista de inquilinos y su información.")}",
                $"- {LanguageService.Translate("Descargar el reporte en formato Excel.")}",
                "",
                LanguageService.Translate("Para más ayuda, contacte con el administrador.")
            });

            MessageBox.Show(helpMessage,
                            LanguageService.Translate("Ayuda del sistema"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        private void frmTenantsReport_Load(object sender, EventArgs e)
        {

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
                LanguageService.Translate("MÓDULO DE REPORTE DE ARRENDATARIOS").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Este formulario te permite visualizar y generar un reporte de todos los arrendatarios ").ToString() +
                LanguageService.Translate("(inquilinos) registrados en el sistema. Podrás ver información como nombre, apellido, ").ToString() +
                LanguageService.Translate("domicilio, teléfono y número de documento, entre otros.").ToString() + "\r\n\r\n" +
                LanguageService.Translate("INSTRUCCIONES PASO A PASO:").ToString() + "\r\n" +
                LanguageService.Translate("1. Observa la lista de arrendatarios en la parte superior. Aparecerán todos los inquilinos registrados.").ToString() + "\r\n" +
                LanguageService.Translate("2. Para descargar un reporte en formato Excel con los arrendatarios mostrados, ").ToString() +
                LanguageService.Translate("   haz clic en ‘Descargar Reporte’. Se te pedirá la ubicación donde deseas guardar el archivo.").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Con estos pasos, podrás obtener fácilmente un informe de todos los arrendatarios ").ToString() +
                LanguageService.Translate("registrados en el sistema. Si necesitas más ayuda, por favor contacta al administrador del sistema.").ToString());

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
