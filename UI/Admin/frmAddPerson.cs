﻿using Domain;
using LOGIC.Facade;
using Services.Facade;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using UI.Admin; // Asegúrate de tener esta directiva

namespace UI
{
    public partial class frmAddPerson : Form
    {
        private readonly PersonService _personService;
        private Guid _selectedUserId; // Almacena el ID del usuario seleccionado para asignarlo a la persona

        public frmAddPerson()
        {
            InitializeComponent();
            LoadPersonTypes();
            LoadDocumentTypes();
            _personService = new PersonService();
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