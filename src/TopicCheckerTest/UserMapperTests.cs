// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserMapperTests.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   A test class to test the <see cref="UserMapper" /> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TopicCheckerTest;

/// <summary>
///     A test class to test the <see cref="UserMapper" /> class. The mapping replaced AutoMapper, which mapped by
///     name, so a property that is added to one side and forgotten on the other is silently dropped now.
/// </summary>
[TestClass]
public class UserMapperTests
{
    /// <summary>
    ///     The timestamp used by the tests.
    /// </summary>
    private static readonly DateTimeOffset CreatedAt = new(2026, 8, 15, 10, 0, 0, TimeSpan.FromHours(2));

    /// <summary>
    ///     Checks whether every value of a user reaches the read data transfer object.
    /// </summary>
    [TestMethod]
    public void ToDtoReadUserMapsEveryValue()
    {
        var user = GetUser();

        var dto = user.ToDtoReadUser();

        Assert.AreEqual(user.Id, dto.Id);
        Assert.AreEqual(user.UserName, dto.UserName);
        Assert.AreEqual(user.Email, dto.Email);
        Assert.AreEqual(user.EmailConfirmed, dto.EmailConfirmed);
        Assert.AreEqual(user.PhoneNumber, dto.PhoneNumber);
        Assert.AreEqual(user.PhoneNumberConfirmed, dto.PhoneNumberConfirmed);
        Assert.AreEqual(user.TwoFactorEnabled, dto.TwoFactorEnabled);
        Assert.AreEqual(user.LockoutEnd, dto.LockoutEnd);
        Assert.AreEqual(user.LockoutEnabled, dto.LockoutEnabled);
        Assert.AreEqual(user.AccessFailedCount, dto.AccessFailedCount);
        Assert.AreEqual(user.ClientIdPrefix, dto.ClientIdPrefix);
        Assert.AreEqual(user.ClientId, dto.ClientId);
        Assert.AreEqual(user.CreatedAt, dto.CreatedAt);
        Assert.AreEqual(user.UpdatedAt, dto.UpdatedAt);
    }

    /// <summary>
    ///     Checks whether a user without a user name, a mail address or a phone number becomes empty strings instead
    ///     of nulls. Identity declares those three as nullable, the data transfer object does not.
    /// </summary>
    [TestMethod]
    public void ToDtoReadUserTurnsNullIntoAnEmptyString()
    {
        var dto = new User { UserName = null, Email = null, PhoneNumber = null }.ToDtoReadUser();

        Assert.AreEqual(string.Empty, dto.UserName);
        Assert.AreEqual(string.Empty, dto.Email);
        Assert.AreEqual(string.Empty, dto.PhoneNumber);
    }

    /// <summary>
    ///     Checks whether the password hash stays in the entity. It has no counterpart in the read data transfer
    ///     object, so it must never leave through the API.
    /// </summary>
    [TestMethod]
    public void ToDtoReadUserDoesNotCarryThePasswordHash()
    {
        var user = GetUser();

        var json = user.ToDtoReadUser().ToString();

        Assert.IsFalse(json.Contains("PasswordHash", StringComparison.OrdinalIgnoreCase), json);
        Assert.IsFalse(json.Contains("SomeHash", StringComparison.Ordinal), json);
    }

    /// <summary>
    ///     Checks whether a collection of users is mapped completely and in order.
    /// </summary>
    [TestMethod]
    public void ToDtoReadUsersMapsEveryUser()
    {
        var first = GetUser();
        var second = GetUser();
        second.UserName = "Hans2";

        var dtos = new[] { first, second }.ToDtoReadUsers().ToList();

        Assert.AreEqual(2, dtos.Count);
        Assert.AreEqual("Hans", dtos[0].UserName);
        Assert.AreEqual("Hans2", dtos[1].UserName);
    }

