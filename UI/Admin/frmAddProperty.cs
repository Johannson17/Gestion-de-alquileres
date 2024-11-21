using Domain;
using LOGIC.Facade;
using Services.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    public partial class frmAddProperty : Form
    {
        private readonly PropertyService _propertyService;
        private readonly PersonService _personService;
        private Property _newProperty; // Objeto Property que se irá llenando

        public frmAddProperty()
        {
            InitializeComponent();
            _propertyService = new PropertyService();
            _personService = new PersonService();
            LoadOwners();
            LoadPropertyStates();
        }

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

        private void LoadPropertyStates()
        {
            // Cargar el enum PropertyStatusEnum en el ComboBox
            cmbStatus.DataSource = Enum.GetValues(typeof(PropertyStatusEnum)).Cast<PropertyStatusEnum>().ToList();
            cmbStatus.SelectedIndex = 0; // Seleccionar el primer valor por defecto
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Crear un nuevo objeto Property con los datos del formulario
                _newProperty = new Property
                {
                    IdProperty = Guid.NewGuid(),
                    DescriptionProperty = txtDescription.Text,
                    // Convertir el valor seleccionado del ComboBox a enum
                    StatusProperty = (PropertyStatusEnum)cmbStatus.SelectedItem,
                    CountryProperty = txtCountry.Text,
                    ProvinceProperty = txtProvince.Text,
                    MunicipalityProperty = txtMunicipality.Text,
                    AddressProperty = txtAddress.Text,
                    OwnerProperty = new Person
                    {
                        IdPerson = (Guid)cmbOwner.SelectedValue
                    },
                    InventoryProperty = new List<InventoryProperty>() // Inicializar la lista de inventarios
                };

                // Preguntar al usuario si desea agregar inventario
                var result = MessageBox.Show(
                    LanguageService.Translate("¿Desea agregar inventario a esta propiedad?"),
                    LanguageService.Translate("Agregar inventario"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    // Abrir el formulario de inventario para agregar items
                    var inventoryForm = new frmAddPropertyInventory(_newProperty);
                    inventoryForm.ShowDialog(); // Esperar a que termine de agregar inventario
                }

                // Después de agregar el inventario (si es necesario), guardar la propiedad con su inventario
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
    }
}