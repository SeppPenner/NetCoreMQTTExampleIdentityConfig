// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserClaimMapper.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The user claim mapper class containing manual mappings between the user claim entity and its data transfer objects.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Mappings;

/// <summary>
///     The user claim mapper class containing manual mappings between the <see cref="UserClaim" /> entity and its data
///     transfer objects.
/// </summary>
/// <remarks>
///     These are the four converters that used to live in <c>Storage.Converter</c> as AutoMapper
///     <c>ITypeConverter</c> implementations, unchanged except for the interface that is gone.
///     <see cref="UserClaim.ClaimType" /> is the <see cref="string" /> that <c>IdentityUserClaim</c> defines, the
///     <see cref="Storage.Enumerations.ClaimType" /> enumeration only exists on the data transfer object side, and
///     <see cref="UserClaim.ClaimValue" /> holds the topic list as a JSON array.
/// </remarks>
public static class UserClaimMapper
{
    /// <summary>
    ///     Maps a <see cref="UserClaim" /> to a <see cref="DtoReadUserClaim" />.
    /// </summary>
    /// <param name="userClaim">The <see cref="UserClaim" /> to map.</param>
    /// <returns>The mapped <see cref="DtoReadUserClaim" />.</returns>
    public static DtoReadUserClaim ToDtoReadUserClaim(this UserClaim userClaim)
    {
        _ = Enum.TryParse(userClaim.ClaimType, out ClaimType claimType);

        return new DtoReadUserClaim
        {
            Id = userClaim.Id,
            UserId = userClaim.UserId,
            ClaimType = claimType,
            ClaimValues = DeserializeClaimValues(userClaim.ClaimValue),
            CreatedAt = userClaim.CreatedAt,
            UpdatedAt = userClaim.UpdatedAt
        };
    }

    /// <summary>
    ///     Maps a collection of <see cref="UserClaim" /> to a collection of <see cref="DtoReadUserClaim" />.
    /// </summary>
    /// <param name="userClaims">The <see cref="UserClaim" /> collection to map.</param>
    /// <returns>The mapped <see cref="DtoReadUserClaim" /> collection.</returns>
    public static List<DtoReadUserClaim> ToDtoReadUserClaims(this IEnumerable<UserClaim> userClaims)
    {
        return userClaims.Select(userClaim => userClaim.ToDtoReadUserClaim()).ToList();
    }

    /// <summary>
    ///     Maps a <see cref="UserClaim" /> to a <see cref="DtoCreateUpdateUserClaim" />.
    /// </summary>
    /// <param name="userClaim">The <see cref="UserClaim" /> to map.</param>
    /// <returns>The mapped <see cref="DtoCreateUpdateUserClaim" />.</returns>
    public static DtoCreateUpdateUserClaim ToDtoCreateUpdateUserClaim(this UserClaim userClaim)
    {
        _ = Enum.TryParse(userClaim.ClaimType, out ClaimType claimType);

        return new DtoCreateUpdateUserClaim
        {
            UserId = userClaim.UserId,
            ClaimType = claimType,
            ClaimValues = DeserializeClaimValues(userClaim.ClaimValue)
        };
    }

    /// <summary>
    ///     Maps a <see cref="DtoCreateUpdateUserClaim" /> to a <see cref="UserClaim" />.
    /// </summary>
    /// <param name="userClaim">The <see cref="DtoCreateUpdateUserClaim" /> to map.</param>
    /// <returns>The mapped <see cref="UserClaim" />.</returns>
    public static UserClaim ToUserClaim(this DtoCreateUpdateUserClaim userClaim)
    {
        return new UserClaim
        {
            UserId = userClaim.UserId,
            ClaimType = userClaim.ClaimType.ToString(),
            ClaimValue = JsonConvert.SerializeObject(userClaim.ClaimValues)
        };
    }

    /// <summary>
    ///     Maps a <see cref="DtoReadUserClaim" /> to a <see cref="UserClaim" />.
    /// </summary>
    /// <param name="userClaim">The <see cref="DtoReadUserClaim" /> to map.</param>
    /// <returns>The mapped <see cref="UserClaim" />.</returns>
    public static UserClaim ToUserClaim(this DtoReadUserClaim userClaim)
    {
        return new UserClaim
        {
            Id = userClaim.Id,
            UserId = userClaim.UserId,
            ClaimType = userClaim.ClaimType.ToString(),
            ClaimValue = JsonConvert.SerializeObject(userClaim.ClaimValues),
            CreatedAt = userClaim.CreatedAt,
            UpdatedAt = userClaim.UpdatedAt
        };
    }

    /// <summary>
    ///     Deserializes the JSON array a claim value holds into the list of topics it represents.
    /// </summary>
    /// <param name="claimValue">The claim value to deserialize.</param>
    /// <returns>The topics of the claim value, an empty list if there are none.</returns>
    private static List<string> DeserializeClaimValues(string? claimValue)
    {
        if (string.IsNullOrWhiteSpace(claimValue))
        {
            return [];
        }

        return JsonConvert.DeserializeObject<List<string>>(claimValue) ?? [];
    }
}
