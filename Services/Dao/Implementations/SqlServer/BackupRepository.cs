using Services.Dao.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace Services.Dao.Implementations.SqlServer
{
    /// <summary>
    /// Repositorio encargado de realizar operaciones de respaldo y restauración 
    /// para las bases de datos de seguridad y alquileres.
    /// </summary>
    public class BackupRepository : IBackupRepository
    {
        private readonly string _connectionStringSecurity;
        private readonly string _connectionStringRents;
        private readonly string basePath;
        private readonly string logFilePath;
        private static BackupRepository _instance;
        private static readonly object _lock = new object();

        public static BackupRepository Current
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new BackupRepository();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Constructor que inicializa las cadenas de conexión, las rutas necesarias para los respaldos y el log.
        /// </summary>
        public BackupRepository()
        {
            _connectionStringSecurity = ConfigurationManager.ConnectionStrings["ServicesSqlConnection"]?.ConnectionString
                ?? throw new InvalidOperationException("La cadena de conexión 'ServicesSqlConnection' no está configurada o es nula.");
            _connectionStringRents = ConfigurationManager.ConnectionStrings["RentsSqlConnection"]?.ConnectionString
                ?? throw new InvalidOperationException("La cadena de conexión 'RentsSqlConnection' no está configurada o es nula.");

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            basePath = Path.Combine(Directory.GetParent(baseDirectory)?.Parent?.Parent?.FullName ?? throw new DirectoryNotFoundException("No se pudo determinar el directorio base."), ConfigurationManager.AppSettings["BackupPath"] ?? "Backups");

            logFilePath = Path.Combine(basePath, "backup_log.txt");

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
        }

        /// <summary>
        /// Realiza un respaldo completo de la base de datos de seguridad con un nombre de archivo que incluye la fecha actual.
        /// </summary>
        public void BackupDatabaseSecurity()
        {
            string fileName = $"Security_{DateTime.Now:yyyyMMdd}.bak";
            string fullPath = Path.Combine(basePath, fileName);
            string commandText = $"BACKUP DATABASE Security TO DISK = '{fullPath}' WITH FORMAT, MEDIANAME = 'DB_Backup', NAME = 'Full Backup of Security';";

            using (SqlConnection connection = new SqlConnection(_connectionStringSecurity))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        /// <summary>
        /// Restaura la base de datos de seguridad desde un archivo de respaldo especificado.
        /// </summary>
        /// <param name="fileName">Nombre del archivo de respaldo.</param>
        public void RestoreDatabaseSecurity(string fileName)
        {
            string fullPath = Path.Combine(basePath, fileName);
            string commandText = $"USE master; ALTER DATABASE Security SET SINGLE_USER WITH ROLLBACK IMMEDIATE; RESTORE DATABASE Security FROM DISK = '{fullPath}' WITH REPLACE; ALTER DATABASE Security SET MULTI_USER;";

            ExecuteSqlCommand(_connectionStringSecurity, commandText, "Error al restaurar la base de datos Security.");
        }

        /// <summary>
        /// Realiza un respaldo completo de la base de datos de alquileres con un nombre de archivo que incluye la fecha actual.
        /// </summary>
        public void BackupDatabaseRents()
        {
            string fileName = $"Rents_{DateTime.Now:yyyyMMdd}.bak";
            string fullPath = Path.Combine(basePath, fileName);
            string commandText = $"BACKUP DATABASE Rents TO DISK = '{fullPath}' WITH FORMAT, MEDIANAME = 'DB_Backup', NAME = 'Full Backup of Rents';";

            ExecuteSqlCommand(_connectionStringRents, commandText, "Error al generar el respaldo de la base de datos Rents.");
        }

        /// <summary>
        /// Restaura la base de datos de alquileres desde un archivo de respaldo especificado.
        /// </summary>
        /// <param name="fileName">Nombre del archivo de respaldo.</param>
        public void RestoreDatabaseRents(string fileName)
        {
            string fullPath = Path.Combine(basePath, fileName);
            string commandText = $"USE master; ALTER DATABASE Rents SET SINGLE_USER WITH ROLLBACK IMMEDIATE; RESTORE DATABASE Rents FROM DISK = '{fullPath}' WITH REPLACE; ALTER DATABASE Rents SET MULTI_USER;";

            ExecuteSqlCommand(_connectionStringRents, commandText, "Error al restaurar la base de datos Rents.");
        }

        /// <summary>
        /// Crea un archivo .txt vacío con el nombre basado en la fecha proporcionada.
        /// </summary>
        /// <param name="date">Fecha en formato 'yyyyMMdd'.</param>
        public void CreateEmptyFile(string date)
        {
            try
            {
                string fileName = $"{date}.txt";
                string fullPath = Path.Combine(basePath, fileName);

                if (!File.Exists(fullPath))
                {
                    File.Create(fullPath).Dispose();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Devuelve los nombres de todos los archivos .txt en el directorio de respaldos.
        /// </summary>
        public List<string> GetAllTxtFiles()
        {
            if (!Directory.Exists(basePath))
            {
                throw new DirectoryNotFoundException($"El directorio '{basePath}' no existe.");
            }

            string[] txtFiles = Directory.GetFiles(basePath, "*.txt");
            List<string> fileNames = new List<string>();

            foreach (var file in txtFiles)
            {
                fileNames.Add(Path.GetFileName(file));
            }

            return fileNames;
        }

        /// <summary>
        /// Ejecuta un comando SQL con manejo de excepciones.
        /// </summary>
        private void ExecuteSqlCommand(string connectionString, string commandText, string errorMessage)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(commandText, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(errorMessage, ex);
            }
        }
    }
}
