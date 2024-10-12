using Services.Domain;
using Services.Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
                LoggerService.WriteLog($"Usuario registrado con éxito: {user.UserName}", TraceLevel.Info);
            }
            catch (Exception ex)
            {
                LoggerService.WriteException(ex);
                throw;
            }
        }

        /// <summary>
        /// Valida las credenciales de un usuario.
        /// </summary>
        /// <param name="user">El usuario a validar.</param>
        /// <returns>Retorna true si las credenciales son válidas; de lo contrario, false.</returns>
        public static Usuario Validate(Usuario user)
        {
            try
            {
                return UserLogic.Validate(user);
            }
            catch (Exception ex)
            {
                LoggerService.WriteException(ex);
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
                LoggerService.WriteLog($"Usuario actualizado con éxito: {user.UserName}", TraceLevel.Info);
            }
            catch (Exception ex)
            {
                LoggerService.WriteException(ex);
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
                LoggerService.WriteLog($"Usuario eliminado con éxito: {idUsuario}", TraceLevel.Info);
            }
            catch (Exception ex)
            {
                LoggerService.WriteException(ex);
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
                LoggerService.WriteException(ex);
                throw;
            }
        }

        /// <summary>
        /// Obtiene todos los usuarios del sistema.
        /// </summary>
        /// <returns>Una lista de todas las instancias de Usuario.</returns>
        public static List<Usuario> GetAllUsuarios()
        {
            try
            {
                return UserLogic.GetAllUsuarios();
            }
            catch (Exception ex)
            {
                LoggerService.WriteException(ex);
                throw;
            }
        }

        /// <summary>
        /// Obtiene todas las patentes del sistema.
        /// </summary>
        /// <returns>Una lista de todas las instancias de Patente.</returns>
        public static List<Patente> GetAllPatentes()
        {
            try
            {
                return UserLogic.GetAllPatentes();
            }
            catch (Exception ex)
            {
                LoggerService.WriteException(ex);
                throw;
            }
        }

        /// <summary>
        /// Obtiene todas las familias del sistema.
        /// </summary>
        /// <returns>Una lista de todas las instancias de Familia.</returns>
        public static List<Familia> GetAllFamilias()
        {
            try
            {
                return UserLogic.GetAllFamilias();
            }
            catch (Exception ex)
            {
                LoggerService.WriteException(ex);
                throw;
            }
        }

        /// <summary>
        /// Sincroniza las patentes con los formularios del sistema.
        /// </summary>
        /// <param name="formNames">Lista de nombres de formularios presentes en el sistema.</param>
        public static void SyncPatentesWithForms(List<string> formNames)
        {
            try
            {
                UserLogic.SyncPatentesWithForms(formNames);
                LoggerService.WriteLog("Sincronización de patentes con formularios completada con éxito.", TraceLevel.Info);
            }
            catch (Exception ex)
            {
                LoggerService.WriteException(ex);
                throw;
            }
        }

        /// <summary>
        /// Obtiene un usuario por su nombre de usuario.
        /// </summary>
        /// <param name="userName">El nombre de usuario.</param>
        /// <returns>La instancia del usuario si existe, de lo contrario, null.</returns>
        public static Usuario GetByUserName(string userName)
        {
            try
            {
                return UserLogic.GetByUserName(userName);
            }
            catch (Exception ex)
            {
                LoggerService.WriteException(ex);
                throw;
            }
        }

        /// <summary>
        /// Agrega una nueva familia al sistema.
        /// </summary>
        /// <param name="familia">La familia a agregar.</param>
        public static void AddFamilia(Familia familia)
        {
            try
            {
                UserLogic.AddFamilia(familia);
                LoggerService.WriteLog($"Familia agregada con éxito: {familia.Nombre}", TraceLevel.Info);
            }
            catch (Exception ex)
            {
                LoggerService.WriteException(ex);
                throw;
            }
        }
    }
}