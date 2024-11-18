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

        private void ticketsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Crear una nueva instancia del formulario frmTicket, pasando el ID del inquilino
            frmTicket Ticket = new frmTicket(_loggedInPerson.IdPerson);

            // Establecer el formulario frmTicket como el padre MDI
            Ticket.MdiParent = this;

            // Mostrar el formulario hijo
            Ticket.Show();
        }

        private void propiedadesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Crear una nueva instancia del formulario frmPropertiesReport, pasando el ID del inquilino
            frmPropertiesReport PropertiesReport = new frmPropertiesReport(_loggedInPerson.IdPerson);

            // Establecer el formulario frmPropertiesReport como el padre MDI
            PropertiesReport.MdiParent = this;

            // Mostrar el formulario hijo
            PropertiesReport.Show();
        }

        private void contratosToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Crear una nueva instancia del formulario frmContractsReport, pasando el ID del inquilino
            frmContractsReport ontractsReport = new frmContractsReport(_loggedInPerson.IdPerson);

            // Establecer el formulario frmContractsReport como el padre MDI
            ontractsReport.MdiParent = this;

            // Mostrar el formulario hijo
            ontractsReport.Show();
        }
    }
}
