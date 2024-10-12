using Services.Dao.Contracts;
using Services.Dao.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Services.Dao.Implementations.SqlServer
{
    /// <summary>
    /// Repositorio para manejar las relaciones entre Familia y Patente.
    /// </summary>
    public sealed class FamiliaPatenteRepository : IJoinRepository<Familia>
    {
        #region Singleton Pattern
        private static readonly FamiliaPatenteRepository _instance = new FamiliaPatenteRepository();

        /// <summary>
        /// Acceso a la instancia singleton del repositorio.
        /// </summary>
        public static FamiliaPatenteRepository Current => _instance;

        private FamiliaPatenteRepository()
        {
            // Aquí se puede implementar la inicialización del singleton si es necesario.
        }
        #endregion

        /// <summary>
        /// Añade una relación entre Familia y Patente.
        /// </summary>
        /// <param name="familia">La instancia de Familia.</param>
        /// <param name="patente">La instancia de Patente.</param>
        public void Add(Familia familia, Patente patente)
        {
            SqlHelper.ExecuteNonQuery("Familia_PatenteInsert", CommandType.StoredProcedure,
                new SqlParameter("@IdFamilia", familia.Id),
                new SqlParameter("@IdPatente", patente.Id));
        }

        /// <summary>
        /// Elimina una relación entre Familia y Patente.
        /// </summary>
        /// <param name="familia">La instancia de Familia.</param>
        /// <param name="patente">La instancia de Patente.</param>
        public void Remove(Familia familia, Patente patente)
        {
            SqlHelper.ExecuteNonQuery("Familia_PatenteDelete", CommandType.StoredProcedure,
                new SqlParameter("@IdFamilia", familia.Id),
                new SqlParameter("@IdPatente", patente.Id));
        }

        /// <summary>
        /// Elimina todas las relaciones de una Familia.
        /// </summary>
        /// <param name="familia">La instancia de Familia.</param>
        public void RemoveByFamilia(Familia familia)
        {
            SqlHelper.ExecuteNonQuery("Familia_PatenteDeleteByIdFamilia", CommandType.StoredProcedure,
                new SqlParameter("@IdFamilia", familia.Id));
        }

        /// <summary>
        /// Elimina todas las relaciones de una Patente.
        /// </summary>
        /// <param name="patente">La instancia de Patente.</param>
        public void RemoveByPatente(Patente patente)
        {
            SqlHelper.ExecuteNonQuery("Familia_PatenteDeleteByIdPatente", CommandType.StoredProcedure,
                new SqlParameter("@IdPatente", patente.Id));
        }

        /// <summary>
        /// Obtiene todas las Patentes asociadas a una Familia.
        /// </summary>
        /// <param name="familia">La instancia de Familia.</param>
        /// <returns>Una lista de Patentes asociadas a la Familia.</returns>
        public List<Patente> GetPatentesByFamilia(Familia familia)
        {
            List<Patente> patentes = new List<Patente>();

            using (var reader = SqlHelper.ExecuteReader("Familia_PatenteSelectByIdFamilia", CommandType.StoredProcedure,
                new SqlParameter("@IdFamilia", familia.Id)))
            {
                while (reader.Read())
                {
                    object[] data = new object[reader.FieldCount];
                    reader.GetValues(data);
                    patentes.Add(PatenteRepository.Current.GetById(Guid.Parse(data[1].ToString())));
                }
            }

            return patentes;
        }

        /// <summary>
        /// Asocia las Patentes correspondientes a una Familia.
        /// </summary>
        /// <param name="entity">La instancia de Familia.</param>
        public void Join(Familia entity)
        {
            entity.Accesos.AddRange(GetPatentesByFamilia(entity));
        }
    }
}