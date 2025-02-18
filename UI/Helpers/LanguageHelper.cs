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
        private static readonly ConcurrentDictionary<string, string> _translationCache =
            new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        /// <summary>
        /// Aplica las traducciones al formulario principal y a todos los formularios hijos recursivamente.
        /// </summary>
        /// <param name="language">Idioma a aplicar.</param>
        /// <param name="frm">Formulario principal.</param>
        public void ApplyLanguage(string language, Form frm)
        {
            try
            {
                // Recolectar todos los formularios y controles a traducir
                var itemsToTranslate = CollectAllFormsAndControls(frm);

                // Traducir en paralelo
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
        /// Recolecta todos los controles y formularios hijos que necesitan traducción.
        /// </summary>
        /// <param name="parentForm">Formulario principal.</param>
        /// <returns>Lista de elementos a traducir.</returns>
        private List<(object item, string originalText)> CollectAllFormsAndControls(Form parentForm)
        {
            var itemsToTranslate = new List<(object item, string originalText)>();

            void CollectRecursively(Control parentControl)
            {
                if (parentControl == null) return;

                if (!(parentControl is TextBox || parentControl is ComboBox) && !string.IsNullOrWhiteSpace(parentControl.Text))
                {
                    itemsToTranslate.Add((parentControl, parentControl.Text));
                }

                foreach (Control child in parentControl.Controls)
                {
                    CollectRecursively(child);
                }

                if (parentControl is MenuStrip menuStrip)
                {
                    foreach (ToolStripMenuItem menuItem in menuStrip.Items)
                    {
                        CollectMenuItemsRecursively(menuItem, itemsToTranslate);
                    }
                }
            }

            // Traducir el título del formulario principal
            if (!string.IsNullOrWhiteSpace(parentForm.Text))
            {
                itemsToTranslate.Add((parentForm, parentForm.Text));
            }

            CollectRecursively(parentForm);

            // Traducir todos los formularios hijos abiertos
            foreach (Form childForm in Application.OpenForms)
            {
                if (childForm != parentForm)
                {
                    if (!string.IsNullOrWhiteSpace(childForm.Text))
                    {
                        itemsToTranslate.Add((childForm, childForm.Text));
                    }
                    CollectRecursively(childForm);
                }
            }

            return itemsToTranslate;
        }

        /// <summary>
        /// Recolecta elementos de menú recursivamente.
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
        /// Traduce los textos en paralelo con caché y lotes para optimizar rendimiento.
        /// </summary>
        private Dictionary<object, string> TranslateTextsParallel(List<(object item, string originalText)> itemsToTranslate)
        {
            var translations = new ConcurrentDictionary<object, string>();

            const int chunkSize = 20;
            var chunks = itemsToTranslate
                .Select((item, index) => new { item, index })
                .GroupBy(x => x.index / chunkSize, x => x.item)
                .ToList();

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
        /// Aplica las traducciones en la UI.
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
        /// Traduce un texto utilizando caché para mejorar el rendimiento.
        /// </summary>
        private string TranslateWithCache(string originalText)
        {
            if (_translationCache.TryGetValue(originalText, out var cachedTranslation))
            {
                return cachedTranslation;
            }

            var translated = LanguageService.Translate(originalText, originalText);
            _translationCache[originalText] = translated;
            return translated;
        }
    }
}
