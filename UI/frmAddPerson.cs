using Domain;
using LOGIC.Facade;
using LOGIC;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace UI
{
    public partial class frmAddPerson : Form
    {
        private readonly PersonService _personService;

        public frmAddPerson()
        {
            InitializeComponent();
            LoadPersonTypes();
            LoadDocumentTypes();

            // Inicializa el servicio, que interactúa con la lógica a través del Facade
            _personService = new PersonService();
        }

        /// <summary>
        /// Carga los tipos de persona (Propietario, Inquilino) en el ComboBox correspondiente.
        /// </summary>
        private void LoadPersonTypes()
        {
            cmbTypeOfPerson.DataSource = Enum.GetValues(typeof(Person.PersonTypeEnum));
        }

        /// <summary>
        /// Carga los tipos de documento en el ComboBox correspondiente.
        /// </summary>
        private void LoadDocumentTypes()
        {
            List<string> documentTypes = new List<string> { "DNI", "Pasaporte"};
            cmbTypeOfDocument.DataSource = documentTypes;
        }

        /// <summary>
        /// Evento que se dispara al hacer clic en el botón "Guardar cambios".
        /// Guarda la persona en la base de datos utilizando la capa de servicio.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar los campos antes de guardar
                if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtLastName.Text) || string.IsNullOrEmpty(txtPhoneNumber.Text))
                {
                    MessageBox.Show("Todos los campos son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Crear una nueva instancia de la clase Person con los datos ingresados
                Person newPerson = new Person
                {
                    IdPerson = Guid.NewGuid(),
                    NamePerson = txtName.Text,
                    LastNamePerson = txtLastName.Text,
                    DomicilePerson = txtDomicile.Text,
                    ElectronicDomicilePerson = txtEmail.Text,
                    PhoneNumberPerson = int.Parse(txtPhoneNumber.Text),
                    NumberDocumentPerson = int.Parse(txtDocumentNumber.Text),
                    TypeDocumentPerson = cmbTypeOfDocument.SelectedItem.ToString(),
                    EnumTypePerson = (Person.PersonTypeEnum)cmbTypeOfPerson.SelectedItem
                };

                // Llamar al servicio para crear una nueva persona
                _personService.CreatePerson(newPerson);

                MessageBox.Show("Persona guardada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Limpiar los campos después de guardar
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la persona: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Limpia los campos del formulario después de guardar la persona.
        /// </summary>
        private void ClearForm()
        {
            txtName.Clear();
            txtLastName.Clear();
            txtDomicile.Clear();
            txtEmail.Clear();
            txtPhoneNumber.Clear();
            txtDocumentNumber.Clear();
            cmbTypeOfPerson.SelectedIndex = -1;
            cmbTypeOfDocument.SelectedIndex = -1;
        }
    }
}
