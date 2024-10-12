using System;

public class Patente : Acceso
{
    /// <summary>
    /// Defines the type of access granted by this patent.
    /// </summary>
    public TipoAcceso TipoAcceso { get; set; }

    /// <summary>
    /// Additional data key associated with the patent.
    /// </summary>
    public string DataKey { get; set; }

    /// <summary>
    /// Initializes a new instance of the Patente class with an optional specific access type.
    /// </summary>
    /// <param name="tipoAcceso">The type of access this patent grants (default is TipoAcceso.UI).</param>
    public Patente(TipoAcceso tipoAcceso = TipoAcceso.UI)
    {
        this.TipoAcceso = tipoAcceso;
    }

    /// <summary>
    /// Prevents addition of subcomponents to a leaf, which is not allowed in a composite pattern.
    /// </summary>
    /// <param name="component">The component attempted to be added.</param>
    public override void Add(Acceso component)
    {
        throw new InvalidOperationException("Cannot add to a Patente, as it is a leaf component.");
    }

    /// <summary>
    /// Prevents removal of subcomponents from a leaf, as it does not contain any.
    /// </summary>
    /// <param name="component">The component attempted to be removed.</param>
    public override void Remove(Acceso component)
    {
        throw new InvalidOperationException("Cannot remove from a Patente, as it is a leaf component.");
    }

    /// <summary>
    /// Returns the count of subcomponents, which is always zero for a leaf.
    /// </summary>
    /// <returns>The count of subcomponents, zero.</returns>
    public override int GetCount()
    {
        return 0;
    }
}

/// <summary>
/// Enumerates the different types of access that can be granted by a patent.
/// </summary>
public enum TipoAcceso
{
    UI,
    Control,
    UseCases
}
