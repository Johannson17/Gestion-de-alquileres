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
    /// Repositorio para la gestión de entidades Patente con operaciones CRUD.
    /// </summary>
    public sealed class PatenteRepository : IGenericDao<Patente>
    {
        #region Singleton Pattern
        private static readonly PatenteRepository _instance = new PatenteRepository();

        /// <summary>
        /// Acceso a la instancia singleton del repositorio.
        /// </summary>
        public static PatenteRepository Current => _instance;

        private PatenteRepository()
        {
            // Aquí se puede implementar la inicialización del singleton si es necesario.
        }
        #endregion

        /// <summary>
        /// Añade una nueva Patente al sistema.
        /// </summary>
        /// <param name="obj">La instancia de Patente a añadir.</param>
        public void Add(Patente obj)
        {
            // Generar un nuevo GUID para la relación si no existe
            obj.Id = Guid.NewGuid();

            SqlHelper.ExecuteNonQuery("PatenteInsert", CommandType.StoredProcedure,
                new SqlParameter("@IdPatente", obj.Id),
                new SqlParameter("@Nombre", obj.Nombre),
                new SqlParameter("@DataKey", obj.DataKey),
                new SqlParameter("@TipoAcceso", (int)obj.TipoAcceso));
        }

        /// <summary>
        /// Actualiza una Patente existente en el sistema.
        /// </summary>
        /// <param name="obj">La instancia de Patente a actualizar.</param>
        public void Update(Patente obj)
        {
            SqlHelper.ExecuteNonQuery("PatenteUpdate", CommandType.StoredProcedure,
                new SqlParameter("@IdPatente", obj.Id),
                new SqlParameter("@Nombre", obj.Nombre),
                new SqlParameter("@DataKey", obj.DataKey),
                new SqlParameter("@TipoAcceso", (int)obj.TipoAcceso));
        }

        /// <summary>
        /// Elimina una Patente del sistema por su identificador único.
        /// </summary>
        /// <param name="id">El identificador único de la Patente a eliminar.</param>
        public void Remove(Guid id)
        {
            SqlHelper.ExecuteNonQuery("PatenteDelete", CommandType.StoredProcedure,
                new SqlParameter("@IdPatente", id));
        }

        /// <summary>
        /// Obtiene una Patente por su identificador único.
        /// </summary>
        /// <param name="id">El identificador único de la Patente.</param>
        /// <returns>La instancia de Patente si existe, de lo contrario, null.</returns>
        public Patente GetById(Guid id)
        {
            Patente patente = null;

            using (var reader = SqlHelper.ExecuteReader("PatenteSelect", CommandType.StoredProcedure,
                new SqlParameter("@IdPatente", id)))
            {
                if (reader.Read())
                {
                    object[] data = new object[reader.FieldCount];
                    reader.GetValues(data);
                    patente = PatenteMapper.Current.Fill(data);
                }
            }

            return patente;
        }

        /// <summary>
        /// Obtiene todas las Patentes del sistema.
        /// </summary>
        /// <returns>Una lista de todas las instancias de Patente.</returns>
        public List<Patente> GetAll()
        {
            List<Patente> patentes = new List<Patente>();

            using (var reader = SqlHelper.ExecuteReader("PatenteSelectAll", CommandType.StoredProcedure))
            {
                while (reader.Read())
                {
                    object[] data = new object[reader.FieldCount];
                    reader.GetValues(data);
                    patentes.Add(PatenteMapper.Current.Fill(data));
                }
            }

            return patentes;
        }
    }
}