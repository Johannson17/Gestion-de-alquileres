using Services.Dao.Contracts;
using Services.Dao.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Services.Dao.Implementations.SqlServer
{
    /// <summary>
    /// Repositorio para manejar las relaciones entre Usuario y Patente.
    /// </summary>
    public sealed class UsuarioPatenteRepository
    {
        #region Singleton Pattern
        private static readonly UsuarioPatenteRepository _instance = new UsuarioPatenteRepository();

        /// <summary>
        /// Acceso a la instancia singleton del repositorio.
        /// </summary>
        public static UsuarioPatenteRepository Current => _instance;

        private UsuarioPatenteRepository()
        {
            // Aquí se puede implementar la inicialización del singleton si es necesario.
        }
        #endregion

        /// <summary>
        /// Añade una relación entre Usuario y Patente.
        /// </summary>
        /// <param name="usuario">La instancia de Usuario.</param>
        /// <param name="patente">La instancia de Patente.</param>
        public void Add(Usuario usuario, Patente patente)
        {
            SqlHelper.ExecuteNonQuery("Usuario_PatenteInsert", CommandType.StoredProcedure,
                new SqlParameter("@IdUsuario", usuario.IdUsuario),
                new SqlParameter("@IdPatente", patente.Id));
        }

        /// <summary>
        /// Elimina una relación entre Usuario y Patente.
        /// </summary>
        /// <param name="usuario">La instancia de Usuario.</param>
        /// <param name="patente">La instancia de Patente.</param>
        public void Remove(Usuario usuario, Patente patente)
        {
            SqlHelper.ExecuteNonQuery("Usuario_PatenteDelete", CommandType.StoredProcedure,
                new SqlParameter("@IdUsuario", usuario.IdUsuario),
                new SqlParameter("@IdPatente", patente.Id));
        }

        /// <summary>
        /// Elimina todas las relaciones de un Usuario.
        /// </summary>
        /// <param name="usuario">La instancia de Usuario.</param>
        public void RemoveByUsuario(Usuario usuario)
        {
            SqlHelper.ExecuteNonQuery("Usuario_PatenteDeleteByIdUsuario", CommandType.StoredProcedure,
                new SqlParameter("@IdUsuario", usuario.IdUsuario));
        }

        /// <summary>
        /// Elimina todas las relaciones de una Patente.
        /// </summary>
        /// <param name="patente">La instancia de Patente.</param>
        public void RemoveByPatente(Patente patente)
        {
            SqlHelper.ExecuteNonQuery("Usuario_PatenteDeleteByIdPatente", CommandType.StoredProcedure,
                new SqlParameter("@IdPatente", patente.Id));
        }

        /// <summary>
        /// Obtiene todas las Patentes asociadas a un Usuario.
        /// </summary>
        /// <param name="usuario">La instancia de Usuario.</param>
        /// <returns>Una lista de Patentes asociadas al Usuario.</returns>
        public List<Patente> GetPatentesByUsuario(Usuario usuario)
        {
            List<Patente> patentes = new List<Patente>();

            using (var reader = SqlHelper.ExecuteReader("Usuario_PatenteSelectByIdUsuario", CommandType.StoredProcedure,
                new SqlParameter("@IdUsuario", usuario.IdUsuario)))
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
        /// Asocia las Patentes correspondientes a un Usuario.
        /// </summary>
        /// <param name="usuario">La instancia de Usuario.</param>
        public void Join(Usuario usuario)
        {
            usuario.Accesos.AddRange(GetPatentesByUsuario(usuario));
        }
    }
}