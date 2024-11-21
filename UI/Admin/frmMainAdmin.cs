using System;
using System.Linq;
using System.Windows.Forms;
using Services.Facade;
using UI.Admin;
using UI.Service;
using UI.Helpers;
using System.Collections.Generic;

namespace UI
{
    public partial class frmMainAdmin : Form
    {
        private static LanguageHelper language;
        private readonly BackupService _backupService;

        public frmMainAdmin()
        {
            InitializeComponent();
            LoadAvailableLanguages();
            language = new LanguageHelper();
            _backupService = new BackupService();
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
                    foreach (var language in availableLanguages)
                    {
                        cmbLanguage.Items.Add(language);
                    }

                    var currentLanguage = LanguageService.GetCurrentLanguage();
                    cmbLanguage.SelectedItem = cmbLanguage.Items.Contains(currentLanguage)
                        ? currentLanguage
                        : cmbLanguage.Items[0];
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
                    LanguageService.Translate("Error al cargar idiomas:") + " " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnTranslate_Click(object sender, EventArgs e)
        {
            // Cambiar el idioma según el seleccionado en el ComboBox
            if (cmbLanguage.SelectedItem != null)
            {
                var selectedLanguage = cmbLanguage.SelectedItem.ToString();
                LanguageService.SetCurrentLanguage(selectedLanguage);
                try
                {
                    language.ApplyLanguage(selectedLanguage, this);
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
                MessageBox.Show(
                    LanguageService.Translate("Seleccione un idioma antes de traducir."),
                    LanguageService.Translate("Advertencia"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void idiomasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(new frmChangeLanguage());
        }

        private void altaToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ShowChildForm(new frmAddPerson());
        }

        private void modificaciónBajaToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ShowChildForm(new frmModifyPerson());
        }

        private void altaDeUsuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(new frmRegister());
        }

        private void altaDeRolesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(new frmAddFamilia());
        }

        private void altaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowChildForm(new frmAddProperty());
        }

        private void modificaciónBajaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowChildForm(new frmModifyProperty());
        }

        private void modificaciónDeUsuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(new frmModifyUser());
        }

        private void modificacionDeRolesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(new frmModifyFamily());
        }

        private void altaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(new frmAddContract());
        }

        private void modificaciónBajaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(new frmModifyContract());
        }

        private void ticketsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(new frmModifyTicket());
        }

        private void contratosToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowChildForm(new frmContractsReport());
        }

        private void propiedadesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowChildForm(new frmPropertiesReport());
        }

        private void arrendatariosToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowChildForm(new frmTenantsReport());
        }

        private void restaurarCopiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(new frmBackup_Restore());
        }

        /// <summary>
        /// Generar respaldos de todas las bases de datos y notificar al usuario.
        /// </summary>
        private void generarCopiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Llamar a la facade para generar los respaldos
                _backupService.GenerateBackup();

                // Mostrar mensaje de éxito
                MessageBox.Show(
                    LanguageService.Translate("Respaldo generado con éxito."),
                    LanguageService.Translate("Éxito"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                // Mostrar mensaje de error
                MessageBox.Show(
                    LanguageService.Translate("Error al generar el respaldo:") + " " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
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
            language.ApplyLanguage(cmbLanguage.SelectedItem.ToString(), this);
        }
    }

    /// <summary>
    /// Interfaz para formularios traducibles.
    /// </summary>
    public interface ITranslatable
    {
        void ApplyTranslation();
    }
}