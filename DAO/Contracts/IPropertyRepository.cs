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
        Guid Create(Property property);
        void Update(Property property);
        void Delete(Guid propertyId);
        List<Property> GetAll();
        Property GetById(Guid propertyId);
        List<Property> GetByStatus(string status);
        List<InventoryProperty> GetInventoryByPropertyId(Guid propertyId);
        void AddInventory(Guid propertyId, List<InventoryProperty> inventoryItems);
        void UpdateInventory(Guid propertyId, List<InventoryProperty> inventoryItems);
        void DeleteInventory(Guid propertyId);

        /// <summary>
        /// Elimina un elemento de inventario por su ID y el ID de la propiedad.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad asociada al inventario.</param>
        /// <param name="inventoryId">El ID del inventario a eliminar.</param>
        void DeleteInventoryById(Guid propertyId, Guid inventoryId);
    }
}