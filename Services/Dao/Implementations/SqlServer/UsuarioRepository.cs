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
    /// Repositorio para la gestión de entidades Usuario con operaciones CRUD.
    /// </summary>
    public sealed class UsuarioRepository : IGenericDao<Usuario>
    {
        #region Singleton Pattern
        private static readonly UsuarioRepository _instance = new UsuarioRepository();

        /// <summary>
        /// Acceso a la instancia singleton del repositorio.
        /// </summary>
        public static UsuarioRepository Current => _instance;

        private UsuarioRepository()
        {
            // Aquí se puede implementar la inicialización del singleton si es necesario.
        }
        #endregion

        /// <summary>
        /// Añade un nuevo Usuario al sistema.
        /// </summary>
        /// <param name="obj">La instancia de Usuario a añadir.</param>
        public void Add(Usuario obj)
        {
            SqlHelper.ExecuteNonQuery("UsuarioInsert", CommandType.StoredProcedure,
                new SqlParameter("@IdUsuario", obj.IdUsuario),
                new SqlParameter("@UserName", obj.UserName),
                new SqlParameter("@Password", obj.Password));

            foreach (var acceso in obj.Accesos)
            {
                if (acceso.GetType() == typeof(Patente))
                {
                    SqlHelper.ExecuteNonQuery("Usuario_PatenteInsert", CommandType.StoredProcedure,
                        new SqlParameter("@IdUsuario", obj.IdUsuario),
                        new SqlParameter("@IdPatente", acceso.Id));
                }
                else if (acceso.GetType() == typeof(Familia))
                {
                    SqlHelper.ExecuteNonQuery("Usuario_FamiliaInsert", CommandType.StoredProcedure,
                        new SqlParameter("@IdUsuario", obj.IdUsuario),
                        new SqlParameter("@IdFamilia", acceso.Id));
                }
            }
        }

        /// <summary>
        /// Actualiza un Usuario existente en el sistema.
        /// </summary>
        /// <param name="obj">La instancia de Usuario a actualizar.</param>
        public void Update(Usuario obj)
        {
            SqlHelper.ExecuteNonQuery("UsuarioUpdate", CommandType.StoredProcedure,
                new SqlParameter("@IdUsuario", obj.IdUsuario),
                new SqlParameter("@UserName", obj.UserName),
                new SqlParameter("@Password", obj.Password));
        }

        /// <summary>
        /// Elimina un Usuario del sistema por su identificador único.
        /// </summary>
        /// <param name="id">El identificador único del Usuario a eliminar.</param>
        public void Remove(Guid id)
        {
            SqlHelper.ExecuteNonQuery("UsuarioDelete", CommandType.StoredProcedure,
                new SqlParameter("@IdUsuario", id));
        }

        /// <summary>
        /// Obtiene un Usuario por su identificador único.
        /// </summary>
        /// <param name="id">El identificador único del Usuario.</param>
        /// <returns>La instancia de Usuario si existe, de lo contrario, null.</returns>
        public Usuario GetById(Guid id)
        {
            Usuario usuario = null;

            using (var reader = SqlHelper.ExecuteReader("UsuarioSelect", CommandType.StoredProcedure,
                new SqlParameter("@IdUsuario", id)))
            {
                if (reader.Read())
                {
                    object[] data = new object[reader.FieldCount];
                    reader.GetValues(data);
                    usuario = UsuarioMapper.Current.Fill(data);
                }
            }

            return usuario;
        }

        /// <summary>
        /// Obtiene todos los Usuarios del sistema.
        /// </summary>
        /// <returns>Una lista de todas las instancias de Usuario.</returns>
        public List<Usuario> GetAll()
        {
            List<Usuario> usuarios = new List<Usuario>();

            using (var reader = SqlHelper.ExecuteReader("UsuarioSelectAll", CommandType.StoredProcedure))
            {
                while (reader.Read())
                {
                    object[] data = new object[reader.FieldCount];
                    reader.GetValues(data);
                    usuarios.Add(UsuarioMapper.Current.Fill(data));
                }
            }

            return usuarios;
        }

        /// <summary>
        /// Obtiene un usuario por su nombre de usuario.
        /// </summary>
        /// <param name="userName">El nombre de usuario a buscar.</param>
        /// <returns>La instancia del usuario si existe, de lo contrario, null.</returns>
        public Usuario GetByUserName(string userName)
        {
            Usuario user = null;

            string commandText = "SELECT IdUsuario, UserName, Password FROM dbo.Usuario WHERE UserName = @UserName";

            string trimmedUserName = userName.Trim(); // elimina espacios en blanco

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserName", trimmedUserName)
            };

            using (var reader = SqlHelper.ExecuteReader(commandText, System.Data.CommandType.Text, parameters))
            {
                if (reader.Read())
                {
                    // Verificar si el IdUsuario es DBNull y lanzar una excepción si es nulo
                    if (reader.IsDBNull(0))
                    {
                        throw new InvalidCastException("El campo 'IdUsuario' no puede ser nulo.");
                    }

                    // Verificar si el UserName es DBNull y lanzar una excepción si es nulo
                    if (reader.IsDBNull(1))
                    {
                        throw new InvalidCastException("El campo 'UserName' no puede ser nulo.");
                    }

                    // Verificar si la contraseña es DBNull y lanzar una excepción si es nulo
                    if (reader.IsDBNull(2))
                    {
                        throw new InvalidCastException("El campo 'Password' no puede ser nulo.");
                    }

                    // Si todo está bien, realizar las conversiones
                    user = new Usuario
                    {
                        IdUsuario = reader.GetGuid(0),        // Suponiendo que la columna es de tipo GUID
                        UserName = reader.GetString(1),       // Suponiendo que es de tipo string
                        Password = reader.GetString(2)        // Suponiendo que es de tipo string
                    };
                }
                else
                {
                    throw new Exception("No se encontró ningún registro para el nombre de usuario proporcionado.");
                }
            }

            return user;
        }
    }
}