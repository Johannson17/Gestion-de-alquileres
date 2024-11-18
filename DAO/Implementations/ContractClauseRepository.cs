using DAO.Contracts;
using DAO.RentsDataSetTableAdapters;
using Domain;
using Services.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAO.Implementations
{
    /// <summary>
    /// Repositorio para la gestión de cláusulas de contratos (ContractClause).
    /// </summary>
    public class ContractClauseRepository : IContractClauseRepository
    {
        private readonly ContractClauseTableAdapter _contractClauseTableAdapter;

        /// <summary>
        /// Constructor que inicializa el ContractClauseTableAdapter.
        /// </summary>
        public ContractClauseRepository()
        {
            _contractClauseTableAdapter = new ContractClauseTableAdapter();
        }

        /// <summary>
        /// Crea una nueva cláusula de contrato y la inserta en la base de datos.
        /// </summary>
        /// <param name="contractClause">La cláusula de contrato a crear.</param>
        /// <returns>El ID de la cláusula creada.</returns>
        public Guid Create(ContractClause contractClause)
        {
            if (contractClause == null)
            {
                throw new ArgumentNullException(nameof(contractClause), "La cláusula de contrato no puede ser nula.");
            }

            Guid newId = Guid.NewGuid();
            _contractClauseTableAdapter.Insert(
                newId,
                contractClause.FkIdContract,
                contractClause.TitleClause,
                contractClause.DetailClause
            );

            return newId;
        }

        /// <summary>
        /// Actualiza una cláusula de contrato existente.
        /// </summary>
        /// <param name="contractClause">La cláusula de contrato a actualizar.</param>
        public void Update(ContractClause contractClause)
        {
            if (contractClause == null)
            {
                throw new ArgumentNullException(nameof(contractClause), "La cláusula de contrato no puede ser nula.");
            }

            var clauseRow = _contractClauseTableAdapter.GetData().FirstOrDefault(c => c.IdContractClause == contractClause.IdContractClause);
            if (clauseRow == null)
            {
                throw new KeyNotFoundException($"No se encontró una cláusula con el ID: {contractClause.IdContractClause}");
            }

            clauseRow.FkIdContract = contractClause.FkIdContract;
            clauseRow.TitleClause = contractClause.TitleClause;
            clauseRow.DetailClause = contractClause.DetailClause;

            _contractClauseTableAdapter.Update(clauseRow);
        }

        /// <summary>
        /// Elimina una cláusula de contrato por su identificador único.
        /// </summary>
        /// <param name="idContractClause">El identificador de la cláusula a eliminar.</param>
        public void Delete(Guid idContractClause)
        {
            var clauseRow = _contractClauseTableAdapter.GetData().FirstOrDefault(c => c.IdContractClause == idContractClause);
            if (clauseRow == null)
            {
                throw new KeyNotFoundException($"No se encontró una cláusula con el ID: {idContractClause}");
            }

            _contractClauseTableAdapter.Delete(
                clauseRow.IdContractClause,
                clauseRow.FkIdContract,
                clauseRow.TitleClause,
                clauseRow.DetailClause,
                clauseRow.IdAuxiliar
            );
        }

        /// <summary>
        /// Obtiene todas las cláusulas de contratos de la base de datos.
        /// </summary>
        /// <returns>Lista de todas las cláusulas de contrato.</returns>
        public List<ContractClause> GetAll()
        {
            var clausesData = _contractClauseTableAdapter.GetData();
            return clausesData.Select(row => new ContractClause
            {
                IdContractClause = row.IdContractClause,
                FkIdContract = row.FkIdContract,
                TitleClause = row.TitleClause,
                DetailClause = row.DetailClause
            }).ToList();
        }

        /// <summary>
        /// Obtiene todas las cláusulas asociadas a un contrato específico.
        /// </summary>
        /// <param name="idContract">El identificador del contrato.</param>
        /// <returns>Lista de cláusulas asociadas al contrato.</returns>
        public List<ContractClause> GetClausesByContractId(Guid idContract)
        {
            var clausesData = _contractClauseTableAdapter.GetClauseByContractId(idContract);
            return clausesData.Select(row => new ContractClause
            {
                IdContractClause = row.IdContractClause,
                FkIdContract = row.FkIdContract,
                TitleClause = row.TitleClause,
                DetailClause = row.DetailClause
            }).ToList();
        }
    }
}
