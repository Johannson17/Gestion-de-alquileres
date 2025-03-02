using Services.Facade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LOGIC.Facade;
using Domain;
using System.Drawing;

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
            this.KeyPreview = true; // Permite que el formulario intercepte teclas como F1
            this.HelpRequested += FrmAddProperty_HelpRequested_f1; // <-- Asignación del evento
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

        private void FrmAddProperty_HelpRequested_f1(object sender, HelpEventArgs hlpevent)
        {
            hlpevent.Handled = true;

            // 1. Construimos el nombre del archivo de imagen según el formulario
            string imageFileName = $"{this.Name}.png";
            // 2. Ruta completa de la imagen (ajusta la carpeta si difiere)
            string imagePath = Path.Combine(Application.StartupPath, "..", "..", "images", imageFileName);
            imagePath = Path.GetFullPath(imagePath);

            // 3. Texto de ayuda específico para "Agregar propiedad"
            var helpMessage = string.Format(
                LanguageService.Translate("MÓDULO DE CREACIÓN DE INCIDENCIAS (TICKETS)").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Este formulario te permite reportar un problema o solicitud de mantenimiento ").ToString() +
                LanguageService.Translate("para una de las propiedades que tienes alquiladas. Podrás incluir un título, ").ToString() +
                LanguageService.Translate("descripción y opcionalmente adjuntar una imagen para ilustrar la incidencia.").ToString() + "\r\n\r\n" +
                LanguageService.Translate("INSTRUCCIONES PASO A PASO:").ToString() + "\r\n" +
                LanguageService.Translate("1. En el campo ‘Título’, ingresa un nombre breve que describa el problema (p. ej. ‘Fuga de agua’, ‘Falla eléctrica’).").ToString() + "\r\n" +
                LanguageService.Translate("2. En el campo ‘Detalle’, proporciona una descripción completa de la situación. ").ToString() +
                LanguageService.Translate("   Explica lo que sucede, desde cuándo ocurre o cualquier otra información relevante.").ToString() + "\r\n" +
                LanguageService.Translate("3. Selecciona la propiedad correspondiente en la lista desplegable ‘Propiedad’ ").ToString() +
                LanguageService.Translate("   (si tienes más de una propiedad alquilada, elige la que se ve afectada).").ToString() + "\r\n" +
                LanguageService.Translate("4. Si deseas adjuntar una imagen como evidencia (por ejemplo, una foto del daño), ").ToString() +
                LanguageService.Translate("   haz clic en ‘Subir imagen’ y elige el archivo desde tu dispositivo.").ToString() + "\r\n" +
                LanguageService.Translate("5. Para **enviar** la incidencia, haz clic en ‘Guardar incidencia’. ").ToString() +
                LanguageService.Translate("   El sistema registrará tu reporte y quedará marcado como ‘Pendiente’ en el sistema.").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Con estos pasos, habrás creado un ticket de incidencia asociado a la propiedad seleccionada. ").ToString() +
                LanguageService.Translate("Si necesitas más ayuda, por favor contacta al administrador del sistema.").ToString());

            // 4. Creamos el formulario de ayuda
            using (Form helpForm = new Form())
            {
                // El formulario se autoajusta a su contenido
                helpForm.AutoSize = true;
                helpForm.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                helpForm.StartPosition = FormStartPosition.CenterScreen;
                helpForm.Text = LanguageService.Translate("Ayuda del sistema");
                helpForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                helpForm.MaximizeBox = false;
                helpForm.MinimizeBox = false;

                // (Opcional) Limitamos el tamaño máximo para no salirnos de la pantalla
                // Esto hace que aparezca scroll si el contenido excede este tamaño
                helpForm.MaximumSize = new Size(
                    (int)(Screen.PrimaryScreen.WorkingArea.Width * 0.9),
                    (int)(Screen.PrimaryScreen.WorkingArea.Height * 0.9)
                );

                // Para permitir scroll si el contenido excede el tamaño máximo
                helpForm.AutoScroll = true;

                // 5. Creamos un FlowLayoutPanel que también se autoajuste
                FlowLayoutPanel flowPanel = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.TopDown,   // Apilar texto e imagen verticalmente
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    WrapContents = false,                    // No “romper” en más columnas
                    Padding = new Padding(10)
                };

                // 6. Label para el texto de ayuda
                Label lblHelp = new Label
                {
                    Text = helpMessage,
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    Margin = new Padding(5)
                };

                // (Opcional) Si deseas forzar que el texto no exceda cierto ancho y haga wrap:
                lblHelp.MaximumSize = new Size(800, 0);

                flowPanel.Controls.Add(lblHelp);

                // 7. PictureBox para la imagen
                PictureBox pbHelpImage = new PictureBox
                {
                    Margin = new Padding(5),
                    // Si quieres mostrar la imagen a tamaño real:
                    SizeMode = PictureBoxSizeMode.AutoSize,
                    // O si prefieres que se ajuste pero mantenga proporción:
                    // SizeMode = PictureBoxSizeMode.Zoom,
                };

                if (File.Exists(imagePath))
                {
                    pbHelpImage.Image = Image.FromFile(imagePath);
                }
                else
                {
                    lblHelp.Text += "\r\n\r\n" +
                                    LanguageService.Translate("No se encontró la imagen de ayuda en la ruta: ") + imagePath;
                }

                // (Opcional) Si usas Zoom, el PictureBox por defecto no hace auto-size, 
                // puedes darle un tamaño inicial y dejar que el form se ajuste
                // pbHelpImage.Size = new Size(600, 400);

                flowPanel.Controls.Add(pbHelpImage);

                // 8. Agregamos el FlowLayoutPanel al formulario
                helpForm.Controls.Add(flowPanel);

                // 9. Mostramos el formulario
                helpForm.ShowDialog();
            }
        }
    }
}
