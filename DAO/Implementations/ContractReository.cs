using DAO.Contracts;
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
using OfficeOpenXml;
using OfficeOpenXml.Style;
using iTextSharp.text;
using static DAO.RentsDataSet;

namespace DAO.Implementations
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
        /// Genera un archivo PDF con las cláusulas ordenadas de un contrato específico.
        /// </summary>
        /// <param name="orderedClauses">Lista de cláusulas ordenadas.</param>
        /// <param name="outputPath">La ruta de salida para el archivo PDF.</param>
        public void GenerateContractPDF(List<ContractClause> orderedClauses, string outputPath)
        {
            if (orderedClauses == null || orderedClauses.Count == 0)
            {
                throw new ArgumentException("No se encontraron cláusulas para el contrato especificado.");
            }

            // Generar el PDF con las cláusulas ordenadas
            using (FileStream stream = new FileStream(outputPath, FileMode.Create))
            {
                Document pdfDoc = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();

                pdfDoc.Add(new Paragraph("Contrato de locación", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16)));
                pdfDoc.Add(new Paragraph("\n"));

                foreach (var clause in orderedClauses)
                {
                    pdfDoc.Add(new Paragraph(clause.TitleClause, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));
                    pdfDoc.Add(new Paragraph(clause.DetailClause, FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                    pdfDoc.Add(new Paragraph("\n"));
                }

                pdfDoc.Close();
                writer.Close();
            }
        }

        /// <summary>
        /// Guarda la imagen del contrato en la base de datos.
        /// </summary>
        /// <param name="contractId">El identificador único del contrato.</param>
        /// <param name="imageData">La imagen en formato de arreglo de bytes.</param>
        public void SaveContractImage(Guid contractId, byte[] imageData)
        {
            var contract = _contractTableAdapter.GetData().FirstOrDefault(c => c.IdContract == contractId);

            if (contract == null)
                throw new KeyNotFoundException($"No se encontró un contrato con el ID: {contractId}");

            contract.SignedContract = imageData;
            contract.StatusContract = "Activo";

            // Actualiza la fila correspondiente en la base de datos
            _contractTableAdapter.Update(contract);
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
                contract.StatusContract,  // Corresponde a la columna StatusContract
                contract.IdContract.ToString() + contract.FkIdProperty.ToString() + contract.FkIdTenant.ToString() + contract.DateStartContract.ToString() + contract.DateFinalContract.ToString() + contract.AnnualRentPrice.ToString() + contract.StatusContract.ToString(),
                null
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
            contractRow.MensualRentPrice = contract.AnnualRentPrice;
            contractRow.StatusContract = contract.StatusContract;
            contractRow.Hash = contract.IdContract.ToString() + contract.FkIdProperty.ToString() + contract.FkIdTenant.ToString() + contract.DateStartContract.ToString() + contract.DateFinalContract.ToString() + contract.AnnualRentPrice.ToString() + contract.StatusContract.ToString();

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
                Guid.Parse(contractRow.IdContract.ToString().ToUpper()),
                contractRow.FkIdProperty,
                contractRow.FkIdTenant,
                contractRow.DateStartContract,
                contractRow.DateFinalContract,
                contractRow.MensualRentPrice,
                contractRow.StatusContract,
                contractRow.IdContract.ToString() + contractRow.FkIdProperty.ToString() + contractRow.FkIdTenant.ToString() + contractRow.DateStartContract.ToString() + contractRow.DateFinalContract.ToString() + contractRow.MensualRentPrice.ToString() + contractRow.StatusContract.ToString()
            );
            _contractTableAdapter.Delete(
                contractRow.IdContract,
                contractRow.FkIdProperty,
                contractRow.FkIdTenant,
                contractRow.DateStartContract,
                contractRow.DateFinalContract,
                contractRow.MensualRentPrice,
                contractRow.StatusContract,
                contractRow.IdContract.ToString() + contractRow.FkIdProperty.ToString() + contractRow.FkIdTenant.ToString() + contractRow.DateStartContract.ToString() + contractRow.DateFinalContract.ToString() + contractRow.MensualRentPrice.ToString() + contractRow.StatusContract.ToString()
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
                AnnualRentPrice = row.MensualRentPrice,
                StatusContract = row.StatusContract,
                ContractImage = row.IsSignedContractNull() ? Array.Empty<byte>() : row.SignedContract
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
                AnnualRentPrice = contractRow.MensualRentPrice,
                StatusContract = contractRow.StatusContract,
                ContractImage = contractRow.IsSignedContractNull() ? Array.Empty<byte>() : contractRow.SignedContract
            };
        }

        /// <summary>
        /// Obtiene todas las cláusulas de un contrato por su identificador.
        /// </summary>
        /// <param name="contractId">El identificador del contrato.</param>
        /// <returns>Una lista de cláusulas asociadas al contrato.</returns>
        public List<ContractClause> GetContractClauses(Guid contractId)
        {
            var clausesData = _contractClauseTableAdapter.GetClauseByContractId(contractId);

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
                    AnnualRentPrice = row.MensualRentPrice,
                    StatusContract = row.StatusContract,
                    ContractImage = row.IsSignedContractNull() ? Array.Empty<byte>() : row.SignedContract
                }).ToList();
        }

        /// <summary>
        /// Genera un archivo Excel con los datos visibles en el DataGridView.
        /// </summary>
        /// <param name="filePath">Ruta donde se guardará el archivo Excel.</param>
        /// <param name="contracts">Lista de contratos visibles.</param>
        public void ExportContractsToExcel(string filePath, List<Contract> contracts)
        {
            if (contracts == null || contracts.Count == 0)
                throw new InvalidOperationException("No hay contratos para exportar.");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Reporte de Contratos");

                // Encabezados
                worksheet.Cells[1, 1].Value = "Propiedad";
                worksheet.Cells[1, 2].Value = "Fecha Inicio";
                worksheet.Cells[1, 3].Value = "Fecha Fin";
                worksheet.Cells[1, 4].Value = "Precio Anual";
                worksheet.Cells[1, 5].Value = "Estado";
                worksheet.Cells[1, 6].Value = "DNI Arrendatario";

                for (int i = 1; i <= 7; i++)
                {
                    worksheet.Cells[1, i].Style.Font.Bold = true;
                    worksheet.Cells[1, i].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // Datos
                for (int i = 0; i < contracts.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = contracts[i].PropertyAddres.ToString();
                    worksheet.Cells[i + 2, 2].Value = contracts[i].DateStartContract.ToString("yyyy-MM-dd");
                    worksheet.Cells[i + 2, 3].Value = contracts[i].DateFinalContract.ToString("yyyy-MM-dd");
                    worksheet.Cells[i + 2, 4].Value = contracts[i].AnnualRentPrice;
                    worksheet.Cells[i + 2, 5].Value = contracts[i].StatusContract;
                    worksheet.Cells[i + 2, 6].Value = contracts[i].TenantName.ToString();
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                FileInfo fileInfo = new FileInfo(filePath);
                excelPackage.SaveAs(fileInfo);
            }
        }
    }
}