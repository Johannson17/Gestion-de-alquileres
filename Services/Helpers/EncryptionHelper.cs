using System;
using System.Security.Cryptography;
using System.Text;

namespace Services.Helpers
{
    public static class EncryptionHelper
    {
        /// <summary>
        /// Genera un hash SHA256 para la contraseña especificada.
        /// </summary>
        /// <param name="password">La contraseña a encriptar.</param>
        /// <returns>La contraseña encriptada en formato hash.</returns>
        public static string EncryptPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convertir los bytes a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Verifica si la contraseña proporcionada coincide con el hash almacenado.
        /// </summary>
        /// <param name="password">La contraseña ingresada por el usuario.</param>
        /// <param name="hashedPassword">La contraseña encriptada almacenada en la base de datos.</param>
        /// <returns>True si las contraseñas coinciden; de lo contrario, false.</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Compara el hash de la contraseña ingresada con el hash almacenado
            string hashedInput = EncryptPassword(password);
            return string.Equals(hashedInput, hashedPassword, StringComparison.OrdinalIgnoreCase);
        }
    }
}
