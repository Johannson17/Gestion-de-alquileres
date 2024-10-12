using Dao.Contracts;
using Services.Dao.Implementations.SqlServer;
using System;

namespace Services.Dao.Implementations.SqlServer.Mappers
{
    /// <summary>
    /// Mapeador para convertir arrays de objetos en instancias de la clase Familia.
    /// </summary>
    public sealed class FamiliaMapper : IObjectMapper<Familia>
    {
        #region Singleton Pattern
        private static readonly FamiliaMapper _instance = new FamiliaMapper();

        /// <summary>
        /// Acceso a la instancia singleton del mapeador.
        /// </summary>
        public static FamiliaMapper Current => _instance;

        private FamiliaMapper()
        {
            // Aquí se puede implementar la inicialización del singleton si es necesario.
        }
        #endregion

        /// <summary>
        /// Convierte un array de valores en una instancia de Familia.
        /// </summary>
        /// <param name="values">Array que contiene los valores de los campos de una entidad Familia.</param>
        /// <returns>Una instancia de Familia mapeada desde los valores proporcionados.</returns>
        public Familia Fill(object[] values)
        {
            if (values == null || values.Length < 2)
            {
                throw new ArgumentException("Values array is null or does not have enough elements to map a Familia object.");
            }

            Familia familia = new Familia
            {
                Id = Guid.Parse(values[0]?.ToString()),
                Nombre = values[1]?.ToString()
            };

            // Hidratación de relaciones con otras entidades como Patentes y Familias
            // Agrega patentes asociadas a la familia.
            FamiliaPatenteRepository.Current.Join(familia);
            // Si existen asociaciones familiares, descomentar y asegurarse que FamiliaFamiliaRepository está implementado.
            // FamiliaFamiliaRepository.Current.Join(familia);

            return familia;
        }
    }
}