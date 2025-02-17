using System;
using System.Collections.Generic;
using System.Data;
using DAO.Contracts;
using Domain;
using DAO.RentsDataSetTableAdapters;
using System.Linq;
using static DAO.RentsDataSet;

namespace DAO.Implementations
{
    /// <summary>
    /// Repositorio para la gestión de solicitudes de mantenimiento en la base de datos.
    /// </summary>
    public class TicketRepository : ITicketRepository
    {
        private readonly MaintenanceRequestTableAdapter _ticketTableAdapter;

        /// <summary>
        /// Constructor que inicializa el table adapter.
        /// </summary>
        public TicketRepository()
        {
            _ticketTableAdapter = new MaintenanceRequestTableAdapter();
        }

        /// <summary>
        /// Crea un nuevo ticket de mantenimiento.
        /// </summary>
        public Guid Create(Ticket ticket)
        {
            ticket.IdRequest = Guid.NewGuid();
            _ticketTableAdapter.Insert(
                ticket.IdRequest,
                ticket.FkIdProperty,
                ticket.FkIdPerson,
                ticket.TitleTicket,
                ticket.DescriptionTicket,
                ticket.StatusTicket,
                ticket.ImageTicket
            );
            return ticket.IdRequest;
        }

        /// <summary>
        /// Obtiene un ticket de mantenimiento por su ID.
        /// </summary>
        public Ticket GetById(Guid ticketId)
        {
            return GetAll().FirstOrDefault(ticket => ticket.IdRequest == ticketId);
        }

        /// <summary>
        /// Obtiene todos los tickets de mantenimiento.
        /// </summary>
        public List<Ticket> GetAll()
        {
            var tickets = new List<Ticket>();
            var dataTable = _ticketTableAdapter.GetData();
            foreach (var row in dataTable)
            {
                tickets.Add(MapRowToTicket(row));
            }
            return tickets;
        }

        /// <summary>
        /// Actualiza un ticket de mantenimiento existente.
        /// </summary>
        public void Update(Ticket ticket)
        {
            // Obtener el DataTable completo
            var dataTable = _ticketTableAdapter.GetData();

            // Buscar la fila que corresponde al ticket específico
            var row = dataTable.FirstOrDefault(ticketRow => ticketRow.IdRequest == ticket.IdRequest);

            if (row == null)
            {
                throw new KeyNotFoundException("No se encontró el ticket con el ID especificado.");
            }

            // Actualizar los campos en la fila
            row.FkIdProperty = ticket.FkIdProperty;
            row.FkIdPerson = ticket.FkIdPerson;
            row.TitleTicket = ticket.TitleTicket;
            row.DescriptionTicket = ticket.DescriptionTicket;
            row.StatusTicket = ticket.StatusTicket;

            // Verificar si la imagen es nula antes de asignarla
            if (ticket.ImageTicket != null)
            {
                row.ImageTicket = ticket.ImageTicket;
            }
            else
            {
                row.SetImageTicketNull();
            }

            // Guardar los cambios en la base de datos
            _ticketTableAdapter.Update(dataTable);
        }

        /// <summary>
        /// Elimina un ticket de mantenimiento por su ID.
        /// </summary>
        public void Delete(Guid ticketId)
        {
            // Obtener el ticket específico por su ID
            var ticket = GetById(ticketId);
            if (ticket == null)
            {
                throw new KeyNotFoundException("No se encontró el ticket con el ID especificado.");
            }

            // Llamar al método Delete con todos los valores originales necesarios
            _ticketTableAdapter.Delete(
                Guid.Parse(ticket.IdRequest.ToString().ToUpper()),
                ticket.FkIdProperty,
                ticket.FkIdPerson,
                ticket.TitleTicket,
                ticket.DescriptionTicket,
                ticket.StatusTicket
            );

            _ticketTableAdapter.Delete(
               ticket.IdRequest,
               ticket.FkIdProperty,
               ticket.FkIdPerson,
               ticket.TitleTicket,
               ticket.DescriptionTicket,
               ticket.StatusTicket
           );
        }

        /// <summary>
        /// Guarda la imagen del ticket en la base de datos.
        /// </summary>
        public void SaveTicketImage(Guid ticketId, byte[] imageData)
        {
            var ticket = GetById(ticketId);
            if (ticket != null)
            {
                ticket.ImageTicket = imageData;
                Update(ticket);
            }
        }

        /// <summary>
        /// Convierte una fila de DataTable en un objeto Ticket.
        /// </summary>
        private Ticket MapRowToTicket(RentsDataSet.MaintenanceRequestRow row)
        {
            return new Ticket
            {
                IdRequest = row.IdRequest,
                FkIdProperty = row.FkIdProperty,
                FkIdPerson = row.FkIdPerson,
                TitleTicket = row.TitleTicket,
                DescriptionTicket = row.DescriptionTicket,
                StatusTicket = row.StatusTicket,
                ImageTicket = row.IsImageTicketNull() ? null : row.ImageTicket
            };
        }
    }
}
