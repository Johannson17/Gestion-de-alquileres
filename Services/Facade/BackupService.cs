using Services.Logic;
using System;
using System.Collections.Generic;

namespace Services.Facade
{
    /// <summary>
    /// Servicio para manejar operaciones de respaldo y restauración de bases de datos.
    /// Actúa como intermediario entre la lógica y la capa UI.
    /// </summary>
    public class BackupService
    {
        private readonly BackupLogic _backupLogic;

        /// <summary>
        /// Constructor que inicializa el servicio con la lógica de respaldos.
        /// </summary>
        public BackupService()
        {
            _backupLogic = new BackupLogic();
        }

        /// <summary>
        /// Realiza un respaldo de todas las bases de datos (Seguridad y Alquileres).
        /// </summary>
        public void GenerateBackup()
        {
            try
            {
                _backupLogic.BackupAllDatabases();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar el respaldo: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Restaura todas las bases de datos desde respaldos basados en una fecha específica.
        /// </summary>
        /// <param name="date">Fecha del respaldo en formato 'yyyyMMdd'.</param>
        public void RestoreBackup(string date)
        {
            try
            {
                _backupLogic.RestoreAllDatabases(date);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al restaurar el respaldo: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene los nombres de todos los archivos .txt en el directorio de respaldos.
        /// </summary>
        /// <returns>Lista de nombres de archivos .txt.</returns>
        public List<string> GetBackupFiles()
        {
            try
            {
                return _backupLogic.GetAllBackupFiles();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la lista de archivos .txt: {ex.Message}", ex);
            }
        }
    }
}
