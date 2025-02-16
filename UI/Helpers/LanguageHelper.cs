using Services.Facade;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI.Helpers
{
    public class LanguageHelper
    {
        // Cache de traducciones para evitar llamadas repetitivas a Translate.
        // Thread-safe para uso en Parallel.
        private static readonly ConcurrentDictionary<string, string> _translationCache =
            new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        /// <summary>
        /// Aplica las traducciones o restaura los textos predeterminados para el formulario principal y sus hijos.
        /// </summary>
        /// <param name="language">Idioma a aplicar.</param>
        /// <param name="frm">Formulario principal.</param>
        public void ApplyLanguage(string language, Form frm)
        {
            try
            {
                // Recolectar textos
                var itemsToTranslate = CollectControlsAndMenuItems(frm);

                // Incluir el formulario principal y su texto
                if (!string.IsNullOrWhiteSpace(frm.Text))
                {
                    itemsToTranslate.Add((frm, frm.Text));
                }

                // Traducir en lotes para no saturar la UI
                var translations = TranslateTextsParallel(itemsToTranslate);

                // Aplicar traducciones en el hilo principal
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
        /// Recolecta todos los controles y elementos de menú que necesitan traducción.
        /// </summary>
        /// <param name="control">Control raíz.</param>
        private List<(object item, string originalText)> CollectControlsAndMenuItems(Control control)
        {
            var itemsToTranslate = new List<(object item, string originalText)>();

            void CollectRecursively(Control parentControl)
            {
                if (parentControl == null) return;

                // Si el control no es TextBox o ComboBox y tiene texto, se incluye
                if (!(parentControl is TextBox) && !(parentControl is ComboBox)
                    && !string.IsNullOrWhiteSpace(parentControl.Text))
                {
                    itemsToTranslate.Add((parentControl, parentControl.Text));
                }

                // Recorrer hijos recursivamente
                foreach (Control child in parentControl.Controls)
                {
                    CollectRecursively(child);
                }

                // Recolectar elementos de menú si es un MenuStrip
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
        /// Recolecta recursivamente los sub-items de menú para traducir.
        /// </summary>
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
        /// Traduce los textos en paralelo con cache y chunking para evitar bloqueo de la UI.
        /// </summary>
        /// <param name="itemsToTranslate">Elementos que requieren traducción.</param>
        /// <returns>Diccionario con los elementos y sus textos traducidos.</returns>
        private Dictionary<object, string> TranslateTextsParallel(List<(object item, string originalText)> itemsToTranslate)
        {
            var translations = new ConcurrentDictionary<object, string>();

            // Se divide la lista en lotes (chunks) para procesarlos en paralelo sin saturar
            const int chunkSize = 20;
            var chunks = itemsToTranslate
                .Select((item, index) => new { item, index })
                .GroupBy(x => x.index / chunkSize, x => x.item)
                .ToList(); // Se obtiene una lista de lotes

            Parallel.ForEach(chunks, chunk =>
            {
                foreach (var (item, originalText) in chunk)
                {
                    string translatedText = TranslateWithCache(originalText);
                    translations[item] = translatedText;
                }
            });

            return translations.ToDictionary(k => k.Key, v => v.Value);
        }

        /// <summary>
        /// Aplica las traducciones a los controles y menús en el hilo principal.
        /// </summary>
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

        /// <summary>
        /// Traduce un texto usando la cache para no llamar repetidamente a LanguageService.
        /// </summary>
        private string TranslateWithCache(string originalText)
        {
            if (_translationCache.TryGetValue(originalText, out var cachedTranslation))
            {
                return cachedTranslation;
            }

            // Si no está en cache, llamamos al LanguageService
            var translated = LanguageService.Translate(originalText, originalText);

            // Guardar en cache
            _translationCache[originalText] = translated;
            return translated;
        }
    }
}
