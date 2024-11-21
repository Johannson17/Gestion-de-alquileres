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

        public frmModifyTicket()
        {
            InitializeComponent();
            _ticketService = new TicketService();
            _propertyService = new PropertyService();

            LoadTickets();

            // Subscribe to events
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
                        PropertyAddress = property?.AddressProperty ?? LanguageService.Translate("Property not found")
                    };
                }).ToList();

                dgvTickets.DataSource = enrichedTickets;

                // Adjust columns to fill available width
                dgvTickets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Column configuration
                dgvTickets.Columns["IdRequest"].Visible = false;
                dgvTickets.Columns["FkIdProperty"].Visible = false;
                dgvTickets.Columns["FkIdPerson"].Visible = false;
                dgvTickets.Columns["ImageTicket"].Visible = false;

                dgvTickets.Columns["TitleTicket"].HeaderText = LanguageService.Translate("Title");
                dgvTickets.Columns["DescriptionTicket"].HeaderText = LanguageService.Translate("Description");
                dgvTickets.Columns["StatusTicket"].HeaderText = LanguageService.Translate("Status");
                dgvTickets.Columns["PropertyAddress"].HeaderText = LanguageService.Translate("Property Address");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{LanguageService.Translate("Error loading tickets")}: {ex.Message}",
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void dgvTickets_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTickets.CurrentRow == null) return;

            try
            {
                dynamic selectedRow = dgvTickets.CurrentRow.DataBoundItem;

                if (selectedRow == null) return;

                // Assign data to form fields
                txtTitle.Text = selectedRow.TitleTicket;
                txtDetail.Text = selectedRow.DescriptionTicket;
                txtProperty.Text = selectedRow.PropertyAddress;

                var availableStatuses = new List<string>
                {
                    LanguageService.Translate("Pending"),
                    LanguageService.Translate("In Progress"),
                    LanguageService.Translate("Completed")
                };

                cmbStatus.DataSource = availableStatuses;
                cmbStatus.SelectedItem = selectedRow.StatusTicket;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{LanguageService.Translate("Error selecting ticket")}: {ex.Message}",
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
                    LanguageService.Translate("Please select a ticket to modify."),
                    LanguageService.Translate("Warning"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                dynamic selectedRow = dgvTickets.CurrentRow.DataBoundItem;
                if (selectedRow == null)
                {
                    MessageBox.Show(
                        LanguageService.Translate("Error selecting ticket."),
                        LanguageService.Translate("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                Guid ticketId = selectedRow.IdRequest;
                string newStatus = cmbStatus.SelectedItem?.ToString();

                if (string.IsNullOrWhiteSpace(newStatus))
                {
                    MessageBox.Show(
                        LanguageService.Translate("Please select a valid status."),
                        LanguageService.Translate("Warning"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                _ticketService.UpdateTicketStatus(ticketId, newStatus);

                MessageBox.Show(
                    LanguageService.Translate("Ticket status updated successfully."),
                    LanguageService.Translate("Success"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadTickets();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{LanguageService.Translate("Error saving changes:")} {ex.Message}",
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
                    LanguageService.Translate("Please select a ticket to view its image."),
                    LanguageService.Translate("Warning"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                dynamic selectedRow = dgvTickets.CurrentRow.DataBoundItem;
                if (selectedRow == null)
                {
                    MessageBox.Show(
                        LanguageService.Translate("Error retrieving ticket data."),
                        LanguageService.Translate("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                if (selectedRow.ImageTicket == null || selectedRow.ImageTicket.Length == 0)
                {
                    MessageBox.Show(
                        LanguageService.Translate("The selected ticket does not have an associated image."),
                        LanguageService.Translate("Warning"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                ShowImage(selectedRow.ImageTicket);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{LanguageService.Translate("Error displaying image:")} {ex.Message}",
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ShowImage(byte[] imageBytes)
        {
            using (Form imageForm = new Form())
            {
                imageForm.Text = LanguageService.Translate("Ticket Image");
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
