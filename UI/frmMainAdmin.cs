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
    }
}