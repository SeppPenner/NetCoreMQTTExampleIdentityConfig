// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DtoCreateUpdateUserClaimToUserClaimConverter.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   Converts a <see cref="DtoCreateUpdateUserClaim" /> to a <see cref="UserClaim" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Converter;

/// <summary>
///     Converts a <see cref="DtoCreateUpdateUserClaim" /> to a <see cref="UserClaim" />.
/// </summary>
/// <seealso cref="ITypeConverter{DtoCreateUpdateUserClaim, UserClaim}" />
public class DtoCreateUpdateUserClaimToUserClaimConverter : ITypeConverter<DtoCreateUpdateUserClaim, UserClaim>
{
    /// <summary>
    ///     Performs conversion from source to destination type.
    /// </summary>
    /// <param name="source">Source object.</param>
    /// <param name="destination">Destination object.</param>
    /// <param name="context">Resolution context.</param>
    /// <returns>
    ///     Destination object.
    /// </returns>
    public UserClaim Convert(DtoCreateUpdateUserClaim source, UserClaim destination, ResolutionContext context)
    {
        return new UserClaim
        {
            ClaimType = source.ClaimType.ToString(),
            ClaimValue = JsonConvert.SerializeObject(source.ClaimValues),
            UserId = source.UserId
        };
    }
}
