using Services.Factory;
using Services.Domain;
using System;
using System.Collections.Generic;
using Services.Dao.Implementations.SqlServer;

namespace Services.Logic
{
    /// <summary>
    /// Lógica de negocio para la gestión de usuarios.
    /// </summary>
    internal static class UserLogic
    {
        /// <summary>
        /// Registra un nuevo usuario en el sistema, asignándole accesos existentes (Patentes y Familias).
        /// </summary>
        /// <param name="user">El usuario a registrar.</param>
        public static void Register(Usuario user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "El usuario no puede ser nulo.");
            }

            var usuarioRepository = FactoryDao.CreateRepository<UsuarioRepository>();
            var patenteRepository = FactoryDao.CreateRepository<PatenteRepository>();
            var familiaRepository = FactoryDao.CreateRepository<FamiliaRepository>();
            var usuarioPatenteRepository = FactoryDao.CreateRepository<UsuarioPatenteRepository>();
            var usuarioFamiliaRepository = FactoryDao.CreateRepository<UsuarioFamiliaRepository>();

            // Guardar el usuario en la base de datos
            usuarioRepository.Add(user);

            // Asignar Patentes y Familias existentes al usuario
            foreach (var acceso in user.Accesos)
            {
                if (acceso is Patente patente)
                {
                    // Verificar si la patente existe antes de asignarla
                    Patente existingPatente = patenteRepository.GetById(patente.Id);
                    if (existingPatente != null)
                    {
                        usuarioPatenteRepository.Add(user, patente);
                    }
                    else
                    {
                        throw new ArgumentException($"La patente con ID {patente.Id} no existe.", nameof(user.Accesos));
                    }
                }
                else if (acceso is Familia familia)
                {
                    // Verificar si la familia existe antes de asignarla
                    Familia existingFamilia = familiaRepository.GetById(familia.Id);
                    if (existingFamilia != null)
                    {
                        usuarioFamiliaRepository.Add(user, familia);
                    }
                    else
                    {
                        throw new ArgumentException($"La familia con ID {familia.Id} no existe.", nameof(user.Accesos));
                    }
                }
            }

