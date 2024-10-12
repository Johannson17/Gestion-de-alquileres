using Services.Dao.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

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

        /// <summary>
        /// Carga todas las traducciones desde el archivo de idioma con formato clave=valor.
        /// </summary>
        public Dictionary<string, string> LoadAllTranslations(string language)
        {
            Dictionary<string, string> translations = new Dictionary<string, string>();
            string filePath = Path.Combine(basePath, $"idioma.{language}");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"El archivo de idioma no se encontró en la ruta: {filePath}");
            }

            // Leer cada línea y dividir en clave y valor
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line) && line.Contains('='))
                {
                    var keyValue = line.Split('=');
                    if (keyValue.Length == 2)
                    {
                        string key = keyValue[0].Trim();
                        string value = keyValue[1].Trim();
                        translations[key] = value;
                    }
                }
            }

            return translations;
        }

        /// <summary>
        /// Agrega una nueva traducción o actualiza una existente en el archivo de idioma.
        /// </summary>
        public void AddTranslation(string language, string key, string value)
        {
            string filePath = Path.Combine(basePath, $"idioma.{language}");

            // Cargar las traducciones actuales
            var translations = LoadAllTranslations(language);

            // Agregar o actualizar la traducción
            translations[key] = value;

            // Guardar las traducciones actualizadas
            SaveAllTranslations(filePath, translations);
        }

        /// <summary>
        /// Guarda todas las traducciones en el archivo especificado.
        /// </summary>
        private void SaveAllTranslations(string filePath, Dictionary<string, string> translations)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var translation in translations)
                {
                    writer.WriteLine($"{translation.Key}={translation.Value}");
                }
            }
        }

        /// <summary>
        /// Guarda una traducción en el archivo de idioma proporcionado.
        /// </summary>
        public void SaveTranslation(string key, string value, string languageFile)
        {
            string filePath = Path.Combine(basePath, languageFile);  // El archivo proporcionado ya tiene el nombre completo

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();  // Crear el archivo si no existe
            }

            var lines = new List<string>(File.ReadAllLines(filePath));
            bool keyExists = false;

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith($"{key}="))
                {
                    lines[i] = $"{key}={value}";  // Actualizar el valor si la clave ya existe
                    keyExists = true;
                }
            }

            if (!keyExists)
            {
                lines.Add($"{key}={value}");  // Agregar nueva clave-valor si no existe
            }

            File.WriteAllLines(filePath, lines);
        }
    }
}