using System;
using System.Collections.Generic;
using System.Linq;

public class Familia : Acceso
{
    private List<Acceso> accesos = new List<Acceso>();

    /// <summary>
    /// Describes the group's characteristics or purpose.
    /// </summary>
    public string Descripcion { get; set; }

    /// <summary>
    /// Initializes a new instance of the Familia class with an optional initial access component.
    /// </summary>
    /// <param name="acceso">Optional access component to add on initialization.</param>
    public Familia(Acceso acceso = null)
    {
        if (acceso != null)
        {
            accesos.Add(acceso);
        }
    }

    /// <summary>
    /// Adds an access component to the family.
    /// </summary>
    /// <param name="component">The access component to add.</param>
    public override void Add(Acceso component)
    {
        accesos.Add(component);
    }

    /// <summary>
    /// Removes an access component from the family based on its unique identifier.
    /// </summary>
    /// <param name="component">The access component to remove.</param>
    public override void Remove(Acceso component)
    {
        accesos.RemoveAll(o => o.Id == component.Id);
    }

    /// <summary>
    /// Returns the count of access components in the family.
    /// </summary>
    /// <returns>The number of access components.</returns>
    public override int GetCount()
    {
        return accesos.Count;
    }

    /// <summary>
    /// Provides access to the list of Acceso components.
    /// </summary>
    public List<Acceso> Accesos
    {
        get { return accesos; }
    }
}
