using System;

namespace Domain
{
    public class Ticket
    {
        public Guid IdRequest { get; set; } // Identificador único del ticket
        public Guid FkIdProperty { get; set; } // Identificador de la propiedad asociada
        public Property Property { get; set; }
        public Guid FkIdPerson { get; set; } // Identificador de la persona que creó el ticket
        public string TitleTicket { get; set; } // Título del ticket
        public string DescriptionTicket { get; set; } // Descripción detallada del ticket
        public string StatusTicket { get; set; } // Estado del ticket (e.g., "Abierto", "En proceso", "Cerrado")
        public byte[] ImageTicket { get; set; } // Imagen del ticket, almacenada como un arreglo de bytes
    }
}
