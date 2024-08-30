using Services.Dao;
using Services.Dao.Contracts;
using Services.Domain;
using Services.Factory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

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
            catch (KeyNotFoundException ex)
            {
                // Obtener instancia de LoggerDao desde el FactoryDao
                var loggerDao = FactoryDao.CreateRepository<ILoggerDao>();

                // Envía la clave no encontrada al grupo correspondiente (por ejemplo, vía web service).
                LanguageDao.WriteKey(Thread.CurrentThread.CurrentUICulture.Name, key, $"[{key}]");

                // Registra el problema en el sistema de logs.
                loggerDao.WriteLog(new Log($"La clave '{key}' no se encontró y fue registrada.", TraceLevel.Warning), ex);

                // Retorna la clave original entre corchetes para indicar que no se encontró.
                return $"[{key}]";
            }
            catch (Exception ex)
            {
                // Obtener instancia de LoggerDao desde el FactoryDao
                var loggerDao = FactoryDao.CreateRepository<ILoggerDao>();

                // Registra cualquier otra excepción que ocurra.
                loggerDao.WriteLog(new Log("Error al traducir la clave.", TraceLevel.Error), ex);

                // Retorna la clave original si ocurre una excepción inesperada.
                return key;
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
            try
            {
                LanguageDao.WriteKey(language, key, value);

                // Obtener instancia de LoggerDao desde el FactoryDao
                var loggerDao = FactoryDao.CreateRepository<ILoggerDao>();
                loggerDao.WriteLog(new Log($"La clave '{key}' fue agregada al idioma '{language}'.", TraceLevel.Info));
            }
            catch (Exception ex)
            {
                // Obtener instancia de LoggerDao desde el FactoryDao
                var loggerDao = FactoryDao.CreateRepository<ILoggerDao>();

                // Registra cualquier excepción que ocurra al agregar la clave.
                loggerDao.WriteLog(new Log("Error al agregar la clave de traducción.", TraceLevel.Error), ex);
            }
        }

        /// <summary>
        /// Recarga todos los archivos de idioma en la caché.
        /// </summary>
        public static void ReloadLanguages()
        {
            try
            {
                LanguageDao.ReloadLanguages();

                // Obtener instancia de LoggerDao desde el FactoryDao
                var loggerDao = FactoryDao.CreateRepository<ILoggerDao>();
                loggerDao.WriteLog(new Log("Se recargaron todos los archivos de idioma.", TraceLevel.Info));
            }
            catch (Exception ex)
            {
                // Obtener instancia de LoggerDao desde el FactoryDao
                var loggerDao = FactoryDao.CreateRepository<ILoggerDao>();

                // Registra cualquier excepción que ocurra al recargar los idiomas.
                loggerDao.WriteLog(new Log("Error al recargar los archivos de idioma.", TraceLevel.Error), ex);
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
                return LanguageDao.GetLanguages();
            }
            catch (Exception ex)
            {
                // Obtener instancia de LoggerDao desde el FactoryDao
                var loggerDao = FactoryDao.CreateRepository<ILoggerDao>();

                // Registra cualquier excepción que ocurra al obtener los idiomas.
                loggerDao.WriteLog(new Log("Error al obtener la lista de idiomas.", TraceLevel.Error), ex);

                // Retorna una lista vacía si ocurre una excepción.
                return new List<string>();
            }
        }
    }
}