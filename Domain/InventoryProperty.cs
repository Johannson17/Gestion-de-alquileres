using System;

namespace Domain
{
    /// <summary>
    /// Clase que representa el inventario asociado a una propiedad.
    /// </summary>
    public class InventoryProperty
    {
        /// <summary>
        /// Identificador único del inventario.
        /// </summary>
        public Guid IdInventoryProperty { get; set; }

        /// <summary>
        /// Identificador único de la propiedad a la que pertenece el inventario (FK).
        /// </summary>
        public Guid FkIdProperty { get; set; }

        /// <summary>
        /// Nombre del artículo en el inventario.
        /// </summary>
        public string NameInventory { get; set; }

        /// <summary>
        /// Descripción del artículo en el inventario.
        /// </summary>
        public string DescriptionInventory { get; set; }

        /// <summary>
        /// Constructor por defecto.
        /// </summary>
        public InventoryProperty()
        {
            // Inicialización por defecto
        }

        /// <summary>
        /// Constructor que inicializa todos los campos del inventario.
        /// </summary>
        /// <param name="idInventoryProperty">Identificador único del inventario.</param>
        /// <param name="fkIdProperty">Identificador único de la propiedad asociada.</param>
        /// <param name="nameInventory">Nombre del artículo.</param>
        /// <param name="descriptionInventory">Descripción del artículo.</param>
        public InventoryProperty(Guid idInventoryProperty, Guid fkIdProperty, string nameInventory, string descriptionInventory)
        {
            IdInventoryProperty = idInventoryProperty;
            FkIdProperty = fkIdProperty;
            NameInventory = nameInventory;
            DescriptionInventory = descriptionInventory;
        }
    }
}