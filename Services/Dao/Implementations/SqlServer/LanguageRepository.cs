using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using System.Text;

namespace Services.Dao.Implementations
{
    public class LanguageRepository
    {
        private readonly string basePath;
        private static readonly Lazy<LanguageRepository> _instance = new Lazy<LanguageRepository>(() => new LanguageRepository());

        public static LanguageRepository Current => _instance.Value;

        private LanguageRepository()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.FullName;

            basePath = Path.Combine(projectDirectory, ConfigurationManager.AppSettings["LanguagePath"]);

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath); // Si la carpeta no existe, la crea
            }

            EnsureLanguageFileExists(); // Verifica que el archivo `idiomas.json` exista
        }

        /// <summary>
        /// 📌 Carga el diccionario de idiomas desde el archivo `idiomas.json`
        /// </summary>
        public Dictionary<string, string> LoadLanguageMap()
        {
            string filePath = Path.Combine(basePath, "idiomas.json");

            if (!File.Exists(filePath))
            {
                EnsureLanguageFileExists();
            }

            try
            {
                string jsonContent = File.ReadAllText(filePath, Encoding.UTF8);
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent) ?? new Dictionary<string, string>();
            }
            catch (JsonException jsonEx)
            {
                throw new InvalidOperationException($"Error al deserializar el archivo JSON de idiomas: {filePath}", jsonEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al leer el archivo de idiomas: {filePath}", ex);
            }
        }

        /// <summary>
        /// 📌 Guarda el diccionario actualizado de idiomas en el archivo `idiomas.json`
        /// </summary>
        public void SaveLanguageMap(Dictionary<string, string> updatedLanguages)
        {
            string filePath = Path.Combine(basePath, "idiomas.json");

            try
            {
                string jsonContent = JsonConvert.SerializeObject(updatedLanguages, Formatting.Indented);
                File.WriteAllText(filePath, jsonContent, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al guardar el archivo de idiomas: {filePath}", ex);
            }
        }

        /// <summary>
        /// 📌 Verifica que el archivo `idiomas.json` exista y lo crea con los idiomas predeterminados si no existe.
        /// </summary>
        private void EnsureLanguageFileExists()
        {
            string filePath = Path.Combine(basePath, "idiomas.json");

            if (!File.Exists(filePath))
            {
                var defaultLanguages = new Dictionary<string, string>
                {
                    { "Español", "es" },
                    { "English", "en" }
                };

                SaveLanguageMap(defaultLanguages);
            }
        }
    }
}
