using DAO.Contracts;
using DAO.RentsDataSetTableAdapters;
using Domain;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static DAO.RentsDataSet;
using static Domain.Person;

namespace DAO.Implementations
{
    /// <summary>
    /// Repositorio para manejar las operaciones CRUD relacionadas con propiedades en la base de datos.
    /// </summary>
    public class PropertyRepository : IPropertyRepository
    {
        private readonly PropertyTableAdapter _propertyTableAdapter;
        private readonly PropertyPersonTableAdapter _propertyPersonTableAdapter;
        private readonly InventoryPropertyTableAdapter _inventoryPropertyTableAdapter;
        private readonly PersonTableAdapter _personTableAdapter;

        /// <summary>
        /// Constructor que inicializa los adaptadores de tabla necesarios para las operaciones en la base de datos.
        /// </summary>
        public PropertyRepository()
        {
            _propertyTableAdapter = new PropertyTableAdapter();
            _propertyPersonTableAdapter = new PropertyPersonTableAdapter();
            _inventoryPropertyTableAdapter = new InventoryPropertyTableAdapter();
            _personTableAdapter = new PersonTableAdapter();
        }

        /// <summary>
        /// Inserta una nueva propiedad en la base de datos y registra al propietario en la tabla PropertyPerson.
        /// </summary>
        /// <param name="property">El objeto Property que contiene los datos de la propiedad.</param>
        /// <returns>Devuelve el ID generado para la propiedad creada.</returns>
        public Guid Create(Property property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property), "La entidad propiedad no puede ser nula.");
            }

            Guid newId = Guid.NewGuid();

            // Inserta la propiedad con el campo AddressProperty incluido y StatusProperty como texto (enum en la BD)
            _propertyTableAdapter.Insert(
                newId,
                property.DescriptionProperty,  // Descripción
                property.CountryProperty,      // Campo país
                property.ProvinceProperty,     // Campo provincia
                property.MunicipalityProperty, // Campo municipalidad
                property.AddressProperty,      // Campo dirección
                property.StatusProperty.ToString()  // Guardar el valor del enum como texto
            );

            Guid newId2 = Guid.NewGuid();
            _propertyPersonTableAdapter.Insert(newId2, newId, (Guid)property.OwnerProperty.IdPerson);

            if (property.InventoryProperty != null && property.InventoryProperty.Any())
            {
                AddInventory(newId, property.InventoryProperty);
            }

            return newId;
        }

        /// <summary>
        /// Inserta un nuevo inventario relacionado con la propiedad.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad a la que se añadirá el inventario.</param>
        /// <param name="inventoryItems">Lista de objetos InventoryProperty que representan los elementos del inventario.</param>
        public void AddInventory(Guid propertyId, List<InventoryProperty> inventoryItems)
        {
            foreach (var item in inventoryItems)
            {
                Guid newInventoryId = Guid.NewGuid();
                _inventoryPropertyTableAdapter.Insert(
                    newInventoryId,
                    propertyId,
                    item.NameInventory,
                    item.DescriptionInventory
                );
            }
        }

        /// <summary>
        /// Actualiza los datos de una propiedad existente en la base de datos.
        /// </summary>
        /// <param name="property">El objeto Property con los datos actualizados.</param>
        public void Update(Property property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property), "La entidad propiedad no puede ser nula.");
            }

            var propertyRow = _propertyTableAdapter.GetData().FirstOrDefault(r => r.IdProperty == property.IdProperty);

            if (propertyRow == null)
            {
                throw new KeyNotFoundException($"No se encontró una propiedad con el ID: {property.IdProperty}");
            }

            // Actualiza la propiedad con los nuevos campos
            propertyRow.DescriptionProperty = property.DescriptionProperty;
            propertyRow.StatusProperty = property.StatusProperty.ToString();  // Guardar el enum como texto
            propertyRow.CountryProperty = property.CountryProperty;          // Campo país
            propertyRow.ProvinceProperty = property.ProvinceProperty;        // Campo provincia
            propertyRow.MunicipalityProperty = property.MunicipalityProperty; // Campo municipalidad
            propertyRow.AddressProperty = property.AddressProperty;          // Campo dirección

            _propertyTableAdapter.Update(propertyRow);

            if (property.InventoryProperty != null && property.InventoryProperty.Any())
            {
                UpdateInventory(property.IdProperty, property.InventoryProperty);
            }
        }

        /// <summary>
        /// Actualiza el inventario relacionado con una propiedad.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad.</param>
        /// <param name="inventoryItems">Lista de objetos InventoryProperty con los datos actualizados del inventario.</param>
        public void UpdateInventory(Guid propertyId, List<InventoryProperty> inventoryItems)
        {
            foreach (var item in inventoryItems)
            {
                var existingItem = _inventoryPropertyTableAdapter.GetData().FirstOrDefault(r => r.IdInventoryProperty == item.IdInventoryProperty && r.FkIdProperty == propertyId);

                if (existingItem != null)
                {
                    existingItem.NameInventory = item.NameInventory;
                    existingItem.DescriptionInventory = item.DescriptionInventory;

                    _inventoryPropertyTableAdapter.Update(existingItem);
                }
                else
                {
                    AddInventory(propertyId, new List<InventoryProperty> { item });
                }
            }
        }

        /// <summary>
        /// Elimina una propiedad y su inventario asociado.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad a eliminar.</param>
        public void Delete(Guid propertyId)
        {
            var propertyRow = _propertyTableAdapter.GetData().FirstOrDefault(r => r.IdProperty == propertyId);

            if (propertyRow == null)
            {
                throw new KeyNotFoundException($"No se encontró una propiedad con el ID: {propertyId}");
            }

            // Eliminar primero la relación con el propietario
            DeletePropertyPersonRelation(propertyId);

            // Eliminar el inventario
            DeleteInventory(propertyId);

            // Eliminar la propiedad
            _propertyTableAdapter.Delete1(
                Guid.Parse(propertyRow.IdProperty.ToString().ToUpper())
            );

            _propertyTableAdapter.Delete1(
                Guid.Parse(propertyRow.IdProperty.ToString())
            );
        }

        /// <summary>
        /// Elimina la relación entre la propiedad y el propietario.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad cuya relación se eliminará.</param>
        private void DeletePropertyPersonRelation(Guid propertyId)
        {
            var personRows = _propertyPersonTableAdapter.GetData().Where(r => r.FkIdProperty == propertyId).ToList();

            foreach (var personRow in personRows)
            {
                _propertyPersonTableAdapter.Delete(personRow.IdPropertyPerson, personRow.FkIdProperty, personRow.FkIdPerson);
            }
        }

        /// <summary>
        /// Elimina el inventario relacionado con una propiedad.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad cuyo inventario se eliminará.</param>
        public void DeleteInventory(Guid propertyId)
        {
            var inventoryItems = _inventoryPropertyTableAdapter.GetData().Where(r => r.FkIdProperty == propertyId).ToList();

            foreach (var item in inventoryItems)
            {
                _inventoryPropertyTableAdapter.Delete(item.IdInventoryProperty, item.FkIdProperty, item.NameInventory, item.DescriptionInventory);
            }
        }

        /// <summary>
        /// Elimina un elemento de inventario específico por su ID.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad asociada al inventario.</param>
        /// <param name="inventoryId">El ID del inventario a eliminar.</param>
        public void DeleteInventoryById(Guid propertyId, Guid inventoryId)
        {
            var inventoryItem = _inventoryPropertyTableAdapter.GetData()
                .FirstOrDefault(r => r.FkIdProperty == propertyId && r.IdInventoryProperty == inventoryId);

            if (inventoryItem != null)
            {
                _inventoryPropertyTableAdapter.Delete(
                    inventoryItem.IdInventoryProperty,
                    inventoryItem.FkIdProperty,
                    inventoryItem.NameInventory,
                    inventoryItem.DescriptionInventory
                );
            }
        }

        /// <summary>
        /// Obtiene todas las propiedades en la base de datos.
        /// </summary>
        /// <returns>Una lista de todas las propiedades.</returns>
        public List<Property> GetAll()
        {
            var propertyData = _propertyTableAdapter.GetData();

            return propertyData.Select(row => new Property
            {
                IdProperty = row.IdProperty,
                DescriptionProperty = row.DescriptionProperty,
                StatusProperty = (PropertyStatusEnum)Enum.Parse(typeof(PropertyStatusEnum), row.StatusProperty), // Convertir el texto en enum
                CountryProperty = row.CountryProperty,          // Campo país
                ProvinceProperty = row.ProvinceProperty,        // Campo provincia
                MunicipalityProperty = row.MunicipalityProperty, // Campo municipalidad
                AddressProperty = row.AddressProperty           // Campo dirección
            }).ToList();
        }

        /// <summary>
        /// Obtiene una propiedad por su ID, incluyendo su inventario y propietario.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad a obtener.</param>
        /// <returns>La propiedad con el inventario y propietario asociados.</returns>
        public Property GetById(Guid propertyId)
        {
            var propertyRow = _propertyTableAdapter.GetData().FirstOrDefault(r => r.IdProperty == propertyId);

            if (propertyRow == null)
            {
                return null;
            }

            var property = new Property
            {
                IdProperty = propertyRow.IdProperty,
                DescriptionProperty = propertyRow.DescriptionProperty,
                StatusProperty = (PropertyStatusEnum)Enum.Parse(typeof(PropertyStatusEnum), propertyRow.StatusProperty), // Convertir el texto en enum
                CountryProperty = propertyRow.CountryProperty,         // Campo país
                ProvinceProperty = propertyRow.ProvinceProperty,       // Campo provincia
                MunicipalityProperty = propertyRow.MunicipalityProperty, // Campo municipalidad
                AddressProperty = propertyRow.AddressProperty           // Campo dirección
            };

            // Cargar el inventario asociado a la propiedad
            property.InventoryProperty = GetInventoryByPropertyId(propertyId);

            return property;
        }

        /// <summary>
        /// Obtiene el inventario relacionado con una propiedad.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad cuyo inventario se obtendrá.</param>
        /// <returns>Una lista de objetos InventoryProperty relacionados con la propiedad.</returns>
        public List<InventoryProperty> GetInventoryByPropertyId(Guid propertyId)
        {
            var inventoryData = _inventoryPropertyTableAdapter.GetData().Where(r => r.FkIdProperty == propertyId).ToList();

            return inventoryData.Select(row => new InventoryProperty
            {
                IdInventoryProperty = row.IdInventoryProperty,
                NameInventory = row.NameInventory,
                DescriptionInventory = row.DescriptionInventory
            }).ToList();
        }

        /// <summary>
        /// Obtiene propiedades por su estado (Alquilada o Disponible).
        /// </summary>
        /// <param name="status">El estado de la propiedad a filtrar (por ejemplo, Alquilada o Disponible).</param>
        /// <returns>Una lista de propiedades filtradas por estado.</returns>
        public List<Property> GetByStatus(string status)
        {
            var propertyData = _propertyTableAdapter.GetData();

            return propertyData
                .Where(row => row.StatusProperty == status) // Comparar el estado como texto
                .Select(row => new Property
                {
                    IdProperty = row.IdProperty,
                    DescriptionProperty = row.DescriptionProperty,
                    StatusProperty = (PropertyStatusEnum)Enum.Parse(typeof(PropertyStatusEnum), row.StatusProperty), // Convertir el texto en enum
                    CountryProperty = row.CountryProperty,         // Campo país
                    ProvinceProperty = row.ProvinceProperty,       // Campo provincia
                    MunicipalityProperty = row.MunicipalityProperty, // Campo municipalidad
                    AddressProperty = row.AddressProperty           // Campo dirección
                }).ToList();
        }

        /// <summary>
        /// Genera un archivo Excel con la lista de propiedades proporcionada.
        /// </summary>
        /// <param name="filePath">Ruta donde se guardará el archivo Excel.</param>
        /// <param name="properties">Lista de propiedades a exportar.</param>
        public void GeneratePropertiesExcel(string filePath, List<Property> properties)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("La ruta del archivo no puede estar vacía.");

            if (properties == null || !properties.Any())
                throw new ArgumentException("No hay propiedades para exportar.");

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Propiedades");

                // Encabezados
                worksheet.Cells[1, 1].Value = "Descripción";
                worksheet.Cells[1, 2].Value = "Estado";
                worksheet.Cells[1, 3].Value = "País";
                worksheet.Cells[1, 4].Value = "Provincia";
                worksheet.Cells[1, 5].Value = "Municipio";
                worksheet.Cells[1, 6].Value = "Dirección";

                // Datos
                for (int i = 0; i < properties.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = properties[i].DescriptionProperty ?? "Sin descripción";
                    worksheet.Cells[i + 2, 2].Value = properties[i].StatusProperty.ToString() ?? "Sin estado";
                    worksheet.Cells[i + 2, 3].Value = properties[i].CountryProperty ?? "Sin país";
                    worksheet.Cells[i + 2, 4].Value = properties[i].ProvinceProperty ?? "Sin provincia";
                    worksheet.Cells[i + 2, 5].Value = properties[i].MunicipalityProperty ?? "Sin municipio";
                    worksheet.Cells[i + 2, 6].Value = properties[i].AddressProperty ?? "Sin dirección";
                }

                package.SaveAs(new FileInfo(filePath));
            }
        }
    }
}