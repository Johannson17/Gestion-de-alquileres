using Domain;
using System;
using System.Collections.Generic;

namespace Services.Domain
{
    /// <summary>
    /// Representa un contrato de arrendamiento en el sistema.
    /// </summary>
    public class Contract
    {
        public Guid IdContract { get; set; } // Identificador del contrato
        public Guid FkIdProperty { get; set; } // Relación con la propiedad
        public Guid FkIdTenant { get; set; } // Relación con el arrendatario
        public Guid FkIdOwner { get; set; } // Relación con el propietario
        public DateTime DateStartContract { get; set; } // Fecha de inicio del contrato
        public DateTime DateFinalContract { get; set; } // Fecha de finalización del contrato
        public double AnnualRentPrice { get; set; } // Monto del alquiler anual
        public string StatusContract { get; set; } // Estado del contrato
        public bool ContractModel { get; set; } // Indica si es un contrato modelo
        public DateTime CreatedAt { get; set; } // Fecha de creación del contrato

        // Relación con las cláusulas del contrato
        public List<ContractClause> Clauses { get; set; } = new List<ContractClause>();
    }
}
