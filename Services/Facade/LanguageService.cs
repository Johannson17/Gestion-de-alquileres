using Services.Logic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Services.Facade
{
    public static class LanguageService
    {
        /// <summary>
        /// Obtiene el idioma actual seleccionado.
        /// </summary>
        public static string GetCurrentLanguage()
        {
            return LanguageLogic.GetCurrentLanguage();
        }

        /// <summary>
        /// Establece el idioma actual y actualiza la configuración de globalización.
        /// </summary>
        public static void SetCurrentLanguage(string language)
        {
            ValidateParameter(language, nameof(language));

            LanguageLogic.SetCurrentLanguage(language);

            try
            {
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
        public static string Translate(string key, string defaultValue = null)
        {
            try
            {
                return LanguageLogic.Translate(key);
            }
            catch (KeyNotFoundException)
            {
                return defaultValue ?? key;
            }
        }

        /// <summary>
        /// Recarga todas las traducciones desde el archivo para un idioma específico.
        /// </summary>
        public static void ReloadLanguages(string language)
        {
            ValidateParameter(language, nameof(language));

            try
            {
                LanguageLogic.SetCurrentLanguage(language);
            }
            catch (Exception ex)
            {
                LogException(ex, nameof(ReloadLanguages));
                throw new InvalidOperationException($"Error al recargar las traducciones para el idioma: {language}.", ex);
            }
        }

        /// <summary>
        /// Obtiene una lista de todas las claves de traducción disponibles.
        /// </summary>
        public static List<string> GetLanguages()
        {
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

        /// <summary>
        /// 📌 Obtiene el diccionario de idiomas disponibles
        /// </summary>
        public static Dictionary<string, string> GetLanguageMap()
        {
            return LanguageLogic.GetLanguageMap();
        }

        /// <summary>
        /// 📌 Guarda los cambios en `idiomas.json`
        /// </summary>
        public static void SaveLanguageMap(Dictionary<string, string> updatedLanguages)
        {
            if (updatedLanguages == null)
            {
                throw new ArgumentException("El diccionario de idiomas no puede ser nulo.");
            }

            LanguageLogic.SaveLanguageMap(updatedLanguages);
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
        private static void LogException(Exception ex, string methodName)
        {
            LoggerService.WriteLog($"Error en {methodName}: {ex.Message}", System.Diagnostics.TraceLevel.Error);
            LoggerService.WriteException(ex);
        }
    }
}
