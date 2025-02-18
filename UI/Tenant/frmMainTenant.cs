using System;
using System.Linq;
using System.Windows.Forms;
using Services.Facade;
using Domain;
using LOGIC.Facade;
using UI.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Logic;

namespace UI.Tenant
{
    public partial class frmMainTenant : Form
    {
        private readonly PersonService _tenantService;
        private readonly Guid _userId;
        private Person _loggedInPerson; // Persona asociada al usuario
        private static LanguageHelper language;

        private Dictionary<ToolStripItem, string> helpMessages;
        private Timer toolTipTimer;
        private ToolStripItem currentItem; // El elemento actual sobre el cual está el mouse
        private bool isInitializingLanguage = true;

        public frmMainTenant(Guid userId)
        {
            InitializeComponent();
            _tenantService = new PersonService();
            _userId = userId;

            // Inicialización de idiomas
            language = new LanguageHelper();
            InitializeHelpMessages(); // Cargar las ayudas en el idioma actual

            try
            {
                LoadLoggedInPerson(); // Cargar la persona asociada al usuario
                LoadAvailableLanguages();
            }
            catch (Exception ex)
            {

            }

            // Configurar el Timer
            toolTipTimer = new Timer { Interval = 1000 }; // 1 segundo
            toolTipTimer.Tick += ToolTipTimer_Tick;

            // Suscribir eventos a los elementos del menú
            SubscribeHelpMessagesEvents();

            isInitializingLanguage = false; // Finaliza la inicialización
        }

        /// <summary>
        /// Inicializa los mensajes de ayuda con la traducción actual.
        /// </summary>
        private void InitializeHelpMessages()
        {
            helpMessages = new Dictionary<ToolStripItem, string>
            {
                { contratosToolStripMenuItem, LanguageService.Translate("Abra el módulo de contratos para gestionar contratos relacionados con el inquilino actual.") },
                { ticketsToolStripMenuItem, LanguageService.Translate("Acceda al módulo de tickets para gestionar solicitudes o incidencias.") },
                { propiedadesToolStripMenuItem1, LanguageService.Translate("Genere un reporte de las propiedades asociadas al inquilino.") },
                { contratosToolStripMenuItem1, LanguageService.Translate("Genere un reporte de los contratos asociados al inquilino.") },
                { cmbLanguage, LanguageService.Translate("Seleccione un idioma para traducir la interfaz del sistema.") }
            };
        }

        /// <summary>
        /// Suscribe los eventos de los ToolTips a los elementos del menú.
        /// </summary>
        private void SubscribeHelpMessagesEvents()
        {
            if (helpMessages != null)
            {
                foreach (var item in helpMessages.Keys)
                {
                    if (item is ToolStripItem toolStripItem)
                    {
                        toolStripItem.MouseEnter += ToolStripItem_MouseEnter;
                        toolStripItem.MouseLeave += ToolStripItem_MouseLeave;
                    }
                }
            }
        }

        private void ToolStripItem_MouseEnter(object sender, EventArgs e)
        {
            if (sender is ToolStripItem toolStripItem)
            {
                currentItem = toolStripItem;
                toolTipTimer.Start();
            }
        }

        private void ToolStripItem_MouseLeave(object sender, EventArgs e)
        {
            toolTipTimer.Stop();
            currentItem = null;
        }

        private void ToolTipTimer_Tick(object sender, EventArgs e)
        {
            if (currentItem != null && helpMessages.ContainsKey(currentItem))
            {
                ToolTip toolTip = new ToolTip();
                toolTip.Show(helpMessages[currentItem], this, PointToClient(MousePosition), 3000);
            }
            toolTipTimer.Stop();
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
                this.Close();
            }
        }

        private void LoadAvailableLanguages()
        {
            cmbLanguage.Items.Clear();

            try
            {
                var availableLanguages = LanguageService.GetAvailableLanguages();
                if (availableLanguages.Any())
                {
                    cmbLanguage.Items.AddRange(availableLanguages.ToArray());
                    cmbLanguage.SelectedItem = LanguageService.GetCurrentLanguage();
                    language.ApplyLanguage(LanguageService.GetCurrentLanguage(), this);
                }
                else
                {
                    MessageBox.Show(LanguageService.Translate("No se encontraron idiomas disponibles."),
                                    LanguageService.Translate("Error"),
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageService.Translate("Error al cargar idiomas:") + " " + ex.Message,
                                LanguageService.Translate("Error"),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private async void btnTranslate_Click(object sender, EventArgs e)
        {
            if (isInitializingLanguage) return;

            if (cmbLanguage.SelectedItem != null)
            {
                var selectedLanguage = cmbLanguage.SelectedItem.ToString();
                await LanguageService.SetCurrentLanguageAsync(selectedLanguage);
                language.ApplyLanguage(selectedLanguage, this);
                UpdateHelpMessages(); // Recargar las ayudas después de cambiar el idioma
            }
            else
            {
                MessageBox.Show(LanguageService.Translate("Seleccione un idioma antes de traducir."),
                                LanguageService.Translate("Advertencia"),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Actualiza los mensajes de ayuda cuando se cambia el idioma.
        /// </summary>
        private void UpdateHelpMessages()
        {
            InitializeHelpMessages();
        }

        private void FrmMainTenant_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helpMessage = string.Join(Environment.NewLine, new[]
            {
                LanguageService.Translate("Bienvenido al sistema de administración."),
                "",
                LanguageService.Translate("Opciones disponibles:"),
                $"- {LanguageService.Translate("Gestionar tickets")}",
                $"- {LanguageService.Translate("Gestionar contratos pendientes")}",
                "",
                LanguageService.Translate("Configuraciones internas:"),
                $"- {LanguageService.Translate("Cambiar idioma")}",
                "",
                LanguageService.Translate("Para más ayuda, contacte con el administrador.")
            });

            MessageBox.Show(helpMessage,
                            LanguageService.Translate("Ayuda del sistema"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        private async Task ShowChildFormAsync(Form childForm)
        {
            try
            {
                childForm.MdiParent = this;
                childForm.Show();

                // Aplicar traducción en segundo plano sin bloquear la UI
                await Task.Yield(); // Permite a la UI seguir respondiendo antes de ejecutar la traducción
                await Task.Delay(50).ConfigureAwait(false);

                this.BeginInvoke(new Action(() =>
                {
                    language.ApplyLanguage(LanguageService.GetCurrentLanguage(), childForm);
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el formulario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void contratosToolStripMenuItem_Click(object sender, EventArgs e) => await ShowChildFormAsync(new frmContract(_loggedInPerson.IdPerson));
        private async void ticketsToolStripMenuItem_Click(object sender, EventArgs e) => await ShowChildFormAsync(new frmTicket(_loggedInPerson.IdPerson));
        private async void propiedadesToolStripMenuItem1_Click(object sender, EventArgs e) => await ShowChildFormAsync(new frmPropertiesReport(_loggedInPerson.IdPerson));
        private async void contratosToolStripMenuItem1_Click(object sender, EventArgs e) => await ShowChildFormAsync(new frmContractsReport(_loggedInPerson.IdPerson));

        public interface ITranslatable
        {
            void ApplyTranslation();
        }
    }
}
