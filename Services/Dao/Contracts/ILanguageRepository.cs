using System.Collections.Generic;

namespace Services.Dao.Contracts
{
    public interface ILanguageRepository
    {
        /// <summary>
        /// Agrega una nueva traducción al archivo de idioma.
        /// </summary>
        /// <param name="language">Idioma.</param>
        /// <param name="key">Clave de traducción.</param>
        /// <param name="value">Valor de la traducción.</param>
        void AddTranslation(string language, string key, string value);

        /// <summary>
        /// Guarda una traducción en el archivo de idioma.
        /// </summary>
        /// <param name="key">Clave de traducción.</param>
        /// <param name="value">Valor de la traducción.</param>
        /// <param name="newLanguageFile">Nombre del archivo de idioma.</param>
        void SaveTranslation(string key, string value, string newLanguageFile);

        /// <summary>
        /// Carga todas las traducciones desde los archivos de idioma.
        /// </summary>
        /// <param name="language">Idioma a cargar.</param>
        /// <returns>Diccionario con todas las traducciones para el idioma especificado.</returns>
        Dictionary<string, string> LoadAllTranslations(string language);

        /// <summary>
        /// Obtiene una lista de todos los idiomas disponibles en la carpeta de idiomas.
        /// </summary>
        /// <returns>Lista de códigos de idiomas disponibles, como "es-AR" o "en-US".</returns>
        List<string> GetAvailableLanguages();

        /// <summary>
        /// Verifica si un archivo de idioma existe para un idioma dado.
        /// </summary>
        /// <param name="language">Idioma para verificar.</param>
        /// <returns>True si el archivo existe, de lo contrario, false.</returns>
        bool LanguageFileExists(string language);
    }
}
