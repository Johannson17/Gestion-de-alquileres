using Services.Dao;
using Services.Dao.Contracts;
using Services.Dao.Implementations;
using Services.Dao.Implementations.SqlServer;
using System;

namespace Services.Factory
{
    /// <summary>
    /// Factory para la creación de los Repositories (DAOs).
    /// Se encarga de devolver instancias de los Repositories según el tipo solicitado utilizando el patrón Singleton o devolviendo clases estáticas.
    /// </summary>
    public static class FactoryDao
    {
        /// <summary>
        /// Crea y devuelve una instancia del Repository correspondiente al tipo especificado.
        /// </summary>
        /// <typeparam name="T">El tipo del Repository que se desea obtener.</typeparam>
        /// <returns>Instancia del Repository solicitado.</returns>
        public static T CreateRepository<T>() where T : class
        {
            if (typeof(T) == typeof(FamiliaRepository))
            {
                return FamiliaRepository.Current as T;
            }
            else if (typeof(T) == typeof(PatenteRepository))
            {
                return PatenteRepository.Current as T;
            }
            else if (typeof(T) == typeof(UsuarioRepository))
            {
                return UsuarioRepository.Current as T;
            }
            else if (typeof(T) == typeof(FamiliaPatenteRepository))
            {
                return FamiliaPatenteRepository.Current as T;
            }
            else if (typeof(T) == typeof(UsuarioFamiliaRepository))
            {
                return UsuarioFamiliaRepository.Current as T;
            }
            else if (typeof(T) == typeof(UsuarioPatenteRepository))
            {
                return UsuarioPatenteRepository.Current as T;
            }
            else if (typeof(T) == typeof(ILoggerDao))
            {
                return LoggerDao.Current as T;
            }
            else if (typeof(T) == typeof(ILanguageRepository))
            {
                return LanguageRepository.Current as T;
            }
            
            else if (typeof(T) == typeof(ILanguageRepository))
            {
                return LanguageRepository.Current as T;
            }
            else
            {
                throw new ArgumentException($"El tipo '{typeof(T)}' no tiene un Repository registrado en la fábrica.");
            }
        }
    }
}