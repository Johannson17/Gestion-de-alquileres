using Services.Dao;
using Services.Domain;
using Services.Domain.Exceptions;
using System;
using System.Diagnostics;

namespace Services.Logic
{
    /// <summary>
    /// Lógica de negocio para la traducción de claves de texto en diferentes idiomas.
    /// </summary>
    internal static class LanguageLogic
    {
        /// <summary>
        /// Traduce una clave especificada al texto correspondiente en el idioma actual.
        /// </summary>
        /// <param name="key">La clave a traducir.</param>
        /// <returns>El texto traducido asociado con la clave especificada.</returns>
        public static string Translate(string key)
        {
            try
            {
                return LanguageDao.Translate(key);
            }
            catch (WordNotFoundException ex)
            {
                // Envía la clave no encontrada al grupo correspondiente (por ejemplo, vía web service).
                LanguageDao.WriteKey(key);

                // Registra el problema en el sistema de logs.
                LoggerDao.WriteLog(new Log($"La clave '{key}' no se encontró y fue registrada.", TraceLevel.Warning), ex);
            }
            catch (Exception ex)
            {
                // Registra cualquier otra excepción que ocurra.
                LoggerDao.WriteLog(new Log("Error al traducir la clave.", TraceLevel.Error), ex);
            }

            // Retorna la clave original si no se encontró la traducción.
            return key;
        }
    }
}
