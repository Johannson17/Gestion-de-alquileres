using DAO.Contracts;
using DAO.Implementations.SqlServer;
using Domain;
using Services.Logic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using static Domain.Person;
using Services.Domain;

namespace LOGIC
{
    /// <summary>
    /// Clase que gestiona la lógica de negocio para propiedades, incluyendo creación, actualización y manipulación de inventarios asociados.
    /// </summary>
    public class PropertyLogic
    {
        private readonly ContractLogic _contractLogic;
        private readonly IPropertyRepository _propertyRepository;

        /// <summary>
        /// Constructor de la clase PropertyLogic. Inicializa el repositorio de propiedades.
        /// </summary>
        public PropertyLogic()
        {
            _propertyRepository = new PropertyRepository();
            _contractLogic = new ContractLogic();
        }

        /// <summary>
        /// Obtiene una propiedad por su ID.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad a obtener.</param>
        /// <returns>Devuelve la propiedad correspondiente al ID proporcionado.</returns>
        public Property GetPropertyById(Guid propertyId)
        {
            return _propertyRepository.GetById(propertyId);
        }

        /// <summary>
        /// Obtiene todas las propiedades almacenadas.
        /// </summary>
        /// <returns>Lista de todas las propiedades.</returns>
        public List<Property> GetAllProperties()
        {
            return _propertyRepository.GetAll();
        }

        /// <summary>
        /// Crea una nueva propiedad.
        /// </summary>
        /// <param name="property">Objeto Property que representa la nueva propiedad a crear.</param>
        /// <returns>Devuelve el ID de la propiedad recién creada.</returns>
        public async Task<Guid> CreateProperty(Property property)
        {
            return await Task.FromResult(_propertyRepository.Create(property));
        }

        /// <summary>
        /// Actualiza los datos de una propiedad existente.
        /// </summary>
        /// <param name="property">Objeto Property con los datos actualizados de la propiedad.</param>
        public void UpdateProperty(Property property)
        {
            _propertyRepository.Update(property);
        }

        /// <summary>
        /// Elimina una propiedad dada su ID.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad a eliminar.</param>
        public void DeleteProperty(Guid propertyId)
        {
            _propertyRepository.Delete(propertyId);
        }

        /// <summary>
        /// Obtiene una lista de propiedades filtradas por su estado.
        /// </summary>
        /// <param name="status">El estado de las propiedades a obtener (e.g., Disponible, Alquilada).</param>
        /// <returns>Lista de propiedades que cumplen con el estado especificado.</returns>
        public List<Property> GetPropertiesByStatus(PropertyStatusEnum status)
        {
            return _propertyRepository.GetByStatus(status.ToString());
        }

        /// <summary>
        /// Añade un nuevo inventario a una propiedad existente.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad a la que se añadirá el inventario.</param>
        /// <param name="inventoryItem">El objeto InventoryProperty que se añadirá.</param>
        public void AddInventoryToProperty(Guid propertyId, InventoryProperty inventoryItem)
        {
            _propertyRepository.AddInventory(propertyId, new List<InventoryProperty> { inventoryItem });
        }

        /// <summary>
        /// Elimina un inventario específico de una propiedad.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad de la que se eliminará el inventario.</param>
        /// <param name="inventoryId">El ID del inventario a eliminar.</param>
        public void RemoveInventoryFromProperty(Guid propertyId, Guid inventoryId)
        {
            _propertyRepository.DeleteInventoryById(propertyId, inventoryId);
        }

        /// <summary>
        /// Obtiene las propiedades para las que el inquilino tiene un contrato activo.
        /// </summary>
        /// <param name="tenantId">El ID del inquilino.</param>
        /// <returns>Lista de propiedades con contratos activos para el inquilino.</returns>
        public List<Property> GetActivePropertiesByTenantId(Guid tenantId)
        {
            List<Contract> activeContracts;

            try
            {
                // Intentar obtener los contratos activos del inquilino
                activeContracts = _contractLogic.GetContractsByTenantIdAndStatus(tenantId, "Activo");
            }
            catch (NullReferenceException ex)
            {
                throw new Exception("No tienes ninguna propiedad alquilada.", ex);
            }

            // Extraer los IDs de las propiedades de estos contratos
            var propertyIds = activeContracts.Select(c => c.FkIdProperty).Distinct();

            // Obtener y retornar las propiedades correspondientes a los IDs de contratos activos
            return GetAllProperties().Where(p => propertyIds.Contains(p.IdProperty)).ToList();
        }
    }
}
