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

        private readonly Dictionary<Control, string> helpMessages;
        private Timer toolTipTimer;
        private Control currentControl;

        public frmTenantsReport()
        {
            InitializeComponent();
            _personService = new PersonService(); // Asegúrate de que PersonService no necesite parámetros.

            // Inicializar mensajes de ayuda
            helpMessages = new Dictionary<Control, string>
            {
                { dgvPersons, "Lista de inquilinos con su información detallada." },
                { btnDownload, "Descargar el reporte en formato Excel." }
            };

            // Configurar el Timer
            toolTipTimer = new Timer();
            toolTipTimer.Interval = 1000; // 2 segundos
            toolTipTimer.Tick += ToolTipTimer_Tick;

            // Asociar eventos a los controles
            foreach (var control in helpMessages.Keys)
            {
                control.MouseEnter += Control_MouseEnter;
                control.MouseLeave += Control_MouseLeave;
            }

            LoadPersonData();
        }

        private void Control_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Control control && helpMessages.ContainsKey(control))
            {
                currentControl = control; // Guardar el control actual
                toolTipTimer.Start(); // Iniciar el temporizador
            }
        }

        private void Control_MouseLeave(object sender, EventArgs e)
        {
            toolTipTimer.Stop(); // Detener el temporizador
            currentControl = null; // Limpiar el control actual
        }

        private void ToolTipTimer_Tick(object sender, EventArgs e)
        {
            if (currentControl != null && helpMessages.ContainsKey(currentControl))
            {
                // Mostrar el ToolTip para el control actual
                ToolTip toolTip = new ToolTip();
                toolTip.Show(helpMessages[currentControl], currentControl, 3000); // Mostrar durante 3 segundos
            }

            toolTipTimer.Stop(); // Detener el temporizador
        }

        private void LoadPersonData()
        {
            try
            {
                var persons = _personService.GetAllPersonsByType(Person.PersonTypeEnum.Tenant);

                // Asignar los datos al DataGridView
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

                dgvPersons.Columns["IdPerson"].Visible = false; // Oculta la columna de ID

                // Ajusta los encabezados de las columnas
                dgvPersons.Columns["Nombre"].HeaderText = LanguageService.Translate("Nombre");
                dgvPersons.Columns["Apellido"].HeaderText = LanguageService.Translate("Apellido");
                dgvPersons.Columns["Domicilio"].HeaderText = LanguageService.Translate("Domicilio");
                dgvPersons.Columns["DomicilioElectronico"].HeaderText = LanguageService.Translate("Domicilio Electrónico");
                dgvPersons.Columns["Telefono"].HeaderText = LanguageService.Translate("Teléfono");
                dgvPersons.Columns["TipoDocumento"].HeaderText = LanguageService.Translate("Tipo de Documento");
                dgvPersons.Columns["NumeroDocumento"].HeaderText = LanguageService.Translate("Número de Documento");

                dgvPersons.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Ajusta las columnas al ancho del DataGridView
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
                        // Obtener la lista de personas directamente del DataGridView
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

                        // Llamar al servicio para exportar el archivo Excel
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
    }
}
