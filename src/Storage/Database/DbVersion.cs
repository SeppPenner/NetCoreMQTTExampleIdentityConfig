// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbVersion.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The database version.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Database;

/// <summary>
///     The database version.
/// </summary>
public class DbVersion
{
    /// <summary>
    ///     Gets or sets the identifier.
    /// </summary>
    public virtual long Id { get; set; }

    /// <summary>
    ///     Gets or sets the version.
    /// </summary>
    public virtual string Version { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the version name.
    /// </summary>
    public virtual string VersionName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the created at timestamp.
    /// </summary>
    public virtual DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    ///     Returns a <seealso cref="string" /> which represents the object instance.
    /// </summary>
    /// <returns>A <seealso cref="string" /> representation of the instance.</returns>
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
