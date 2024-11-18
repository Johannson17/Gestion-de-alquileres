using LOGIC.Facade;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Domain;

namespace UI.Admin
{
    public partial class frmTenantsReport : Form
    {
        private readonly PersonService _personService;

        public frmTenantsReport()
        {
            InitializeComponent();
            _personService = new PersonService(); // Asegúrate de que PersonService no necesite parámetros.
            LoadPersonData();
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
                    DomicilioElectronico = p.ElectronicDomicilePerson, // Sin espacio en el nombre
                    Telefono = p.PhoneNumberPerson, // Sin tilde en el identificador
                    TipoDocumento = p.TypeDocumentPerson, // Nuevo campo añadido
                    NumeroDocumento = p.NumberDocumentPerson // Sin espacio en el nombre
                }).ToList();

                dgvPersons.Columns["IdPerson"].Visible = false; // Oculta la columna de ID

                // Ajusta los encabezados de las columnas
                dgvPersons.Columns["Nombre"].HeaderText = "Nombre";
                dgvPersons.Columns["Apellido"].HeaderText = "Apellido";
                dgvPersons.Columns["Domicilio"].HeaderText = "Domicilio";
                dgvPersons.Columns["DomicilioElectronico"].HeaderText = "Domicilio Electrónico";
                dgvPersons.Columns["Telefono"].HeaderText = "Teléfono";
                dgvPersons.Columns["TipoDocumento"].HeaderText = "Tipo de Documento"; // Encabezado para el nuevo campo
                dgvPersons.Columns["NumeroDocumento"].HeaderText = "Número de Documento";

                dgvPersons.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Ajusta las columnas al ancho del DataGridView
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Archivos de Excel (*.xlsx)|*.xlsx";
                    saveFileDialog.Title = "Guardar Reporte de Inquilinos";
                    saveFileDialog.FileName = "ReporteInquilinos.xlsx";

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
                                TypeDocumentPerson = row.Cells["TipoDocumento"].Value?.ToString(), // Nuevo campo añadido
                                NumberDocumentPerson = int.Parse(row.Cells["NumeroDocumento"].Value?.ToString())
                            })
                            .ToList();

                        // Llamar al servicio para exportar el archivo Excel
                        _personService.ExportPersonsToExcel(saveFileDialog.FileName, personsToExport);

                        MessageBox.Show("El archivo se guardó exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar el archivo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
