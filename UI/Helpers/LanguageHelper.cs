using Services.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI.Helpers
{
    public class LanguageHelper
    {
        /// <summary>
        /// Aplica las traducciones a todos los elementos del formulario principal y sus hijos.
        /// </summary>
        /// <param name="language">Idioma a aplicar.</param>
        public void ApplyLanguage(string language, Form frm)
        {
            try
            {
                // Recarga las traducciones desde el archivo
                LanguageService.ReloadLanguages(language);

                // Aplicar traducción al formulario principal
                ApplyTranslationsRecursively(frm);

                // Traducir los formularios hijos abiertos
                foreach (Form child in frm.MdiChildren)
                {
                    if (child is ITranslatable translatableForm)
                    {
                        translatableForm.ApplyTranslation();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al aplicar el idioma: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Busca y aplica traducciones recursivamente a todos los controles dentro de un formulario o contenedor.
        /// </summary>
        /// <param name="control">El control raíz desde el cual comenzar la búsqueda.</param>
        public void ApplyTranslationsRecursively(Control control)
        {
            if (control == null) return;

            // Ignorar ciertos controles como TextBox y ComboBox
            if (!(control is TextBox))
            {
                // Intentar traducir el texto del control si existe
                if (!string.IsNullOrWhiteSpace(control.Text))
                {
                    control.Text = LanguageService.Translate(control.Text, control.Text);
                }
            }

            // Procesar controles hijos recursivamente
            foreach (Control childControl in control.Controls)
            {
                ApplyTranslationsRecursively(childControl);
            }

            // Si el control es un menú (MenuStrip), traducir sus items
            if (control is MenuStrip menuStrip)
            {
                foreach (ToolStripMenuItem menuItem in menuStrip.Items)
                {
                    TranslateMenuItem(menuItem);
                }
            }
        }

        /// <summary>
        /// Traduce recursivamente los textos de los elementos de un menú.
        /// </summary>
        /// <param name="menuItem">El elemento de menú a traducir.</param>
        public void TranslateMenuItem(ToolStripMenuItem menuItem)
        {
            if (!string.IsNullOrWhiteSpace(menuItem.Text))
            {
                menuItem.Text = LanguageService.Translate(menuItem.Text, menuItem.Text);
            }

            // Traducir los sub-items del menú
            foreach (ToolStripItem subItem in menuItem.DropDownItems)
            {
                if (subItem is ToolStripMenuItem subMenuItem)
                {
                    TranslateMenuItem(subMenuItem);
                }
            }
        }

        /// <summary>
        /// Traduce un string al idioma que está siendo usado actualmente en el sistema.
        /// </summary>
        /// <param name="text">El texto que se desea traducir.</param>
        /// <returns>El texto traducido si existe, de lo contrario el texto original.</returns>
        public string TranslateString(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            try
            {
                // Obtener el idioma actual del sistema
                var currentLanguage = LanguageService.GetCurrentLanguage();

                // Intentar traducir la cadena
                var translatedText = LanguageService.Translate(text, currentLanguage);

                // Retornar el texto traducido si es diferente al original
                return !string.IsNullOrWhiteSpace(translatedText) ? translatedText : text;
            }
            catch
            {
                // En caso de error, retornar el texto original
                return text;
            }
        }
    }
}
