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
        void SaveTranslation(string key, string value, string newLanguageFile);

        /// <summary>
        /// Carga todas las traducciones desde los archivos de idioma.
        /// </summary>
        /// <returns>Diccionario con todas las traducciones.</returns>

        Dictionary<string, string> LoadAllTranslations(string language);

    }
}