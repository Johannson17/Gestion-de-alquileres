using Services.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI.Helpers
{
    public class LanguageHelper
    {
        /// <summary>
        /// Aplica las traducciones o restaura los textos predeterminados para el formulario principal y sus hijos.
        /// </summary>
        /// <param name="language">Idioma a aplicar.</param>
        /// <param name="frm">Formulario principal.</param>
        public void ApplyLanguage(string language, Form frm)
        {
            try
            {
                // Recolectar textos en el hilo principal
                var itemsToTranslate = CollectControlsAndMenuItems(frm);

                // Asegurarse de incluir el formulario principal y su texto
                if (!string.IsNullOrWhiteSpace(frm.Text))
                {
                    itemsToTranslate.Add((frm, frm.Text));
                }

                // Traducir los textos en paralelo
                var translations = TranslateTextsParallel(itemsToTranslate);

                // Aplicar las traducciones en el hilo principal
                frm.Invoke(new Action(() =>
                {
                    ApplyTranslations(translations);
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al aplicar el idioma: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Recolecta controles y elementos de menú que necesitan traducción.
        /// </summary>
        /// <param name="control">Control raíz desde el cual comenzar la recolección.</param>
        /// <returns>Lista de controles y elementos de menú con sus textos originales.</returns>
        private List<(object item, string originalText)> CollectControlsAndMenuItems(Control control)
        {
            var itemsToTranslate = new List<(object item, string originalText)>();

            void CollectRecursively(Control parentControl)
            {
                if (parentControl == null) return;

                // Asegurar que el control actual sea incluido
                if (!(parentControl is TextBox || parentControl is ComboBox) && !string.IsNullOrWhiteSpace(parentControl.Text))
                {
                    itemsToTranslate.Add((parentControl, parentControl.Text));
                }

                // Procesar controles hijos
                foreach (Control child in parentControl.Controls)
                {
                    CollectRecursively(child);
                }

                // Recolectar elementos del menú
                if (parentControl is MenuStrip menuStrip)
                {
                    foreach (ToolStripMenuItem menuItem in menuStrip.Items)
                    {
                        CollectMenuItemsRecursively(menuItem, itemsToTranslate);
                    }
                }
            }

            CollectRecursively(control);
            return itemsToTranslate;
        }

        /// <summary>
        /// Recolecta elementos de menú que necesitan traducción.
        /// </summary>
        /// <param name="menuItem">Elemento de menú raíz.</param>
        /// <param name="itemsToTranslate">Lista de elementos y textos originales.</param>
        private void CollectMenuItemsRecursively(ToolStripMenuItem menuItem, List<(object item, string originalText)> itemsToTranslate)
        {
            if (!string.IsNullOrWhiteSpace(menuItem.Text))
            {
                itemsToTranslate.Add((menuItem, menuItem.Text));
            }

            foreach (ToolStripItem subItem in menuItem.DropDownItems)
            {
                if (subItem is ToolStripMenuItem subMenuItem)
                {
                    CollectMenuItemsRecursively(subMenuItem, itemsToTranslate);
                }
            }
        }

        /// <summary>
        /// Traduce los textos recolectados utilizando hilos paralelos.
        /// </summary>
        /// <param name="itemsToTranslate">Lista de elementos y textos a traducir.</param>
        /// <returns>Diccionario con los elementos y sus textos traducidos.</returns>
        private Dictionary<object, string> TranslateTextsParallel(List<(object item, string originalText)> itemsToTranslate)
        {
            var translations = new Dictionary<object, string>();

            Parallel.ForEach(itemsToTranslate, item =>
            {
                var translatedText = LanguageService.Translate(item.originalText, item.originalText);
                lock (translations) // Bloqueo necesario para acceso seguro al diccionario
                {
                    translations[item.item] = translatedText;
                }
            });

            return translations;
        }

        /// <summary>
        /// Aplica las traducciones a los controles y elementos de menú en el hilo principal.
        /// </summary>
        /// <param name="translations">Diccionario con los elementos y sus textos traducidos.</param>
        private void ApplyTranslations(Dictionary<object, string> translations)
        {
            foreach (var translation in translations)
            {
                if (translation.Key is Control control)
                {
                    control.Text = translation.Value;
                }
                else if (translation.Key is ToolStripMenuItem menuItem)
                {
                    menuItem.Text = translation.Value;
                }
            }
        }
    }
}
