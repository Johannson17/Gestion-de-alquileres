using Services.Domain;
using Services.Logic;
using System;
using System.Collections.Generic;

namespace Services.Facade
{
    /// <summary>
    /// Proporciona servicios relacionados con la gestión de usuarios.
    /// </summary>
    public static class UserService
    {
        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="user">El usuario a registrar.</param>
        public static void Register(Usuario user)
        {
            try
            {
                UserLogic.Register(user);
                Console.WriteLine("Usuario registrado con éxito.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar el usuario: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Valida las credenciales de un usuario.
        /// </summary>
        /// <param name="user">El usuario a validar.</param>
        /// <returns>Retorna true si las credenciales son válidas; de lo contrario, false.</returns>
        public static bool Validate(Usuario user)
        {
            try
            {
                return UserLogic.Validate(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al validar el usuario: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un usuario en el sistema.
        /// </summary>
        /// <param name="user">El usuario a actualizar.</param>
        public static void Update(Usuario user)
        {
            try
            {
                UserLogic.Update(user);
                Console.WriteLine("Usuario actualizado con éxito.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el usuario: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Elimina un usuario del sistema.
        /// </summary>
        /// <param name="idUsuario">El identificador del usuario a eliminar.</param>
        public static void Delete(Guid idUsuario)
        {
            try
            {
                UserLogic.Delete(idUsuario);
                Console.WriteLine("Usuario eliminado con éxito.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el usuario: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un usuario por su identificador único.
        /// </summary>
        /// <param name="idUsuario">El identificador único del usuario.</param>
        /// <returns>La instancia del usuario si existe, de lo contrario, null.</returns>
        public static Usuario GetById(Guid idUsuario)
        {
            try
            {
                return UserLogic.GetById(idUsuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el usuario: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Obtiene todos los usuarios del sistema.
        /// </summary>
        /// <returns>Una lista de todas las instancias de Usuario.</returns>
        public static List<Usuario> GetAll()
        {
            try
            {
                return UserLogic.GetAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los usuarios: {ex.Message}");
                throw;
            }
        }
    }
}