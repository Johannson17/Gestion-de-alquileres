using Services.Logic;
using System;

namespace Services.Facade
{
    /// <summary>
    /// Proporciona una fachada sobre la lógica de traducción de idiomas para simplificar el acceso desde otras partes de la aplicación.
    /// </summary>
    internal static class LanguageService
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
    }
}
