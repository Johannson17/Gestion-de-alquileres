using Services.Dao.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

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
            string filePath = Path.Combine(basePath, $"idioma.{language}.json");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"El archivo de idioma no se encontró en la ruta: {filePath}");
            }

            try
            {
                // Leer el archivo con detección automática de codificación
                string jsonContent = ReadJsonFileWithEncodingFallback(filePath);

                // Deserializar el JSON al diccionario
                translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);

                // Validar contenido
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

        /// <summary>
        /// Lee el contenido de un archivo JSON utilizando una codificación predeterminada con detección automática de marcas de orden de bytes (BOM).
        /// </summary>
        /// <param name="filePath">La ruta completa del archivo JSON a leer.</param>
        /// <returns>El contenido del archivo como una cadena.</returns>
        /// <exception cref="FileNotFoundException">Se lanza si el archivo especificado no existe.</exception>
        /// <exception cref="IOException">Se lanza si ocurre un error al leer el archivo.</exception>
        /// <remarks>
        /// Este método utiliza <see cref="System.Text.Encoding.Default"/> como codificación predeterminada y detecta automáticamente 
        /// la codificación del archivo si contiene marcas de orden de bytes (BOM). Es útil para archivos con codificaciones no estándar.
        /// </remarks>
        private string ReadJsonFileWithEncodingFallback(string filePath)
        {
            using (var reader = new StreamReader(filePath, System.Text.Encoding.Default, detectEncodingFromByteOrderMarks: true))
            {
                return reader.ReadToEnd();
            }
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
            string filePath = Path.Combine(basePath, languageFile); // El archivo proporcionado ya tiene el nombre completo

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose(); // Crear el archivo si no existe
            }

            var lines = new List<string>(File.ReadAllLines(filePath));
            bool keyExists = false;

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith($"{key}="))
                {
                    lines[i] = $"{key}={value}"; // Actualizar el valor si la clave ya existe
                    keyExists = true;
                }
            }

            if (!keyExists)
            {
                lines.Add($"{key}={value}"); // Agregar nueva clave-valor si no existe
            }

            File.WriteAllLines(filePath, lines);
        }

        /// <summary>
        /// Obtiene una lista de todos los idiomas disponibles en la carpeta de idiomas.
        /// </summary>
        /// <returns>Lista de códigos de idiomas disponibles, como "es-AR" o "en-US".</returns>
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
                        languages.Add(parts[1]); // Extraer "es-AR", "en-US", etc.
                    }
                }
            }

            return languages;
        }

        /// <summary>
        /// Verifica si un archivo de idioma existe para un idioma dado.
        /// </summary>
        /// <param name="language">Idioma para verificar.</param>
        /// <returns>True si el archivo existe, de lo contrario, false.</returns>
        public bool LanguageFileExists(string language)
        {
            string filePath = Path.Combine(basePath, $"idioma.{language}");
            return File.Exists(filePath);
        }
    }
}