            Console.WriteLine($"Usuario {user.UserName} registrado con éxito.");
        }

        /// <summary>
        /// Valida las credenciales de un usuario.
        /// </summary>
        /// <param name="user">El usuario a validar.</param>
        /// <returns>Retorna true si las credenciales son válidas; de lo contrario, false.</returns>
        public static bool Validate(Usuario user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "El usuario no puede ser nulo.");
            }

            var usuarioRepository = FactoryDao.CreateRepository<UsuarioRepository>();

            // Verificar el nombre de usuario y la contraseña contra la base de datos
            Usuario usuarioDB = usuarioRepository.GetById(user.IdUsuario);

            if (usuarioDB != null && usuarioDB.UserName == user.UserName && usuarioDB.Password == user.Password)
            {
                Console.WriteLine("Usuario validado con éxito.");
                return true;
            }
            else
            {
                Console.WriteLine("Validación de usuario fallida.");
                return false;
            }
        }

        /// <summary>
        /// Actualiza un usuario en el sistema, incluyendo sus accesos (Patentes y Familias).
        /// </summary>
        /// <param name="user">El usuario a actualizar.</param>
        public static void Update(Usuario user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "El usuario no puede ser nulo.");
            }

            var usuarioRepository = FactoryDao.CreateRepository<UsuarioRepository>();
            var patenteRepository = FactoryDao.CreateRepository<PatenteRepository>();
            var familiaRepository = FactoryDao.CreateRepository<FamiliaRepository>();
            var usuarioPatenteRepository = FactoryDao.CreateRepository<UsuarioPatenteRepository>();
            var usuarioFamiliaRepository = FactoryDao.CreateRepository<UsuarioFamiliaRepository>();

            // Actualizar el usuario en la base de datos
            usuarioRepository.Update(user);

            // Actualizar Patentes y Familias del usuario
            usuarioPatenteRepository.RemoveByUsuario(user);
            usuarioFamiliaRepository.RemoveByUsuario(user);

            foreach (var acceso in user.Accesos)
            {
                if (acceso is Patente patente)
                {
                    // Verificar si la patente existe antes de asignarla
                    Patente existingPatente = patenteRepository.GetById(patente.Id);
                    if (existingPatente != null)
                    {
                        usuarioPatenteRepository.Add(user, patente);
                    }
                    else
                    {
                        throw new ArgumentException($"La patente con ID {patente.Id} no existe.", nameof(user.Accesos));
                    }
                }
                else if (acceso is Familia familia)
                {
                    // Verificar si la familia existe antes de asignarla
                    Familia existingFamilia = familiaRepository.GetById(familia.Id);
                    if (existingFamilia != null)
                    {
                        usuarioFamiliaRepository.Add(user, familia);
                    }
                    else
                    {
                        throw new ArgumentException($"La familia con ID {familia.Id} no existe.", nameof(user.Accesos));
                    }
                }
            }

            Console.WriteLine($"Usuario {user.UserName} actualizado con éxito.");
        }

        /// <summary>
        /// Elimina un usuario del sistema, incluyendo sus accesos (Patentes y Familias).
        /// </summary>
        /// <param name="idUsuario">El identificador del usuario a eliminar.</param>
        public static void Delete(Guid idUsuario)
        {
            var usuarioRepository = FactoryDao.CreateRepository<UsuarioRepository>();
            var usuarioPatenteRepository = FactoryDao.CreateRepository<UsuarioPatenteRepository>();
            var usuarioFamiliaRepository = FactoryDao.CreateRepository<UsuarioFamiliaRepository>();

            Usuario user = usuarioRepository.GetById(idUsuario);
            if (user == null)
            {
                throw new ArgumentException("El usuario no existe.", nameof(idUsuario));
            }

            // Eliminar accesos del usuario
            usuarioPatenteRepository.RemoveByUsuario(user);
            usuarioFamiliaRepository.RemoveByUsuario(user);

            // Eliminar el usuario de la base de datos
            usuarioRepository.Remove(idUsuario);

            Console.WriteLine($"Usuario {user.UserName} eliminado con éxito.");
        }

        /// <summary>
        /// Obtiene un usuario por su identificador único.
        /// </summary>
        /// <param name="idUsuario">El identificador único del usuario.</param>
        /// <returns>La instancia del usuario si existe, de lo contrario, null.</returns>
        public static Usuario GetById(Guid idUsuario)
        {
            var usuarioRepository = FactoryDao.CreateRepository<UsuarioRepository>();
            var usuarioPatenteRepository = FactoryDao.CreateRepository<UsuarioPatenteRepository>();
            var usuarioFamiliaRepository = FactoryDao.CreateRepository<UsuarioFamiliaRepository>();

            Usuario user = usuarioRepository.GetById(idUsuario);
            if (user != null)
            {
                usuarioPatenteRepository.Join(user);
                usuarioFamiliaRepository.Join(user);
            }
            return user;
        }

        /// <summary>
        /// Obtiene todos los usuarios del sistema.
        /// </summary>
        /// <returns>Una lista de todas las instancias de Usuario.</returns>
        public static List<Usuario> GetAll()
        {
            var usuarioRepository = FactoryDao.CreateRepository<UsuarioRepository>();
            var usuarioPatenteRepository = FactoryDao.CreateRepository<UsuarioPatenteRepository>();
            var usuarioFamiliaRepository = FactoryDao.CreateRepository<UsuarioFamiliaRepository>();

            List<Usuario> usuarios = usuarioRepository.GetAll();
            foreach (var user in usuarios)
            {
                usuarioPatenteRepository.Join(user);
                usuarioFamiliaRepository.Join(user);
            }
            return usuarios;
        }
    }
}