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

            LoadTickets();

            // Suscripción a eventos
            dgvTickets.SelectionChanged += dgvTickets_SelectionChanged;
            btnSave.Click += btnSave_Click;
            btnImage.Click += btnImage_Click;
        }

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
                        PropertyAddress = property?.AddressProperty ?? "Propiedad no encontrada"
                    };
                }).ToList();

                dgvTickets.DataSource = enrichedTickets;

                // Ajustar tamaño de columnas automáticamente para ocupar todo el ancho disponible
                dgvTickets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Configuración de columnas
                dgvTickets.Columns["IdRequest"].Visible = false;
                dgvTickets.Columns["FkIdProperty"].Visible = false;
                dgvTickets.Columns["FkIdPerson"].Visible = false;
                dgvTickets.Columns["ImageTicket"].Visible = false;

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
            if (dgvTickets.CurrentRow == null) return;

            try
            {
                dynamic selectedRow = dgvTickets.CurrentRow.DataBoundItem;

                if (selectedRow == null) return;

                // Asignar datos al formulario
                txtTitle.Text = selectedRow.TitleTicket;
                txtDetail.Text = selectedRow.DescriptionTicket;
                txtProperty.Text = selectedRow.PropertyAddress;

                var availableStatuses = new List<string> { "Pendiente", "En Progreso", "Completado" };
                cmbStatus.DataSource = availableStatuses;
                cmbStatus.SelectedItem = selectedRow.StatusTicket;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al seleccionar el ticket: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (dgvTickets.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un ticket para modificar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                dynamic selectedRow = dgvTickets.CurrentRow.DataBoundItem;
                if (selectedRow == null)
                {
                    MessageBox.Show("Error al seleccionar el ticket.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Guid ticketId = selectedRow.IdRequest;
                string newStatus = cmbStatus.SelectedItem?.ToString();

                if (string.IsNullOrWhiteSpace(newStatus))
                {
                    MessageBox.Show("Seleccione un estado válido.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _ticketService.UpdateTicketStatus(ticketId, newStatus);

                MessageBox.Show("Estado del ticket actualizado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadTickets();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar los cambios: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImage_Click(object sender, EventArgs e)
        {
            if (dgvTickets.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un ticket para ver su imagen.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                dynamic selectedRow = dgvTickets.CurrentRow.DataBoundItem;
                if (selectedRow == null)
                {
                    MessageBox.Show("Error al obtener los datos del ticket.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (selectedRow.ImageTicket == null || selectedRow.ImageTicket.Length == 0)
                {
                    MessageBox.Show("El ticket seleccionado no tiene una imagen asociada.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ShowImage(selectedRow.ImageTicket);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al mostrar la imagen: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowImage(byte[] imageBytes)
        {
            using (Form imageForm = new Form())
            {
                imageForm.Text = "Imagen del Ticket";
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
