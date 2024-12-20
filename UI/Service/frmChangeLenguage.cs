﻿using Services.Facade;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace UI
{
    public partial class frmChangeLanguage : Form
    {
        public frmChangeLanguage()
        {
            InitializeComponent();
            LoadLanguageFile(); // Cargar los datos del archivo de idioma al iniciar el formulario
        }

        /// <summary>
        /// Carga el archivo de idioma existente en el DataGridView usando la capa de servicio.
        /// </summary>
        private void LoadLanguageFile()
        {
            try
            {
                // Asegurarse de que el DataGridView tenga columnas
                if (dgvLanguages.Columns.Count == 0)
                {
                    dgvLanguages.Columns.Add("Key", LanguageService.Translate("Clave"));
                    dgvLanguages.Columns.Add("Value", LanguageService.Translate("Valor"));

                    dgvLanguages.Columns[0].FillWeight = 50;
                    dgvLanguages.Columns[1].FillWeight = 50;
                }

                // Obtener todas las traducciones desde la capa de servicio
                var translations = LanguageService.LoadAllTranslations("en-US");

                // Cargar las traducciones en el DataGridView
                dgvLanguages.Rows.Clear(); // Limpiar cualquier dato anterior
                foreach (var translation in translations)
                {
                    dgvLanguages.Rows.Add(translation.Key, translation.Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar el archivo de idioma:") + " " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Maneja el evento de clic del botón "Guardar".
        /// Toma las traducciones modificadas del DataGridView y pregunta si se desea crear un nuevo archivo o modificar el existente.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Obtener los datos del DataGridView
                var updatedTranslations = new Dictionary<string, string>();
                foreach (DataGridViewRow row in dgvLanguages.Rows)
                {
                    if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                    {
                        string key = row.Cells[0].Value.ToString();
                        string value = row.Cells[1].Value.ToString();
                        updatedTranslations[key] = value;
                    }
                }

                // Preguntar al usuario si desea crear un archivo nuevo
                DialogResult result = MessageBox.Show(
                    LanguageService.Translate("¿Desea crear un archivo nuevo?"),
                    LanguageService.Translate("Guardar archivo de idioma"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    // Solicitar el nombre completo del nuevo archivo
                    string newFileName = PromptForNewFileName();
                    if (!string.IsNullOrEmpty(newFileName))
                    {
                        // Guardar los datos en un nuevo archivo usando la capa de servicios
                        LanguageService.SaveTranslationsToNewFile(updatedTranslations, newFileName); // Usamos el nombre completo dado
                        MessageBox.Show(
                            LanguageService.Translate("Idioma guardado en un nuevo archivo con éxito."),
                            LanguageService.Translate("Éxito"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    else
                    {
                        MessageBox.Show(
                            LanguageService.Translate("Debe proporcionar un nombre válido para el nuevo archivo."),
                            LanguageService.Translate("Advertencia"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }
                }
                else
                {
                    // Guardar en el archivo existente
                    LanguageService.SaveTranslations(updatedTranslations, "es-AR");
                    MessageBox.Show(
                        LanguageService.Translate("Idioma guardado con éxito en el archivo existente."),
                        LanguageService.Translate("Éxito"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al guardar el archivo:") + " " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Método para solicitar al usuario el nombre completo del nuevo archivo de idioma.
        /// </summary>
        /// <returns>El nombre completo del archivo de idioma ingresado por el usuario.</returns>
        private string PromptForNewFileName()
        {
            using (Form prompt = new Form())
            {
                prompt.Width = 300;
                prompt.Height = 150;
                prompt.Text = LanguageService.Translate("Ingrese el nombre completo del archivo de idioma");

                Label label = new Label() { Left = 20, Top = 20, Text = LanguageService.Translate("Nombre completo del archivo:") };
                TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 200 };

                Button confirmation = new Button() { Text = LanguageService.Translate("Guardar"), Left = 100, Top = 80, Width = 100 };
                confirmation.Click += (sender, e) => { prompt.DialogResult = DialogResult.OK; prompt.Close(); };

                prompt.Controls.Add(label);
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);

                return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : string.Empty;
            }
        }
    }
}