// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClaimType.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   The claim type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Enumerations;

/// <summary>
///     The claim type.
/// </summary>
public enum ClaimType
{
    /// <summary>
    ///     The subscription blacklist claim type.
    /// </summary>
    SubscriptionBlacklist,

    /// <summary>
    ///     The subscription whitelist claim type.
    /// </summary>
    SubscriptionWhitelist,

    /// <summary>
    ///     The publish blacklist claim type.
    /// </summary>
    PublishBlacklist,

    /// <summary>
    ///     The publish whitelist claim type.
    /// </summary>
    PublishWhitelist
}
