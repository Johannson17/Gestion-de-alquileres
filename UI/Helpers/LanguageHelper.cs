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
        // Diccionario concurrente para almacenar traducciones y evitar llamadas repetidas.
        // La clave se compone del idioma y el texto original: "language:text"
        private static ConcurrentDictionary<string, string> _translationCache = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Método asíncrono optimizado que recopila todos los textos de los formularios y controles,
        /// traduce los textos únicos en paralelo y luego actualiza la UI.
        /// </summary>
        public async Task ApplyLanguage(string language, Form frm)
        {
            // Obtener todos los formularios relacionados (recibido, abiertos, sus propietarios y MDI padres).
            var formsToUpdate = GetAllFormsToUpdate(frm);

            // Lista que contendrá pares de (acción para actualizar texto, texto original).
            var translationItems = new List<(Action<string> UpdateAction, string OriginalText)>();

            // Recorrer formularios y sus controles para recopilar los textos a traducir.
            foreach (var form in formsToUpdate)
            {
                if (!string.IsNullOrEmpty(form.Text))
                    translationItems.Add((translated => form.Text = translated, form.Text));
                CollectControlTranslationItems(form, translationItems);
            }

            // Obtener la lista de textos únicos para traducir.
            var distinctTexts = translationItems.Select(item => item.OriginalText).Distinct().ToList();

            // Diccionario para almacenar las traducciones resultantes.
            var translationsMap = new ConcurrentDictionary<string, string>();

            // Realizar la traducción en paralelo de los textos únicos.
            await Task.Run(() =>
            {
                Parallel.ForEach(distinctTexts, originalText =>
                {
                    var translated = TranslateWithCache(originalText, language);
                    translationsMap.TryAdd(originalText, translated);
                });
            });

            // Actualizar la UI con las traducciones obtenidas.
            // (Si este código se ejecuta fuera del hilo UI, se deberá utilizar form.Invoke)
            foreach (var item in translationItems)
            {
                item.UpdateAction(translationsMap[item.OriginalText]);
            }
        }

        /// <summary>
        /// Recolecta todos los formularios que se deben actualizar (incluye propietarios, MDI padres y formularios abiertos).
        /// </summary>
        private HashSet<Form> GetAllFormsToUpdate(Form frm)
        {
            HashSet<Form> forms = new HashSet<Form>();
            forms.Add(frm);
            AddOwnerAndMdiParent(frm, forms);

            foreach (Form openForm in Application.OpenForms)
            {
                forms.Add(openForm);
                AddOwnerAndMdiParent(openForm, forms);
            }
            return forms;
        }

        /// <summary>
        /// Agrega recursivamente el Owner y el MdiParent del formulario al conjunto.
        /// </summary>
        private void AddOwnerAndMdiParent(Form form, HashSet<Form> forms)
        {
            if (form.Owner != null && forms.Add(form.Owner))
            {
                AddOwnerAndMdiParent(form.Owner, forms);
            }
            if (form.MdiParent != null && forms.Add(form.MdiParent))
            {
                AddOwnerAndMdiParent(form.MdiParent, forms);
            }
        }

        /// <summary>
        /// Recorre recursivamente los controles y agrega a la lista los elementos que requieran traducción,
        /// evitando controles como TextBox, ComboBox o DateTimePicker.
        /// </summary>
        private void CollectControlTranslationItems(Control parent, List<(Action<string>, string)> items)
        {
            foreach (Control child in parent.Controls)
            {
                // Excluir ciertos controles que pueden tener contenido editable.
                if (!(child is TextBox || child is ComboBox || child is DateTimePicker))
                {
                    if (!string.IsNullOrEmpty(child.Text))
                        items.Add((translated => child.Text = translated, child.Text));
                }
                // Recursividad en controles hijos.
                CollectControlTranslationItems(child, items);
            }

            // Si el control es un MenuStrip, procesar sus elementos de menú.
            if (parent is MenuStrip menuStrip)
            {
                foreach (ToolStripMenuItem item in menuStrip.Items)
                {
                    if (!string.IsNullOrEmpty(item.Text))
                        items.Add((translated => item.Text = translated, item.Text));
                    CollectToolStripTranslationItems(item, items);
                }
            }
        }

        /// <summary>
        /// Recorre recursivamente los ToolStripMenuItems y agrega sus textos a la lista.
        /// </summary>
        private void CollectToolStripTranslationItems(ToolStripMenuItem menuItem, List<(Action<string>, string)> items)
        {
            foreach (ToolStripItem subItem in menuItem.DropDownItems)
            {
                if (subItem is ToolStripMenuItem subMenuItem)
                {
                    if (!string.IsNullOrEmpty(subMenuItem.Text))
                        items.Add((translated => subMenuItem.Text = translated, subMenuItem.Text));
                    CollectToolStripTranslationItems(subMenuItem, items);
                }
            }
        }

        /// <summary>
        /// Traduce el texto usando el caché para evitar traducciones repetidas. 
        /// La clave incluye el idioma para soportar múltiples idiomas simultáneamente.
        /// </summary>
        private string TranslateWithCache(string originalText, string language)
        {
            // La clave de caché es "idioma:textoOriginal"
            string key = $"{language}:{originalText}";
            if (_translationCache.TryGetValue(key, out var translated))
            {
                return translated;
            }
            // Se asume que LanguageService.Translate es un método que realiza la traducción.
            // En este ejemplo, se pasan ambos parámetros como originalText, adapta esto según tu lógica.
            translated = LanguageService.Translate(originalText, originalText);
            _translationCache[key] = translated;
            return translated;
        }

        /// <summary>
        /// Método existente para traducir. Se mantiene para compatibilidad,
        /// pero internamente utiliza el método optimizado con caché.
        /// </summary>
        public string Translate(string originalText)
        {
            return TranslateWithCache(originalText, "");
        }
    }
}
