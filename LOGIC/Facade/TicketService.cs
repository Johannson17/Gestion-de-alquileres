using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using Domain; // Asegúrate de tener el namespace correcto para los modelos de dominio
using LOGIC; // Importa la lógica de negocios y repositorios necesarios

namespace LOGIC.Facade
{
    /// <summary>
    /// Servicio de fachada para la gestión de tickets, proporcionando una capa de abstracción entre la UI y la lógica de negocio.
    /// </summary>
    public class TicketService
    {
        private readonly TicketLogic _ticketLogic;

        /// <summary>
        /// Constructor que inicializa la lógica de tickets.
        /// </summary>
        public TicketService()
        {
            _ticketLogic = new TicketLogic(); // Asegúrate de que TicketLogic esté correctamente implementado
        }

        /// <summary>
        /// Crea un nuevo ticket en el sistema.
        /// </summary>
        /// <param name="ticket">Objeto Ticket que representa el nuevo ticket.</param>
        /// <returns>Devuelve el ID del ticket recién creado.</returns>
        public Guid CreateTicket(Ticket ticket)
        {
            return _ticketLogic.AddTicket(ticket);
        }

        /// <summary>
        /// Obtiene un ticket por su ID.
        /// </summary>
        /// <param name="ticketId">El ID del ticket a obtener.</param>
        /// <returns>El objeto Ticket correspondiente al ID proporcionado.</returns>
        public Ticket GetTicketById(Guid ticketId)
        {
            return _ticketLogic.GetTicketById(ticketId);
        }

        /// <summary>
        /// Obtiene todos los tickets en el sistema.
        /// </summary>
        /// <returns>Lista de todos los tickets.</returns>
        public List<Ticket> GetAllTickets()
        {
            return _ticketLogic.GetAllTickets();
        }

        /// <summary>
        /// Actualiza un ticket existente.
        /// </summary>
        /// <param name="ticket">El ticket a actualizar.</param>
        public void UpdateTicket(Ticket ticket)
        {
            _ticketLogic.UpdateTicket(ticket);
        }

        /// <summary>
        /// Elimina un ticket por su ID.
        /// </summary>
        /// <param name="ticketId">El ID del ticket a eliminar.</param>
        public void DeleteTicket(Guid ticketId)
        {
            _ticketLogic.DeleteTicket(ticketId);
        }

        /// <summary>
        /// Guarda una imagen asociada a un ticket.
        /// </summary>
        /// <param name="ticketId">El ID del ticket.</param>
        /// <param name="imagePath">La ruta de la imagen a subir.</param>
        public void SaveTicketImage(Guid ticketId, string imagePath)
        {
            if (File.Exists(imagePath))
            {
                byte[] imageData = File.ReadAllBytes(imagePath);
                _ticketLogic.SaveTicketImage(ticketId, imageData);
            }
            else
            {
                throw new FileNotFoundException("La imagen no fue encontrada en la ruta especificada.");
            }
        }

        /// <summary>
        /// Actualiza el estado de un ticket existente.
        /// </summary>
        /// <param name="ticketId">El ID del ticket a actualizar.</param>
        /// <param name="newStatus">El nuevo estado del ticket.</param>
        public void UpdateTicketStatus(Guid ticketId, string newStatus)
        {
            if (string.IsNullOrWhiteSpace(newStatus))
            {
                throw new ArgumentException("El estado no puede estar vacío.", nameof(newStatus));
            }

            try
            {
                _ticketLogic.UpdateTicketStatus(ticketId, newStatus);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar el estado del ticket con ID {ticketId}: {ex.Message}", ex);
            }
        }
    }
}
