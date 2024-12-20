﻿using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Person;

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
        Guid Create(Person person, Guid userId);

        /// <summary>
        /// Obtiene la persona asociada a un ID de usuario.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <returns>Objeto Person si se encuentra, de lo contrario null.</returns>
        Person GetPersonByUserId(Guid userId);

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

        Person GetPersonByPropertyAndType(Guid propertyId, PersonTypeEnum personType);

        void ExportPersonsToExcel(string filePath, List<Person> persons);
    }
}
