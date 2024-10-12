using DAO.Implementations.SqlServer;  // Asegúrate de tener el using correcto para el repositorio
using Domain;
using System;
using System.Collections.Generic;

namespace LOGIC
{
    /// <summary>
    /// Lógica de negocio para la gestión de personas (propietarios e inquilinos).
    /// Esta clase se encarga de manejar las reglas de negocio y coordinar las operaciones de DAO.
    /// </summary>
    public class PersonLogic
    {
        private readonly PersonRepository _personRepository;

        /// <summary>
        /// Constructor de la clase PersonLogic que instancia el repositorio de personas.
        /// </summary>
        public PersonLogic()
        {
            _personRepository = new PersonRepository(); // Instanciamos el repositorio manualmente
        }

        /// <summary>
        /// Obtiene todas las personas (propietarios e inquilinos) registradas en el sistema.
        /// </summary>
        /// <returns>Lista de objetos Person.</returns>
        public List<Person> GetAllPersons()
        {
            try
            {
                return _personRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la lista de personas.", ex);
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
                return _personRepository.GetById(personId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la persona con ID: {personId}", ex);
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
                return _personRepository.Create(person);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear la persona.", ex);
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
                _personRepository.Update(person);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la persona.", ex);
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
                _personRepository.Delete(personId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar la persona con ID: {personId}", ex);
            }
        }
    }
}