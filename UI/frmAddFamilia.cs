using Services.Domain;
using Services.Facade;
using System;
using System.Windows.Forms;

namespace UI
{
    public partial class frmAddFamilia : Form
    {
        public frmAddFamilia()
        {
            InitializeComponent();
            LoadAccesos(); // Cargar patentes y familias existentes en los controles
        }

        /// <summary>
        /// Carga las patentes y familias existentes en el CheckedListBox correspondiente.
        /// </summary>
        private void LoadAccesos()
        {
            // Configurar DisplayMember para mostrar el nombre de los accesos
            chlbAccesos.DisplayMember = "Nombre"; // Asegúrate de que las clases Patente y Familia tengan la propiedad 'Nombre'

            // Cargar Patentes
            var patentes = UserService.GetAllPatentes();
            foreach (var patente in patentes)
            {
                chlbAccesos.Items.Add(patente, false); // Añadir cada patente a la lista sin seleccionar
            }

            // Cargar Familias
            var familias = UserService.GetAllFamilias();
            foreach (var familia in familias)
            {
                chlbAccesos.Items.Add(familia, false); // Añadir cada familia a la lista sin seleccionar
            }
        }

        /// <summary>
        /// Evento para el botón de guardar nueva familia.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Crear una nueva instancia de Familia
                var nuevaFamilia = new Familia
                {
                    Nombre = txtName.Text,
                    Descripcion = txtDescription.Text
                };

                // Iterar sobre los elementos seleccionados en CheckedListBox sin modificar la colección durante la enumeración
                for (int i = 0; i < chlbAccesos.CheckedItems.Count; i++)
                {
                    var acceso = chlbAccesos.CheckedItems[i] as Acceso;
                    if (acceso != null)
                    {
                        nuevaFamilia.Add(acceso);
                    }
                }

                // Registrar la nueva familia en la base de datos usando la lógica del backend
                UserService.AddFamilia(nuevaFamilia);

                // Mostrar un mensaje de éxito
                MessageBox.Show("Familia agregada con éxito.", "Registro de Familia", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Cerrar el formulario de alta de familia
                this.Close();
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                MessageBox.Show($"Error al agregar la familia: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}