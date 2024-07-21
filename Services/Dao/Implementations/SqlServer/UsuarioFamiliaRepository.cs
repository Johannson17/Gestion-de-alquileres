using Services.Dao.Contracts;
using Services.Dao.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Services.Dao.Implementations.SqlServer
{
    /// <summary>
    /// Repositorio para manejar las relaciones entre Usuario y Familia.
    /// </summary>
    public sealed class UsuarioFamiliaRepository
    {
        #region Singleton Pattern
        private static readonly UsuarioFamiliaRepository _instance = new UsuarioFamiliaRepository();

        /// <summary>
        /// Acceso a la instancia singleton del repositorio.
        /// </summary>
        public static UsuarioFamiliaRepository Current => _instance;

        private UsuarioFamiliaRepository()
        {
            // Aquí se puede implementar la inicialización del singleton si es necesario.
        }
        #endregion

        /// <summary>
        /// Añade una relación entre Usuario y Familia.
        /// </summary>
        /// <param name="usuario">La instancia de Usuario.</param>
        /// <param name="familia">La instancia de Familia.</param>
        public void Add(Usuario usuario, Familia familia)
        {
            SqlHelper.ExecuteNonQuery("Usuario_FamiliaInsert", CommandType.StoredProcedure,
                new SqlParameter("@IdUsuario", usuario.IdUsuario),
                new SqlParameter("@IdFamilia", familia.Id));
        }

        /// <summary>
        /// Elimina una relación entre Usuario y Familia.
        /// </summary>
        /// <param name="usuario">La instancia de Usuario.</param>
        /// <param name="familia">La instancia de Familia.</param>
        public void Remove(Usuario usuario, Familia familia)
        {
            SqlHelper.ExecuteNonQuery("Usuario_FamiliaDelete", CommandType.StoredProcedure,
                new SqlParameter("@IdUsuario", usuario.IdUsuario),
                new SqlParameter("@IdFamilia", familia.Id));
        }

        /// <summary>
        /// Elimina todas las relaciones de un Usuario.
        /// </summary>
        /// <param name="usuario">La instancia de Usuario.</param>
        public void RemoveByUsuario(Usuario usuario)
        {
            SqlHelper.ExecuteNonQuery("Usuario_FamiliaDeleteByIdUsuario", CommandType.StoredProcedure,
                new SqlParameter("@IdUsuario", usuario.IdUsuario));
        }

        /// <summary>
        /// Elimina todas las relaciones de una Familia.
        /// </summary>
        /// <param name="familia">La instancia de Familia.</param>
        public void RemoveByFamilia(Familia familia)
        {
            SqlHelper.ExecuteNonQuery("Usuario_FamiliaDeleteByIdFamilia", CommandType.StoredProcedure,
                new SqlParameter("@IdFamilia", familia.Id));
        }

        /// <summary>
        /// Obtiene todas las Familias asociadas a un Usuario.
        /// </summary>
        /// <param name="usuario">La instancia de Usuario.</param>
        /// <returns>Una lista de Familias asociadas al Usuario.</returns>
        public List<Familia> GetFamiliasByUsuario(Usuario usuario)
        {
            List<Familia> familias = new List<Familia>();

            using (var reader = SqlHelper.ExecuteReader("Usuario_FamiliaSelectByIdUsuario", CommandType.StoredProcedure,
                new SqlParameter("@IdUsuario", usuario.IdUsuario)))
            {
                while (reader.Read())
                {
                    object[] data = new object[reader.FieldCount];
                    reader.GetValues(data);
                    familias.Add(FamiliaRepository.Current.GetById(Guid.Parse(data[1].ToString())));
                }
            }

            return familias;
        }

        /// <summary>
        /// Asocia las Familias correspondientes a un Usuario.
        /// </summary>
        /// <param name="usuario">La instancia de Usuario.</param>
        public void Join(Usuario usuario)
        {
            usuario.Accesos.AddRange(GetFamiliasByUsuario(usuario));
        }
    }
}