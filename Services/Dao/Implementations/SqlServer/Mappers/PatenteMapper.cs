using Dao.Contracts;
using System;

namespace Services.Dao.Implementations.SqlServer.Mappers
{
    /// <summary>
    /// Mapeador para convertir arrays de objetos en instancias de Patente.
    /// </summary>
    public sealed class PatenteMapper : IObjectMapper<Patente>
    {
        #region Singleton Pattern
        private static readonly PatenteMapper _instance = new PatenteMapper();

        /// <summary>
        /// Acceso a la instancia singleton del mapeador.
        /// </summary>
        public static PatenteMapper Current => _instance;

        private PatenteMapper()
        {
            // Aquí se puede implementar la inicialización del singleton si es necesario.
        }
        #endregion

        /// <summary>
        /// Convierte un array de valores en una instancia de Patente.
        /// </summary>
        /// <param name="values">Array que contiene los valores de los campos de una entidad Patente.</param>
        /// <returns>Una instancia de Patente mapeada desde los valores proporcionados.</returns>
        public Patente Fill(object[] values)
        {
            if (values == null || values.Length < 4)
            {
                throw new ArgumentException("Values array is null or does not have enough elements to map a Patente object.");
            }

            Patente patente = new Patente
            {
                Id = Guid.Parse(values[0]?.ToString()),
                Nombre = values[1]?.ToString(),
                DataKey = values[2]?.ToString(),
                TipoAcceso = (TipoAcceso)Enum.Parse(typeof(TipoAcceso), values[3]?.ToString())
            };

            return patente;
        }
    }
}
