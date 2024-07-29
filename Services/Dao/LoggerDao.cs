using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using Services.Domain;
using Services.Dao.Helpers;

namespace Services.Dao
{
    internal static class LoggerDao
    {
        private static readonly string PathLogError = ConfigurationManager.AppSettings["PathLogError"];
        private static readonly string PathLogInfo = ConfigurationManager.AppSettings["PathLogInfo"];
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["ServicesSqlConnection"].ConnectionString;

        /// <summary>
        /// Escribe un log en el archivo correspondiente según el nivel de severidad del mensaje
        /// y también en la base de datos.
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
            string fullPath = Path.Combine(Path.GetDirectoryName(path), $"{DateTime.Now:yyyy-MM-dd}-{Path.GetFileName(path)}");
            WriteToFile(fullPath, formatMessage);
            WriteToDatabase(log, ex);
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

        /// <summary>
        /// Escribe el log en la tabla de logs de la base de datos.
        /// </summary>
        /// <param name="log">Información del log a escribir.</param>
        /// <param name="ex">Excepción opcional cuya traza se debe incluir en el log.</param>
        private static void WriteToDatabase(Log log, Exception ex = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Message", log.Message),
                new SqlParameter("@TraceLevel", (int)log.TraceLevel),
                new SqlParameter("@Date", log.Date),
                new SqlParameter("@Exception", ex != null ? (object)ex.ToString() : DBNull.Value)
            };

            string commandText = @"
                INSERT INTO Log (Message, TraceLevel, Date, Exception)
                VALUES (@Message, @TraceLevel, @Date, @Exception)";

            SqlHelper.ExecuteNonQuery(commandText, CommandType.Text, parameters);
        }
    }
}