﻿using Services.Factory;
using Services.Domain;
using Services.Helpers;
using System;
using System.Collections.Generic;
using Services.Dao.Implementations.SqlServer;
using System.Linq;

namespace Services.Logic
{
    /// <summary>
    /// Lógica de negocio para la gestión de usuarios.
    /// </summary>
    public static class UserLogic
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

            user.IdUsuario = Guid.NewGuid();

            // Encriptar la contraseña antes de guardar en la base de datos
            user.Password = EncryptionHelper.EncryptPassword(user.Password);

            // Guardar el usuario en la base de datos
            usuarioRepository.Add(user);
        }

        /// <summary>
        /// Valida las credenciales de un usuario.
        /// </summary>
        /// <param name="user">El usuario a validar.</param>
        /// <returns>Retorna true si las credenciales son válidas; de lo contrario, false.</returns>
        public static Usuario Validate(Usuario user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "El usuario no puede ser nulo.");
            }

            var usuarioRepository = FactoryDao.CreateRepository<UsuarioRepository>();

            // Obtener todos los usuarios de la base de datos
            var usuarios = usuarioRepository.GetAll();

            // Filtrar por nombre de usuario
            Usuario usuarioDB = usuarios.FirstOrDefault(u => u.UserName == user.UserName);

            if (usuarioDB != null)
            {
                // Comparar la contraseña encriptada almacenada con la contraseña proporcionada
                if (EncryptionHelper.EncryptPassword(user.Password) == usuarioDB.Password)
                {
                    Console.WriteLine("Usuario validado con éxito.");
                    return usuarioDB; // Devolver el usuario válido
                }
            }

            Console.WriteLine("Validación de usuario fallida.");
            return null; // Si no es válido, devolver null
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

            // Encriptar la nueva contraseña antes de actualizar
            user.Password = EncryptionHelper.EncryptPassword(user.Password);

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
        /// Obtiene un usuario por su nombre de usuario.
        /// </summary>
        /// <param name="userName">El nombre de usuario a buscar.</param>
        /// <returns>La instancia del usuario si existe, de lo contrario, null.</returns>
        public static Usuario GetByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException(nameof(userName), "El nombre de usuario no puede ser nulo o estar vacío.");
            }

            var usuarioRepository = FactoryDao.CreateRepository<UsuarioRepository>();

            // Llama al repositorio para obtener el usuario por nombre de usuario
            Usuario user = usuarioRepository.GetByUserName(userName);

            if (user != null)
            {
                // Asigna las patentes y familias al usuario desde los repositorios correspondientes
                var usuarioPatenteRepository = FactoryDao.CreateRepository<UsuarioPatenteRepository>();
                var usuarioFamiliaRepository = FactoryDao.CreateRepository<UsuarioFamiliaRepository>();

                usuarioPatenteRepository.Join(user);
                usuarioFamiliaRepository.Join(user);
            }

            return user;
        }

        /// <summary>
        /// Obtiene todas las patentes disponibles en el sistema.
        /// </summary>
        /// <returns>Una lista de todas las patentes.</returns>
        public static List<Patente> GetAllPatentes()
        {
            var patenteRepository = FactoryDao.CreateRepository<PatenteRepository>();
            return patenteRepository.GetAll();
        }

        /// <summary>
        /// Obtiene todas las familias disponibles en el sistema.
        /// </summary>
        /// <returns>Una lista de todas las familias.</returns>
        public static List<Familia> GetAllFamilias()
        {
            var familiaRepository = FactoryDao.CreateRepository<FamiliaRepository>();
            return familiaRepository.GetAll();
        }

        /// <summary>
        /// Sincroniza las patentes en la base de datos con los nombres de formularios proporcionados.
        /// </summary>
        /// <param name="formNames">Lista de nombres de formularios en el sistema.</param>
        public static void SyncPatentesWithForms(List<string> formNames)
        {
            var patenteRepository = FactoryDao.CreateRepository<PatenteRepository>();

            // Obtener todas las patentes desde la base de datos
            var patentesExistentes = patenteRepository.GetAll().Select(p => p.Nombre).ToList();

            // Encontrar formularios que no tienen patente asociada
            var formulariosSinPatente = formNames.Except(patentesExistentes).ToList();

            foreach (var formName in formulariosSinPatente)
            {
                // Crear una nueva patente para cada formulario sin patente
                var nuevaPatente = new Patente
                {
                    Nombre = formName,
                    TipoAcceso = TipoAcceso.UI, // Asignar el tipo de acceso como "Read"
                    DataKey = formName
                };

                // Insertar la nueva patente en la base de datos
                patenteRepository.Add(nuevaPatente);
            }
        }

        /// <summary>
        /// Obtiene todos los usuarios del sistema.
        /// </summary>
        /// <returns>Una lista de todas las instancias de Usuario.</returns>
        public static List<Usuario> GetAllUsuarios()
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

        /// <summary>
        /// Agrega una nueva familia al sistema, incluyendo sus accesos (Patentes y otras Familias).
        /// </summary>
        /// <param name="familia">La familia a agregar.</param>
        /// <exception cref="ArgumentNullException">Si la familia es nula.</exception>
        /// <exception cref="ArgumentException">Si alguna de las patentes o familias asociadas no existe.</exception>
        public static void AddFamilia(Familia familia)
        {
            if (familia == null)
            {
                throw new ArgumentNullException(nameof(familia), "La familia no puede ser nula.");
            }

            var familiaRepository = FactoryDao.CreateRepository<FamiliaRepository>();
            var patenteRepository = FactoryDao.CreateRepository<PatenteRepository>();

            // Guardar la familia en la base de datos
            familiaRepository.Add(familia);

            // Crear una lista temporal de accesos para evitar modificar la colección durante la iteración
            var accesosTemp = new List<Acceso>(familia.Accesos);

            // Asignar accesos (Patentes y Familias) a la familia desde la lista temporal
            foreach (var acceso in accesosTemp)
            {
                if (acceso is Patente patente)
                {
                    // Verificar si la patente existe antes de asignarla
                    Patente existingPatente = patenteRepository.GetById(patente.Id);
                    if (existingPatente != null)
                    {
                        familia.Add(existingPatente);
                    }
                    else
                    {
                        throw new ArgumentException($"La patente con ID {patente.Id} no existe.", nameof(familia.Accesos));
                    }
                }
                else if (acceso is Familia subFamilia)
                {
                    // Verificar si la subfamilia existe antes de asignarla
                    Familia existingFamilia = familiaRepository.GetById(subFamilia.Id);
                    if (existingFamilia != null)
                    {
                        familia.Add(existingFamilia);
                    }
                    else
                    {
                        throw new ArgumentException($"La familia con ID {subFamilia.Id} no existe.", nameof(familia.Accesos));
                    }
                }
            }
        }
    }
}