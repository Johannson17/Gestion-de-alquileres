using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LOGIC.Facade;
using Domain;
using Services.Facade;

namespace UI.Admin
{
    public partial class frmModifyTicket : Form
    {
        private readonly TicketService _ticketService;
        private readonly PropertyService _propertyService;

        private Dictionary<Control, string> helpMessages;
        private Timer toolTipTimer;
        private Control currentControl; // Control actual donde está el mouse

        public frmModifyTicket()
        {
            InitializeComponent();
            _ticketService = new TicketService();
            _propertyService = new PropertyService();

            InitializeHelpMessages(); // Inicializa los mensajes de ayuda traducidos
            SubscribeHelpMessagesEvents(); // Suscribe los eventos de ToolTips

            // Configurar el Timer
            toolTipTimer = new Timer { Interval = 1000 }; // 1 segundo
            toolTipTimer.Tick += ToolTipTimer_Tick;

            LoadTickets(); // Cargar los tickets al abrir el formulario

            // Suscribir eventos adicionales
            dgvTickets.SelectionChanged += dgvTickets_SelectionChanged;
            btnSave.Click += btnSave_Click;
            btnImage.Click += btnImage_Click;
        }

        /// <summary>
        /// Inicializa los mensajes de ayuda con la traducción actual.
        /// </summary>
        private void InitializeHelpMessages()
        {
            helpMessages = new Dictionary<Control, string>
            {
                { dgvTickets, LanguageService.Translate("Seleccione un ticket para modificar o ver su imagen.") },
                { txtTitle, LanguageService.Translate("Muestra el título del ticket seleccionado.") },
                { txtDetail, LanguageService.Translate("Muestra la descripción del ticket seleccionado.") },
                { txtProperty, LanguageService.Translate("Muestra la dirección de la propiedad asociada al ticket.") },
                { cmbStatus, LanguageService.Translate("Seleccione el estado que desea asignar al ticket.") },
                { btnSave, LanguageService.Translate("Guarda los cambios realizados al estado del ticket.") },
                { btnImage, LanguageService.Translate("Muestra la imagen asociada al ticket seleccionado.") }
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

        /// <summary>
        /// Carga los tickets y los muestra en el `DataGridView`.
        /// </summary>
        private void LoadTickets()
        {
            try
            {
                var tickets = _ticketService.GetAllTickets();
                var properties = _propertyService.GetAllProperties();

                var enrichedTickets = tickets.Select(ticket =>
                {
                    var property = properties.FirstOrDefault(p => p.IdProperty == ticket.FkIdProperty);
                    return new
                    {
                        ticket.IdRequest,
                        ticket.FkIdProperty,
                        ticket.FkIdPerson,
                        ticket.TitleTicket,
                        ticket.DescriptionTicket,
                        ticket.StatusTicket,
                        ticket.ImageTicket,
                        PropertyAddress = property?.AddressProperty ?? LanguageService.Translate("Dirección no encontrada")
                    };
                }).ToList();

                dgvTickets.DataSource = enrichedTickets;

                dgvTickets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvTickets.Columns["IdRequest"].Visible = false;
                dgvTickets.Columns["FkIdProperty"].Visible = false;
                dgvTickets.Columns["FkIdPerson"].Visible = false;
                dgvTickets.Columns["ImageTicket"].Visible = false;

                dgvTickets.Columns["TitleTicket"].HeaderText = LanguageService.Translate("Título");
                dgvTickets.Columns["DescriptionTicket"].HeaderText = LanguageService.Translate("Descripción");
                dgvTickets.Columns["StatusTicket"].HeaderText = LanguageService.Translate("Estado");
                dgvTickets.Columns["PropertyAddress"].HeaderText = LanguageService.Translate("Dirección de la Propiedad");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{LanguageService.Translate("Error al cargar los tickets")}: {ex.Message}",
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Actualiza las ayudas cuando se cambia el idioma.
        /// </summary>
        public void UpdateHelpMessages()
        {
            InitializeHelpMessages();
        }

        private void FrmModifyTicket_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helpMessage = string.Join(Environment.NewLine, new[]
            {
                LanguageService.Translate("Bienvenido al módulo de gestión de tickets."),
                "",
                LanguageService.Translate("Opciones disponibles:"),
                $"- {LanguageService.Translate("Seleccionar un ticket de la lista para modificar su estado o ver su imagen.")}",
                $"- {LanguageService.Translate("Modificar el estado de un ticket usando la lista de estados disponibles.")}",
                $"- {LanguageService.Translate("Presionar el botón 'Guardar' para actualizar el estado del ticket.")}",
                $"- {LanguageService.Translate("Presionar el botón 'Imagen' para visualizar la imagen asociada al ticket.")}",
                "",
                LanguageService.Translate("Para más ayuda, contacte con el administrador.")
            });

            MessageBox.Show(helpMessage,
                            LanguageService.Translate("Ayuda del sistema"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

    private void dgvTickets_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTickets.CurrentRow == null) return;

            try
            {
                dynamic selectedRow = dgvTickets.CurrentRow.DataBoundItem;

                if (selectedRow == null) return;

                txtTitle.Text = selectedRow.TitleTicket;
                txtDetail.Text = selectedRow.DescriptionTicket;
                txtProperty.Text = selectedRow.PropertyAddress;

                var availableStatuses = new List<string>
                {
                    LanguageService.Translate("Pendiente"),
                    LanguageService.Translate("En Progreso"),
                    LanguageService.Translate("Completado")
                };

                cmbStatus.DataSource = availableStatuses;
                cmbStatus.SelectedItem = selectedRow.StatusTicket;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{LanguageService.Translate("Error al seleccionar el ticket")}: {ex.Message}",
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (dgvTickets.CurrentRow == null)
            {
                MessageBox.Show(
                    LanguageService.Translate("Seleccione un ticket para modificar."),
                    LanguageService.Translate("Advertencia"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                dynamic selectedRow = dgvTickets.CurrentRow.DataBoundItem;
                if (selectedRow == null) return;

                Guid ticketId = selectedRow.IdRequest;
                string newStatus = cmbStatus.SelectedItem?.ToString();

                if (string.IsNullOrWhiteSpace(newStatus))
                {
                    MessageBox.Show(
                        LanguageService.Translate("Seleccione un estado válido."),
                        LanguageService.Translate("Advertencia"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                _ticketService.UpdateTicketStatus(ticketId, newStatus);

                MessageBox.Show(
                    LanguageService.Translate("Estado del ticket actualizado con éxito."),
                    LanguageService.Translate("Éxito"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadTickets();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{LanguageService.Translate("Error al guardar los cambios:")} {ex.Message}",
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnImage_Click(object sender, EventArgs e)
        {
            if (dgvTickets.CurrentRow == null)
            {
                MessageBox.Show(
                    LanguageService.Translate("Seleccione un ticket para ver su imagen."),
                    LanguageService.Translate("Advertencia"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                dynamic selectedRow = dgvTickets.CurrentRow.DataBoundItem;
                if (selectedRow == null || selectedRow.ImageTicket == null)
                {
                    MessageBox.Show(
                        LanguageService.Translate("El ticket seleccionado no tiene una imagen asociada."),
                        LanguageService.Translate("Advertencia"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                ShowImage(selectedRow.ImageTicket);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{LanguageService.Translate("Error al mostrar la imagen:")} {ex.Message}",
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ShowImage(byte[] imageBytes)
        {
            using (Form imageForm = new Form())
            {
                imageForm.Text = LanguageService.Translate("Imagen del Ticket");
                imageForm.Size = new Size(600, 400);
                imageForm.StartPosition = FormStartPosition.CenterScreen;

                PictureBox pictureBox = new PictureBox
                {
                    Image = Image.FromStream(new System.IO.MemoryStream(imageBytes)),
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom
                };

                imageForm.Controls.Add(pictureBox);
                imageForm.ShowDialog();
            }
        }
    }
}
