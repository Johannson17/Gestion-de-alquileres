using Services.Dao.Contracts;
using Services.Factory;
using Services.Logic;
using System;
using System.Collections.Generic;

namespace Services.Facade
{
    /// <summary>
    /// Proporciona una fachada sobre la lógica de traducción de idiomas para simplificar el acceso desde otras partes de la aplicación.
    /// </summary>
    public static class LanguageService
    {
        /// <summary>
        /// Guarda las traducciones modificadas en un archivo de idioma existente.
        /// </summary>
        /// <param name="translations">Diccionario con las claves y valores de traducción.</param>
        /// <param name="languageFile">Nombre del archivo de idioma a modificar.</param>
        public static void SaveTranslations(Dictionary<string, string> translations, string languageFile)
        {
            LanguageLogic.SaveTranslations(translations, languageFile);
        }

        /// <summary>
        /// Guarda las traducciones en un nuevo archivo de idioma.
        /// </summary>
        /// <param name="translations">Diccionario con las claves y valores de traducción.</param>
        /// <param name="newLanguageFile">Nombre del nuevo archivo de idioma.</param>
        public static void SaveTranslationsToNewFile(Dictionary<string, string> translations, string newLanguageFile)
        {
            LanguageLogic.SaveTranslationsToNewFile(translations, newLanguageFile);
        }

        /// <summary>
        /// Traduce una clave especificada utilizando la lógica de negocio.
        /// </summary>
        /// <param name="key">La clave que se desea traducir.</param>
        /// <returns>El valor de la traducción asociado a la clave especificada.</returns>
        public static string Translate(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("La clave no puede ser nula o estar vacía.", nameof(key));
            }

            try
            {
                return LanguageLogic.Translate(key);
            }
            catch (KeyNotFoundException ex)
            {
                // Registrar excepción en la bitácora
                LoggerService.WriteException(ex);
                throw new InvalidOperationException($"La clave '{key}' no existe en las traducciones cargadas.", ex);
            }
            catch (Exception ex)
            {
                // Registrar excepción en la bitácora
                LoggerService.WriteException(ex);
                throw new InvalidOperationException($"Error al traducir la clave '{key}'", ex);
            }
        }

        /// <summary>
        /// Agrega una nueva clave y su valor a un archivo de idioma especificado.
        /// </summary>
        /// <param name="language">El idioma al que pertenece la clave.</param>
        /// <param name="key">La clave a agregar.</param>
        /// <param name="value">El valor de la clave a agregar.</param>
        public static void AddTranslation(string language, string key, string value)
        {
            if (string.IsNullOrEmpty(language))
            {
                throw new ArgumentException("El idioma no puede ser nulo o estar vacío.", nameof(language));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("La clave no puede ser nula o estar vacía.", nameof(key));
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("El valor no puede ser nulo o estar vacío.", nameof(value));
            }

            try
            {
                LanguageLogic.AddTranslation(language, key, value);
            }
            catch (InvalidOperationException ex)
            {
                LoggerService.WriteException(ex);
                throw new InvalidOperationException($"La clave '{key}' ya existe en el idioma '{language}'.", ex);
            }
            catch (Exception ex)
            {
                LoggerService.WriteException(ex);
                throw new InvalidOperationException($"Error al agregar la traducción para la clave '{key}' en el idioma '{language}'.", ex);
            }
        }

        /// <summary>
        /// Recarga todas las traducciones desde un archivo de idioma específico en la caché.
        /// </summary>
        /// <param name="language">El idioma para recargar las traducciones.</param>
        public static void ReloadLanguages(string language)
        {
            if (string.IsNullOrEmpty(language))
            {
                throw new ArgumentException("El idioma no puede ser nulo o estar vacío.", nameof(language));
            }

            try
            {
                LanguageLogic.ReloadLanguages(language);
            }
            catch (Exception ex)
            {
                LoggerService.WriteException(ex);
                throw new InvalidOperationException($"Error al recargar los archivos de idioma: {language}.", ex);
            }
        }

        /// <summary>
        /// Obtiene una lista de todas las claves de traducción disponibles.
        /// </summary>
        /// <returns>Lista de claves de traducción.</returns>
        public static List<string> GetLanguages()
        {
            try
            {
                return LanguageLogic.GetLanguages();
            }
            catch (Exception ex)
            {
                LoggerService.WriteException(ex);
                throw new InvalidOperationException("Error al obtener la lista de claves de traducción.", ex);
            }
        }

        /// <summary>
        /// Guarda una traducción específica en un archivo de idioma.
        /// </summary>
        /// <param name="key">Clave de la traducción.</param>
        /// <param name="translation">Texto de la traducción.</param>
        /// <param name="newLanguageFile">Nombre del archivo de idioma donde se guardará la traducción.</param>
        public static void SaveTranslation(string key, string translation, string newLanguageFile)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("La clave no puede ser nula o estar vacía.", nameof(key));
            }

            if (string.IsNullOrEmpty(translation))
            {
                throw new ArgumentException("La traducción no puede ser nula o estar vacía.", nameof(translation));
            }

            if (string.IsNullOrEmpty(newLanguageFile))
            {
                throw new ArgumentException("El archivo de idioma no puede ser nulo o estar vacío.", nameof(newLanguageFile));
            }

            try
            {
                LanguageLogic.SaveTranslation(key, translation, newLanguageFile);
                LoggerService.WriteLog($"Traducción guardada para la clave '{key}' en el archivo '{newLanguageFile}'.", System.Diagnostics.TraceLevel.Info);
            }
            catch (Exception ex)
            {
                LoggerService.WriteException(ex);
                throw;
            }
        }

        /// <summary>
        /// Carga todas las traducciones para un idioma especificado.
        /// </summary>
        /// <param name="language">El idioma para el cual cargar las traducciones.</param>
        /// <returns>Diccionario con las traducciones del idioma.</returns>
        public static Dictionary<string, string> LoadAllTranslations(string language)
        {
            if (string.IsNullOrEmpty(language))
            {
                throw new ArgumentException("El idioma no puede ser nulo o estar vacío.", nameof(language));
            }

            try
            {
                return LanguageLogic.LoadAllTranslations(language);
            }
            catch (Exception ex)
            {
                LoggerService.WriteException(ex);
                throw new InvalidOperationException($"Error al cargar todas las traducciones del idioma: {language}.", ex);
            }
        }
    }
}