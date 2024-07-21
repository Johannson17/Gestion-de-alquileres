using System;

namespace Dao.Contracts
{
    /// <summary>
    /// Interfaz interna para mapear objetos desde una representación de backend a un objeto específico de la aplicación.
    /// </summary>
    /// <typeparam name="T">Tipo del objeto en la aplicación.</typeparam>
    internal interface IObjectMapper<T>
    {
        /// <summary>
        /// Mapea un arreglo de valores a un objeto específico de tipo T.
        /// </summary>
        /// <param name="values">Array de objetos que representa los datos desde el backend, como valores de una fila de base de datos.</param>
        /// <returns>Una instancia de tipo T adaptada con los datos del backend. Ejemplo: Customer.</returns>
        T Fill(object[] values);
    }
}