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
        public static LoggerDao Current => _instance;
        private LoggerDao() { }
        #endregion

        private static readonly string PathLogError = ConfigurationManager.AppSettings["PathLogError"];
        private static readonly string PathLogInfo = ConfigurationManager.AppSettings["PathLogInfo"];
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["ServicesSqlConnection"].ConnectionString;

        // Lee el nivel de trazabilidad configurado en app.config
        private static readonly TraceLevel ConfiguredTraceLevel = (TraceLevel)Enum.Parse(typeof(TraceLevel), ConfigurationManager.AppSettings["LogLevel"]);

        public void WriteLog(Log log, Exception ex = null)
        {
            // Aplica el filtro de nivel de log: solo guarda si el nivel de log es <= al configurado
            if (log.TraceLevel > ConfiguredTraceLevel)
            {
                return; // Ignora logs de mayor nivel al configurado
            }

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

        public List<Log> GetAllLogs()
        {
            List<Log> logs = new List<Log>();

            string commandText = "SELECT Id, Message, TraceLevel, Date, Exception FROM Log";

            using (var reader = SqlHelper.ExecuteReader(commandText, CommandType.Text))
            {
                while (reader.Read())
                {
                    logs.Add(new Log(
                        reader.GetString(1),
                        (TraceLevel)reader.GetInt32(2),
                        reader.IsDBNull(4) ? null : reader.GetString(4),
                        reader.GetDateTime(3)));
                }
            }

            return logs;
        }

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
                        reader.GetString(1),
                        (TraceLevel)reader.GetInt32(2),
                        reader.IsDBNull(4) ? null : reader.GetString(4),
                        reader.GetDateTime(3));
                }
            }

            return log;
        }

        public void DeleteLog(Guid logId)
        {
            string commandText = "DELETE FROM Log WHERE Id = @Id";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", logId)
            };

            SqlHelper.ExecuteNonQuery(commandText, CommandType.Text, parameters);
        }

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
