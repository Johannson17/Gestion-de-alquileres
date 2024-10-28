using System;

namespace Services.Domain
{
    /// <summary>
    /// Representa una cláusula dentro de un contrato de arrendamiento.
    /// </summary>
    public class ContractClause
    {
        public Guid IdContractClause { get; set; } // Identificador de la cláusula
        public Guid FkIdContract { get; set; } // Relación con el contrato
        public string TitleClause { get; set; } // Título de la cláusula
        public string DetailClause { get; set; } // Detalles de la cláusula
        public int IdAuxiliar { get; set; } // Identificador de la cláusula
    }
}
