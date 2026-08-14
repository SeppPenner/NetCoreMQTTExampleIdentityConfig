// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserMapper.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The user mapper class containing manual mappings between the user entity and its data transfer objects.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Mappings;

/// <summary>
///     The user mapper class containing manual mappings between the <see cref="User" /> entity and its data transfer
///     objects.
/// </summary>
/// <remarks>
///     The mapped properties are exactly the ones AutoMapper matched by name before it was removed, so the API contract
///     is unchanged. <see cref="User.ValidateClientId" />, <see cref="User.ThrottleUser" /> and
///     <see cref="User.MonthlyByteLimit" /> have no counterpart in the data transfer objects and are therefore not
///     mapped in either direction.
/// </remarks>
public static class UserMapper
{
    /// <summary>
    ///     Maps a <see cref="User" /> to a <see cref="DtoReadUser" />.
    /// </summary>
    /// <param name="user">The <see cref="User" /> to map.</param>
    /// <returns>The mapped <see cref="DtoReadUser" />.</returns>
    public static DtoReadUser ToDtoReadUser(this User user)
    {
        return new DtoReadUser
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            TwoFactorEnabled = user.TwoFactorEnabled,
            LockoutEnd = user.LockoutEnd,
            LockoutEnabled = user.LockoutEnabled,
            AccessFailedCount = user.AccessFailedCount,
            ClientIdPrefix = user.ClientIdPrefix,
            ClientId = user.ClientId,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    /// <summary>
    ///     Maps a collection of <see cref="User" /> to a collection of <see cref="DtoReadUser" />.
    /// </summary>
    /// <param name="users">The <see cref="User" /> collection to map.</param>
    /// <returns>The mapped <see cref="DtoReadUser" /> collection.</returns>
    public static IEnumerable<DtoReadUser> ToDtoReadUsers(this IEnumerable<User> users)
    {
        return users.Select(user => user.ToDtoReadUser()).ToList();
    }

    /// <summary>
    ///     Maps a <see cref="DtoCreateUpdateUser" /> to a <see cref="User" />.
    /// </summary>
    /// <param name="user">The <see cref="DtoCreateUpdateUser" /> to map.</param>
    /// <returns>The mapped <see cref="User" />.</returns>
    public static User ToUser(this DtoCreateUpdateUser user)
    {
        return new User
        {
            UserName = user.UserName,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumber = user.PhoneNumber,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            TwoFactorEnabled = user.TwoFactorEnabled,
            LockoutEnd = user.LockoutEnd,
            LockoutEnabled = user.LockoutEnabled,
            AccessFailedCount = user.AccessFailedCount,
            ClientIdPrefix = user.ClientIdPrefix,
            ClientId = user.ClientId
        };
    }
}
