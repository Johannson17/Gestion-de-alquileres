using Services.Domain;
using System;
using System.Collections.Generic;

namespace Services.Dao.Contracts
{
    /// <summary>
    /// Interfaz para la DAO de Logger.
    /// Define los métodos necesarios para escribir y gestionar logs.
    /// </summary>
    public interface ILoggerDao
    {
        /// <summary>
        /// Escribe un log en el sistema.
        /// </summary>
        /// <param name="log">Información del log a escribir.</param>
        /// <param name="ex">Excepción opcional cuya traza se debe incluir en el log.</param>
        void WriteLog(Log log, Exception ex = null);

        /// <summary>
        /// Obtiene todos los logs almacenados en el sistema.
        /// </summary>
        /// <returns>Una lista de todos los logs almacenados.</returns>
        List<Log> GetAllLogs();

        /// <summary>
        /// Obtiene un log específico por su ID.
        /// </summary>
        /// <param name="logId">El identificador del log.</param>
        /// <returns>El log correspondiente al ID proporcionado, o null si no existe.</returns>
        Log GetLogById(Guid logId);

        /// <summary>
        /// Elimina un log específico por su ID.
        /// </summary>
        /// <param name="logId">El identificador del log.</param>
        void DeleteLog(Guid logId);

        /// <summary>
        /// Elimina todos los logs en el sistema.
        /// </summary>
        void DeleteAllLogs();
    }
}