using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using Services.Domain;
using Services.Dao.Helpers;
using Services.Dao.Contracts;
using System.Collections.Generic;
using Services.Dao.Implementations.SqlServer;

namespace Services.Dao
{
    public class LoggerDao : ILoggerDao
    {

        #region Singleton Pattern
        private static readonly LoggerDao _instance = new LoggerDao();

        /// <summary>
        /// Acceso a la instancia singleton.
        /// </summary>
        public static LoggerDao Current => _instance;

        private LoggerDao()
        {
            // Aquí se puede implementar la inicialización del singleton si es necesario.
        }
        #endregion

        private static readonly string PathLogError = ConfigurationManager.AppSettings["PathLogError"];
        private static readonly string PathLogInfo = ConfigurationManager.AppSettings["PathLogInfo"];
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["ServicesSqlConnection"].ConnectionString;

        /// <summary>
        /// Escribe un log en el archivo correspondiente según el nivel de severidad del mensaje
        /// y también en la base de datos.
        /// </summary>
        /// <param name="log">Información del log a escribir.</param>
        /// <param name="ex">Excepción opcional cuya traza se debe incluir en el log.</param>
        public void WriteLog(Log log, Exception ex = null)
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

        /// <summary>
        /// Escribe el log en la tabla de logs de la base de datos.
        /// </summary>
        /// <param name="log">Información del log a escribir.</param>
        /// <param name="ex">Excepción opcional cuya traza se debe incluir en el log.</param>
        private void WriteToDatabase(Log log, Exception ex = null)
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

        /// <summary>
        /// Obtiene todos los logs almacenados en el sistema.
        /// </summary>
        /// <returns>Una lista de todos los logs almacenados.</returns>
        public List<Log> GetAllLogs()
        {
            List<Log> logs = new List<Log>();

            string commandText = "SELECT Id, Message, TraceLevel, Date, Exception FROM Log";

            using (var reader = SqlHelper.ExecuteReader(commandText, CommandType.Text))
            {
                while (reader.Read())
                {
                    logs.Add(new Log(
                        reader.GetString(1), // Message
                        (TraceLevel)reader.GetInt32(2), // TraceLevel
                        reader.IsDBNull(4) ? null : reader.GetString(4), // Exception
                        reader.GetDateTime(3))); // Date
                }
            }

            return logs;
        }

        /// <summary>
        /// Obtiene un log específico por su ID.
        /// </summary>
        /// <param name="logId">El identificador del log.</param>
        /// <returns>El log correspondiente al ID proporcionado, o null si no existe.</returns>
        public Log GetLogById(Guid logId)
        {
            Log log = null;

            string commandText = "SELECT Id, Message, TraceLevel, Date, Exception FROM Log WHERE Id = @Id";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", logId)
            };

            using (var reader = SqlHelper.ExecuteReader(commandText, CommandType.Text, parameters))
            {
                if (reader.Read())
                {
                    log = new Log(
                        reader.GetString(1), // Message
                        (TraceLevel)reader.GetInt32(2), // TraceLevel
                        reader.IsDBNull(4) ? null : reader.GetString(4), // Exception
                        reader.GetDateTime(3)); // Date
                }
            }

            return log;
        }

        /// <summary>
        /// Elimina un log específico por su ID.
        /// </summary>
        /// <param name="logId">El identificador del log.</param>
        public void DeleteLog(Guid logId)
        {
            string commandText = "DELETE FROM Log WHERE Id = @Id";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", logId)
            };

            SqlHelper.ExecuteNonQuery(commandText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Elimina todos los logs en el sistema.
        /// </summary>
        public void DeleteAllLogs()
        {
            string commandText = "DELETE FROM Log";
            SqlHelper.ExecuteNonQuery(commandText, CommandType.Text);
        }

        private string FormatMessage(Log log)
        {
            return $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} [{log.TraceLevel}] : {log.Message}";
        }

        private void WriteToFile(string path, string message)
        {
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(message);
            }
        }
    }
}