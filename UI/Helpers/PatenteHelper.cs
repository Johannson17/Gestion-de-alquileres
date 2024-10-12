using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Services.Facade;

namespace UI.Helpers
{
    internal static class PatenteHelper
    {
        /// <summary>
        /// Obtiene una lista de todos los nombres de formularios dentro del ensamblado actual y los sincroniza con las patentes.
        /// </summary>
        public static void SyncPatentesWithForms()
        {
            // Obtén el ensamblado actual que contiene los formularios
            Assembly uiAssembly = Assembly.GetExecutingAssembly(); // Esto obtiene el ensamblado de la UI actual

            // Filtrar todas las clases que heredan de Form en este ensamblado
            var formTypes = uiAssembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Form)) && !t.IsAbstract)
                .ToList();

            // Convertir los tipos de formularios a una lista de nombres de formularios
            var formNames = formTypes.Select(t => t.Name).ToList();

            // Llamar al servicio para sincronizar las patentes con los formularios
            UserService.SyncPatentesWithForms(formNames);
        }
    }
}
