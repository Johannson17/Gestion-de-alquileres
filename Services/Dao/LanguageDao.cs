using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace Services.Dao
{
    internal static class LanguageDao
    {
        private static readonly string path = ConfigurationManager.AppSettings["LanguagePath"];
        private static readonly Dictionary<string, Dictionary<string, string>> cache = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// Traduce una clave a su valor correspondiente en el idioma actual del hilo.
        /// </summary>
        /// <param name="key">Clave que identifica el texto a traducir.</param>
        /// <returns>Texto traducido.</returns>
        public static string Translate(string key)
        {
            string language = Thread.CurrentThread.CurrentUICulture.Name;
            string fileName = Path.Combine(path, language + ".txt");

            // Cargar las traducciones en cache si aún no están cargadas
            if (!cache.ContainsKey(language))
            {
                LoadLanguageFile(language, fileName);
            }

            // Buscar la clave en el diccionario de la caché
            if (cache[language].TryGetValue(key.ToLower(), out string value))
            {
                return value;
            }

            // No encontré la clave...
            throw new KeyNotFoundException($"Key '{key}' not found for language '{language}'.");
        }

        /// <summary>
        /// Carga y parsea el archivo de idioma en la caché.
        /// </summary>
        /// <param name="language">Identificador del lenguaje.</param>
        /// <param name="fileName">Nombre del archivo a cargar.</param>
        private static void LoadLanguageFile(string language, string fileName)
        {
            var translations = new Dictionary<string, string>();

            try
            {
                using (StreamReader reader = new StreamReader(fileName))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                        {
                            continue; // Saltar líneas vacías o comentarios
                        }

                        string[] columns = line.Split('=');
                        if (columns.Length == 2)
                        {
                            translations[columns[0].ToLower()] = columns[1];
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException($"Language file '{fileName}' not found.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading language file '{fileName}': {ex.Message}");
            }

            cache[language] = translations;
        }

        /// <summary>
        /// Escribe una clave y su valor en el archivo de idioma especificado.
        /// </summary>
        /// <param name="language">El idioma al que pertenece la clave.</param>
        /// <param name="key">Clave a escribir.</param>
        /// <param name="value">Valor de la clave a escribir.</param>
        public static void WriteKey(string language, string key, string value)
        {
            string fileName = Path.Combine(path, language + ".txt");

            // Asegurarse de que el archivo y la caché estén cargados
            if (!cache.ContainsKey(language))
            {
                LoadLanguageFile(language, fileName);
            }

            // Escribir la clave en la caché
            cache[language][key.ToLower()] = value;

            // Escribir la clave en el archivo
            using (StreamWriter writer = new StreamWriter(fileName, true))
            {
                writer.WriteLine($"{key}={value}");
            }
        }

        /// <summary>
        /// Obtiene una lista de todos los idiomas disponibles.
        /// </summary>
        /// <returns>Lista de identificadores de idiomas.</returns>
        public static List<string> GetLanguages()
        {
            return Directory.GetFiles(path, "*.txt")
                .Select(Path.GetFileNameWithoutExtension)
                .ToList();
        }

        /// <summary>
        /// Recarga los archivos de idioma en la caché.
        /// </summary>
        public static void ReloadLanguages()
        {
            cache.Clear();
            foreach (var language in GetLanguages())
            {
                string fileName = Path.Combine(path, language + ".txt");
                LoadLanguageFile(language, fileName);
            }
        }
    }
}