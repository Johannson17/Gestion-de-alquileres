using Domain;
using LOGIC.Facade;
using Services.Facade;
using System;
using System.Collections.Generic;
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
    }
}
