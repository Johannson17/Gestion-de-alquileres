using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Services.Facade; // Asegúrate de tener la referencia al proyecto Services

namespace UI.Helpers
{
    internal static class PatenteHelper
    {
        /// <summary>
        /// Obtiene una lista de todos los nombres de formularios en la aplicación y los envía a la lógica.
        /// </summary>
        public static void SyncPatentesWithForms()
        {
            // Obtener todos los formularios en el proyecto
            var formTypes = typeof(Form).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Form)) && !t.IsAbstract)
                .ToList();

            // Convertir los tipos de formularios a una lista de nombres de formularios
            var formNames = formTypes.Select(t => t.Name).ToList();

            // Llamar a la lógica de negocio para sincronizar las patentes
            UserService.SyncPatentesWithForms(formNames);
        }
    }
}