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
        private readonly Guid _loggedInTenantId; // ID del arrendatario logueado
        private byte[] _ticketImage; // Para almacenar la imagen seleccionada temporalmente

        public frmTicket(Guid loggedInTenantId)
        {
            InitializeComponent();
            _ticketService = new TicketService();
            _propertyService = new PropertyService();
            _loggedInTenantId = loggedInTenantId;

            LoadProperties(); // Cargar propiedades activas al abrir el formulario
        }

        private void btnImage_Click(object sender, EventArgs e)
        {
            // Abre un diálogo para seleccionar una imagen
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                openFileDialog.Title = "Seleccione una imagen para el ticket";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Leer la imagen y guardarla en la variable _ticketImage
                    _ticketImage = File.ReadAllBytes(openFileDialog.FileName);
                    MessageBox.Show("Imagen cargada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void LoadProperties()
        {
            // Obtener las propiedades con contrato activo del inquilino logueado
            List<Property> activeProperties = new List<Property>();
            try
            {
                activeProperties = _propertyService.GetActivePropertiesByTenantId(_loggedInTenantId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No tienes ninguna propiedad alquilada.");
            }

            // Asignar las propiedades al ComboBox
            cmbProperty.DataSource = activeProperties;
            cmbProperty.DisplayMember = "AddressProperty"; // Asegúrate de que esta propiedad exista en la clase Property
            cmbProperty.ValueMember = "IdProperty"; // Asegúrate de que esta propiedad exista en la clase Property
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Validación de campos
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtDetail.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos antes de guardar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Crear un nuevo objeto Ticket con la imagen cargada en _ticketImage
            Ticket newTicket = new Ticket
            {
                FkIdProperty = (Guid)cmbProperty.SelectedValue,
                FkIdPerson = _loggedInTenantId,
                TitleTicket = txtTitle.Text,
                DescriptionTicket = txtDetail.Text,
                ImageTicket = _ticketImage,
                StatusTicket = "Pendiente" // Puedes ajustar el estado inicial del ticket si es necesario
            };

            // Guardar el ticket en la base de datos
            _ticketService.CreateTicket(newTicket);

            MessageBox.Show("Ticket guardado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Limpiar los campos
            txtTitle.Clear();
            txtDetail.Clear();
            _ticketImage = null; // Limpiar la imagen cargada
        }
    }
}