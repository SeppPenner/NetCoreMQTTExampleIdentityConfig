// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserClaimToDtoCreateUpdateUserClaimConverter.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   Converts a <see cref="UserClaim" /> to a <see cref="DtoCreateUpdateUserClaim" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Converter;

/// <summary>
///     Converts a <see cref="UserClaim" /> to a <see cref="DtoCreateUpdateUserClaim" />.
/// </summary>
/// <seealso cref="ITypeConverter{UserClaim, DtoCreateUpdateUserClaim}" />
public class UserClaimToDtoCreateUpdateUserClaimConverter : ITypeConverter<UserClaim, DtoCreateUpdateUserClaim>
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
    public DtoCreateUpdateUserClaim Convert(UserClaim source, DtoCreateUpdateUserClaim destination,
        ResolutionContext context)
    {
        _ = Enum.TryParse(source.ClaimType, out ClaimType enumValue);

        return new DtoCreateUpdateUserClaim
        {
            ClaimType = enumValue,
            UserId = source.UserId,
            ClaimValues = JsonConvert.DeserializeObject<List<string>>(source.ClaimValue) ?? new List<string>()
        };
    }
}
