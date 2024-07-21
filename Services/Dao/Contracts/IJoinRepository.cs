using System;

namespace Services.Dao.Contracts
{
    /// <summary>
    /// Interfaz interna para realizar operaciones de unión con entidades genéricas.
    /// </summary>
    /// <typeparam name="T">Tipo de la entidad con la cual se realizará la unión.</typeparam>
    internal interface IJoinRepository<T>
    {
        /// <summary>
        /// Realiza una operación de unión con la entidad especificada.
        /// </summary>
        /// <param name="entity">La entidad con la que se realizará la unión.</param>
        void Join(T entity);
    }
}
