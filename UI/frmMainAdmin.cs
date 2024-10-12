using System;
using System.Windows.Forms;

namespace UI
{
    public partial class frmMainAdmin : Form
    {
        public frmMainAdmin()
        {
            InitializeComponent();
        }

        private void idiomasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Crear una nueva instancia del formulario frmChangeLanguage
            frmChangeLanguage changeLanguageForm = new frmChangeLanguage();

            // Establecer el formulario frmMainAdmin como el padre MDI
            changeLanguageForm.MdiParent = this;

            // Mostrar el formulario hijo
            changeLanguageForm.Show();
        }

        private void altaToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // Crear una nueva instancia del formulario frmAddPerson
            frmAddPerson addPersonForm = new frmAddPerson();

            // Establecer el formulario frmMainAdmin como el padre MDI
            addPersonForm.MdiParent = this;

            // Mostrar el formulario hijo
            addPersonForm.Show();
        }

        private void modificaciónBajaToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // Crear una nueva instancia del formulario frmModifyPerson
            frmModifyPerson modifyPersonForm = new frmModifyPerson();

            // Establecer el formulario frmMainAdmin como el padre MDI
            modifyPersonForm.MdiParent = this;

            // Mostrar el formulario hijo
            modifyPersonForm.Show();
        }

        private void bajaDeUsuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
