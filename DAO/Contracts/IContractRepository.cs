﻿using System;
using System.Collections.Generic;
using Services.Domain;

namespace Services.Dao.Contracts
{
    /// <summary>
    /// Interfaz para la gestión de contratos en el sistema.
    /// </summary>
    public interface IContractRepository
    {
        /// <summary>
        /// Agregar un nuevo contrato.
        /// </summary>
        /// <param name="contract">El contrato a agregar.</param>
        void AddContract(Contract contract);

        /// <summary>
        /// Actualizar un contrato existente.
        /// </summary>
        /// <param name="contract">El contrato a actualizar.</param>
        void UpdateContract(Contract contract);

        /// <summary>
        /// Eliminar un contrato por su identificador.
        /// </summary>
        /// <param name="contractId">El identificador del contrato a eliminar.</param>
        void DeleteContract(Guid contractId);

        /// <summary>
        /// Obtener un contrato por su identificador.
        /// </summary>
        /// <param name="contractId">El identificador del contrato.</param>
        /// <returns>El contrato si se encuentra, de lo contrario null.</returns>
        Contract GetContractById(Guid contractId);

        /// <summary>
        /// Obtener todos los contratos.
        /// </summary>
        /// <returns>Lista de todos los contratos.</returns>
        List<Contract> GetAllContracts();

        /// <summary>
        /// Obtener todas las cláusulas de un contrato.
        /// </summary>
        /// <param name="contractId">El identificador del contrato.</param>
        /// <returns>Lista de cláusulas asociadas al contrato.</returns>
        List<ContractClause> GetContractClauses(Guid contractId);

        /// <summary>
        /// Obtener contratos por su estado.
        /// </summary>
        /// <param name="status">El estado de los contratos a obtener.</param>
        /// <returns>Lista de contratos con el estado especificado.</returns>
        List<Contract> GetContractsByStatus(string status);
    }
}
