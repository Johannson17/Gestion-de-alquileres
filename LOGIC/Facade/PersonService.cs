using Domain;
using LOGIC;
using System;
using System.Collections.Generic;

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
            // Aquí solo se llama a la lógica, sin interacción con la DAO.
            _personLogic = new PersonLogic();
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
        public Guid CreatePerson(Person person)
        {
            try
            {
                return _personLogic.CreatePerson(person);
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
    }
}
