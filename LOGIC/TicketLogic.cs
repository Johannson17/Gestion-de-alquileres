using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using DAO.Contracts; // Asegúrate de tener el namespace correcto para el repositorio
using DAO.Implementations;
using DAO.Implementations.SqlServer; // Importa la implementación de repositorio si es necesario
using Domain; // Asegúrate de que tienes el namespace correcto para el modelo Ticket

namespace LOGIC
{
    public class TicketLogic
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly PropertyLogic _propertyLogic;

        /// <summary>
        /// Constructor que inicializa el repositorio de tickets.
        /// </summary>
        public TicketLogic()
        {
            _ticketRepository = new TicketRepository();
            _propertyLogic = new PropertyLogic();
        }

        /// <summary>
        /// Agrega un nuevo ticket al sistema.
        /// </summary>
        /// <param name="ticket">El ticket a agregar.</param>
        /// <returns>El ID del ticket recién creado.</returns>
        public Guid AddTicket(Ticket ticket)
        {
            ticket.IdRequest = Guid.NewGuid(); // Genera un nuevo ID para el ticket
            return _ticketRepository.Create(ticket);
        }

        /// <summary>
        /// Obtiene un ticket por su ID.
        /// </summary>
        /// <param name="ticketId">El ID del ticket.</param>
        /// <returns>El ticket correspondiente.</returns>
        public Ticket GetTicketById(Guid ticketId)
        {
            return _ticketRepository.GetById(ticketId);
        }

        /// <summary>
        /// Obtiene todos los tickets en el sistema.
        /// </summary>
        /// <returns>Lista de tickets.</returns>
        public List<Ticket> GetAllTickets()
        {
            // Obtener todos los tickets desde el repositorio
            var tickets = _ticketRepository.GetAll();

            // Obtener todas las propiedades desde la lógica de propiedades
            var properties = _propertyLogic.GetAllProperties();

            // Realizar el join para enriquecer cada ticket con su propiedad asociada
            var enrichedTickets = tickets.Select(ticket =>
            {
                var associatedProperty = properties.FirstOrDefault(prop => prop.IdProperty == ticket.FkIdProperty);
                ticket.Property = associatedProperty; // Asignar la propiedad asociada al ticket
                return ticket;
            }).ToList();

            return enrichedTickets;
        }

        /// <summary>
        /// Actualiza la información de un ticket existente.
        /// </summary>
        /// <param name="ticket">El ticket con la información actualizada.</param>
        public void UpdateTicket(Ticket ticket)
        {
            _ticketRepository.Update(ticket);
        }

        /// <summary>
        /// Elimina un ticket del sistema.
        /// </summary>
        /// <param name="ticketId">El ID del ticket a eliminar.</param>
        public void DeleteTicket(Guid ticketId)
        {
            _ticketRepository.Delete(ticketId);
        }

        /// <summary>
        /// Guarda una imagen asociada a un ticket en la base de datos.
        /// </summary>
        /// <param name="ticketId">El ID del ticket.</param>
        /// <param name="imageData">Los datos de la imagen en formato byte array.</param>
        public void SaveTicketImage(Guid ticketId, byte[] imageData)
        {
            var ticket = _ticketRepository.GetById(ticketId);
            if (ticket == null)
            {
                throw new KeyNotFoundException("No se encontró un ticket con el ID especificado.");
            }

            ticket.ImageTicket = imageData;
            _ticketRepository.Update(ticket);
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

            // Obtener el ticket existente desde el repositorio
            var ticket = _ticketRepository.GetById(ticketId);
            if (ticket == null)
            {
                throw new KeyNotFoundException($"No se encontró un ticket con el ID: {ticketId}");
            }

            // Actualizar el estado del ticket
            ticket.StatusTicket = newStatus;

            // Guardar los cambios en la base de datos
            _ticketRepository.Update(ticket);
        }
    }
}
