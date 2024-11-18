using System;
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

        /// <summary>
        /// Genera un PDF con las cláusulas de contrato ordenadas.
        /// </summary>
        /// <param name="orderedClauses">Lista de cláusulas ordenadas.</param>
        /// <param name="outputPath">Ruta de salida del archivo PDF.</param>
        void GenerateContractPDF(List<ContractClause> orderedClauses, string outputPath);

        /// <summary>
        /// Guarda la imagen del contrato en la base de datos.
        /// </summary>
        /// <param name="contractId">El identificador único del contrato.</param>
        /// <param name="imageData">La imagen en formato de arreglo de bytes.</param>
        void SaveContractImage(Guid contractId, byte[] imageData);

        /// <summary>
        /// Genera un archivo Excel con los datos de los contratos.
        /// </summary>
        /// <param name="filePath">Ruta donde se guardará el archivo Excel.</param>
        void ExportContractsToExcel(string filePath, List<Contract> contracts);
    }
}
