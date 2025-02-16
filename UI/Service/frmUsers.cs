using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Services.Facade;
using Domain;
using Services.Domain;

namespace UI.Admin
{
    public partial class frmUsers : Form
    {
        public Guid SelectedUserId { get; private set; } // Property to store the selected User ID

        private Dictionary<Control, string> helpMessages; // Diccionario para mensajes de ayuda
        private Timer toolTipTimer; // Temporizador para mostrar ToolTips
        private Control currentControl; // Control actual donde está el mouse

        public frmUsers()
        {
            InitializeComponent();
            LoadUsers(); // Cargar usuarios al abrir el formulario
            InitializeHelpMessages(); // Inicializar mensajes de ayuda traducidos
            SubscribeHelpMessagesEvents(); // Suscribir eventos de ayuda

            // Configurar el Timer
            toolTipTimer = new Timer { Interval = 1000 }; // 1 segundo
            toolTipTimer.Tick += ToolTipTimer_Tick;

            dgvUsers.CellDoubleClick += dgvUsers_CellDoubleClick; // Vincular evento de doble clic
            this.HelpRequested += FrmUsers_HelpRequested; // Vincular ayuda contextual
        }

        /// <summary>
        /// Inicializa los mensajes de ayuda con la traducción actual.
        /// </summary>
        private void InitializeHelpMessages()
        {
            helpMessages = new Dictionary<Control, string>
            {
                { dgvUsers, LanguageService.Translate("Seleccione un usuario haciendo doble clic para continuar.") }
            };
        }

        /// <summary>
        /// Suscribe los eventos de ayuda (`MouseEnter` y `MouseLeave`) a los controles.
        /// </summary>
        private void SubscribeHelpMessagesEvents()
        {
            if (helpMessages != null)
            {
                foreach (var control in helpMessages.Keys)
                {
                    control.MouseEnter += Control_MouseEnter;
                    control.MouseLeave += Control_MouseLeave;
                }
            }
        }

        private void Control_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Control control && helpMessages.ContainsKey(control))
            {
                currentControl = control;
                toolTipTimer.Start();
            }
        }

        private void Control_MouseLeave(object sender, EventArgs e)
        {
            toolTipTimer.Stop();
            currentControl = null;
        }

        private void ToolTipTimer_Tick(object sender, EventArgs e)
        {
            if (currentControl != null && helpMessages.ContainsKey(currentControl))
            {
                ToolTip toolTip = new ToolTip();
                toolTip.Show(helpMessages[currentControl], currentControl, 3000);
            }
            toolTipTimer.Stop();
        }

        /// <summary>
        /// Carga los usuarios y los muestra en el `DataGridView`.
        /// </summary>
        private void LoadUsers()
        {
            try
            {
                List<Usuario> users = UserService.GetAllUsers();
                dgvUsers.DataSource = users;

                dgvUsers.Columns["IdUsuario"].HeaderText = LanguageService.Translate("ID de Usuario");
                dgvUsers.Columns["UserName"].HeaderText = LanguageService.Translate("Nombre de Usuario");
                dgvUsers.Columns["Password"].Visible = false; // Ocultar contraseña

                dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{LanguageService.Translate("Error al cargar los usuarios")}: {ex.Message}",
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void dgvUsers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    SelectedUserId = (Guid)dgvUsers.Rows[e.RowIndex].Cells["IdUsuario"].Value;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"{LanguageService.Translate("Error al seleccionar el usuario")}: {ex.Message}",
                        LanguageService.Translate("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Actualiza los mensajes de ayuda cuando se cambia el idioma.
        /// </summary>
        public void UpdateHelpMessages()
        {
            InitializeHelpMessages();
        }

        private void FrmUsers_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helpMessage = string.Join(Environment.NewLine, new[]
            {
                LanguageService.Translate("Bienvenido al módulo de gestión de usuarios."),
                "",
                LanguageService.Translate("Opciones disponibles:"),
                $"- {LanguageService.Translate("Seleccionar un usuario haciendo doble clic en la lista.")}",
                $"- {LanguageService.Translate("Se cerrará este formulario y el usuario seleccionado será devuelto.")}",
                "",
                LanguageService.Translate("Para más ayuda, contacte con el administrador.")
            });

            MessageBox.Show(helpMessage,
                            LanguageService.Translate("Ayuda del sistema"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }
    }
}
