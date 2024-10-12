using System;
using System.Collections.Generic;

namespace Dao.Contracts
{
    /// <summary>
    /// Interfaz genérica para operaciones CRUD en cualquier entidad.
    /// </summary>
    /// <typeparam name="T">Tipo de la entidad sobre la cual se realizarán las operaciones CRUD.</typeparam>
    public interface IGenericDao<T>
    {
        /// <summary>
        /// Añade una nueva entidad al repositorio.
        /// </summary>
        /// <param name="obj">La entidad a añadir.</param>
        void Add(T obj);

        /// <summary>
        /// Actualiza una entidad existente en el repositorio.
        /// </summary>
        /// <param name="obj">La entidad con los datos actualizados.</param>
        void Update(T obj);

        /// <summary>
        /// Elimina una entidad del repositorio basado en su identificador único.
        /// </summary>
        /// <param name="id">El identificador único de la entidad a eliminar.</param>
        void Remove(Guid id);

        /// <summary>
        /// Obtiene una entidad por su identificador único.
        /// </summary>
        /// <param name="id">El identificador único de la entidad deseada.</param>
        /// <returns>La entidad solicitada.</returns>
        T GetById(Guid id);

        /// <summary>
        /// Obtiene todas las entidades del repositorio.
        /// </summary>
        /// <returns>Una lista de todas las entidades.</returns>
        List<T> GetAll();
    }
}