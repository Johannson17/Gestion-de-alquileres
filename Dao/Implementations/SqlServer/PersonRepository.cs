using DAO.Contracts;
using Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DAO.Implementations.SqlServer
{
    /// <summary>
    /// Implementación del repositorio de personas que utiliza Entity Framework para interactuar con la base de datos.
    /// </summary>
    public class PersonRepository : IPersonRepository
    {
        private readonly DbContext _context;

        /// <summary>
        /// Constructor que inyecta el DbContext.
        /// </summary>
        /// <param name="context">El contexto de base de datos que se utilizará para las operaciones CRUD.</param>
        public PersonRepository(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Inserta una nueva persona en la base de datos y devuelve el ID de la persona creada.
        /// </summary>
        /// <param name="person">La entidad Person que se desea crear.</param>
        /// <returns>El ID de la persona creada.</returns>
        public Guid Create(Person person)
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person), "La entidad persona no puede ser nula.");
            }

            _context.Set<Person>().Add(person);
            _context.SaveChanges();

            return person.IdPerson; // Asumiendo que la entidad tiene un campo IdPerson de tipo Guid
        }

        /// <summary>
        /// Actualiza la información de una persona existente en la base de datos.
        /// </summary>
        /// <param name="person">La entidad Person que se desea actualizar.</param>
        public void Update(Person person)
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person), "La entidad persona no puede ser nula.");
            }

            _context.Set<Person>().Update(person);
            _context.SaveChanges();
        }

        /// <summary>
        /// Elimina una persona de la base de datos usando su identificador único.
        /// </summary>
        /// <param name="personId">El identificador único de la persona.</param>
        public void Delete(Guid personId)
        {
            var person = _context.Set<Person>().Find(personId);

            if (person == null)
            {
                throw new KeyNotFoundException($"No se encontró una persona con el ID: {personId}");
            }

            _context.Set<Person>().Remove(person);
            _context.SaveChanges();
        }

        /// <summary>
        /// Obtiene todas las personas almacenadas en la base de datos.
        /// </summary>
        /// <returns>Una lista de objetos Person.</returns>
        public List<Person> GetAll()
        {
            return _context.Set<Person>().ToList();
        }

        /// <summary>
        /// Obtiene una persona específica por su identificador único.
        /// </summary>
        /// <param name="personId">El identificador único de la persona.</param>
        /// <returns>La entidad Person correspondiente o null si no se encuentra.</returns>
        public Person GetById(Guid personId)
        {
            return _context.Set<Person>().Find(personId);
        }
    }
}
