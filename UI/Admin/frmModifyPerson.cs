using Domain;
using LOGIC.Facade;
using Services.Facade;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace UI
{
    public partial class frmModifyPerson : Form
    {
        private readonly PersonService _personService;

        private Timer toolTipTimer;
        private Control currentControl;
        private readonly Dictionary<Control, string> helpMessages;

        public frmModifyPerson()
        {
            InitializeComponent();
            _personService = new PersonService();
            LoadPersonData();

            // Asigna el evento de clic de celda al DataGridView
            dgvPerson.CellClick += dgvPerson_CellClick;

            // Inicializar el Timer
            toolTipTimer = new Timer { Interval = 1000 }; // 2 segundos
            toolTipTimer.Tick += ToolTipTimer_Tick;

            // Inicializar los mensajes de ayuda
            helpMessages = new Dictionary<Control, string>
            {
                { txtName, "Ingrese el nombre de la persona." },
                { txtLastName, "Ingrese el apellido de la persona." },
                { txtDomicile, "Ingrese el domicilio legal de la persona." },
                { txtEmail, "Ingrese el correo electrónico de la persona." },
                { txtPhoneNumber, "Ingrese el número de teléfono de la persona." },
                { txtDocumentNumber, "Ingrese el número de documento de la persona." },
                { dgvPerson, "Seleccione una persona para modificar o eliminar sus datos." },
                { btnSave, "Guarde los cambios realizados en la persona seleccionada." },
                { btnDelete, "Elimine a la persona seleccionada de la lista." }
            };

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

        private void LoadPersonData()
        {
            try
            {
                var persons = _personService.GetAllPersonsByType(Person.PersonTypeEnum.Tenant);

                dgvPerson.DataSource = persons.Select(p => new
                {
                    p.IdPerson,
                    Name = p.NamePerson,
                    LastName = p.LastNamePerson,
                    Address = p.DomicilePerson,
                    ElectronicAddress = p.ElectronicDomicilePerson,
                    PhoneNumber = p.PhoneNumberPerson,
                    DocumentNumber = p.NumberDocumentPerson
                }).ToList();

                dgvPerson.Columns["IdPerson"].Visible = false;

                dgvPerson.Columns["Name"].HeaderText = LanguageService.Translate("Nombre");
                dgvPerson.Columns["LastName"].HeaderText = LanguageService.Translate("Apellido");
                dgvPerson.Columns["Address"].HeaderText = LanguageService.Translate("Domicilio");
                dgvPerson.Columns["ElectronicAddress"].HeaderText = LanguageService.Translate("Domicilio Electrónico");
                dgvPerson.Columns["PhoneNumber"].HeaderText = LanguageService.Translate("Teléfono");
                dgvPerson.Columns["DocumentNumber"].HeaderText = LanguageService.Translate("Número de Documento");

                dgvPerson.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar los datos") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void dgvPerson_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    var selectedPerson = (Person)dgvPerson.Rows[e.RowIndex].DataBoundItem;

                    txtName.Text = selectedPerson.NamePerson;
                    txtLastName.Text = selectedPerson.LastNamePerson;
                    txtDomicile.Text = selectedPerson.DomicilePerson;
                    txtEmail.Text = selectedPerson.ElectronicDomicilePerson;
                    txtPhoneNumber.Text = selectedPerson.PhoneNumberPerson.ToString();
                    txtDocumentNumber.Text = selectedPerson.NumberDocumentPerson.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        LanguageService.Translate("Error al seleccionar la persona") + ": " + ex.Message,
                        LanguageService.Translate("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedPerson = (Person)dgvPerson.CurrentRow?.DataBoundItem;

                if (selectedPerson == null)
                {
                    MessageBox.Show(
                        LanguageService.Translate("Seleccione una persona de la lista para modificar."),
                        LanguageService.Translate("Advertencia"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                selectedPerson.NamePerson = txtName.Text;
                selectedPerson.LastNamePerson = txtLastName.Text;
                selectedPerson.DomicilePerson = txtDomicile.Text;
                selectedPerson.ElectronicDomicilePerson = txtEmail.Text;
                selectedPerson.PhoneNumberPerson = int.Parse(txtPhoneNumber.Text);
                selectedPerson.NumberDocumentPerson = int.Parse(txtDocumentNumber.Text);

                _personService.UpdatePerson(selectedPerson);

                MessageBox.Show(
                    LanguageService.Translate("Datos actualizados correctamente."),
                    LanguageService.Translate("Éxito"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LoadPersonData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al guardar los cambios") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedPerson = (Person)dgvPerson.CurrentRow?.DataBoundItem;

                if (selectedPerson == null)
                {
                    MessageBox.Show(
                        LanguageService.Translate("Seleccione una persona de la lista para eliminar."),
                        LanguageService.Translate("Advertencia"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                var confirmation = MessageBox.Show(
                    LanguageService.Translate("¿Está seguro de que desea eliminar esta persona?"),
                    LanguageService.Translate("Confirmación"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmation == DialogResult.Yes)
                {
                    _personService.DeletePerson(selectedPerson.IdPerson);

                    MessageBox.Show(
                        LanguageService.Translate("Persona eliminada correctamente."),
                        LanguageService.Translate("Éxito"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    LoadPersonData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al eliminar la persona") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
