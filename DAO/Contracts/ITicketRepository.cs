using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace DAO.Contracts
{
    /// <summary>
    /// Interfaz para el repositorio de tickets que define las operaciones CRUD.
    /// </summary>
    public interface ITicketRepository
    {
        /// <summary>
        /// Crea un nuevo ticket en la base de datos.
        /// </summary>
        /// <param name="ticket">El ticket a crear.</param>
        /// <returns>El ID del ticket recién creado.</returns>
        Guid Create(Ticket ticket);

        /// <summary>
        /// Obtiene un ticket específico por su ID.
        /// </summary>
        /// <param name="ticketId">El ID del ticket a obtener.</param>
        /// <returns>El ticket correspondiente al ID proporcionado.</returns>
        Ticket GetById(Guid ticketId);

        /// <summary>
        /// Obtiene todos los tickets almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de todos los tickets.</returns>
        List<Ticket> GetAll();

        /// <summary>
        /// Actualiza un ticket existente en la base de datos.
        /// </summary>
        /// <param name="ticket">El ticket con la información actualizada.</param>
        void Update(Ticket ticket);

        /// <summary>
        /// Elimina un ticket específico de la base de datos.
        /// </summary>
        /// <param name="ticketId">El ID del ticket a eliminar.</param>
        void Delete(Guid ticketId);

        /// <summary>
        /// Guarda una imagen asociada a un ticket en la base de datos.
        /// </summary>
        /// <param name="ticketId">El ID del ticket.</param>
        /// <param name="imageData">Los datos de la imagen en formato byte array.</param>
        void SaveTicketImage(Guid ticketId, byte[] imageData);
    }
}
