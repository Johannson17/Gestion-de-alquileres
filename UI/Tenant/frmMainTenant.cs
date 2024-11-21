using System;
using System.Linq;
using System.Windows.Forms;
using Services.Facade;
using Domain;
using LOGIC.Facade;
using UI.Helpers;

namespace UI.Tenant
{
    public partial class frmMainTenant : Form
    {
        private readonly PersonService _tenantService;
        private readonly Guid _userId;
        private Person _loggedInPerson; // Persona asociada al usuario
        private static LanguageHelper language;

        public frmMainTenant(Guid userId)
        {
            InitializeComponent();
            _tenantService = new PersonService();
            _userId = userId;

            // Inicialización de idiomas
            language = new LanguageHelper();
            LoadAvailableLanguages();

            LoadLoggedInPerson(); // Cargar la persona asociada al usuario
        }

        private void LoadLoggedInPerson()
        {
            _loggedInPerson = _tenantService.GetPersonByUserId(_userId);

            if (_loggedInPerson == null || _loggedInPerson.EnumTypePerson != Person.PersonTypeEnum.Tenant)
            {
                MessageBox.Show(
                    LanguageService.Translate("No se encontró un inquilino asociado a este usuario."),
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                this.Close(); // Cerrar el formulario si no se encuentra un inquilino
            }
        }

        /// <summary>
        /// Carga los idiomas disponibles en el ComboBox cmbLanguage.
        /// </summary>
        private void LoadAvailableLanguages()
        {
            cmbLanguage.Items.Clear();

            try
            {
                var availableLanguages = LanguageService.GetAvailableLanguages();

                if (availableLanguages.Any())
                {
                    foreach (var lang in availableLanguages)
                    {
                        cmbLanguage.Items.Add(lang);
                    }

                    var currentLanguage = LanguageService.GetCurrentLanguage();
                    cmbLanguage.SelectedItem = cmbLanguage.Items.Contains(currentLanguage)
                        ? currentLanguage
                        : cmbLanguage.Items[0];

                    // Aplicar idioma al formulario
                    language.ApplyLanguage(currentLanguage, this);
                }
                else
                {
                    MessageBox.Show(
                        LanguageService.Translate("No se encontraron idiomas disponibles."),
                        LanguageService.Translate("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar idiomas") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Cambia el idioma del formulario principal según el seleccionado en el ComboBox.
        /// </summary>
        private void btnTranslate_Click(object sender, EventArgs e)
        {
            if (cmbLanguage.SelectedItem != null)
            {
                var selectedLanguage = cmbLanguage.SelectedItem.ToString();
                LanguageService.SetCurrentLanguage(selectedLanguage);

                try
                {
                    language.ApplyLanguage(selectedLanguage, this);
                }
                catch (Exception)
                {
                    MessageBox.Show(
                        LanguageService.Translate("Error al aplicar el idioma."),
                        LanguageService.Translate("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void contratosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(new frmContract(_loggedInPerson.IdPerson));
        }

        private void ticketsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(new frmTicket(_loggedInPerson.IdPerson));
        }

        private void propiedadesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowChildForm(new frmPropertiesReport(_loggedInPerson.IdPerson));
        }

        private void contratosToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowChildForm(new frmContractsReport(_loggedInPerson.IdPerson));
        }

        /// <summary>
        /// Abre un formulario hijo dentro del formulario principal y aplica la traducción actual.
        /// </summary>
        /// <param name="childForm">Formulario hijo a mostrar.</param>
        private void ShowChildForm(Form childForm)
        {
            childForm.MdiParent = this;

            // Suscribir al evento Shown para aplicar traducción después de mostrar el formulario
            childForm.Shown += (sender, e) =>
            {
                if (childForm is ITranslatable translatableForm)
                {
                    translatableForm.ApplyTranslation();
                }
            };

            childForm.Show();

            // Aplicar idioma actual al formulario hijo
            language.ApplyLanguage(cmbLanguage.SelectedItem.ToString(), this);
        }
    }
}
