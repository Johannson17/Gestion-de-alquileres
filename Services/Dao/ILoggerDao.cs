using Services.Domain;
using System;

namespace Services.Dao
{
    /// <summary>
    /// Interfaz para definir las operaciones de registro de logs en la aplicación.
    /// </summary>
    internal interface ILoggerDao
    {
        /// <summary>
        /// Escribe un registro de log en el sistema de almacenamiento de logs.
        /// </summary>
        /// <param name="log">El objeto log que contiene información sobre el evento a registrar.</param>
        /// <param name="ex">Excepción opcional asociada con el evento de log, si existe.</param>
        void WriteLog(Log log, Exception ex = null);
    }
}
