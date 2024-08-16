using Services.Factory;
using Services.Domain;
using System;
using System.Diagnostics;
using Services.Dao;
using Services.Dao.Contracts;

namespace Services.Logic
{
    /// <summary>
    /// Lógica para el manejo de registros (logs) en la aplicación.
    /// </summary>
    internal static class LoggerLogic
    {
        /// <summary>
        /// Escribe un registro de log en el sistema, con opción de incluir detalles de una excepción.
        /// </summary>
        /// <param name="log">El objeto de log que contiene la información a registrar.</param>
        /// <param name="ex">Excepción opcional cuyos detalles se incluirán en el log.</param>
        public static void WriteLog(Log log, Exception ex = null)
        {
            if (log.TraceLevel == TraceLevel.Error)
            {
                // Enviar mensaje vía WhatsApp a un destinatario específico
                NotifyError(log, ex);
            }

            // Obtener el "LoggerDao" desde el Factory
            var loggerDao = FactoryDao.CreateRepository<ILoggerDao>();

            // Llamar al método WriteLog de la instancia de LoggerDao para escribir el log en el archivo y la base de datos.
            loggerDao.WriteLog(log, ex);
        }

        /// <summary>
        /// Notifica un error crítico enviando un mensaje específico.
        /// </summary>
        /// <param name="log">El objeto de log que contiene la información del error.</param>
        /// <param name="ex">Excepción asociada al error.</param>
        private static void NotifyError(Log log, Exception ex)
        {
            // Aquí va la lógica para enviar un mensaje vía WhatsApp a un destinatario específico.
            Console.WriteLine($"Enviando mensaje de error crítico a fulanito: {log.Message}\nDetalles: {ex?.Message}");
        }
    }
}