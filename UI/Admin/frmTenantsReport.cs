using LOGIC.Facade;
using System;
using System.Linq;
using System.Windows.Forms;
using Domain;
using Services.Facade;
using System.Collections.Generic;

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
    }
}
