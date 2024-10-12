using Dao.Contracts;
using System;

namespace Services.Dao.Implementations.SqlServer.Mappers
{
    /// <summary>
    /// Mapeador para convertir arrays de objetos en instancias de Usuario.
    /// </summary>
    public sealed class UsuarioMapper : IObjectMapper<Usuario>
    {
        #region Singleton Pattern
        private static readonly UsuarioMapper _instance = new UsuarioMapper();

        /// <summary>
        /// Acceso a la instancia singleton del mapeador.
        /// </summary>
        public static UsuarioMapper Current => _instance;

        private UsuarioMapper()
        {
            // Aquí se puede implementar la inicialización del singleton si es necesario.
        }
        #endregion

        /// <summary>
        /// Convierte un array de valores en una instancia de Usuario.
        /// </summary>
        /// <param name="values">Array que contiene los valores de los campos de una entidad Usuario.</param>
        /// <returns>Una instancia de Usuario mapeada desde los valores proporcionados.</returns>
        public Usuario Fill(object[] values)
        {
            if (values == null || values.Length < 3)
            {
                throw new ArgumentException("Values array is null or does not have enough elements to map a Usuario object.");
            }

            Usuario usuario = new Usuario
            {
                IdUsuario = Guid.Parse(values[0].ToString()),
                UserName = values[1]?.ToString(),
                Password = values[2]?.ToString()
            };

            return usuario;
        }
    }
}
