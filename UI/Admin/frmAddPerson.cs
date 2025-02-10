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
        private readonly Dictionary<Control, string> helpMessages; // Diccionario de mensajes de ayuda

        public frmAddPerson()
        {
            InitializeComponent();
            LoadPersonTypes();
            LoadDocumentTypes();
            _personService = new PersonService();

            // Inicializar el Timer
            toolTipTimer = new Timer();
            toolTipTimer.Interval = 1000; // 2 segundos
            toolTipTimer.Tick += ToolTipTimer_Tick;

            // Inicializar el diccionario de mensajes de ayuda
            helpMessages = new Dictionary<Control, string>
            {
                { txtName, "Ingrese el nombre de la persona." },
                { txtLastName, "Ingrese el apellido de la persona." },
                { txtDomicile, "Ingrese el domicilio legal de la persona." },
                { txtEmail, "Ingrese el correo electrónico de la persona." },
                { txtPhoneNumber, "Ingrese el número de teléfono de la persona (solo números)." },
                { txtDocumentNumber, "Ingrese el número de documento de identidad de la persona." },
                { cmbTypeOfPerson, "Seleccione el tipo de persona (Ej.: Arrendatario, Propietario)." },
                { cmbTypeOfDocument, "Seleccione el tipo de documento (Ej.: DNI, Pasaporte)." },
                { btnSave, "Guarde los datos de la persona." }
            };

            // Registrar los eventos para cada control
            RegisterHelpEvents(this);
        }

        private void RegisterHelpEvents(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (helpMessages.ContainsKey(control))
                {
                    control.MouseEnter += Control_MouseEnter;
                    control.MouseLeave += Control_MouseLeave;
                }

                // Recursión para manejar controles hijos
                if (control.HasChildren)
                {
                    RegisterHelpEvents(control);
                }
            }
        }

        private void Control_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Control control && helpMessages.ContainsKey(control))
            {
                currentControl = control; // Guardar el control actual
                toolTipTimer.Start(); // Iniciar el temporizador
            }
        }

        private void Control_MouseLeave(object sender, EventArgs e)
        {
            toolTipTimer.Stop(); // Detener el temporizador
            currentControl = null; // Limpiar el control actual
        }

        private void ToolTipTimer_Tick(object sender, EventArgs e)
        {
            if (currentControl != null && helpMessages.ContainsKey(currentControl))
            {
                // Mostrar el mensaje de ayuda
                ToolTip toolTip = new ToolTip();
                toolTip.Show(helpMessages[currentControl], currentControl, 3000); // Mostrar durante 3 segundos
            }

            toolTipTimer.Stop(); // Detener el temporizador después de mostrar el mensaje
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
    }
}
