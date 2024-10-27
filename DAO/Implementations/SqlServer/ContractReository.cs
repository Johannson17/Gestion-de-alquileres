﻿using DAO.Contracts;
using DAO.RentsDataSetTableAdapters;
using Domain;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;
using Services.Dao.Contracts;
using Services.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DAO.Implementations.SqlServer
{
    /// <summary>
    /// Implementación del repositorio de contratos utilizando ContractTableAdapter.
    /// </summary>
    public class ContractRepository : IContractRepository
    {
        private readonly ContractTableAdapter _contractTableAdapter;
        private readonly ContractClauseTableAdapter _contractClauseTableAdapter;

        /// <summary>
        /// Constructor que inicializa los adaptadores de tabla.
        /// </summary>
        public ContractRepository()
        {
            _contractTableAdapter = new ContractTableAdapter();
            _contractClauseTableAdapter = new ContractClauseTableAdapter();
        }

        /// <summary>
        /// Agrega un nuevo contrato a la base de datos.
        /// </summary>
        /// <param name="contract">El contrato a agregar.</param>
        public void AddContract(Contract contract)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract), "El contrato no puede ser nulo.");
            }

            Guid newId = Guid.NewGuid();
            contract.IdContract = newId;

            _contractTableAdapter.Insert(
                contract.IdContract,
                contract.FkIdProperty,  // Corresponde a la columna FkIdProperty
                contract.FkIdTenant,    // Corresponde a la columna FkIdTenant
                contract.DateStartContract,  // Corresponde a la columna DateStartContract
                contract.DateFinalContract,  // Corresponde a la columna DateFinalContract
                contract.AnnualRentPrice,    // Corresponde a la columna AnnualRentPrice
                contract.StatusContract      // Corresponde a la columna StatusContract
            );
        }

        /// <summary>
        /// Actualiza un contrato existente en la base de datos.
        /// </summary>
        /// <param name="contract">El contrato a actualizar.</param>
        public void UpdateContract(Contract contract)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract), "El contrato no puede ser nulo.");
            }

            var contractRow = _contractTableAdapter.GetData().FirstOrDefault(c => c.IdContract == contract.IdContract);

            if (contractRow == null)
            {
                throw new KeyNotFoundException($"No se encontró un contrato con el ID: {contract.IdContract}");
            }

            contractRow.FkIdProperty = contract.FkIdProperty;
            contractRow.FkIdTenant = contract.FkIdTenant;
            contractRow.DateStartContract = contract.DateStartContract;
            contractRow.DateFinalContract = contract.DateFinalContract;
            contractRow.AnnualRentPrice = contract.AnnualRentPrice;
            contractRow.StatusContract = contract.StatusContract;

            _contractTableAdapter.Update(contractRow);
        }

        /// <summary>
        /// Elimina un contrato de la base de datos.
        /// </summary>
        /// <param name="contractId">El identificador único del contrato.</param>
        public void DeleteContract(Guid contractId)
        {
            var contractRow = _contractTableAdapter.GetData().FirstOrDefault(c => c.IdContract == contractId);

            if (contractRow == null)
            {
                throw new KeyNotFoundException($"No se encontró un contrato con el ID: {contractId}");
            }

            _contractTableAdapter.Delete(
                contractRow.IdContract,
                contractRow.FkIdProperty,
                contractRow.FkIdTenant,
                contractRow.DateStartContract,
                contractRow.DateFinalContract,
                contractRow.AnnualRentPrice,
                contractRow.StatusContract
            );
        }

        /// <summary>
        /// Obtiene todos los contratos de la base de datos.
        /// </summary>
        /// <returns>Una lista de contratos.</returns>
        public List<Contract> GetAllContracts()
        {
            var contractsData = _contractTableAdapter.GetData();

            return contractsData.Select(row => new Contract
            {
                IdContract = row.IdContract,
                FkIdProperty = row.FkIdProperty,
                FkIdTenant = row.FkIdTenant,
                DateStartContract = row.DateStartContract,
                DateFinalContract = row.DateFinalContract,
                AnnualRentPrice = row.AnnualRentPrice,
                StatusContract = row.StatusContract
            }).ToList();
        }

        /// <summary>
        /// Obtiene un contrato por su identificador único.
        /// </summary>
        /// <param name="contractId">El identificador del contrato.</param>
        /// <returns>El contrato correspondiente.</returns>
        public Contract GetContractById(Guid contractId)
        {
            var contractRow = _contractTableAdapter.GetData().FirstOrDefault(c => c.IdContract == contractId);

            if (contractRow == null)
            {
                return null;
            }

            return new Contract
            {
                IdContract = contractRow.IdContract,
                FkIdProperty = contractRow.FkIdProperty,
                FkIdTenant = contractRow.FkIdTenant,
                DateStartContract = contractRow.DateStartContract,
                DateFinalContract = contractRow.DateFinalContract,
                AnnualRentPrice = contractRow.AnnualRentPrice,
                StatusContract = contractRow.StatusContract
            };
        }

        /// <summary>
        /// Obtiene todas las cláusulas de un contrato por su identificador.
        /// </summary>
        /// <param name="contractId">El identificador del contrato.</param>
        /// <returns>Una lista de cláusulas asociadas al contrato.</returns>
        public List<ContractClause> GetContractClauses(Guid contractId)
        {
            var clausesData = _contractClauseTableAdapter.GetDataByIdContract(contractId);

            return clausesData.Select(row => new ContractClause
            {
                IdContractClause = row.IdContractClause,
                FkIdContract = row.FkIdContract,
                TitleClause = row.TitleClause,
                DetailClause = row.DetailClause
            }).ToList();
        }

        /// <summary>
        /// Obtiene todos los contratos que tienen un estado específico.
        /// </summary>
        /// <param name="status">El estado de los contratos a obtener.</param>
        /// <returns>Una lista de contratos con el estado especificado.</returns>
        public List<Contract> GetContractsByStatus(string status)
        {
            var contractsData = _contractTableAdapter.GetData();

            return contractsData
                .Where(row => row.StatusContract == status)
                .Select(row => new Contract
                {
                    IdContract = row.IdContract,
                    FkIdProperty = row.FkIdProperty,
                    FkIdTenant = row.FkIdTenant,
                    DateStartContract = row.DateStartContract,
                    DateFinalContract = row.DateFinalContract,
                    AnnualRentPrice = row.AnnualRentPrice,
                    StatusContract = row.StatusContract
                }).ToList();
        }

    }
}