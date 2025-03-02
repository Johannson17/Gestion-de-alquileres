using Domain;
using LOGIC.Facade;
using Services.Facade;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using UI.Admin;

namespace UI
{
    public partial class frmAddPerson : Form
    {
        private readonly PersonService _personService;
        private Guid _selectedUserId; // Almacena el ID del usuario seleccionado para asignarlo a la persona

        private Timer toolTipTimer;
        private Control currentControl; // Control actual sobre el que se deja el mouse
        private Dictionary<Control, string> helpMessages; // Diccionario de mensajes de ayuda

        public frmAddPerson()
        {
            InitializeComponent();
            LoadPersonTypes();
            LoadDocumentTypes();
            _personService = new PersonService();

            InitializeHelpMessages(); // Inicializar mensajes de ayuda traducidos
            SubscribeHelpMessagesEvents(); // Suscribir eventos de ToolTips

            // Inicializar el Timer
            toolTipTimer = new Timer { Interval = 1000 }; // 1 segundo
            toolTipTimer.Tick += ToolTipTimer_Tick;
            this.KeyPreview = true; // Permite que el formulario intercepte teclas como F1
            this.HelpRequested += FrmAddPerson_HelpRequested_f1; // <-- Asignación del evento
        }

        /// <summary>
        /// Inicializa los mensajes de ayuda con la traducción actual.
        /// </summary>
        private void InitializeHelpMessages()
        {
            helpMessages = new Dictionary<Control, string>
            {
                { txtName, LanguageService.Translate("Ingrese el nombre de la persona.") },
                { txtLastName, LanguageService.Translate("Ingrese el apellido de la persona.") },
                { txtDomicile, LanguageService.Translate("Ingrese el domicilio legal de la persona.") },
                { txtEmail, LanguageService.Translate("Ingrese el correo electrónico de la persona.") },
                { txtPhoneNumber, LanguageService.Translate("Ingrese el número de teléfono de la persona (solo números).") },
                { txtDocumentNumber, LanguageService.Translate("Ingrese el número de documento de identidad de la persona.") },
                { cmbTypeOfPerson, LanguageService.Translate("Seleccione el tipo de persona (Ej.: Arrendatario, Propietario).") },
                { cmbTypeOfDocument, LanguageService.Translate("Seleccione el tipo de documento (Ej.: DNI, Pasaporte).") },
                { btnSave, LanguageService.Translate("Guarde los datos de la persona.") }
            };
        }

        /// <summary>
        /// Suscribe los eventos `MouseEnter` y `MouseLeave` a los controles con mensajes de ayuda.
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

        private void LoadPersonTypes()
        {
            cmbTypeOfPerson.DataSource = Enum.GetValues(typeof(Person.PersonTypeEnum));
        }

