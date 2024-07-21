using System;

public abstract class Acceso
{
    /// <summary>
    /// Unique identifier for the access component.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the access component.
    /// </summary>
    public string Nombre { get; set; }

    /// <summary>
    /// Initializes a new instance of the Acceso class.
    /// </summary>
    public Acceso()
    {
    }

    /// <summary>
    /// Adds a child component to the current component.
    /// </summary>
    /// <param name="component">The child component to add.</param>
    public abstract void Add(Acceso component);

    /// <summary>
    /// Removes a child component from the current component.
    /// </summary>
    /// <param name="component">The child component to remove.</param>
    public abstract void Remove(Acceso component);

    /// <summary>
    /// Returns the number of child components.
    /// </summary>
    /// <returns>The number of child components.</returns>
    public abstract int GetCount();
}
