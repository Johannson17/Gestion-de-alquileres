using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LOGIC.Facade;
using Domain;
using System.Drawing;

namespace UI.Admin
{
    public partial class frmModifyTicket : Form
    {
        private readonly TicketService _ticketService;
        private readonly PropertyService _propertyService;

        public frmModifyTicket()
        {
            InitializeComponent();
            _ticketService = new TicketService();
            _propertyService = new PropertyService();
            LoadTickets(); // Cargar los tickets al inicializar el formulario

            // Suscribirse al evento SelectionChanged del DataGridView
            dgvTickets.SelectionChanged += dgvTickets_SelectionChanged;
        }

        private void LoadTickets()
        {
            try
            {
                // Obtener todos los tickets
                List<Ticket> tickets = _ticketService.GetAllTickets();

                // Obtener todas las propiedades
                List<Property> properties = _propertyService.GetAllProperties();

                // Enriquecer los tickets con la dirección de la propiedad asociada
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
                        PropertyAddress = property?.AddressProperty ?? "Propiedad no encontrada"
                    };
                }).ToList();

                // Asignar los tickets enriquecidos al DataGridView
                dgvTickets.DataSource = enrichedTickets;

                // Configurar columnas visibles y sus encabezados
                dgvTickets.Columns["IdRequest"].Visible = false; // Ocultar ID del ticket
                dgvTickets.Columns["FkIdProperty"].Visible = false; // Ocultar ID de la propiedad
                dgvTickets.Columns["FkIdPerson"].Visible = false; // Ocultar ID de la persona
                dgvTickets.Columns["ImageTicket"].Visible = false; // Ocultar columna de la imagen

                dgvTickets.Columns["TitleTicket"].HeaderText = "Título";
                dgvTickets.Columns["DescriptionTicket"].HeaderText = "Descripción";
                dgvTickets.Columns["StatusTicket"].HeaderText = "Estado";
                dgvTickets.Columns["PropertyAddress"].HeaderText = "Dirección de la Propiedad";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los tickets: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvTickets_SelectionChanged(object sender, EventArgs e)
        {
            // Verificar si hay una fila seleccionada
            if (dgvTickets.CurrentRow == null)
                return;

            // Obtener el ticket seleccionado
            dynamic selectedRow = dgvTickets.CurrentRow.DataBoundItem;

            if (selectedRow == null)
                return;

            // Asignar los datos del ticket seleccionado a los TextBox
            txtTitle.Text = selectedRow.TitleTicket;
            txtDetail.Text = selectedRow.DescriptionTicket;
            txtProperty.Text = selectedRow.PropertyAddress;

            // Llenar el ComboBox con el estado actual y los otros disponibles
            var availableStatuses = new List<string> { "Pendiente", "En Progreso", "Completado" }; // Personaliza los estados disponibles
            cmbStatus.DataSource = availableStatuses;

            // Seleccionar el estado actual en el ComboBox
            cmbStatus.SelectedItem = selectedRow.StatusTicket;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Validar selección de ticket
            if (dgvTickets.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un ticket para modificar el estado.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Obtener el ticket seleccionado
                dynamic selectedRow = dgvTickets.CurrentRow.DataBoundItem;

                if (selectedRow == null)
                {
                    MessageBox.Show("Error al seleccionar el ticket. Intente nuevamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Obtener el ID del ticket y el nuevo estado
                Guid ticketId = selectedRow.IdRequest;
                string newStatus = cmbStatus.SelectedItem?.ToString();

                if (string.IsNullOrWhiteSpace(newStatus))
                {
                    MessageBox.Show("Por favor, seleccione un estado válido.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Actualizar únicamente el estado del ticket
                _ticketService.UpdateTicketStatus(ticketId, newStatus);

                MessageBox.Show("Estado del ticket actualizado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Recargar la lista de tickets
                LoadTickets();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar el estado del ticket: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImage_Click(object sender, EventArgs e)
        {
            if (dgvTickets.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un ticket para ver su imagen.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener el ticket seleccionado desde el DataGridView
            dynamic selectedRow = dgvTickets.CurrentRow.DataBoundItem;

            if (selectedRow == null)
            {
                MessageBox.Show("Error al obtener los datos del ticket seleccionado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Verificar si el ticket tiene una imagen
            if (selectedRow.ImageTicket == null || selectedRow.ImageTicket.Length == 0)
            {
                MessageBox.Show("El ticket seleccionado no tiene una imagen asociada.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Mostrar la imagen en un formulario emergente
            using (Form imageForm = new Form())
            {
                imageForm.Text = "Imagen del Ticket";
                imageForm.Size = new Size(600, 400); // Tamaño del formulario
                imageForm.StartPosition = FormStartPosition.CenterScreen; // Centrar el formulario

                PictureBox pictureBox = new PictureBox
                {
                    Image = Image.FromStream(new System.IO.MemoryStream(selectedRow.ImageTicket)),
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom // Ajustar la imagen al tamaño del PictureBox
                };

                imageForm.Controls.Add(pictureBox); // Agregar el PictureBox al formulario
                imageForm.ShowDialog(); // Mostrar el formulario como emergente
            }
        }
    }
}
