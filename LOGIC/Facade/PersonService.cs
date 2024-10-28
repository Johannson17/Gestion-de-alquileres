using Domain;
using LOGIC;
using System;
using System.Collections.Generic;
using static Domain.Person;

namespace LOGIC.Facade
{
    /// <summary>
    /// Servicio para gestionar la lógica de personas (propietarios e inquilinos).
    /// Simplifica la interacción entre la capa de UI y la lógica de negocio.
    /// </summary>
    public class PersonService
    {
        private readonly PersonLogic _personLogic;

        /// <summary>
        /// Constructor que inicializa la lógica de negocio de personas.
        /// </summary>
        public PersonService()
        {
            _personLogic = new PersonLogic(); // Instanciación directa de la lógica.
        }

        /// <summary>
        /// Obtiene todas las personas (propietarios e inquilinos) registradas en el sistema.
        /// </summary>
        /// <returns>Lista de objetos Person.</returns>
        public List<Person> GetAllPersons()
        {
            try
            {
                return _personLogic.GetAllPersons();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la lista de personas desde el servicio.", ex);
            }
        }

        /// <summary>
        /// Obtiene una lista de personas filtrada por su tipo (propietario o inquilino).
        /// </summary>
        /// <param name="personType">El tipo de persona (Owner o Tenant).</param>
        /// <returns>Lista de objetos Person.</returns>
        public List<Person> GetAllPersonsByType(Person.PersonTypeEnum personType)
        {
            try
            {
                return _personLogic.GetAllPersonsByType(personType);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la lista de personas del tipo {personType} desde el servicio.", ex);
            }
        }

        /// <summary>
        /// Obtiene una persona por su ID.
        /// </summary>
        /// <param name="personId">ID de la persona.</param>
        /// <returns>Objeto Person si se encuentra, de lo contrario null.</returns>
        public Person GetPersonById(Guid personId)
        {
            try
            {
                return _personLogic.GetPersonById(personId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la persona con ID: {personId} desde el servicio.", ex);
            }
        }

        /// <summary>
        /// Crea una nueva persona (propietario o inquilino) en el sistema.
        /// </summary>
        /// <param name="person">Objeto Person que representa a la persona a crear.</param>
        /// <returns>ID de la persona recién creada.</returns>
        public Guid CreatePerson(Person person, Guid userId)
        {
            try
            {
                return _personLogic.CreatePerson(person, userId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear la persona desde el servicio.", ex);
            }
        }

        /// <summary>
        /// Actualiza los datos de una persona existente.
        /// </summary>
        /// <param name="person">Objeto Person que representa a la persona a actualizar.</param>
        public void UpdatePerson(Person person)
        {
            try
            {
                _personLogic.UpdatePerson(person);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la persona desde el servicio.", ex);
            }
        }

        /// <summary>
        /// Elimina una persona del sistema.
        /// </summary>
        /// <param name="personId">ID de la persona a eliminar.</param>
        public void DeletePerson(Guid personId)
        {
            try
            {
                _personLogic.DeletePerson(personId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar la persona con ID: {personId} desde el servicio.", ex);
            }
        }

        /// <summary>
        /// Obtiene una persona asociada a una propiedad específica y tipo de persona.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad.</param>
        /// <param name="personType">El tipo de persona (por ejemplo, Propietario o Inquilino).</param>
        /// <returns>La persona correspondiente a la propiedad y tipo proporcionados.</returns>
        public Person GetPersonByPropertyAndType(Guid propertyId, PersonTypeEnum personType)
        {
            return _personLogic.GetPersonByPropertyAndType(propertyId, personType);
        }

        /// <summary>
        /// Obtiene la persona asociada al ID de usuario.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <returns>Objeto Person si se encuentra, de lo contrario null.</returns>
        public Person GetPersonByUserId(Guid userId)
        {
            try
            {
                return _personLogic.GetPersonByUserId(userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la persona asociada al ID de usuario: {userId}", ex);
            }
        }
    }
}