using System;
using System.Diagnostics;
using System.Configuration;
using System.IO;
using Services.Domain;

namespace Services.Dao
{
    internal static class LoggerDao
    {
        private static readonly string PathLogError = ConfigurationManager.AppSettings["PathLogError"];
        private static readonly string PathLogInfo = ConfigurationManager.AppSettings["PathLogInfo"];

        /// <summary>
        /// Escribe un log en el archivo correspondiente según el nivel de severidad del mensaje.
        /// </summary>
        /// <param name="log">Información del log a escribir.</param>
        /// <param name="ex">Excepción opcional cuya traza se debe incluir en el log.</param>
        public static void WriteLog(Log log, Exception ex = null)
        {
            string path;
            string formatMessage = FormatMessage(log);

            if (log.TraceLevel == TraceLevel.Error && ex != null)
            {
                formatMessage += $"\nException Stack Trace: {ex.StackTrace}";
                path = PathLogError;
            }
            else
            {
                path = PathLogInfo;
            }

            // Concatenar la fecha al nombre del archivo para gestionar el corte diario de logs.
            string fullPath = Path.Combine(Path.GetDirectoryName(path), $"{DateTime.Now.ToString("yyyy-MM-dd")}-{Path.GetFileName(path)}");
            WriteToFile(fullPath, formatMessage);
        }

        private static string FormatMessage(Log log)
        {
            return $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} [{log.TraceLevel}] : {log.Message}";
        }

        /// <summary>
        /// Escribe el mensaje formateado al archivo de log especificado.
        /// </summary>
        /// <param name="path">Ruta del archivo donde se escribe el log.</param>
        /// <param name="message">Mensaje formateado para escribir en el log.</param>
        private static void WriteToFile(string path, string message)
        {
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(message);
            }
        }
    }
}