        private void LoadDocumentTypes()
        {
            List<string> documentTypes = new List<string> { "DNI", "Pasaporte" };
            cmbTypeOfDocument.DataSource = documentTypes;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    LanguageService.Translate("¿Desea asignar un usuario a esta persona?"),
                    LanguageService.Translate("Asignar Usuario"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    OpenUserSelectionForm();
                    if (_selectedUserId == Guid.Empty)
                    {
                        MessageBox.Show(
                            LanguageService.Translate("No se seleccionó ningún usuario. No se guardará la persona."),
                            LanguageService.Translate("Error"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return;
                    }
                }

                SavePerson();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al guardar la persona") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void OpenUserSelectionForm()
        {
            using (frmUsers userSelectionForm = new frmUsers())
            {
                if (userSelectionForm.ShowDialog() == DialogResult.OK)
                {
                    _selectedUserId = userSelectionForm.SelectedUserId;
                }
            }
        }

        private void SavePerson()
        {
            if (string.IsNullOrEmpty(txtName.Text) ||
                string.IsNullOrEmpty(txtLastName.Text) ||
                string.IsNullOrEmpty(txtPhoneNumber.Text))
            {
                MessageBox.Show(
                    LanguageService.Translate("Todos los campos son obligatorios."),
                    LanguageService.Translate("Validación"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            Person newPerson = new Person
            {
                IdPerson = Guid.NewGuid(),
                NamePerson = txtName.Text,
                LastNamePerson = txtLastName.Text,
                DomicilePerson = txtDomicile.Text,
                ElectronicDomicilePerson = txtEmail.Text,
                PhoneNumberPerson = int.Parse(txtPhoneNumber.Text),
                NumberDocumentPerson = int.Parse(txtDocumentNumber.Text),
                TypeDocumentPerson = cmbTypeOfDocument.SelectedItem.ToString(),
                EnumTypePerson = (Person.PersonTypeEnum)cmbTypeOfPerson.SelectedItem
            };

            _personService.CreatePerson(newPerson, _selectedUserId);

            MessageBox.Show(
                LanguageService.Translate("Persona guardada con éxito."),
                LanguageService.Translate("Éxito"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
            ClearForm();
        }

        private void ClearForm()
        {
            txtName.Clear();
            txtLastName.Clear();
            txtDomicile.Clear();
            txtEmail.Clear();
            txtPhoneNumber.Clear();
            txtDocumentNumber.Clear();
            cmbTypeOfPerson.SelectedIndex = -1;
            cmbTypeOfDocument.SelectedIndex = -1;
            _selectedUserId = Guid.Empty;
        }

        /// <summary>
        /// Actualiza las ayudas cuando se cambia el idioma.
        /// </summary>
        public void UpdateHelpMessages()
        {
            InitializeHelpMessages();
        }

        private void FrmAddPerson_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helpMessage = string.Join(Environment.NewLine, new[]
            {
                LanguageService.Translate("Bienvenido al formulario de registro de personas."),
                "",
                LanguageService.Translate("Opciones disponibles:"),
                $"- {LanguageService.Translate("Ingresar los datos personales de la persona.")}",
                $"- {LanguageService.Translate("Seleccionar el tipo de persona (Arrendatario, Propietario).")}",
                $"- {LanguageService.Translate("Seleccionar el tipo de documento (DNI, Pasaporte).")}",
                $"- {LanguageService.Translate("Opcionalmente, asignar un usuario a la persona.")}",
                $"- {LanguageService.Translate("Presionar el botón 'Guardar' para registrar la persona en el sistema.")}",
                "",
                LanguageService.Translate("Para más ayuda, contacte con el administrador.")
            });

            MessageBox.Show(helpMessage,
                            LanguageService.Translate("Ayuda del sistema"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        private void FrmAddPerson_HelpRequested_f1(object sender, HelpEventArgs hlpevent)
        {
            hlpevent.Handled = true; // Marcamos como manejado para que no se dispare otra ayuda

            // 1. Construimos dinámicamente el nombre del archivo según el nombre del form
            string imageFileName = $"{this.Name}.png";

            // 2. Generamos la ruta completa donde está la imagen
            string imagePath = Path.Combine(Application.StartupPath, "..", "..", "images", imageFileName);
            imagePath = Path.GetFullPath(imagePath);

            // 3. Texto de ayuda (puedes personalizarlo como gustes)
            var helpMessage = string.Format(
                LanguageService.Translate("FORMULARIO DE REGISTRO DE PERSONAS").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Este formulario te permite ingresar toda la información necesaria ") +
                LanguageService.Translate("para dar de alta a una persona en el sistema.") + "\r\n\r\n" +
                LanguageService.Translate("INSTRUCCIONES PASO A PASO:") + "\r\n" +
                LanguageService.Translate("1. Completa los datos personales de la persona (nombre, apellido, domicilio, etc.).") + "\r\n" +
                LanguageService.Translate("2. Selecciona el tipo de persona (Arrendatario, Propietario) y el tipo de documento (DNI, Pasaporte).") + "\r\n" +
                LanguageService.Translate("3. Opcionalmente, asigna un usuario a la persona cuando se te pregunte.") + "\r\n" +
                LanguageService.Translate("4. Presiona el botón 'Guardar' para registrar a la persona en el sistema.") + "\r\n\r\n" +
                LanguageService.Translate("Si necesitas más ayuda, por favor contacta al administrador del sistema."));

            // 4. Crear un formulario emergente para mostrar tanto el texto como la imagen
            using (Form helpForm = new Form())
            {
                helpForm.Text = LanguageService.Translate("Ayuda del sistema");
                helpForm.StartPosition = FormStartPosition.CenterParent;
                helpForm.Size = new Size(900, 700);  // Ajusta el tamaño según tus preferencias
                helpForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                helpForm.MaximizeBox = false;
                helpForm.MinimizeBox = false;

                // 5. Usamos un TableLayoutPanel para organizar texto (fila superior) e imagen (fila inferior)
                TableLayoutPanel tableLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RowCount = 2,
                    ColumnCount = 1
                };
                // Fila 0: Alto automático (para el texto)
                tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                // Fila 1: El resto del espacio (para la imagen)
                tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

                // 6. Label para el texto de ayuda
                Label lblHelp = new Label
                {
                    Text = helpMessage,
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Margin = new Padding(15),  // Márgenes para que no se pegue a los bordes
                    Font = new Font("Segoe UI", 10, FontStyle.Regular)
                };

                // 7. PictureBox para la imagen
                PictureBox pbHelpImage = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Margin = new Padding(10)
                };

                // 8. Cargamos la imagen desde el archivo, si existe
                if (File.Exists(imagePath))
                {
                    pbHelpImage.Image = Image.FromFile(imagePath);
                }
                else
                {
                    // Si no se encuentra la imagen, mostramos un texto de advertencia
                    lblHelp.Text += "\r\n\r\n" +
                                    "$" + LanguageService.Translate("No se encontró la imagen de ayuda en la ruta: ") + imagePath;
                }

                // 9. Agregamos los controles al TableLayoutPanel
                tableLayout.Controls.Add(lblHelp, 0, 0);
                tableLayout.Controls.Add(pbHelpImage, 0, 1);

                // 10. Agregamos el TableLayoutPanel al formulario de ayuda
                helpForm.Controls.Add(tableLayout);

                // 11. Mostrar el formulario de ayuda
                helpForm.ShowDialog();
            }
        }
    }
}
