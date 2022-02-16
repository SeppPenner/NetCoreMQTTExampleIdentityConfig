// --------------------------------------------------------------------------------------------------------------------
// <copyright file="User.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   This class contains the customized user data if necessary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Database;

/// <summary>
///     This class contains the customized user data if necessary.
/// </summary>
/// <seealso cref="IdentityUser{TKey}" />
public class User : IdentityUser<long>
{
    /// <summary>
    ///     Gets or sets the client identifier prefix.
    /// </summary>
    public virtual string ClientIdPrefix { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the client identifier.
    /// </summary>
    public virtual string ClientId { get; set; } = string.Empty;

    /// <summary> 
    /// Gets or sets a value indicating whether the client id is validated or not. 
    /// </summary> 
    public virtual bool ValidateClientId { get; set; }

    /// <summary>
    ///     Gets or sets the created at timestamp.
    /// </summary>
    public virtual DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the updated at timestamp.
    /// </summary>
    public virtual DateTimeOffset? UpdatedAt { get; set; } = null;

    /// <summary>
    /// Gets or sets a value indicating whether the user is throttled after a certain limit or not.
    /// </summary>
    public virtual bool ThrottleUser { get; set; }

    /// <summary>
    /// Gets or sets a user's monthly limit in byte.
    /// </summary>
    public virtual long? MonthlyByteLimit { get; set; }

    /// <summary>
    ///     Returns a <seealso cref="string" /> which represents the object instance.
    /// </summary>
    /// <returns>A <seealso cref="string" /> representation of the instance.</returns>
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
