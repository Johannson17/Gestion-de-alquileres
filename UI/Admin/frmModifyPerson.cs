using Domain;
using LOGIC.Facade;
using Services.Facade;
using System;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    public partial class frmModifyPerson : Form
    {
        private readonly PersonService _personService;

        public frmModifyPerson()
        {
            InitializeComponent();
            _personService = new PersonService();
            LoadPersonData();

            // Asigna el evento de clic de celda al DataGridView
            dgvPerson.CellClick += dgvPerson_CellClick;
        }

        // Cargar los datos de las personas en el DataGridView al iniciar el formulario
        private void LoadPersonData()
        {
            try
            {
                var persons = _personService.GetAllPersonsByType(Person.PersonTypeEnum.Tenant);

                // Asignar los datos al DataGridView
                dgvPerson.DataSource = persons.Select(p => new
                {
                    p.IdPerson,
                    Name = p.NamePerson,
                    LastName = p.LastNamePerson,
                    Address = p.DomicilePerson,
                    ElectronicAddress = p.ElectronicDomicilePerson,
                    PhoneNumber = p.PhoneNumberPerson,
                    DocumentNumber = p.NumberDocumentPerson
                }).ToList();

                dgvPerson.Columns["IdPerson"].Visible = false;

                // Ajusta los encabezados de las columnas
                dgvPerson.Columns["Name"].HeaderText = LanguageService.Translate("Nombre");
                dgvPerson.Columns["LastName"].HeaderText = LanguageService.Translate("Apellido");
                dgvPerson.Columns["Address"].HeaderText = LanguageService.Translate("Domicilio");
                dgvPerson.Columns["ElectronicAddress"].HeaderText = LanguageService.Translate("Domicilio Electrónico");
                dgvPerson.Columns["PhoneNumber"].HeaderText = LanguageService.Translate("Teléfono");
                dgvPerson.Columns["DocumentNumber"].HeaderText = LanguageService.Translate("Número de Documento");

                dgvPerson.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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

        // Evento que se ejecuta al hacer clic en una celda del DataGridView
        private void dgvPerson_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    var selectedPerson = (Person)dgvPerson.Rows[e.RowIndex].DataBoundItem;

                    txtName.Text = selectedPerson.NamePerson;
                    txtLastName.Text = selectedPerson.LastNamePerson;
                    txtDomicile.Text = selectedPerson.DomicilePerson;
                    txtEmail.Text = selectedPerson.ElectronicDomicilePerson;
                    txtPhoneNumber.Text = selectedPerson.PhoneNumberPerson.ToString();
                    txtDocumentNumber.Text = selectedPerson.NumberDocumentPerson.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        LanguageService.Translate("Error al seleccionar la persona") + ": " + ex.Message,
                        LanguageService.Translate("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        // Guardar los cambios realizados en la persona seleccionada
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedPerson = (Person)dgvPerson.CurrentRow?.DataBoundItem;

                if (selectedPerson == null)
                {
                    MessageBox.Show(
                        LanguageService.Translate("Seleccione una persona de la lista para modificar."),
                        LanguageService.Translate("Advertencia"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                selectedPerson.NamePerson = txtName.Text;
                selectedPerson.LastNamePerson = txtLastName.Text;
                selectedPerson.DomicilePerson = txtDomicile.Text;
                selectedPerson.ElectronicDomicilePerson = txtEmail.Text;
                selectedPerson.PhoneNumberPerson = int.Parse(txtPhoneNumber.Text);
                selectedPerson.NumberDocumentPerson = int.Parse(txtDocumentNumber.Text);

                _personService.UpdatePerson(selectedPerson);

                MessageBox.Show(
                    LanguageService.Translate("Datos actualizados correctamente."),
                    LanguageService.Translate("Éxito"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LoadPersonData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al guardar los cambios") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedPerson = (Person)dgvPerson.CurrentRow?.DataBoundItem;

                if (selectedPerson == null)
                {
                    MessageBox.Show(
                        LanguageService.Translate("Seleccione una persona de la lista para eliminar."),
                        LanguageService.Translate("Advertencia"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                var confirmation = MessageBox.Show(
                    LanguageService.Translate("¿Está seguro de que desea eliminar esta persona?"),
                    LanguageService.Translate("Confirmación"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmation == DialogResult.Yes)
                {
                    _personService.DeletePerson(selectedPerson.IdPerson);

                    MessageBox.Show(
                        LanguageService.Translate("Persona eliminada correctamente."),
                        LanguageService.Translate("Éxito"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    LoadPersonData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al eliminar la persona") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}