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
        /// Traduce una clave especificada al texto correspondiente en el idioma actual.
        /// </summary>
        /// <param name="key">La clave a traducir, que representa un identificador para un fragmento de texto.</param>
        /// <returns>El texto traducido asociado con la clave especificada.</returns>
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
            catch (Exception ex)
            {
                // Opcionalmente manejar o registrar la excepción según sea necesario.
                throw new InvalidOperationException($"Error al traducir la clave: {key}", ex);
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
            catch (Exception ex)
            {
                // Opcionalmente manejar o registrar la excepción según sea necesario.
                throw new InvalidOperationException($"Error al agregar la traducción para la clave: {key} en el idioma: {language}", ex);
            }
        }

        /// <summary>
        /// Recarga todos los archivos de idioma en la caché.
        /// </summary>
        public static void ReloadLanguages()
        {
            try
            {
                LanguageLogic.ReloadLanguages();
            }
            catch (Exception ex)
            {
                // Opcionalmente manejar o registrar la excepción según sea necesario.
                throw new InvalidOperationException("Error al recargar los archivos de idioma.", ex);
            }
        }

        /// <summary>
        /// Obtiene una lista de todos los idiomas disponibles.
        /// </summary>
        /// <returns>Lista de identificadores de idiomas.</returns>
        public static List<string> GetLanguages()
        {
            try
            {
                return LanguageLogic.GetLanguages();
            }
            catch (Exception ex)
            {
                // Opcionalmente manejar o registrar la excepción según sea necesario.
                throw new InvalidOperationException("Error al obtener la lista de idiomas.", ex);
            }
        }
    }
}