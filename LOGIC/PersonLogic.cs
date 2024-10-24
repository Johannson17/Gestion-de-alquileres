using Domain;
using DAO.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using DAO.Implementations.SqlServer;

namespace LOGIC
{
    /// <summary>
    /// Lógica de negocio para la gestión de personas (propietarios e inquilinos).
    /// Esta clase se encarga de manejar las reglas de negocio y coordinar las operaciones de DAO.
    /// </summary>
    public class PersonLogic
    {
        private readonly IPersonRepository _personRepository;

        /// <summary>
        /// Constructor de la clase PersonLogic que inicializa el repositorio de personas.
        /// </summary>
        public PersonLogic()
        {
            _personRepository = new PersonRepository(); // Instanciación directa de la DAO.
        }

        /// <summary>
        /// Obtiene todas las personas registradas en el sistema.
        /// </summary>
        public List<Person> GetAllPersons()
        {
            return _personRepository.GetAll();
        }

        /// <summary>
        /// Obtiene personas filtradas por su tipo (propietarios o inquilinos).
        /// </summary>
        /// <param name="personType">Tipo de persona (Owner o Tenant).</param>
        public List<Person> GetAllPersonsByType(Person.PersonTypeEnum personType)
        {
            return _personRepository.GetAll()
                                     .Where(p => p.EnumTypePerson == personType)
                                     .ToList();
        }

        /// <summary>
        /// Obtiene una persona por su identificador único.
        /// </summary>
        /// <param name="personId">ID de la persona.</param>
        /// <returns>Objeto Person si se encuentra, de lo contrario null.</returns>
        public Person GetPersonById(Guid personId)
        {
            return _personRepository.GetById(personId);
        }

        /// <summary>
        /// Crea una nueva persona (propietario o inquilino) en el sistema.
        /// </summary>
        /// <param name="person">Objeto Person que representa a la persona a crear.</param>
        /// <returns>ID de la persona recién creada.</returns>
        public Guid CreatePerson(Person person)
        {
            return _personRepository.Create(person);
        }

        /// <summary>
        /// Actualiza los datos de una persona existente.
        /// </summary>
        /// <param name="person">Objeto Person que representa a la persona a actualizar.</param>
        public void UpdatePerson(Person person)
        {
            _personRepository.Update(person);
        }

        /// <summary>
        /// Elimina una persona del sistema.
        /// </summary>
        /// <param name="personId">ID de la persona a eliminar.</param>
        public void DeletePerson(Guid personId)
        {
            _personRepository.Delete(personId);
        }
    }
}
