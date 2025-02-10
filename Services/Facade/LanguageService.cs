using Services.Logic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Services.Facade
{
    public static class LanguageService
    {
        private static string currentLanguage = CultureInfo.CurrentCulture.Name;

        /// <summary>
        /// Obtiene el idioma actual seleccionado.
        /// </summary>
        public static string GetCurrentLanguage()
        {
            return LanguageLogic.GetCurrentLanguage();
        }

        /// <summary>
        /// Establece el idioma actual seleccionado y actualiza la configuración de globalización.
        /// </summary>
        public static void SetCurrentLanguage(string language)
        {
            ValidateParameter(language, nameof(language));

            // Cambiar el idioma en la lógica
            LanguageLogic.SetCurrentLanguage(language);

            try
            {
                // Cambiar la cultura del hilo actual para reflejar el nuevo idioma
                var languageMap = LanguageLogic.GetLanguageMap();
                if (languageMap.ContainsKey(language))
                {
                    var cultureCode = languageMap[language];
                    CultureInfo newCulture = new CultureInfo(cultureCode);
                    CultureInfo.DefaultThreadCurrentCulture = newCulture;
                    CultureInfo.DefaultThreadCurrentUICulture = newCulture;
                }
            }
            catch (CultureNotFoundException ex)
            {
                throw new InvalidOperationException($"El idioma '{language}' no es válido.", ex);
            }
        }

        /// <summary>
        /// Obtiene la lista de idiomas disponibles.
        /// </summary>
        public static List<string> GetAvailableLanguages()
        {
            return LanguageLogic.GetAvailableLanguages();
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
        /// Recarga todas las traducciones desde el caché para un idioma específico.
        /// </summary>
        /// <param name="language">El idioma para recargar las traducciones.</param>
        public static void ReloadLanguages(string language)
        {
            ValidateParameter(language, nameof(language));

            try
            {
               // LanguageLogic.ReloadLanguages(language);
            }
            catch (Exception ex)
            {
                LogException(ex, nameof(ReloadLanguages));
                throw new InvalidOperationException($"Error al recargar las traducciones para el idioma: {language}.", ex);
            }
        }

        /// <summary>
        /// Obtiene una lista de todas las claves de traducción disponibles en el caché.
        /// </summary>
        /// <returns>Lista de claves de traducción.</returns>
        public static List<string> GetLanguages()
        {
            // Corregido para usar GetAvailableLanguages
            try
            {
                return LanguageLogic.GetAvailableLanguages();
            }
            catch (Exception ex)
            {
                LogException(ex, nameof(GetLanguages));
                throw new InvalidOperationException("Error al obtener la lista de claves de traducción.", ex);
            }
        }

        public static async Task SetCurrentLanguageAsync(string language)
        {
            await Task.Run(() => SetCurrentLanguage(language));
        }

        /// <summary>
        /// Valida que un parámetro no sea nulo o vacío.
        /// </summary>
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
    }
}
