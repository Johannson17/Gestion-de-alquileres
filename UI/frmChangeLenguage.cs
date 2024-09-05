using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Services.Facade;

namespace UI
{
    public partial class frmChangeLanguage : Form
    {
        public frmChangeLanguage()
        {
            InitializeComponent();
            LoadLanguages();
        }

        private void LoadLanguages()
        {
            // Aquí deberías cargar el archivo de idioma existente y mostrarlo en el DataGridView
            var languages = LanguageService.GetLanguages(); // Obtén las claves de idioma existentes
            var translations = new Dictionary<string, string>();

            foreach (var key in languages)
            {
                translations[key] = LanguageService.Translate(key);
            }

            // Crear una tabla para mostrar los datos en el DataGridView
            DataTable dt = new DataTable();
            dt.Columns.Add("Key", typeof(string));
            dt.Columns.Add("Translation", typeof(string));

            foreach (var item in translations)
            {
                dt.Rows.Add(item.Key, item.Value);
            }

            // Asignar la tabla al DataGridView
            dgvLenguages.DataSource = dt;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Guardar las traducciones actualizadas
            DataTable dt = (DataTable)dgvLenguages.DataSource;

            foreach (DataRow row in dt.Rows)
            {
                string key = row["Key"].ToString();
                string translation = row["Translation"].ToString();

                // Guardar la traducción usando el servicio de lenguaje
                LanguageService.SaveTranslation(key, translation);
            }

            MessageBox.Show("Las traducciones han sido guardadas con éxito.", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}