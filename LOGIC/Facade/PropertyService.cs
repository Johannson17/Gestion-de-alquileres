using Domain;
using LOGIC;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Domain.Person;

namespace LOGIC.Facade
{
    /// <summary>
    /// Clase de fachada que proporciona una capa de abstracción para acceder a la lógica de negocio de propiedades.
    /// </summary>
    public class PropertyService
    {
        private readonly PropertyLogic _propertyLogic;

        /// <summary>
        /// Constructor de la clase PropertyService. Inicializa la lógica de propiedades.
        /// </summary>
        public PropertyService()
        {
            _propertyLogic = new PropertyLogic();
        }

        /// <summary>
        /// Obtiene una propiedad por su ID.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad a obtener.</param>
        /// <returns>Devuelve la propiedad correspondiente al ID proporcionado.</returns>
        public Property GetProperty(Guid propertyId)
        {
            return _propertyLogic.GetPropertyById(propertyId);
        }

        /// <summary>
        /// Obtiene todas las propiedades almacenadas.
        /// </summary>
        /// <returns>Lista de todas las propiedades.</returns>
        public List<Property> GetAllProperties()
        {
            return _propertyLogic.GetAllProperties();
        }

        /// <summary>
        /// Crea una nueva propiedad de manera asincrónica.
        /// </summary>
        /// <param name="property">Objeto Property que representa la nueva propiedad a crear.</param>
        /// <returns>Devuelve el ID de la propiedad recién creada.</returns>
        public async Task<Guid> CreatePropertyAsync(Property property)
        {
            return await _propertyLogic.CreateProperty(property);
        }

        /// <summary>
        /// Actualiza los datos de una propiedad existente.
        /// </summary>
        /// <param name="property">Objeto Property con los datos actualizados de la propiedad.</param>
        public void UpdateProperty(Property property)
        {
            _propertyLogic.UpdateProperty(property);
        }

        /// <summary>
        /// Elimina una propiedad dada su ID.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad a eliminar.</param>
        public void DeleteProperty(Guid propertyId)
        {
            _propertyLogic.DeleteProperty(propertyId);
        }

        /// <summary>
        /// Añade un nuevo inventario a una propiedad existente.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad a la que se añadirá el inventario.</param>
        /// <param name="inventoryItem">El objeto InventoryProperty que se añadirá.</param>
        public void AddInventory(Guid propertyId, InventoryProperty inventoryItem)
        {
            _propertyLogic.AddInventoryToProperty(propertyId, inventoryItem);
        }

        /// <summary>
        /// Elimina un elemento de inventario específico de una propiedad.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad asociada al inventario.</param>
        /// <param name="inventoryId">El ID del inventario a eliminar.</param>
        public void DeleteInventory(Guid propertyId, Guid inventoryId)
        {
            _propertyLogic.RemoveInventoryFromProperty(propertyId, inventoryId);
        }

        /// <summary>
        /// Obtiene una lista de propiedades filtradas por su estado.
        /// </summary>
        /// <param name="status">El estado de la propiedad a filtrar (por ejemplo, Disponible o Alquilada).</param>
        /// <returns>Una lista de propiedades que coinciden con el estado especificado.</returns>
        public List<Property> GetPropertiesByStatus(PropertyStatusEnum status)
        {
            return _propertyLogic.GetPropertiesByStatus(status);
        }

        /// <summary>
        /// Obtiene las propiedades para las que el inquilino tiene un contrato activo.
        /// </summary>
        /// <param name="tenantId">El ID del inquilino.</param>
        /// <returns>Lista de propiedades con contratos activos para el inquilino.</returns>
        public List<Property> GetActivePropertiesByTenantId(Guid tenantId)
        {
            try
            {
                return _propertyLogic.GetActivePropertiesByTenantId(tenantId);
            }
            catch (Exception ex)
            {
                throw new Exception("No tienes ninguna propiedad alquilada.", ex);
            }
        }
    }
}
