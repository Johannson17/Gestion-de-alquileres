using Domain;
using LOGIC.Facade;
using Services.Facade;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    public partial class frmAddProperty : Form
    {
        private readonly PropertyService _propertyService;
        private readonly PersonService _personService;
        private Property _newProperty; // Objeto Property que se irá llenando

        private Timer toolTipTimer;
        private Control currentControl; // Control actual sobre el que se deja el mouse
        private Dictionary<Control, string> helpMessages; // Diccionario de mensajes de ayuda

        public frmAddProperty()
        {
            InitializeComponent();
            _propertyService = new PropertyService();
            _personService = new PersonService();
            LoadOwners();
            LoadPropertyStates();

            // Inicializar el Timer
            toolTipTimer = new Timer { Interval = 1000 }; // 1 segundo
            toolTipTimer.Tick += ToolTipTimer_Tick;

            InitializeHelpMessages(); // Inicializar mensajes de ayuda traducidos
            SubscribeHelpMessagesEvents(); // Suscribir eventos de ToolTips
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
                { txtAddress, LanguageService.Translate("Ingrese la dirección completa de la propiedad.") },
                { txtCountry, LanguageService.Translate("Especifique el país donde se encuentra la propiedad.") },
                { txtProvince, LanguageService.Translate("Ingrese la provincia o región donde se encuentra la propiedad.") },
                { txtMunicipality, LanguageService.Translate("Especifique el municipio donde se ubica la propiedad.") },
                { txtDescription, LanguageService.Translate("Agregue una descripción detallada de la propiedad.") },
                { cmbOwner, LanguageService.Translate("Seleccione el propietario de esta propiedad.") },
                { cmbStatus, LanguageService.Translate("Elija el estado actual de la propiedad (Ej.: Disponible, Ocupada).") },
                { btnSave, LanguageService.Translate("Guarde los datos de la propiedad.") }
            };
        }

        /// <summary>
        /// Suscribe los eventos de `MouseEnter` y `MouseLeave` a los controles con mensajes de ayuda.
        /// </summary>
        private void SubscribeHelpMessagesEvents()
        {
            foreach (var control in helpMessages.Keys)
            {
                control.MouseEnter += Control_MouseEnter;
                control.MouseLeave += Control_MouseLeave;
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
        /// Carga los propietarios disponibles en el ComboBox `cmbOwner`.
        /// </summary>
        private void LoadOwners()
        {
            try
            {
                var owners = _personService.GetAllPersonsByType((Person.PersonTypeEnum)Enum.Parse(typeof(Person.PersonTypeEnum), "Owner"));
                cmbOwner.DataSource = owners;
                cmbOwner.DisplayMember = "NamePerson";
                cmbOwner.ValueMember = "IdPerson";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar los propietarios") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Carga los estados de propiedad en el ComboBox `cmbStatus`.
        /// </summary>
        private void LoadPropertyStates()
        {
            cmbStatus.DataSource = Enum.GetValues(typeof(PropertyStatusEnum)).Cast<PropertyStatusEnum>().ToList();
            cmbStatus.SelectedIndex = 0;
        }

        /// <summary>
        /// Guarda la propiedad ingresada en la base de datos.
        /// </summary>
        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _newProperty = new Property
                {
                    IdProperty = Guid.NewGuid(),
                    DescriptionProperty = txtDescription.Text,
                    StatusProperty = (PropertyStatusEnum)cmbStatus.SelectedItem,
                    CountryProperty = txtCountry.Text,
                    ProvinceProperty = txtProvince.Text,
                    MunicipalityProperty = txtMunicipality.Text,
                    AddressProperty = txtAddress.Text,
                    OwnerProperty = new Person
                    {
                        IdPerson = (Guid)cmbOwner.SelectedValue
                    },
                    InventoryProperty = new List<InventoryProperty>()
                };

                var result = MessageBox.Show(
                    LanguageService.Translate("¿Desea agregar inventario a esta propiedad?"),
                    LanguageService.Translate("Agregar inventario"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    var inventoryForm = new frmAddPropertyInventory(_newProperty);
                    inventoryForm.ShowDialog();
                }

                await _propertyService.CreatePropertyAsync(_newProperty);

                MessageBox.Show(
                    LanguageService.Translate("Propiedad y su inventario guardados correctamente."),
                    LanguageService.Translate("Éxito"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al guardar la propiedad") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Limpia los campos del formulario después de guardar.
        /// </summary>
        private void ClearForm()
        {
            txtDescription.Clear();
            txtProvince.Clear();
            txtMunicipality.Clear();
            txtCountry.Clear();
            txtAddress.Clear();
            cmbOwner.SelectedIndex = -1;
            cmbStatus.SelectedIndex = 0;
        }

        /// <summary>
        /// Se ejecuta cuando el usuario solicita ayuda (`F1`).
        /// </summary>
        private void FrmAddProperty_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helpMessage = string.Join(Environment.NewLine, new[]
            {
                LanguageService.Translate("Bienvenido al formulario de registro de propiedades."),
                "",
                LanguageService.Translate("Opciones disponibles:"),
                $"- {LanguageService.Translate("Ingrese la dirección, país, provincia y municipio de la propiedad.")}",
                $"- {LanguageService.Translate("Escriba una descripción detallada de la propiedad.")}",
                $"- {LanguageService.Translate("Seleccione el propietario en la lista desplegable.")}",
                $"- {LanguageService.Translate("Seleccione el estado de la propiedad.")}",
                $"- {LanguageService.Translate("Presione 'Guardar' para registrar la propiedad en el sistema.")}",
                "",
                LanguageService.Translate("Si desea agregar inventario, el sistema le preguntará después de guardar."),
                "",
                LanguageService.Translate("Para más ayuda, contacte con el administrador.")
            });

            MessageBox.Show(helpMessage,
                            LanguageService.Translate("Ayuda del sistema"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        /// <summary>
        /// Actualiza los mensajes de ayuda cuando se cambia el idioma.
        /// </summary>
        public void UpdateHelpMessages()
        {
            InitializeHelpMessages();
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
            var helpMessage =
                LanguageService.Translate("FORMULARIO DE REGISTRO DE PROPIEDADES").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Este formulario te permite dar de alta una nueva propiedad en el sistema, ") +
                LanguageService.Translate("incluyendo su ubicación, descripción, dueño y estado actual. ") +
                LanguageService.Translate("Asegúrate de completar todos los campos obligatorios para un registro correcto.") + "\r\n\r\n" +
                LanguageService.Translate("INSTRUCCIONES PASO A PASO:") + "\r\n" +
                LanguageService.Translate("1. Ingresa la dirección exacta de la propiedad (calle, número, etc.).") + "\r\n" +
                LanguageService.Translate("2. Especifica el país, la provincia y el municipio donde se ubica la propiedad.") + "\r\n" +
                LanguageService.Translate("3. Proporciona una descripción detallada (características, detalles relevantes, etc.).") + "\r\n" +
                LanguageService.Translate("4. Selecciona el dueño (propietario) en la lista desplegable.") + "\r\n" +
                LanguageService.Translate("5. Elige el estado actual de la propiedad (Disponible, Ocupada, etc.).") + "\r\n" +
                LanguageService.Translate("6. Presiona 'Guardar cambios' para registrar la propiedad en el sistema.") + "\r\n\r\n" +
                LanguageService.Translate("Tras guardar, se te preguntará si deseas agregar inventario a esta propiedad. ") +
                LanguageService.Translate("Si aceptas, podrás añadir artículos o elementos (muebles, electrodomésticos, etc.).") + "\r\n\r\n" +
                LanguageService.Translate("Si necesitas más ayuda, por favor contacta al administrador del sistema.");

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
                                    "$" + LanguageService.Translate("No se encontró la imagen de ayuda en la ruta: ") + imagePath;
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
