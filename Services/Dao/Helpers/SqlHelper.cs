using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Services.Dao.Helpers
{
    /// <summary>
    /// Proporciona métodos estáticos para ejecutar operaciones en la base de datos.
    /// </summary>
    internal static class SqlHelper
    {
        private readonly static string conString;

        static SqlHelper()
        {
            conString = ConfigurationManager.ConnectionStrings["ServicesSqlConnection"].ConnectionString;
        }

        /// <summary>
        /// Ejecuta un comando que no devuelve ningún resultado, como INSERT, DELETE o UPDATE.
        /// </summary>
        /// <param name="commandText">Texto del comando SQL a ejecutar.</param>
        /// <param name="commandType">Tipo del comando, como StoredProcedure o Text.</param>
        /// <param name="parameters">Parámetros del comando SQL.</param>
        /// <returns>El número de filas afectadas.</returns>
        public static int ExecuteNonQuery(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            CheckNullables(parameters);

            using (SqlConnection conn = new SqlConnection(conString))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Reemplaza cualquier parámetro nulo por DBNull para evitar errores en la base de datos.
        /// </summary>
        /// <param name="parameters">Array de SqlParameter que pueden contener valores nulos.</param>
        private static void CheckNullables(SqlParameter[] parameters)
        {
            foreach (SqlParameter item in parameters.Where(p => p.Value == null))
            {
                item.Value = DBNull.Value;
            }
        }

        /// <summary>
        /// Ejecuta un comando que devuelve el primer valor de la primera fila en el conjunto de resultados.
        /// </summary>
        /// <param name="commandText">Texto del comando SQL a ejecutar.</param>
        /// <param name="commandType">Tipo del comando, como StoredProcedure o Text.</param>
        /// <param name="parameters">Parámetros del comando SQL.</param>
        /// <returns>El primer valor de la primera fila en el conjunto de resultados.</returns>
        public static object ExecuteScalar(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(conString))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Ejecuta un comando y retorna un SqlDataReader para leer el resultado.
        /// </summary>
        /// <param name="commandText">Texto del comando SQL a ejecutar.</param>
        /// <param name="commandType">Tipo del comando, como StoredProcedure o Text.</param>
        /// <param name="parameters">Parámetros del comando SQL.</param>
        /// <returns>Un SqlDataReader para leer los resultados del comando.</returns>
        public static SqlDataReader ExecuteReader(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            SqlConnection conn = new SqlConnection(conString);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = commandType;
                cmd.Parameters.AddRange(parameters);

                conn.Open();
                // When using CommandBehavior.CloseConnection, the connection will be closed when the 
                // IDataReader is closed.
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }
    }
}