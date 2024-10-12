using System;

namespace Domain
{
    public class Person
    {
        /// <summary>
        /// Identificador único de la persona (GUID).
        /// </summary>
        public Guid IdPerson { get; set; }

        /// <summary>
        /// Nombre de la persona.
        /// </summary>
        public string NamePerson { get; set; }

        /// <summary>
        /// Apellido de la persona.
        /// </summary>
        public string LastNamePerson { get; set; }

        /// <summary>
        /// Número del documento de la persona.
        /// </summary>
        public int NumberDocumentPerson { get; set; }

        /// <summary>
        /// Tipo de documento de la persona (por ejemplo, DNI, pasaporte).
        /// </summary>
        public string TypeDocumentPerson { get; set; }

        /// <summary>
        /// Domicilio físico de la persona.
        /// </summary>
        public string DomicilePerson { get; set; }

        /// <summary>
        /// Domicilio electrónico de la persona (por ejemplo, email).
        /// </summary>
        public string ElectronicDomicilePerson { get; set; }

        /// <summary>
        /// Número de teléfono de la persona.
        /// </summary>
        public int PhoneNumberPerson { get; set; }

        /// <summary>
        /// Tipo de persona como enumerador.
        /// </summary>
        public PersonTypeEnum EnumTypePerson { get; set; }

        /// <summary>
        /// Enumerador que define los tipos de personas posibles.
        /// </summary>
        public enum PersonTypeEnum
        {
            Owner,
            Tenant
        }
    }
}