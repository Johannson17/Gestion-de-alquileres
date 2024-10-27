using Domain;
using System;
using System.Collections.Generic;

namespace DAO.Contracts
{
    /// <summary>
    /// Interfaz que define las operaciones de acceso a datos para las propiedades.
    /// </summary>
    public interface IPropertyRepository
    {
        /// <summary>
        /// Crea una nueva propiedad en la base de datos.
        /// </summary>
        /// <param name="property">Objeto Property que representa la nueva propiedad a crear.</param>
        /// <returns>El ID de la propiedad recién creada.</returns>
        Guid Create(Property property);

        /// <summary>
        /// Actualiza los datos de una propiedad existente.
        /// </summary>
        /// <param name="property">Objeto Property con los datos actualizados de la propiedad.</param>
        void Update(Property property);

        /// <summary>
        /// Elimina una propiedad dado su ID.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad a eliminar.</param>
        void Delete(Guid propertyId);

        /// <summary>
        /// Obtiene todas las propiedades de la base de datos.
        /// </summary>
        /// <returns>Lista de todas las propiedades.</returns>
        List<Property> GetAll();

        /// <summary>
        /// Obtiene una propiedad por su ID.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad a obtener.</param>
        /// <returns>La propiedad correspondiente al ID proporcionado.</returns>
        Property GetById(Guid propertyId);

        /// <summary>
        /// Obtiene una lista de propiedades filtradas por su estado.
        /// </summary>
        /// <param name="status">El estado de las propiedades a obtener (e.g., Disponible, Alquilada).</param>
        /// <returns>Lista de propiedades que cumplen con el estado especificado.</returns>
        List<Property> GetByStatus(string status);

        /// <summary>
        /// Obtiene el inventario asociado a una propiedad específica.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad.</param>
        /// <returns>Lista de elementos de inventario asociados a la propiedad.</returns>
        List<InventoryProperty> GetInventoryByPropertyId(Guid propertyId);

        /// <summary>
        /// Añade uno o más elementos de inventario a una propiedad específica.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad a la que se añadirá el inventario.</param>
        /// <param name="inventoryItems">Lista de elementos de inventario a añadir.</param>
        void AddInventory(Guid propertyId, List<InventoryProperty> inventoryItems);

        /// <summary>
        /// Actualiza los elementos de inventario de una propiedad.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad.</param>
        /// <param name="inventoryItems">Lista actualizada de elementos de inventario.</param>
        void UpdateInventory(Guid propertyId, List<InventoryProperty> inventoryItems);

        /// <summary>
        /// Elimina todos los elementos de inventario asociados a una propiedad.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad cuyos elementos de inventario se eliminarán.</param>
        void DeleteInventory(Guid propertyId);

        /// <summary>
        /// Elimina un elemento de inventario específico por su ID y el ID de la propiedad.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad asociada al inventario.</param>
        /// <param name="inventoryId">El ID del inventario a eliminar.</param>
        void DeleteInventoryById(Guid propertyId, Guid inventoryId);
    }
}
