using DAO.Contracts;
using DAO.RentsDataSetTableAdapters;
using Domain;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static DAO.RentsDataSet;
using static Domain.Person;

namespace DAO.Implementations
{
    /// <summary>
    /// Implementación del repositorio de personas utilizando el PersonTableAdapter.
    /// </summary>
    public class PersonRepository : IPersonRepository
    {
        private readonly PersonTableAdapter _personTableAdapter;
        private readonly PropertyPersonTableAdapter _propertyPersonTableAdapter;

        /// <summary>
        /// Constructor que inicializa el PersonTableAdapter.
        /// </summary>
        public PersonRepository()
        {
            _personTableAdapter = new PersonTableAdapter();
            _propertyPersonTableAdapter = new PropertyPersonTableAdapter();
        }

        /// <summary>
        /// Obtiene la persona asociada a un ID de usuario.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <returns>Objeto Person si se encuentra, de lo contrario null.</returns>
        public Person GetPersonByUserId(Guid userId)
        {
            var personData = _personTableAdapter.GetData().FirstOrDefault(p => p.IdUser.ToString() == userId.ToString());
            if (personData == null)
                return null;

            return new Person
            {
                IdPerson = personData.IdPerson,
                NamePerson = personData.NamePerson,
                LastNamePerson = personData.LastNamePerson,
                NumberDocumentPerson = personData.NumberDocumentPerson,
                TypeDocumentPerson = personData.TypeDocumentPerson,
                PhoneNumberPerson = int.Parse(personData.PhoneNumberPerson),
                ElectronicDomicilePerson = personData.EmailPerson,
                DomicilePerson = personData.DomicilePerson,
                EnumTypePerson = (Person.PersonTypeEnum)Enum.Parse(typeof(Person.PersonTypeEnum), personData.TypePerson)
            };
        }

