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
        private readonly Guid _loggedInTenantId; // ID of the logged-in tenant
        private byte[] _ticketImage; // To temporarily store the selected image

        public frmTicket(Guid loggedInTenantId)
        {
            InitializeComponent();
            _ticketService = new TicketService();
            _propertyService = new PropertyService();
            _loggedInTenantId = loggedInTenantId;

            LoadProperties(); // Load active properties when the form opens
        }

        private void btnImage_Click(object sender, EventArgs e)
        {
            // Open a dialog to select an image
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                openFileDialog.Title = LanguageService.Translate("Seleccione una imagen para el ticket");

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Read the image and store it in the _ticketImage variable
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

        private void LoadProperties()
        {
            // Get properties with active contracts for the logged-in tenant
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

            // Assign the properties to the ComboBox
            cmbProperty.DataSource = activeProperties;
            cmbProperty.DisplayMember = "AddressProperty"; // Ensure this property exists in the Property class
            cmbProperty.ValueMember = "IdProperty"; // Ensure this property exists in the Property class
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Field validation
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

            // Create a new Ticket object with the loaded image in _ticketImage
            Ticket newTicket = new Ticket
            {
                FkIdProperty = (Guid)cmbProperty.SelectedValue,
                FkIdPerson = _loggedInTenantId,
                TitleTicket = txtTitle.Text,
                DescriptionTicket = txtDetail.Text,
                ImageTicket = _ticketImage,
                StatusTicket = LanguageService.Translate("Pendiente") // Adjust initial ticket status as needed
            };

            // Save the ticket to the database
            _ticketService.CreateTicket(newTicket);

            MessageBox.Show(
                LanguageService.Translate("Ticket guardado exitosamente."),
                LanguageService.Translate("Éxito"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            // Clear fields
            txtTitle.Clear();
            txtDetail.Clear();
            _ticketImage = null; // Clear the loaded image
        }
    }
}
