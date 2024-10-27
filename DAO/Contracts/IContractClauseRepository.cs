using Domain;
using Services.Domain;
using System;
using System.Collections.Generic;

namespace DAO.Contracts
{
    /// <summary>
    /// Interfaz para la gestión de cláusulas de contrato (ContractClause) en el sistema.
    /// </summary>
    public interface IContractClauseRepository
    {
        /// <summary>
        /// Crea una nueva cláusula de contrato.
        /// </summary>
        /// <param name="contractClause">La cláusula de contrato a crear.</param>
        /// <returns>El identificador único de la cláusula creada.</returns>
        Guid Create(ContractClause contractClause);

        /// <summary>
        /// Actualiza una cláusula de contrato existente.
        /// </summary>
        /// <param name="contractClause">La cláusula de contrato a actualizar.</param>
        void Update(ContractClause contractClause);

        /// <summary>
        /// Elimina una cláusula de contrato por su identificador.
        /// </summary>
        /// <param name="idContractClause">El identificador de la cláusula a eliminar.</param>
        void Delete(Guid idContractClause);

        /// <summary>
        /// Obtiene todas las cláusulas de contratos almacenadas en la base de datos.
        /// </summary>
        /// <returns>Una lista de todas las cláusulas de contratos.</returns>
        List<ContractClause> GetAll();

        /// <summary>
        /// Obtiene todas las cláusulas asociadas a un contrato específico.
        /// </summary>
        /// <param name="idContract">El identificador del contrato.</param>
        /// <returns>Una lista de cláusulas asociadas al contrato.</returns>
        List<ContractClause> GetClausesByContractId(Guid idContract);
    }
}
