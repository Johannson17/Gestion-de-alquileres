using DAO.Contracts;
using DAO.Implementations;
using Domain;
using Services.Dao.Contracts;
using Services.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Logic
{
    /// <summary>
    /// Lógica de negocios para la gestión de contratos, incluyendo validaciones, creación y manipulación de contratos y sus cláusulas.
    /// </summary>
    public class ContractLogic
    {
        private readonly IContractRepository _contractRepository;
        private readonly IContractClauseRepository _contractClauseRepository;

        /// <summary>
        /// Constructor que inicializa la lógica de contratos con los repositorios necesarios.
        /// </summary>
        /// <param name="contractRepository">Repositorio para operaciones de contratos.</param>
        /// <param name="contractClauseRepository">Repositorio para operaciones de cláusulas de contratos.</param>
        public ContractLogic()
        {
            _contractRepository = new ContractRepository();
            _contractClauseRepository = new ContractClauseRepository();
        }

        /// <summary>
        /// Guarda la imagen del contrato en el repositorio.
        /// </summary>
        /// <param name="contractId">El identificador único del contrato.</param>
        /// <param name="imageData">La imagen en formato de arreglo de bytes.</param>
        public void SaveContractImage(Guid contractId, byte[] imageData)
        {
            _contractRepository.SaveContractImage(contractId, imageData);
        }

        /// <summary>
        /// Agrega un nuevo contrato al sistema, incluyendo sus cláusulas.
        /// </summary>
        /// <param name="contract">El contrato a agregar.</param>
        /// <param name="owner">El propietario del contrato (locador).</param>
        /// <param name="tenant">El arrendatario del contrato (locatario).</param>
        /// <param name="property">La propiedad asociada al contrato.</param>
        /// <returns>El identificador único (GUID) del contrato creado.</returns>
        public Guid AddContract(Contract contract, Person owner, Person tenant, Property property)
        {
            // Validar detalles del contrato
            ValidateContractDetails(contract, owner, tenant, property);

            // Generar nuevo ID y fecha de inicio del contrato
            contract.IdContract = Guid.NewGuid();
            contract.DateStartContract = DateTime.Now;
            contract.StatusContract = "Inactivo";

            // Agregar contrato en el repositorio de contratos
            _contractRepository.AddContract(contract);

            // Generar y agregar cláusulas iniciales al contrato
            var (clause1, clause2) = InitializeContractClauses(owner, tenant, property);
            clause1.FkIdContract = contract.IdContract;
            clause2.FkIdContract = contract.IdContract;

            foreach (var clause in contract.Clauses)
            {
                clause.FkIdContract = contract.IdContract;
                _contractClauseRepository.Create(clause);
            }

            return contract.IdContract;
        }

        /// <summary>
        /// Actualiza un contrato existente en el sistema.
        /// </summary>
        /// <param name="contract">El contrato a actualizar.</param>
        public void UpdateContract(Contract contract)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract), "El contrato no puede ser nulo.");
            if (string.IsNullOrWhiteSpace(contract.StatusContract))
                throw new InvalidOperationException("El estado del contrato no puede estar vacío.");

            contract.DateFinalContract = DateTime.Now.AddYears(2); // Extiende el contrato 2 años, por ejemplo.
            _contractRepository.UpdateContract(contract);
        }

        /// <summary>
        /// Actualiza una cláusula de contrato existente en el sistema.
        /// </summary>
        /// <param name="clause">La cláusula del contrato a actualizar.</param>
        public void UpdateContractClause(ContractClause clause)
        {
            if (clause == null)
                throw new ArgumentNullException(nameof(clause), "La cláusula no puede ser nula.");

            _contractClauseRepository.Update(clause);
        }

        /// <summary>
        /// Elimina un contrato y todas sus cláusulas asociadas, marcándolo como inactivo en lugar de eliminarlo físicamente.
        /// </summary>
        /// <param name="contractId">El identificador único (GUID) del contrato a eliminar.</param>
        public void DeleteContract(Guid contractId)
        {
            // Obtener el contrato por ID
            var contract = _contractRepository.GetContractById(contractId);
            if (contract == null)
                throw new KeyNotFoundException($"No se encontró un contrato con el ID: {contractId}");

            // Obtener todas las cláusulas asociadas al contrato
            var clauses = _contractClauseRepository.GetClausesByContractId(contractId);

            // Eliminar cada cláusula asociada
            foreach (var clause in clauses)
            {
                _contractClauseRepository.Delete(clause.IdContractClause);
            }

            // Finalmente, eliminar el contrato en sí
            _contractRepository.DeleteContract(contract.IdContract);
        }

        /// <summary>
        /// Elimina una cláusula de contrato por su identificador único.
        /// </summary>
        /// <param name="idContractClause">El identificador único (GUID) de la cláusula a eliminar.</param>
        public void DeleteContractClause(Guid idContractClause)
        {
            if (idContractClause == Guid.Empty)
                throw new ArgumentException("El identificador de la cláusula no puede estar vacío.", nameof(idContractClause));

            _contractClauseRepository.Delete(idContractClause);
        }

        /// <summary>
        /// Obtiene un contrato por su identificador único.
        /// </summary>
        /// <param name="contractId">El identificador único (GUID) del contrato.</param>
        /// <returns>El contrato correspondiente al identificador proporcionado.</returns>
        public Contract GetContractById(Guid contractId)
        {
            return _contractRepository.GetContractById(contractId)
                   ?? throw new KeyNotFoundException($"No se encontró un contrato con el ID: {contractId}");
        }

        /// <summary>
        /// Obtiene todos los contratos almacenados en el sistema.
        /// </summary>
        /// <returns>Lista de todos los contratos.</returns>
        public List<Contract> GetAllContracts()
        {
            return _contractRepository.GetAllContracts();
        }

        /// <summary>
        /// Agrega una nueva cláusula a un contrato existente.
        /// </summary>
        /// <param name="clause">La cláusula de contrato a agregar.</param>
        public void AddContractClause(ContractClause clause)
        {
            if (clause == null)
                throw new ArgumentNullException(nameof(clause), "La cláusula no puede ser nula.");
            if (clause.FkIdContract == Guid.Empty)
                throw new ArgumentException("La cláusula debe estar asociada a un contrato válido.", nameof(clause));

            _contractClauseRepository.Create(clause);
        }

        /// <summary>
        /// Obtiene todos los contratos con un estado específico.
        /// </summary>
        /// <param name="status">El estado de los contratos a obtener.</param>
        /// <returns>Lista de contratos que coinciden con el estado especificado.</returns>
        public List<Contract> GetContractsByStatus(string status)
        {
            return _contractRepository.GetContractsByStatus(status);
        }

        /// <summary>
        /// Obtiene todas las cláusulas asociadas a un contrato específico.
        /// </summary>
        /// <param name="contractId">El identificador único (GUID) del contrato.</param>
        /// <returns>Lista de cláusulas asociadas al contrato.</returns>
        public List<ContractClause> GetContractClauses(Guid contractId)
        {
            return _contractClauseRepository.GetClausesByContractId(contractId);
        }

        /// <summary>
        /// Inicializa las primeras dos cláusulas predefinidas del contrato con datos del locador, locatario y propiedad.
        /// </summary>
        /// <param name="contract">El contrato al que se le agregarán las cláusulas.</param>
        /// <param name="owner">El propietario del contrato (locador).</param>
        /// <param name="tenant">El arrendatario del contrato (locatario).</param>
        /// <param name="property">La propiedad asociada al contrato.</param>
        public (ContractClause Clause1, ContractClause Clause2) InitializeContractClauses(Person owner, Person tenant, Property property)
        {
            var clause1 = new ContractClause
            {
                IdContractClause = Guid.NewGuid(),
                TitleClause = "Datos del Contrato",
                DetailClause = $"En la Ciudad de {property.ProvinceProperty}, a los {DateTime.Now.Day} días del mes de {DateTime.Now.ToString("MMMM")} de {DateTime.Now.Year}, entre el/la señor/a {owner.NamePerson} {owner.LastNamePerson}, de {owner.TypeDocumentPerson.ToString()} N°{owner.NumberDocumentPerson}, con domicilio en {owner.DomicilePerson}, en lo sucesivo denominado/a como “LOCADOR”, y por otra parte, {tenant.NamePerson} {tenant.LastNamePerson}, de {tenant.TypeDocumentPerson} N°{tenant.NumberDocumentPerson}, con domicilio en {tenant.DomicilePerson}, en adelante denominado/a como “LOCATARIO”."
            };

            var clause2 = new ContractClause
            {
                IdContractClause = Guid.NewGuid(),
                TitleClause = "Primera (Objeto)",
                DetailClause = $"El LOCADOR da en locación al LOCATARIO, el inmueble ubicado en {property.AddressProperty}, {property.MunicipalityProperty}, {property.ProvinceProperty}, {property.CountryProperty}, para ser destinado exclusivamente a vivienda familiar del LOCATARIO."
            };

            return (clause1, clause2);
        }

        /// <summary>
        /// Obtiene todos los contratos asociados a un arrendatario específico.
        /// </summary>
        /// <param name="tenantId">El ID del arrendatario.</param>
        /// <returns>Una lista de contratos donde el usuario es el arrendatario.</returns>
        public List<Contract> GetContractsByTenantId(Guid tenantId)
        {
            return _contractRepository.GetAllContracts().Where(c => c.FkIdTenant == tenantId).ToList();
        }

        public List<Contract> GetContractsByTenantIdAndStatus(Guid tenantId, string Status)
        {
            return _contractRepository.GetAllContracts().Where(c => c.FkIdTenant == tenantId && c.StatusContract == Status).ToList();
        }

        /// <summary>
        /// Obtiene y ordena las cláusulas de un contrato por IdAuxiliar.
        /// </summary>
        /// <param name="contractId">El ID del contrato.</param>
        /// <returns>Lista de cláusulas ordenadas por IdAuxiliar.</returns>
        public List<ContractClause> GetOrderedClauses(Guid contractId)
        {
            var clauses = _contractClauseRepository.GetClausesByContractId(contractId);
            return clauses.OrderBy(clause => clause.IdAuxiliar).ToList();
        }

        /// <summary>
        /// Genera un PDF de las cláusulas del contrato en orden usando el listado de cláusulas proporcionado.
        /// </summary>
        /// <param name="contractId">El ID del contrato.</param>
        /// <param name="outputPath">La ruta de salida para el archivo PDF.</param>
        public void GenerateContractPDF(Guid contractId, string outputPath)
        {
            var orderedClauses = GetOrderedClauses(contractId);
            _contractRepository.GenerateContractPDF(orderedClauses, outputPath);
        }

        /// <summary>
        /// Valida los detalles del contrato, incluyendo los datos del propietario, arrendatario y la propiedad.
        /// </summary>
        /// <param name="contract">El contrato a validar.</param>
        /// <param name="owner">El propietario del contrato (locador).</param>
        /// <param name="tenant">El arrendatario del contrato (locatario).</param>
        /// <param name="property">La propiedad asociada al contrato.</param>
        private void ValidateContractDetails(Contract contract, Person owner, Person tenant, Property property)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract), "El contrato no puede ser nulo.");
            if (owner == null)
                throw new ArgumentNullException(nameof(owner), "El locador no puede ser nulo.");
            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant), "El locatario no puede ser nulo.");
            if (property == null || property.StatusProperty != PropertyStatusEnum.Disponible)
                throw new InvalidOperationException("La propiedad debe estar disponible para crear un contrato.");

            if (contract.AnnualRentPrice <= 0)
                throw new InvalidOperationException("El precio de renta anual debe ser mayor a cero.");
            if (contract.DateStartContract >= contract.DateFinalContract)
                throw new InvalidOperationException("La fecha de inicio debe ser anterior a la fecha de finalización.");
        }

        /// <summary>
        /// Solicita al repositorio exportar los contratos visibles en el DataGridView.
        /// </summary>
        /// <param name="filePath">Ruta donde se guardará el archivo Excel.</param>
        /// <param name="contracts">Lista de contratos visibles.</param>
        public void ExportContractsToExcel(string filePath, List<Contract> contracts)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("La ruta del archivo no puede estar vacía.", nameof(filePath));

            if (contracts == null || contracts.Count == 0)
                throw new ArgumentException("No hay contratos para exportar.", nameof(contracts));

            _contractRepository.ExportContractsToExcel(filePath, contracts);
        }
    }
}
