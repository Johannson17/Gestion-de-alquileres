using System;
using System.Collections.Generic;
using System.Linq;

public class Usuario
{
    public Guid IdUsuario { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public List<Acceso> Accesos { get; set; } = new List<Acceso>();

    public Usuario()
    {
        IdUsuario = Guid.NewGuid();
    }

    public Usuario(Guid idUsuario)
    {
        IdUsuario = idUsuario;
    }

    /// <summary>
    /// Retrieves all patent access (leaf components) for this user.
    /// </summary>
    public List<Patente> GetPatentes()
    {
        List<Patente> patentes = new List<Patente>();
        GetAllPatentes(Accesos, patentes);
        return patentes;
    }

    private void GetAllPatentes(List<Acceso> accesos, List<Patente> patentesReturn)
    {
        foreach (var acceso in accesos)
        {
            if (acceso.GetCount() == 0 && !patentesReturn.Any(o => o.Id == acceso.Id))
            {
                patentesReturn.Add(acceso as Patente);
            }
            else
            {
                GetAllPatentes((acceso as Familia).Accesos, patentesReturn);
            }
        }
    }

    /// <summary>
    /// Retrieves all family access (composite components) for this user.
    /// </summary>
    public List<Familia> GetFamilias()
    {
        List<Familia> familias = new List<Familia>();
        GetAllFamilias(Accesos, familias);
        return familias;
    }

    private void GetAllFamilias(List<Acceso> accesos, List<Familia> famililaReturn)
    {
        foreach (var acceso in accesos)
        {
            if (acceso.GetCount() > 0)
            {
                Familia familia = acceso as Familia;
                if (!famililaReturn.Any(o => o.Id == familia.Id))
                {
                    famililaReturn.Add(familia);
                }
                GetAllFamilias(familia.Accesos, famililaReturn);
            }
        }
    }
}