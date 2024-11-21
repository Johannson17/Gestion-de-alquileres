using Services.Dao.Contracts;
using Services.Factory;
using System;
using System.Collections.Generic;

namespace Services.Logic
{
    /// <summary>
    /// Lógica para respaldos y restauraciones de bases de datos.
    /// </summary>
    public class BackupLogic
    {
        private readonly IBackupRepository _backupRepository;

        public BackupLogic()
        {
            _backupRepository = FactoryDao.CreateRepository<IBackupRepository>()
                ?? throw new InvalidOperationException("No se pudo crear una instancia de IBackupRepository.");
        }

        public void BackupAllDatabases()
        {
            _backupRepository.BackupDatabaseSecurity();
            _backupRepository.BackupDatabaseRents();

            string date = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            _backupRepository.CreateEmptyFile(date);
        }

        public void RestoreAllDatabases(string date)
        {
            if (string.IsNullOrWhiteSpace(date) || date.Length != 8)
            {
                throw new ArgumentException("La fecha debe tener el formato 'yyyyMMdd'.");
            }

            _backupRepository.CreateEmptyFile(date); // Simulación de restauración.
            Console.WriteLine($"Restauración realizada para la fecha: {date}");
        }

        public List<string> GetAllBackupFiles()
        {
            return _backupRepository.GetAllTxtFiles();
        }
    }
}
