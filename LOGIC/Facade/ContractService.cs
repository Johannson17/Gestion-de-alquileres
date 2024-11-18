using Domain;
using Services.Domain;
using Services.Logic;
using System;
using System.Collections.Generic;

namespace Services.Facade
{
    /// <summary>
    /// Fachada para la gestión de contratos, que proporciona métodos para crear, actualizar, eliminar y obtener contratos y sus cláusulas.
    /// </summary>
    public class ContractService
    {
        private readonly ContractLogic _contractLogic;

        /// <summary>
        /// Constructor que inicializa la fachada de contratos con la lógica de negocios.
        /// </summary>
        public ContractService()
        {
            _contractLogic = new ContractLogic();
        }

        /// <summary>
        /// Crea un nuevo contrato en el sistema.
        /// </summary>
        /// <param name="contract">El contrato a crear.</param>
        /// <param name="owner">El propietario del contrato (locador).</param>
        /// <param name="tenant">El arrendatario del contrato (locatario).</param>
        /// <param name="property">La propiedad asociada al contrato.</param>
        /// <returns>El identificador único (GUID) del contrato creado.</returns>
        public Guid CreateContract(Contract contract, Person owner, Person tenant, Property property)
        {
            return _contractLogic.AddContract(contract, owner, tenant, property);
        }

        /// <summary>
        /// Agrega una nueva cláusula a un contrato existente.
        /// </summary>
        /// <param name="clause">La cláusula de contrato a agregar.</param>
        /// <returns>El identificador único (GUID) de la cláusula creada.</returns>
        public void AddContractClause(ContractClause clause)
        {
            _contractLogic.AddContractClause(clause);
        }

        /// <summary>
        /// Actualiza un contrato existente en el sistema.
        /// </summary>
        /// <param name="contract">El contrato a actualizar.</param>
        public void UpdateContract(Contract contract)
        {
            _contractLogic.UpdateContract(contract);
        }

        /// <summary>
        /// Actualiza una cláusula de contrato existente en el sistema.
        /// </summary>
        /// <param name="clause">La cláusula del contrato a actualizar.</param>
        public void UpdateContractClause(ContractClause clause)
        {
            _contractLogic.UpdateContractClause(clause);
        }

        /// <summary>
        /// Elimina un contrato del sistema.
        /// </summary>
        /// <param name="contractId">El identificador único (GUID) del contrato a eliminar.</param>
        public void DeleteContract(Guid contractId)
        {
            _contractLogic.DeleteContract(contractId);
        }

        /// <summary>
        /// Elimina una cláusula de contrato del sistema.
        /// </summary>
        /// <param name="clauseId">El identificador único (GUID) de la cláusula a eliminar.</param>
        public void DeleteContractClause(Guid clauseId)
        {
            _contractLogic.DeleteContractClause(clauseId);
        }

        /// <summary>
        /// Obtiene un contrato por su identificador único.
        /// </summary>
        /// <param name="contractId">El identificador único (GUID) del contrato.</param>
        /// <returns>El contrato correspondiente al identificador proporcionado.</returns>
        public Contract GetContractById(Guid contractId)
        {
            return _contractLogic.GetContractById(contractId);
        }

        /// <summary>
        /// Obtiene todos los contratos almacenados en el sistema.
        /// </summary>
        /// <returns>Una lista de todos los contratos.</returns>
        public List<Contract> GetAllContracts()
        {
            return _contractLogic.GetAllContracts();
        }

        /// <summary>
        /// Obtiene todos los contratos con un estado específico.
        /// </summary>
        /// <param name="status">El estado de los contratos a obtener.</param>
        /// <returns>Una lista de contratos que coinciden con el estado especificado.</returns>
        public List<Contract> GetContractsByStatus(string status)
        {
            return _contractLogic.GetContractsByStatus(status);
        }

        /// <summary>
        /// Obtiene todas las cláusulas asociadas a un contrato específico.
        /// </summary>
        /// <param name="contractId">El identificador único (GUID) del contrato.</param>
        /// <returns>Una lista de cláusulas asociadas al contrato.</returns>
        public List<ContractClause> GetContractClauses(Guid contractId)
        {
            return _contractLogic.GetContractClauses(contractId);
        }

        /// <summary>
        /// Obtiene el título y la descripción de una cláusula predefinida en función del índice.
        /// </summary>
        /// <param name="clauseIndex">El índice de la cláusula (1 o 2 para predefinidas).</param>
        /// <param name="owner">El propietario de la propiedad.</param>
        /// <param name="tenant">El arrendatario de la propiedad.</param>
        /// <param name="property">La propiedad relacionada con el contrato.</param>
        /// <returns>Una tupla con el título y la descripción de la cláusula.</returns>
        public (ContractClause Clause1, ContractClause Clause2) GetPredefinedClause(int clauseIndex, Person owner, Person tenant, Property property)
        {
            return _contractLogic.InitializeContractClauses(owner, tenant, property);
        }

        /// <summary>
        /// Obtiene todos los contratos en los que una persona logueada es el arrendatario.
        /// </summary>
        /// <param name="tenantId">El identificador único (GUID) del arrendatario.</param>
        /// <returns>Una lista de contratos asociados al arrendatario.</returns>
        public List<Contract> GetContractsByTenantId(Guid tenantId)
        {
            return _contractLogic.GetContractsByTenantId(tenantId);
        }

        public List<Contract> GetContractsByTenantIdAndStatus(Guid tenantId, string Status)
        {
            return _contractLogic.GetContractsByTenantIdAndStatus(tenantId, Status);
        }

        /// <summary>
        /// Genera un PDF de las cláusulas del contrato en orden de IdAuxiliar.
        /// </summary>
        /// <param name="contractId">El identificador único del contrato.</param>
        /// <param name="outputPath">La ruta de salida para el archivo PDF.</param>
        public void GenerateContractPDF(Guid contractId, string outputPath)
        {
            _contractLogic.GenerateContractPDF(contractId, outputPath);
        }

        /// <summary>
        /// Guarda la imagen del contrato en el repositorio.
        /// </summary>
        /// <param name="contractId">El identificador único del contrato.</param>
        /// <param name="imageData">La imagen en formato de arreglo de bytes.</param>
        public void SaveContractImage(Guid contractId, byte[] imageData)
        {
            _contractLogic.SaveContractImage(contractId, imageData);
        }

        /// <summary>
        /// Exporta los contratos visibles en el DataGridView a un archivo Excel.
        /// </summary>
        /// <param name="filePath">Ruta donde se guardará el archivo Excel.</param>
        /// <param name="contracts">Lista de contratos visibles.</param>
        public void ExportContractsToExcel(string filePath, List<Contract> contracts)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("La ruta del archivo no puede estar vacía.", nameof(filePath));

            if (contracts == null || contracts.Count == 0)
                throw new ArgumentException("No hay contratos para exportar.", nameof(contracts));

            _contractLogic.ExportContractsToExcel(filePath, contracts);
        }
    }
}
