using System;
using System.Linq;
using System.Windows.Forms;
using Services.Facade;
using UI.Admin;
using UI.Service;
using UI.Helpers;
using System.Collections.Generic;
using Services.Logic;
using System.Threading.Tasks;

namespace UI
{
    public partial class frmMainAdmin : Form
    {
        private static LanguageHelper language;
        private readonly BackupService _backupService;
        private Dictionary<ToolStripItem, string> helpMessages;
        private Timer toolTipTimer;
        private ToolStripItem currentItem; // Elemento actual sobre el cual está el mouse
        private bool isInitializingLanguage = true; // Evita cambios automáticos de idioma al inicio

        public frmMainAdmin()
        {
            InitializeComponent();
            _backupService = new BackupService();
            language = new LanguageHelper();
            LoadAvailableLanguages(); // Carga los idiomas sin activar eventos
            InitializeHelpMessages(); // Inicializa los mensajes de ayuda

            // Configurar el Timer
            toolTipTimer = new Timer { Interval = 1000 }; // 1 segundo
            toolTipTimer.Tick += ToolTipTimer_Tick;

            // Suscribir eventos a los elementos del menú
            SubscribeHelpMessagesEvents();

            // Vincular el evento HelpRequested al formulario
            this.HelpRequested += FrmMainAdmin_HelpRequested;

            // Fin de la inicialización
            isInitializingLanguage = false;
        }

        /// <summary>
        /// Inicializa los mensajes de ayuda con la traducción actual.
        /// </summary>
        private void InitializeHelpMessages()
        {
            helpMessages = new Dictionary<ToolStripItem, string>
            {
                { altaToolStripMenuItem, LanguageService.Translate("Permite dar de alta un nuevo contrato en el sistema.") },
                { modificaciónBajaToolStripMenuItem, LanguageService.Translate("Permite modificar o eliminar contratos existentes.") },
                { altaToolStripMenuItem1, LanguageService.Translate("Permite registrar una nueva propiedad.") },
                { modificaciónBajaToolStripMenuItem1, LanguageService.Translate("Permite modificar o eliminar propiedades registradas.") },
                { altaToolStripMenuItem2, LanguageService.Translate("Permite agregar una nueva persona al sistema.") },
                { modificaciónBajaToolStripMenuItem2, LanguageService.Translate("Permite modificar o dar de baja personas existentes.") },
                { ticketsToolStripMenuItem, LanguageService.Translate("Gestiona tickets o solicitudes registradas en el sistema.") },
                { contratosToolStripMenuItem1, LanguageService.Translate("Genera reportes detallados sobre los contratos registrados.") },
                { propiedadesToolStripMenuItem1, LanguageService.Translate("Genera reportes sobre propiedades registradas.") },
                { arrendatariosToolStripMenuItem1, LanguageService.Translate("Genera reportes sobre arrendatarios en el sistema.") },
                { restaurarCopiaToolStripMenuItem, LanguageService.Translate("Restaura copias de seguridad de la base de datos.") },
                { generarCopiaToolStripMenuItem, LanguageService.Translate("Genera una copia de seguridad de la base de datos actual.") },
                { altaDeUsuariosToolStripMenuItem, LanguageService.Translate("Permite agregar nuevos usuarios al sistema.") },
                { modificaciónDeUsuariosToolStripMenuItem, LanguageService.Translate("Permite modificar o eliminar usuarios existentes.") },
                { altaDeRolesToolStripMenuItem, LanguageService.Translate("Permite crear nuevos roles en el sistema.") },
                { modificacionDeRolesToolStripMenuItem, LanguageService.Translate("Permite modificar o eliminar roles existentes.") },
                { cmbLanguage, LanguageService.Translate("Selecciona el idioma para traducir la interfaz del sistema.") }
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

        private void FrmMainAdmin_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helpMessage = string.Join(Environment.NewLine, new[]
            {
                LanguageService.Translate("Bienvenido al sistema de administración."),
                "",
                LanguageService.Translate("Opciones disponibles:"),
                $"- {LanguageService.Translate("Gestionar personas")}",
                $"- {LanguageService.Translate("Gestionar propiedades")}",
                $"- {LanguageService.Translate("Gestionar tickets")}",
                $"- {LanguageService.Translate("Gestionar contratos")}",
                "",
                LanguageService.Translate("Configuraciones internas:"),
                $"- {LanguageService.Translate("Cambiar idioma")}",
                $"- {LanguageService.Translate("Gestionar usuarios")}",
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

        // Métodos para abrir formularios hijos de forma asíncrona
        private async void idiomasToolStripMenuItem_Click(object sender, EventArgs e) => await ShowChildFormAsync(new frmChangeLanguage());
        private async void altaToolStripMenuItem2_Click(object sender, EventArgs e) => await ShowChildFormAsync(new frmAddPerson());
        private async void modificaciónBajaToolStripMenuItem2_Click(object sender, EventArgs e) => await ShowChildFormAsync(new frmModifyPerson());
        private async void altaDeUsuariosToolStripMenuItem_Click(object sender, EventArgs e) => await ShowChildFormAsync(new frmRegister());
        private async void altaDeRolesToolStripMenuItem_Click(object sender, EventArgs e) => await ShowChildFormAsync(new frmAddFamilia());
        private async void altaToolStripMenuItem1_Click(object sender, EventArgs e) => await ShowChildFormAsync(new frmAddProperty());
        private async void modificaciónBajaToolStripMenuItem1_Click(object sender, EventArgs e) => await ShowChildFormAsync(new frmModifyProperty());
        private async void modificaciónDeUsuariosToolStripMenuItem_Click(object sender, EventArgs e) => await ShowChildFormAsync(new frmModifyUser());
        private async void modificacionDeRolesToolStripMenuItem_Click(object sender, EventArgs e) => await ShowChildFormAsync(new frmModifyFamily());
        private async void altaToolStripMenuItem_Click(object sender, EventArgs e) => await ShowChildFormAsync(new frmAddContract());
        private async void modificaciónBajaToolStripMenuItem_Click(object sender, EventArgs e) => await ShowChildFormAsync(new frmModifyContract());
        private async void ticketsToolStripMenuItem_Click(object sender, EventArgs e) => await ShowChildFormAsync(new frmModifyTicket());
        private async void contratosToolStripMenuItem1_Click(object sender, EventArgs e) => await ShowChildFormAsync(new frmContractsReport());
        private async void propiedadesToolStripMenuItem1_Click(object sender, EventArgs e) => await ShowChildFormAsync(new frmPropertiesReport());
        private async void arrendatariosToolStripMenuItem1_Click(object sender, EventArgs e) => await ShowChildFormAsync(new frmTenantsReport());
        private async void restaurarCopiaToolStripMenuItem_Click(object sender, EventArgs e) => await ShowChildFormAsync(new frmBackup_Restore());


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


        public interface ITranslatable
        {
            void ApplyTranslation();
        }
    }
}
