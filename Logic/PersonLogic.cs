using DAO.Contracts;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOGIC
{
    /// <summary>
    /// Lógica de negocio para la gestión de personas (propietarios).
    /// Esta clase se encarga de manejar las reglas de negocio y coordinar las operaciones de DAO.
    /// </summary>
    public class PersonLogic
    {
        private readonly IPersonRepository _personRepository;

        /// <summary>
        /// Constructor de la clase PersonLogic que inicializa el repositorio de personas.
        /// Recibe una instancia de IPersonRepository.
        /// </summary>
        /// <param name="personRepository">Instancia de IPersonRepository inyectada.</param>
        public PersonLogic(IPersonRepository personRepository)
        {
            _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository), "El repositorio de personas no puede ser nulo.");
        }

        /// <summary>
        /// Obtiene todos los propietarios (personas) registrados en el sistema.
        /// </summary>
        /// <returns>Lista de objetos Person.</returns>
        public List<Person> GetAllOwners()
        {
            try
            {
                return _personRepository.GetAll().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la lista de propietarios.", ex);
            }
        }

        /// <summary>
        /// Obtiene un propietario (persona) por su ID.
        /// </summary>
        /// <param name="personId">ID del propietario.</param>
        /// <returns>Objeto Person si se encuentra, de lo contrario null.</returns>
        public Person GetOwnerById(Guid personId)
        {
            try
            {
                return _personRepository.GetById(personId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el propietario con ID: {personId}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo propietario (persona) en el sistema.
        /// </summary>
        /// <param name="person">Objeto Person que representa al propietario a crear.</param>
        /// <returns>ID del propietario recién creado.</returns>
        public Guid CreateOwner(Person person)
        {
            try
            {
                if (person == null)
                {
                    throw new ArgumentNullException(nameof(person), "El propietario no puede ser nulo.");
                }

                // Aquí puedes agregar reglas de validación antes de crear el propietario
                return _personRepository.Create(person);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el propietario.", ex);
            }
        }

        /// <summary>
        /// Actualiza los datos de un propietario (persona) existente.
        /// </summary>
        /// <param name="person">Objeto Person que representa al propietario actualizado.</param>
        public void UpdateOwner(Person person)
        {
            try
            {
                if (person == null)
                {
                    throw new ArgumentNullException(nameof(person), "El propietario no puede ser nulo.");
                }

                // Verifica si el propietario ya existe
                var existingOwner = _personRepository.GetById(person.IdPerson);
                if (existingOwner == null)
                {
                    throw new KeyNotFoundException("El propietario no existe.");
                }

                _personRepository.Update(person);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el propietario.", ex);
            }
        }

        /// <summary>
        /// Elimina un propietario (persona) del sistema.
        /// </summary>
        /// <param name="personId">ID del propietario a eliminar.</param>
        public void DeleteOwner(Guid personId)
        {
            try
            {
                var existingOwner = _personRepository.GetById(personId);
                if (existingOwner == null)
                {
                    throw new KeyNotFoundException("El propietario no existe.");
                }

                _personRepository.Delete(personId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar el propietario con ID: {personId}", ex);
            }
        }
    }
}
