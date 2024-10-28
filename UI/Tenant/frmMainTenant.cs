using System;
using System.Windows.Forms;
using Services.Facade;
using Domain;
using LOGIC.Facade;

namespace UI.Tenant
{
    public partial class frmMainTenant : Form
    {
        private readonly PersonService _tenantService;
        private readonly Guid _userId;
        private Person _loggedInPerson; // Persona asociada al usuario

        public frmMainTenant(Guid userId)
        {
            InitializeComponent();
            _tenantService = new PersonService();
            _userId = userId;

            LoadLoggedInPerson(); // Cargar la persona asociada al usuario
        }

        private void LoadLoggedInPerson()
        {
            // Consultar la persona usando el ID de usuario
            _loggedInPerson = _tenantService.GetPersonByUserId(_userId);

            if (_loggedInPerson == null || _loggedInPerson.EnumTypePerson != Person.PersonTypeEnum.Tenant)
            {
                MessageBox.Show("No se encontró un inquilino asociado a este usuario.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close(); // Cerrar el formulario si no se encuentra un inquilino
            }
        }

        private void contratosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Crear una nueva instancia del formulario frmContract, pasando el ID del inquilino
            frmContract Contract = new frmContract(_loggedInPerson.IdPerson);

            // Establecer el formulario frmContract como el padre MDI
            Contract.MdiParent = this;

            // Mostrar el formulario hijo
            Contract.Show();
        }
    }
}
