using System;
using System.Collections.Generic;

namespace Services.Dao.Contracts
{
    /// <summary>
    /// Interfaz para la gestión de respaldos, restauraciones y creación de archivos relacionados con bases de datos.
    /// </summary>
    public interface IBackupRepository
    {
        /// <summary>
        /// Realiza un respaldo completo de la base de datos de seguridad con un nombre de archivo que incluye la fecha actual.
        /// </summary>
        void BackupDatabaseSecurity();

        /// <summary>
        /// Restaura la base de datos de seguridad desde un archivo de respaldo especificado.
        /// </summary>
        /// <param name="fileName">Nombre del archivo de respaldo (incluyendo la extensión).</param>
        void RestoreDatabaseSecurity(string fileName);

        /// <summary>
        /// Realiza un respaldo completo de la base de datos de alquileres con un nombre de archivo que incluye la fecha actual.
        /// </summary>
        void BackupDatabaseRents();

        /// <summary>
        /// Restaura la base de datos de alquileres desde un archivo de respaldo especificado.
        /// </summary>
        /// <param name="fileName">Nombre del archivo de respaldo (incluyendo la extensión).</param>
        void RestoreDatabaseRents(string fileName);

        /// <summary>
        /// Crea un archivo .txt vacío con el nombre basado en la fecha proporcionada.
        /// </summary>
        /// <param name="date">Fecha en formato 'yyyyMMdd_HHmmss' para el nombre del archivo.</param>
        void CreateEmptyFile(string date);

        /// <summary>
        /// Devuelve los nombres de todos los archivos .txt en el directorio de respaldos.
        /// </summary>
        /// <returns>Lista de nombres de archivos .txt (solo los nombres, sin la ruta completa).</returns>
        List<string> GetAllTxtFiles();
    }
}
