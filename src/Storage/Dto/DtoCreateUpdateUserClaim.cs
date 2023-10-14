// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DtoCreateUpdateUserClaim.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The user class to create or update a user claim.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Dto;

/// <summary>
///     The user class to create or update a user claim.
/// </summary>
public class DtoCreateUpdateUserClaim
{
    /// <summary>
    ///     Gets or sets the user identifier.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    ///     Gets or sets the type of the claim.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public ClaimType ClaimType { get; set; }

    /// <summary>
    ///     Gets or sets the claim value.
    /// </summary>
    public List<string> ClaimValues { get; set; } = new();

    /// <summary>
    ///     Returns a <seealso cref="string" /> which represents the object instance.
    /// </summary>
    /// <returns>A <seealso cref="string" /> representation of the instance.</returns>
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
