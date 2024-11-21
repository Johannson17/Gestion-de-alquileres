using Services.Dao.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Text;

namespace Services.Dao.Implementations
{
    public class LanguageRepository : ILanguageRepository
    {
        private readonly string basePath;

        // Singleton instance
        private static readonly Lazy<LanguageRepository> _instance = new Lazy<LanguageRepository>(() => new LanguageRepository());

        // Expose the singleton instance
        public static LanguageRepository Current => _instance.Value;

        // Private constructor to prevent direct instantiation
        private LanguageRepository()
        {
            // Obtener la ruta base del directorio de ejecución
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Subir dos niveles desde bin\Debug\ a la carpeta del proyecto
            string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.FullName;

            // Combinar con la ruta configurada para la carpeta I18n
            basePath = Path.Combine(projectDirectory, ConfigurationManager.AppSettings["LanguagePath"]);

            // Verificar si la carpeta I18n existe en la ruta combinada
            if (!Directory.Exists(basePath))
            {
                throw new DirectoryNotFoundException($"La ruta especificada para los archivos de idioma no es válida: {basePath}");
            }
        }

        public Dictionary<string, string> LoadAllTranslations(string language)
        {
            Dictionary<string, string> translations = new Dictionary<string, string>();
            string filePath = Path.Combine(basePath, $"idioma.{language}.json");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"El archivo de idioma no se encontró en la ruta: {filePath}");
            }

            try
            {
                // Leer el archivo usando Encoding.Default
                string jsonContent = ReadJsonFile(filePath);

                // Deserializar el JSON al diccionario
                translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);

                if (translations == null || translations.Count == 0)
                {
                    throw new InvalidOperationException("El archivo de idioma no contiene datos válidos.");
                }
            }
            catch (JsonException jsonEx)
            {
                throw new InvalidOperationException($"Error al deserializar el archivo JSON: {filePath}", jsonEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al leer el archivo de idioma: {filePath}", ex);
            }

            return translations;
        }

        private string ReadJsonFile(string filePath)
        {
            // Leer el contenido del archivo con Encoding.Default
            using (var reader = new StreamReader(filePath, Encoding.Default))
            {
                return reader.ReadToEnd();
            }
        }

        public void AddTranslation(string language, string key, string value)
        {
            string filePath = Path.Combine(basePath, $"idioma.{language}.json");

            var translations = LoadAllTranslations(language);

            translations[key] = value;

            SaveAllTranslations(filePath, translations);
        }

        private void SaveAllTranslations(string filePath, Dictionary<string, string> translations)
        {
            // Guardar las traducciones en un archivo usando Encoding.Default
            using (var writer = new StreamWriter(filePath, false, Encoding.Default))
            {
                foreach (var translation in translations)
                {
                    writer.WriteLine($"{translation.Key}={translation.Value}");
                }
            }
        }

        public void SaveTranslation(string key, string value, string languageFile)
        {
            string filePath = Path.Combine(basePath, languageFile);

            if (!File.Exists(filePath))
            {
                // Crear el archivo con Encoding.Default si no existe
                using (var writer = new StreamWriter(filePath, false, Encoding.Default))
                {
                    writer.Write(string.Empty);
                }
            }

            var lines = new List<string>(File.ReadAllLines(filePath, Encoding.Default));
            bool keyExists = false;

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith($"{key}="))
                {
                    lines[i] = $"{key}={value}";
                    keyExists = true;
                }
            }

            if (!keyExists)
            {
                lines.Add($"{key}={value}");
            }

            File.WriteAllLines(filePath, lines, Encoding.Default);
        }

        public List<string> GetAvailableLanguages()
        {
            List<string> languages = new List<string>();

            var languageFiles = Directory.GetFiles(basePath, "idioma.*");
            foreach (var file in languageFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                if (fileName.Contains('.'))
                {
                    var parts = fileName.Split('.');
                    if (parts.Length > 1)
                    {
                        languages.Add(parts[1]);
                    }
                }
            }

            return languages;
        }

        public bool LanguageFileExists(string language)
        {
            string filePath = Path.Combine(basePath, $"idioma.{language}.json");
            return File.Exists(filePath);
        }
    }
}
