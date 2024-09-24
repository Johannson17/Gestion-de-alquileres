using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Person
    {
        public Guid IdPerson { get; set; }
        public string NamePerson { get; set; }
        public string LastNamePerson { get; set; }
        public int NumberDocumentPerson { get; set; }
        public string TypeDocumentPerson { get; set; }
        public string PhoneNumberPerson { get; set; }
        public string EmailPerson { get; set; }
    }
}