    /// <summary>
    ///     Checks whether every value of the create data transfer object reaches the user.
    /// </summary>
    [TestMethod]
    public void ToUserMapsEveryValue()
    {
        var dto = GetDto();

        var user = dto.ToUser();

        Assert.AreEqual(dto.UserName, user.UserName);
        Assert.AreEqual(dto.Email, user.Email);
        Assert.AreEqual(dto.EmailConfirmed, user.EmailConfirmed);
        Assert.AreEqual(dto.PhoneNumber, user.PhoneNumber);
        Assert.AreEqual(dto.PhoneNumberConfirmed, user.PhoneNumberConfirmed);
        Assert.AreEqual(dto.TwoFactorEnabled, user.TwoFactorEnabled);
        Assert.AreEqual(dto.LockoutEnd, user.LockoutEnd);
        Assert.AreEqual(dto.LockoutEnabled, user.LockoutEnabled);
        Assert.AreEqual(dto.AccessFailedCount, user.AccessFailedCount);
        Assert.AreEqual(dto.ClientIdPrefix, user.ClientIdPrefix);
        Assert.AreEqual(dto.ClientId, user.ClientId);
    }

    /// <summary>
    ///     Checks whether the plain password stays out of the entity. The controller hands it to the user manager
    ///     separately, a mapped password would end up in the password hash column unhashed.
    /// </summary>
    [TestMethod]
    public void ToUserDoesNotMapThePlainPassword()
    {
        var user = new DtoCreateUpdateUser { UserName = "Hans", Password = "Hans" }.ToUser();

        Assert.IsNull(user.PasswordHash);
    }

    /// <summary>
    ///     Checks whether the three broker settings survive an update. They have no counterpart in the data transfer
    ///     objects, so an update through the API must leave them alone instead of resetting them.
    /// </summary>
    [TestMethod]
    public void ApplyToLeavesTheBrokerSettingsAlone()
    {
        var stored = GetUser();
        stored.ValidateClientId = true;
        stored.ThrottleUser = true;
        stored.MonthlyByteLimit = 10000;

        GetDto().ApplyTo(stored);

        Assert.IsTrue(stored.ValidateClientId);
        Assert.IsTrue(stored.ThrottleUser);
        Assert.AreEqual(10000, stored.MonthlyByteLimit);
    }

    /// <summary>
    ///     Checks whether an update writes onto the instance it was given. The update endpoint depends on that, a
    ///     second user instance with the same key cannot be tracked by Entity Framework.
    /// </summary>
    [TestMethod]
    public void ApplyToWritesOntoTheGivenUser()
    {
        var stored = GetUser();
        var identifier = stored.Id;

        var dto = GetDto();
        dto.UserName = "Hans2";
        dto.ApplyTo(stored);

        Assert.AreEqual("Hans2", stored.UserName);
        Assert.AreEqual(identifier, stored.Id);
    }

    /// <summary>
    ///     Gets a user with every mapped value set.
    /// </summary>
    /// <returns>A <see cref="User" /> with every mapped value set.</returns>
    private static User GetUser()
    {
        return new User
        {
            Id = 42,
            UserName = "Hans",
            PasswordHash = "SomeHash",
            Email = "hans@example.com",
            EmailConfirmed = true,
            PhoneNumber = "0123456789",
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = true,
            LockoutEnd = CreatedAt,
            LockoutEnabled = true,
            AccessFailedCount = 3,
            ClientIdPrefix = "Test_",
            ClientId = "Hans",
            CreatedAt = CreatedAt,
            UpdatedAt = CreatedAt
        };
    }

    /// <summary>
    ///     Gets a create or update data transfer object with every value set.
    /// </summary>
    /// <returns>A <see cref="DtoCreateUpdateUser" /> with every value set.</returns>
    private static DtoCreateUpdateUser GetDto()
    {
        return new DtoCreateUpdateUser
        {
            UserName = "Hans",
            Password = "Hans",
            Email = "hans@example.com",
            EmailConfirmed = true,
            PhoneNumber = "0123456789",
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = true,
            LockoutEnd = CreatedAt,
            LockoutEnabled = true,
            AccessFailedCount = 3,
            ClientIdPrefix = "Test_",
            ClientId = "Hans"
        };
    }
}
