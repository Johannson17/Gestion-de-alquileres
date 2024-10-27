using System;
using System.Windows.Forms;
using UI.Service;

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

        private void altaDeUsuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Crear una nueva instancia del formulario frmRegister
            frmRegister Register = new frmRegister();

            // Establecer el formulario frmRegister como el padre MDI
            Register.MdiParent = this;

            // Mostrar el formulario hijo
            Register.Show();
        }

        private void altaDeRolesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Crear una nueva instancia del formulario frmAddFamilia
            frmAddFamilia AddFamilia = new frmAddFamilia();

            // Establecer el formulario frmAddFamilia como el padre MDI
            AddFamilia.MdiParent = this;

            // Mostrar el formulario hijo
            AddFamilia.Show();
        }

        private void altaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Crear una nueva instancia del formulario frmAddProperty
            frmAddProperty AddProperty = new frmAddProperty();

            // Establecer el formulario frmAddProperty como el padre MDI
            AddProperty.MdiParent = this;

            // Mostrar el formulario hijo
            AddProperty.Show();
        }

        private void modificaciónBajaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Crear una nueva instancia del formulario frmModifyProperty
            frmModifyProperty ModifyProperty = new frmModifyProperty();

            // Establecer el formulario frmAddProperty como el padre MDI
            ModifyProperty.MdiParent = this;

            // Mostrar el formulario hijo
            ModifyProperty.Show();
        }

        private void modificaciónDeUsuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Crear una nueva instancia del formulario frmModifyUser
            frmModifyUser ModifyUser = new frmModifyUser();

            // Establecer el formulario frmModifyUser como el padre MDI
            ModifyUser.MdiParent = this;

            // Mostrar el formulario hijo
            ModifyUser.Show();
        }

        private void modificacionDeRolesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Crear una nueva instancia del formulario frmModifyFamily
            frmModifyFamily ModifyFamily = new frmModifyFamily();

            // Establecer el formulario frmModifyUser como el padre MDI
            ModifyFamily.MdiParent = this;

            // Mostrar el formulario hijo
            ModifyFamily.Show();
        }

        private void altaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Crear una nueva instancia del formulario frmAddContract
            frmAddContract AddContract = new frmAddContract();

            // Establecer el formulario frmModifyUser como el padre MDI
            AddContract.MdiParent = this;

            // Mostrar el formulario hijo
            AddContract.Show();
        }

        private void modificaciónBajaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Crear una nueva instancia del formulario frmModifyContract
            frmModifyContract ModifyContract = new frmModifyContract();

            // Establecer el formulario frmModifyUser como el padre MDI
            ModifyContract.MdiParent = this;

            // Mostrar el formulario hijo
            ModifyContract.Show();
        }
    }
}
