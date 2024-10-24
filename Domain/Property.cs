using System;
using System.Collections.Generic;  // Necesario para usar List

namespace Domain
{
    public class Property
    {
        /// <summary>
        /// Identificador único de la propiedad.
        /// </summary>
        public Guid IdProperty { get; set; }

        /// <summary>
        /// País donde se encuentra la propiedad.
        /// </summary>
        public string CountryProperty { get; set; }

        /// <summary>
        /// Calle y número de la propiedad.
        /// </summary>
        public string AddressProperty { get; set; }

        /// <summary>
        /// Descripción de la propiedad.
        /// </summary>
        public string DescriptionProperty { get; set; }

        /// <summary>
        /// Lista de inventarios asociados a la propiedad.
        /// </summary>
        public List<InventoryProperty> InventoryProperty { get; set; }

        /// <summary>
        /// Provincia donde se encuentra la propiedad.
        /// </summary>
        public string ProvinceProperty { get; set; }

        /// <summary>
        /// Municipio donde se encuentra la propiedad.
        /// </summary>
        public string MunicipalityProperty { get; set; }

        /// <summary>
        /// Propietario de la propiedad.
        /// </summary>
        public Person OwnerProperty { get; set; }

        /// <summary>
        /// Estado de la propiedad (Alquilada, Disponible, etc.).
        /// </summary>
        public PropertyStatusEnum StatusProperty { get; set; }
    }
    public enum PropertyStatusEnum
    {
        Alquilada,
        Disponible
    }
}
