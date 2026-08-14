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
        var target = new User();
        user.ApplyTo(target);
        return target;
    }

    /// <summary>
    ///     Copies the values of a <see cref="DtoCreateUpdateUser" /> onto an existing <see cref="User" />.
    /// </summary>
    /// <param name="user">The <see cref="DtoCreateUpdateUser" /> to take the values from.</param>
    /// <param name="target">The <see cref="User" /> to write the values to.</param>
    /// <remarks>
    ///     Used by the update endpoint, which has to write onto the entity it loaded. Building a second
    ///     <see cref="User" /> with the same key instead makes Entity Framework refuse to track it.
    /// </remarks>
    public static void ApplyTo(this DtoCreateUpdateUser user, User target)
    {
        target.UserName = user.UserName;
        target.Email = user.Email;
        target.EmailConfirmed = user.EmailConfirmed;
        target.PhoneNumber = user.PhoneNumber;
        target.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
        target.TwoFactorEnabled = user.TwoFactorEnabled;
        target.LockoutEnd = user.LockoutEnd;
        target.LockoutEnabled = user.LockoutEnabled;
        target.AccessFailedCount = user.AccessFailedCount;
        target.ClientIdPrefix = user.ClientIdPrefix;
        target.ClientId = user.ClientId;
    }
}
