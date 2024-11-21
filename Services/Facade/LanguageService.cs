using Services.Dao.Contracts;
using Services.Factory;
using Services.Logic;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Services.Facade
{
    /// <summary>
    /// Proporciona una fachada sobre la lógica de traducción de idiomas para simplificar el acceso desde otras partes de la aplicación.
    /// </summary>
    public static class LanguageService
    {
        private static string currentLanguage = CultureInfo.CurrentCulture.Name; // Idioma predeterminado según el sistema

        /// <summary>
        /// Obtiene el idioma actual seleccionado.
        /// </summary>
        /// <returns>El idioma actual.</returns>
        public static string GetCurrentLanguage()
        {
            return currentLanguage;
        }

        /// <summary>
        /// Establece el idioma actual seleccionado y actualiza la configuración de globalización.
        /// </summary>
        /// <param name="language">El idioma a establecer.</param>
        public static void SetCurrentLanguage(string language)
        {
            ValidateParameter(language, nameof(language));
            currentLanguage = language;

            try
            {
                // Cambiar la cultura del hilo actual para reflejar el nuevo idioma
                CultureInfo newCulture = new CultureInfo(language);
                CultureInfo.DefaultThreadCurrentCulture = newCulture;
                CultureInfo.DefaultThreadCurrentUICulture = newCulture;
            }
            catch (CultureNotFoundException ex)
            {
                LogException(ex, nameof(SetCurrentLanguage));
                throw new InvalidOperationException($"El idioma '{language}' no es válido.", ex);
            }
        }

        /// <summary>
        /// Obtiene la lista de idiomas disponibles.
        /// </summary>
        /// <returns>Lista de idiomas disponibles.</returns>
        public static List<string> GetAvailableLanguages()
        {
            return LanguageLogic.GetAvailableLanguages();
        }

        /// <summary>
        /// Valida que un parámetro no sea nulo o vacío.
        /// </summary>
        /// <param name="parameter">Parámetro a validar.</param>
        /// <param name="parameterName">Nombre del parámetro.</param>
        private static void ValidateParameter(string parameter, string parameterName)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                throw new ArgumentException($"El parámetro '{parameterName}' no puede ser nulo o estar vacío.", parameterName);
            }
        }

        /// <summary>
        /// Registra excepciones en el logger con información adicional.
        /// </summary>
        /// <param name="ex">Excepción ocurrida.</param>
        /// <param name="methodName">Nombre del método donde ocurrió la excepción.</param>
        private static void LogException(Exception ex, string methodName)
        {
            LoggerService.WriteLog($"Error en {methodName}: {ex.Message}", System.Diagnostics.TraceLevel.Error);
            LoggerService.WriteException(ex);
        }

        /// <summary>
        /// Guarda las traducciones modificadas en un archivo de idioma existente.
        /// </summary>
        /// <param name="translations">Diccionario con las claves y valores de traducción.</param>
        /// <param name="languageFile">Nombre del archivo de idioma a modificar.</param>
        public static void SaveTranslations(Dictionary<string, string> translations, string languageFile)
        {
            ValidateParameter(languageFile, nameof(languageFile));

            try
            {
                LanguageLogic.SaveTranslations(translations, languageFile);
            }
            catch (Exception ex)
            {
                LogException(ex, nameof(SaveTranslations));
                throw;
            }
        }

        /// <summary>
        /// Guarda las traducciones en un nuevo archivo de idioma.
        /// </summary>
        /// <param name="translations">Diccionario con las claves y valores de traducción.</param>
        /// <param name="newLanguageFile">Nombre del nuevo archivo de idioma.</param>
        public static void SaveTranslationsToNewFile(Dictionary<string, string> translations, string newLanguageFile)
        {
            ValidateParameter(newLanguageFile, nameof(newLanguageFile));

            try
            {
                LanguageLogic.SaveTranslationsToNewFile(translations, newLanguageFile);
            }
            catch (Exception ex)
            {
                LogException(ex, nameof(SaveTranslationsToNewFile));
                throw;
            }
        }

        /// <summary>
        /// Traduce una clave especificada utilizando la lógica de negocio.
        /// </summary>
        /// <param name="key">La clave que se desea traducir.</param>
        /// <param name="defaultValue">Valor predeterminado si no se encuentra la clave.</param>
        /// <returns>El valor de la traducción asociado a la clave especificada.</returns>
        public static string Translate(string key, string defaultValue = null)
        {
            try
            {
                return LanguageLogic.Translate(key);
            }
            catch (KeyNotFoundException)
            {
                // Retorna el valor predeterminado si no existe la clave
                return defaultValue ?? key;
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
            ValidateParameter(language, nameof(language));
            ValidateParameter(key, nameof(key));
            ValidateParameter(value, nameof(value));

            try
            {
                LanguageLogic.AddTranslation(language, key, value);
            }
            catch (InvalidOperationException ex)
            {
                LogException(ex, nameof(AddTranslation));
                throw new InvalidOperationException($"La clave '{key}' ya existe en el idioma '{language}'.", ex);
            }
            catch (Exception ex)
            {
                LogException(ex, nameof(AddTranslation));
                throw;
            }
        }

        /// <summary>
        /// Recarga todas las traducciones desde un archivo de idioma específico en la caché.
        /// </summary>
        /// <param name="language">El idioma para recargar las traducciones.</param>
        public static void ReloadLanguages(string language)
        {
            ValidateParameter(language, nameof(language));

            try
            {
                LanguageLogic.ReloadLanguages(language);
            }
            catch (Exception ex)
            {
                LogException(ex, nameof(ReloadLanguages));
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
                LogException(ex, nameof(GetLanguages));
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
            ValidateParameter(key, nameof(key));
            ValidateParameter(translation, nameof(translation));
            ValidateParameter(newLanguageFile, nameof(newLanguageFile));

            try
            {
                LanguageLogic.SaveTranslation(key, translation, newLanguageFile);
                LoggerService.WriteLog($"Traducción guardada para la clave '{key}' en el archivo '{newLanguageFile}'.", System.Diagnostics.TraceLevel.Info);
            }
            catch (Exception ex)
            {
                LogException(ex, nameof(SaveTranslation));
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
            ValidateParameter(language, nameof(language));

            try
            {
                return LanguageLogic.LoadAllTranslations(language);
            }
            catch (Exception ex)
            {
                LogException(ex, nameof(LoadAllTranslations));
                throw new InvalidOperationException($"Error al cargar todas las traducciones del idioma: {language}.", ex);
            }
        }
    }
}
