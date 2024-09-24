using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO.Contracts
{
    /// <summary>
    /// Interfaz para las operaciones de acceso a datos relacionadas con los propietarios (Person).
    /// Define los métodos CRUD básicos para trabajar con la entidad Person.
    /// </summary>
    public interface IPersonRepository
    {
        /// <summary>
        /// Crea un nuevo propietario (Person) en la base de datos.
        /// </summary>
        /// <param name="person">Objeto Person que se va a crear.</param>
        Guid Create(Person person);

        /// <summary>
        /// Actualiza la información de un propietario (Person) existente en la base de datos.
        /// </summary>
        /// <param name="person">Objeto Person con la información actualizada.</param>
        void Update(Person person);

        /// <summary>
        /// Elimina un propietario (Person) de la base de datos por su identificador.
        /// </summary>
        /// <param name="personId">Identificador único del propietario (Person) que se va a eliminar.</param>
        void Delete(Guid personId);

        /// <summary>
        /// Obtiene todos los propietarios (Person) almacenados en la base de datos.
        /// </summary>
        /// <returns>Una lista de objetos Person.</returns>
        List<Person> GetAll();

        /// <summary>
        /// Obtiene un propietario (Person) específico de la base de datos por su identificador único.
        /// </summary>
        /// <param name="personId">Identificador único del propietario (Person).</param>
        /// <returns>Objeto Person correspondiente al identificador proporcionado, o null si no se encuentra.</returns>
        Person GetById(Guid personId);
    }
}
