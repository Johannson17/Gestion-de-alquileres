using Services.Facade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LOGIC.Facade;
using Domain;

namespace UI.Tenant
{
    public partial class frmTicket : Form
    {
        private readonly TicketService _ticketService;
        private readonly PropertyService _propertyService;
        private readonly Guid _loggedInTenantId; // ID del inquilino logueado
        private byte[] _ticketImage; // Para almacenar temporalmente la imagen seleccionada
        private Timer toolTipTimer;
        private Control currentControl;
        private Dictionary<Control, string> helpMessages; // Mensajes de ayuda traducidos dinámicamente

        public frmTicket(Guid loggedInTenantId)
        {
            InitializeComponent();
            _ticketService = new TicketService();
            _propertyService = new PropertyService();
            _loggedInTenantId = loggedInTenantId;

            InitializeHelpMessages(); // Inicializar las ayudas traducidas
            SubscribeHelpMessagesEvents(); // Suscribir eventos de ToolTips

            toolTipTimer = new Timer { Interval = 2000 }; // 2 segundos
            toolTipTimer.Tick += ToolTipTimer_Tick;

            LoadProperties(); // Cargar propiedades activas al abrir el formulario
        }

        /// <summary>
        /// Inicializa los mensajes de ayuda con la traducción actual.
        /// </summary>
        private void InitializeHelpMessages()
        {
            helpMessages = new Dictionary<Control, string>
            {
                { txtTitle, LanguageService.Translate("Ingrese un título para describir el problema.") },
                { txtDetail, LanguageService.Translate("Proporcione detalles sobre el problema o solicitud.") },
                { cmbProperty, LanguageService.Translate("Seleccione la propiedad relacionada con el ticket.") },
                { btnSave, LanguageService.Translate("Guarda el ticket con los detalles proporcionados.") },
                { btnImage, LanguageService.Translate("Sube una imagen para adjuntar al ticket.") }
            };
        }

        /// <summary>
        /// Suscribe los eventos `MouseEnter` y `MouseLeave` a los controles con mensajes de ayuda.
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

        private void btnImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = LanguageService.Translate("Archivos de Imagen") + "|*.jpg;*.jpeg;*.png;*.bmp";
                openFileDialog.Title = LanguageService.Translate("Seleccione una imagen para el ticket");

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _ticketImage = File.ReadAllBytes(openFileDialog.FileName);
                    MessageBox.Show(
                        LanguageService.Translate("Imagen cargada exitosamente."),
                        LanguageService.Translate("Éxito"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
        }

        /// <summary>
        /// Carga las propiedades activas del inquilino logueado en el ComboBox.
        /// </summary>
        private void LoadProperties()
        {
            List<Property> activeProperties = new List<Property>();
            try
            {
                activeProperties = _propertyService.GetActivePropertiesByTenantId(_loggedInTenantId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("No tienes ninguna propiedad alquilada."),
                    LanguageService.Translate("Advertencia"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }

            cmbProperty.DataSource = activeProperties;
            cmbProperty.DisplayMember = "AddressProperty";
            cmbProperty.ValueMember = "IdProperty";
        }

        /// <summary>
        /// Guarda un nuevo ticket en la base de datos.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtDetail.Text))
            {
                MessageBox.Show(
                    LanguageService.Translate("Por favor, complete todos los campos antes de guardar."),
                    LanguageService.Translate("Advertencia"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            Ticket newTicket = new Ticket
            {
                FkIdProperty = (Guid)cmbProperty.SelectedValue,
                FkIdPerson = _loggedInTenantId,
                TitleTicket = txtTitle.Text,
                DescriptionTicket = txtDetail.Text,
                ImageTicket = _ticketImage,
                StatusTicket = LanguageService.Translate("Pendiente")
            };

            _ticketService.CreateTicket(newTicket);

            MessageBox.Show(
                LanguageService.Translate("Ticket guardado exitosamente."),
                LanguageService.Translate("Éxito"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            txtTitle.Clear();
            txtDetail.Clear();
            _ticketImage = null;
        }

        /// <summary>
        /// Actualiza las ayudas cuando se cambia el idioma.
        /// </summary>
        public void UpdateHelpMessages()
        {
            InitializeHelpMessages();
        }

        private void FrmTicket_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helpMessage = string.Join(Environment.NewLine, new[]
            {
                LanguageService.Translate("Bienvenido al módulo de tickets."),
                "",
                LanguageService.Translate("Opciones disponibles:"),
                $"- {LanguageService.Translate("Ingrese un título y descripción del problema.")}",
                $"- {LanguageService.Translate("Seleccione la propiedad asociada al ticket.")}",
                $"- {LanguageService.Translate("Presione el botón 'Subir Imagen' para adjuntar evidencia.")}",
                $"- {LanguageService.Translate("Presione el botón 'Guardar' para enviar el ticket.")}",
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