        /// <summary>
        /// Inserta una nueva persona en la base de datos y devuelve el ID de la persona creada.
        /// </summary>
        /// <param name="person">La entidad Person que se desea crear.</param>
        /// <param name="userId">El ID del usuario asociado.</param>
        /// <returns>El ID de la persona creada.</returns>
        public Guid Create(Person person, Guid userId)
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person), "La entidad persona no puede ser nula.");
            }

            Guid newId = Guid.NewGuid();

            _personTableAdapter.Insert(
                newId,
                userId.ToString(),
                person.NamePerson,                  // Nombre
                person.LastNamePerson,              // Apellido
                person.NumberDocumentPerson,        // Número de documento
                person.TypeDocumentPerson,          // Tipo de documento
                person.PhoneNumberPerson.ToString(),// Número de teléfono
                person.ElectronicDomicilePerson,    // Correo electrónico
                person.DomicilePerson,              // Domicilio
                person.EnumTypePerson.ToString()   // Tipo de persona
            );

            return newId;
        }


        /// <summary>
        /// Actualiza la información de una persona existente en la base de datos.
        /// </summary>
        /// <param name="person">La entidad Person que se desea actualizar.</param>
        public void Update(Person person)
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person), "La entidad persona no puede ser nula.");
            }

            // Cargar la persona desde el DataSet y actualizar los campos
            var personRow = _personTableAdapter.GetData().FirstOrDefault(r => r.IdPerson == person.IdPerson);

            if (personRow == null)
            {
                throw new KeyNotFoundException($"No se encontró una persona con el ID: {person.IdPerson}");
            }

            personRow.NamePerson = person.NamePerson;
            personRow.LastNamePerson = person.LastNamePerson;
            personRow.NumberDocumentPerson = person.NumberDocumentPerson;
            personRow.TypeDocumentPerson = person.TypeDocumentPerson;
            personRow.DomicilePerson = person.DomicilePerson;
            personRow.EmailPerson = person.ElectronicDomicilePerson;
            personRow.PhoneNumberPerson = person.PhoneNumberPerson.ToString();
            personRow.TypePerson = person.EnumTypePerson.ToString();

            _personTableAdapter.Update(personRow);
        }

        /// <summary>
        /// Elimina una persona de la base de datos usando su identificador único.
        /// </summary>
        /// <param name="personId">El identificador único de la persona.</param>
        public void Delete(Guid personId)
        {
            var personRow = _personTableAdapter.GetData().FirstOrDefault(r => r.IdPerson == personId);

            if (personRow == null)
            {
                throw new KeyNotFoundException($"No se encontró una persona con el ID: {personId}");
            }
            _personTableAdapter.Delete(
                Guid.Parse(personRow.IdPerson.ToString().ToUpper()),
                personRow.IdUser,
                personRow.NamePerson,                  // Nombre
                personRow.LastNamePerson,              // Apellido
                personRow.NumberDocumentPerson,        // Número de documento
                personRow.TypeDocumentPerson,          // Tipo de documento
                personRow.PhoneNumberPerson.ToString(),           // Número de teléfono
                personRow.EmailPerson,    // Correo electrónico (equivalente a EmailPerson)
                personRow.DomicilePerson,              // Domicilio
                personRow.TypePerson.ToString()          // Tipo de persona
            );

            _personTableAdapter.Delete(
                personRow.IdPerson,
                personRow.IdUser,
                personRow.NamePerson,                  // Nombre
                personRow.LastNamePerson,              // Apellido
                personRow.NumberDocumentPerson,        // Número de documento
                personRow.TypeDocumentPerson,          // Tipo de documento
                personRow.PhoneNumberPerson.ToString(),           // Número de teléfono
                personRow.EmailPerson,    // Correo electrónico (equivalente a EmailPerson)
                personRow.DomicilePerson,              // Domicilio
                personRow.TypePerson.ToString()          // Tipo de persona
            );
        }

        /// <summary>
        /// Obtiene todas las personas almacenadas en la base de datos.
        /// </summary>
        /// <returns>Una lista de objetos Person.</returns>
        public List<Person> GetAll()
        {
            var personData = _personTableAdapter.GetData();

            return personData.Select(row => new Person
            {
                IdPerson = row.IdPerson,
                NamePerson = row.NamePerson,
                LastNamePerson = row.LastNamePerson,
                NumberDocumentPerson = row.NumberDocumentPerson,
                TypeDocumentPerson = row.TypeDocumentPerson,
                DomicilePerson = row.DomicilePerson,
                ElectronicDomicilePerson = row.EmailPerson,
                PhoneNumberPerson = int.Parse(row.PhoneNumberPerson),
                EnumTypePerson = (PersonTypeEnum)Enum.Parse(typeof(PersonTypeEnum), row.TypePerson)
            }).ToList();
        }

        /// <summary>
        /// Obtiene una persona específica por su identificador único.
        /// </summary>
        /// <param name="personId">El identificador único de la persona.</param>
        /// <returns>La entidad Person correspondiente o null si no se encuentra.</returns>
        public Person GetById(Guid personId)
        {
            var personRow = _personTableAdapter.GetData().FirstOrDefault(r => r.IdPerson == personId);

            if (personRow == null)
            {
                return null;
            }

            return new Person
            {
                IdPerson = personRow.IdPerson,
                NamePerson = personRow.NamePerson,
                LastNamePerson = personRow.LastNamePerson,
                NumberDocumentPerson = personRow.NumberDocumentPerson,
                TypeDocumentPerson = personRow.TypeDocumentPerson,
                DomicilePerson = personRow.DomicilePerson,
                ElectronicDomicilePerson = personRow.EmailPerson,
                PhoneNumberPerson = int.Parse(personRow.PhoneNumberPerson),
                EnumTypePerson = (PersonTypeEnum)Enum.Parse(typeof(PersonTypeEnum), personRow.TypePerson)
            };
        }

        /// <summary>
        /// Obtiene todas las personas filtradas por su tipo (propietarios o inquilinos).
        /// </summary>
        /// <param name="personType">El tipo de persona (Owner o Tenant).</param>
        /// <returns>Una lista de personas filtradas por tipo.</returns>
        public List<Person> GetAllByType(PersonTypeEnum personType)
        {
            var personData = _personTableAdapter.GetData();

            return personData
                .Where(row => row.TypePerson == personType.ToString())
                .Select(row => new Person
                {
                    IdPerson = row.IdPerson,
                    NamePerson = row.NamePerson,
                    LastNamePerson = row.LastNamePerson,
                    NumberDocumentPerson = row.NumberDocumentPerson,
                    TypeDocumentPerson = row.TypeDocumentPerson,
                    DomicilePerson = row.DomicilePerson,
                    ElectronicDomicilePerson = row.EmailPerson,
                    PhoneNumberPerson = int.Parse(row.PhoneNumberPerson),
                    EnumTypePerson = (PersonTypeEnum)Enum.Parse(typeof(PersonTypeEnum), row.TypePerson)
                }).ToList();
        }

        /// <summary>
        /// Obtiene una persona asociada a una propiedad específica y tipo de persona.
        /// </summary>
        /// <param name="propertyId">El ID de la propiedad.</param>
        /// <param name="personType">El tipo de persona (por ejemplo, Propietario o Inquilino).</param>
        /// <returns>La persona correspondiente a la propiedad y tipo proporcionados.</returns>
        public Person GetPersonByPropertyAndType(Guid propertyId, PersonTypeEnum personType)
        {
            // Busca en PropertyPerson la relación entre la propiedad y la persona
            var propertyPersonRow = _propertyPersonTableAdapter.GetData()
                .FirstOrDefault(r => r.FkIdProperty == propertyId);

            if (propertyPersonRow == null)
            {
                throw new KeyNotFoundException($"No se encontró una persona relacionada para la propiedad con ID: {propertyId}");
            }

            // Obtiene la persona desde la tabla Person en función del ID y del tipo especificado
            var personRow = _personTableAdapter.GetData()
                .FirstOrDefault(p => p.IdPerson == propertyPersonRow.FkIdPerson && p.TypePerson == personType.ToString());

            if (personRow == null)
            {
                throw new KeyNotFoundException($"No se encontró una persona de tipo {personType} para la propiedad con ID: {propertyId}");
            }

            return new Person
            {
                IdPerson = personRow.IdPerson,
                NamePerson = personRow.NamePerson,
                LastNamePerson = personRow.LastNamePerson,
                NumberDocumentPerson = personRow.NumberDocumentPerson,
                TypeDocumentPerson = personRow.TypeDocumentPerson,
                ElectronicDomicilePerson = personRow.EmailPerson,
                DomicilePerson = personRow.DomicilePerson,
                // Cargar otros campos de Person según el modelo
            };
        }

        /// <summary>
        /// Genera un archivo Excel con la lista de personas proporcionada.
        /// </summary>
        /// <param name="filePath">Ruta donde se guardará el archivo Excel.</param>
        /// <param name="persons">Lista de personas a exportar.</param>
        public void ExportPersonsToExcel(string filePath, List<Person> persons)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("La ruta del archivo no puede estar vacía.");

            if (persons == null || !persons.Any())
                throw new ArgumentException("No hay datos de personas para exportar.");

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Reporte de Personas");

                // Encabezados
                worksheet.Cells[1, 1].Value = "Nombre";
                worksheet.Cells[1, 2].Value = "Apellido";
                worksheet.Cells[1, 3].Value = "Domicilio";
                worksheet.Cells[1, 4].Value = "Correo Electrónico";
                worksheet.Cells[1, 5].Value = "Teléfono";
                worksheet.Cells[1, 6].Value = "Tipo de Documento";
                worksheet.Cells[1, 6].Value = "Número de Documento";

                for (int i = 1; i <= 6; i++)
                {
                    worksheet.Cells[1, i].Style.Font.Bold = true;
                    worksheet.Cells[1, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksheet.Cells[1, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                // Datos
                for (int i = 0; i < persons.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = persons[i].NamePerson ?? "Sin Nombre";
                    worksheet.Cells[i + 2, 2].Value = persons[i].LastNamePerson ?? "Sin Apellido";
                    worksheet.Cells[i + 2, 3].Value = persons[i].DomicilePerson ?? "Sin Domicilio";
                    worksheet.Cells[i + 2, 4].Value = persons[i].ElectronicDomicilePerson ?? "Sin Correo Electrónico";
                    worksheet.Cells[i + 2, 5].Value = persons[i].PhoneNumberPerson.ToString() ?? "Sin Teléfono";
                    worksheet.Cells[i + 2, 6].Value = persons[i].TypeDocumentPerson ?? "Sin Tipo de Documento";
                    worksheet.Cells[i + 2, 6].Value = persons[i].NumberDocumentPerson.ToString() ?? "Sin Documento";
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Guardar el archivo
                var fileInfo = new FileInfo(filePath);
                package.SaveAs(fileInfo);
            }
        }
    }
}