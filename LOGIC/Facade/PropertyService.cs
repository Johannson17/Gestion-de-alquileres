using Domain;
using LOGIC;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LOGIC.Facade
{
    public class PropertyService
    {
        private readonly PropertyLogic _propertyLogic;

        public PropertyService()
        {
            _propertyLogic = new PropertyLogic();
        }

        // Método existente para obtener una propiedad por ID
        public Property GetProperty(Guid propertyId)
        {
            return _propertyLogic.GetPropertyById(propertyId);
        }

        // Método para obtener todas las propiedades
        public List<Property> GetAllProperties()
        {
            return _propertyLogic.GetAllProperties();
        }

        // Método para crear una propiedad de manera asincrónica
        public async Task<Guid> CreatePropertyAsync(Property property)
        {
            return await _propertyLogic.CreateProperty(property);
        }

        // Método para actualizar una propiedad
        public void UpdateProperty(Property property)
        {
            _propertyLogic.UpdateProperty(property);
        }

        // Método para eliminar una propiedad
        public void DeleteProperty(Guid propertyId)
        {
            _propertyLogic.DeleteProperty(propertyId);
        }

        // Nuevo método para agregar inventario a una propiedad existente
        public void AddInventory(Guid propertyId, InventoryProperty inventoryItem)
        {
            _propertyLogic.AddInventoryToProperty(propertyId, inventoryItem);
        }

        // Nuevo método para eliminar inventario de una propiedad
        public void DeleteInventory(Guid propertyId, Guid inventoryId)
        {
            _propertyLogic.RemoveInventoryFromProperty(propertyId, inventoryId);
        }
    }
}
