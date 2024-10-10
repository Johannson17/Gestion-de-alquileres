using Domain;
using LOGIC.Facade; // Asegúrate de que la lógica esté accesible aquí.
using System;
using System.Windows.Forms;
using System.Xml;

namespace UI
{
    public partial class frmModifyPerson : Form
    {
        private readonly PersonService _personService;

        public frmModifyPerson()
        {
            InitializeComponent();
            _personService = new PersonService(); // Asegúrate de que PersonService no necesite parámetros.
            LoadPersonData();

            // Asigna el evento de clic de celda al DataGridView
            dgvPerson.CellClick += dgvPerson_CellClick;
        }

        // Cargar los datos de las personas en el DataGridView al iniciar el formulario
        private void LoadPersonData()
        {
            try
            {
                var persons = _personService.GetAllPersons();
                dgvPerson.DataSource = persons;
                dgvPerson.Columns["IdPerson"].Visible = false; // Oculta la columna de ID si no es necesaria en la vista
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento que se ejecuta al hacer clic en una celda del DataGridView
        private void dgvPerson_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Asegurarse de que el índice de fila sea válido
            {
                // Obtiene la persona seleccionada de la fila
                var selectedPerson = (Person)dgvPerson.Rows[e.RowIndex].DataBoundItem;

                // Rellenar los campos de texto con los datos de la persona seleccionada
                txtName.Text = selectedPerson.NamePerson;
                txtLastName.Text = selectedPerson.LastNamePerson;
                txtDomicile.Text = selectedPerson.DomicilePerson;
                txtEmail.Text = selectedPerson.ElectronicDomicilePerson;
                txtPhoneNumber.Text = selectedPerson.PhoneNumberPerson.ToString();
                txtDocumentNumber.Text = selectedPerson.NumberDocumentPerson.ToString();
            }
        }

        // Guardar los cambios realizados en la persona seleccionada
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Obtiene la persona seleccionada
                var selectedPerson = (Person)dgvPerson.CurrentRow?.DataBoundItem;

                if (selectedPerson == null)
                {
                    MessageBox.Show("Seleccione una persona de la lista para modificar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Actualizar los datos de la persona con los valores del formulario
                selectedPerson.NamePerson = txtName.Text;
                selectedPerson.LastNamePerson = txtLastName.Text;
                selectedPerson.DomicilePerson = txtDomicile.Text;
                selectedPerson.ElectronicDomicilePerson = txtEmail.Text;
                selectedPerson.PhoneNumberPerson = int.Parse(txtPhoneNumber.Text);
                selectedPerson.NumberDocumentPerson = int.Parse(txtDocumentNumber.Text);

                // Guardar los cambios en la base de datos
                _personService.UpdatePerson(selectedPerson);

                MessageBox.Show("Datos actualizados correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadPersonData(); // Recargar los datos en el DataGridView para reflejar los cambios
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar los cambios: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}