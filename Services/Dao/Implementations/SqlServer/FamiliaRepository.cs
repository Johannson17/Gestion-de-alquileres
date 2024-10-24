using Dao.Contracts;
using Services.Dao.Helpers;
using Services.Dao.Implementations.SqlServer.Mappers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Services.Dao.Implementations.SqlServer
{
    /// <summary>
    /// Repositorio para la gestión de entidades Familia con operaciones CRUD.
    /// </summary>
    public sealed class FamiliaRepository : IGenericDao<Familia>
    {
        #region Singleton Pattern
        private static readonly FamiliaRepository _instance = new FamiliaRepository();

        /// <summary>
        /// Acceso a la instancia singleton del repositorio.
        /// </summary>
        public static FamiliaRepository Current => _instance;

        private FamiliaRepository()
        {
            // Aquí se puede implementar la inicialización del singleton si es necesario.
        }
        #endregion

        /// <summary>
        /// Añade una nueva Familia al sistema.
        /// </summary>
        /// <param name="obj">La instancia de Familia a añadir.</param>
        public void Add(Familia obj)
        {
            // Generar un nuevo GUID para la relación si no existe
            obj.Id = Guid.NewGuid();

            SqlHelper.ExecuteNonQuery("FamiliaInsert", CommandType.StoredProcedure,
                new SqlParameter("@IdFamilia", obj.Id),
                new SqlParameter("@Nombre", obj.Nombre));
        }

        /// <summary>
        /// Actualiza una Familia existente en el sistema.
        /// </summary>
        /// <param name="obj">La instancia de Familia a actualizar.</param>
        public void Update(Familia obj)
        {
            SqlHelper.ExecuteNonQuery("FamiliaUpdate", CommandType.StoredProcedure,
                new SqlParameter("@IdFamilia", obj.Id),
                new SqlParameter("@Nombre", obj.Nombre));
        }

        /// <summary>
        /// Elimina una Familia del sistema por su identificador único.
        /// </summary>
        /// <param name="id">El identificador único de la Familia a eliminar.</param>
        public void Remove(Guid id)
        {
            SqlHelper.ExecuteNonQuery("FamiliaDelete", CommandType.StoredProcedure,
                new SqlParameter("@IdFamilia", id));
        }

        /// <summary>
        /// Obtiene una Familia por su identificador único.
        /// </summary>
        /// <param name="id">El identificador único de la Familia.</param>
        /// <returns>La instancia de Familia si existe, de lo contrario, null.</returns>
        public Familia GetById(Guid id)
        {
            Familia familia = null;

            using (var reader = SqlHelper.ExecuteReader("FamiliaSelect", CommandType.StoredProcedure,
                new SqlParameter("@IdFamilia", id)))
            {
                if (reader.Read())
                {
                    object[] data = new object[reader.FieldCount];
                    reader.GetValues(data);
                    familia = FamiliaMapper.Current.Fill(data);
                }
            }

            return familia;
        }

        /// <summary>
        /// Obtiene todas las Familias del sistema.
        /// </summary>
        /// <returns>Una lista de todas las instancias de Familia.</returns>
        public List<Familia> GetAll()
        {
            List<Familia> familias = new List<Familia>();

            using (var reader = SqlHelper.ExecuteReader("FamiliaSelectAll", CommandType.StoredProcedure))
            {
                while (reader.Read())
                {
                    object[] data = new object[reader.FieldCount];
                    reader.GetValues(data);
                    familias.Add(FamiliaMapper.Current.Fill(data));
                }
            }

            return familias;
        }

        /// <summary>
        /// Elimina todas las relaciones de una familia con otras familias.
        /// </summary>
        /// <param name="familia">La familia cuyas relaciones se eliminarán.</param>
        public void RemoveRelacionesFamilia(Familia familia)
        {
            SqlHelper.ExecuteNonQuery("FamiliaFamiliaDeleteByIdFamilia", CommandType.StoredProcedure,
                new SqlParameter("@IdFamilia", familia.Id));
        }

        /// <summary>
        /// Añade una relación entre una familia y otra familia.
        /// </summary>
        /// <param name="familia">La familia principal.</param>
        /// <param name="subFamilia">La familia relacionada a añadir.</param>
        public void AddRelacionFamilia(Familia familia, Familia subFamilia)
        {
            SqlHelper.ExecuteNonQuery("FamiliaFamiliaInsert", CommandType.StoredProcedure,
                new SqlParameter("@IdFamilia", familia.Id),
                new SqlParameter("@IdSubFamilia", subFamilia.Id));
        }
    }
}