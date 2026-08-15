// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserClaimMapperTests.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   A test class to test the <see cref="UserClaimMapper" /> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TopicCheckerTest;

/// <summary>
///     A test class to test the <see cref="UserClaimMapper" /> class. A claim carries its type as a string and its
///     topics as a JSON array in a single column, so both conversions are the place where a claim quietly turns into
///     the wrong one.
/// </summary>
[TestClass]
public class UserClaimMapperTests
{
    /// <summary>
    ///     The timestamp used by the tests.
    /// </summary>
    private static readonly DateTimeOffset CreatedAt = new(2026, 8, 15, 10, 0, 0, TimeSpan.FromHours(2));

    /// <summary>
    ///     Checks whether a claim reaches the read data transfer object with its type and its topics.
    /// </summary>
    [TestMethod]
    public void ToDtoReadUserClaimMapsTypeAndValues()
    {
        var claim = new UserClaim
        {
            Id = 7,
            UserId = 42,
            ClaimType = ClaimType.PublishWhitelist.ToString(),
            ClaimValue = """["d","e/+","f/#"]""",
            CreatedAt = CreatedAt,
            UpdatedAt = CreatedAt
        };

        var dto = claim.ToDtoReadUserClaim();

        Assert.AreEqual(7, dto.Id);
        Assert.AreEqual(42, dto.UserId);
        Assert.AreEqual(ClaimType.PublishWhitelist, dto.ClaimType);
        CollectionAssert.AreEqual(new[] { "d", "e/+", "f/#" }, dto.ClaimValues);
        Assert.AreEqual(CreatedAt, dto.CreatedAt);
        Assert.AreEqual(CreatedAt, dto.UpdatedAt);
    }

    /// <summary>
    ///     Checks whether a claim type that is not a member of the enumeration becomes the first member. The database
    ///     holds the type as free text, so this is what a typo in a row turns into.
    /// </summary>
    [TestMethod]
    public void AnUnknownClaimTypeBecomesTheSubscriptionBlacklist()
    {
        var dto = new UserClaim { ClaimType = "SomethingElse" }.ToDtoReadUserClaim();

        Assert.AreEqual(ClaimType.SubscriptionBlacklist, dto.ClaimType);
    }

    /// <summary>
    ///     Checks whether an empty claim value becomes an empty topic list instead of throwing.
    /// </summary>
    [TestMethod]
    public void AnEmptyClaimValueBecomesAnEmptyList()
    {
        Assert.AreEqual(0, new UserClaim { ClaimValue = null }.ToDtoReadUserClaim().ClaimValues.Count);
        Assert.AreEqual(0, new UserClaim { ClaimValue = string.Empty }.ToDtoReadUserClaim().ClaimValues.Count);
    }

    /// <summary>
    ///     Checks whether a claim keeps its type and its topics through both directions.
    /// </summary>
    [TestMethod]
    public void AClaimSurvivesTheRoundTrip()
    {
        var claim = new UserClaim
        {
            Id = 7,
            UserId = 42,
            ClaimType = ClaimType.SubscriptionWhitelist.ToString(),
            ClaimValue = """["a","b/+"]""",
            CreatedAt = CreatedAt,
            UpdatedAt = CreatedAt
        };

        var result = claim.ToDtoReadUserClaim().ToUserClaim();

        Assert.AreEqual(claim.Id, result.Id);
        Assert.AreEqual(claim.UserId, result.UserId);
        Assert.AreEqual(claim.ClaimType, result.ClaimType);
        Assert.AreEqual(claim.ClaimValue, result.ClaimValue);
    }

    /// <summary>
    ///     Checks whether the claim type is written as the name of the enumeration member. The broker compares the
    ///     column against exactly those names, a number or a translated text there hides every claim from it.
    /// </summary>
    [TestMethod]
    public void ToUserClaimWritesTheNameOfTheClaimType()
    {
        var claim = new DtoCreateUpdateUserClaim
        {
            UserId = 42,
            ClaimType = ClaimType.PublishBlacklist,
            ClaimValues = ["a"]
        }.ToUserClaim();

        Assert.AreEqual("PublishBlacklist", claim.ClaimType);
        Assert.AreEqual("""["a"]""", claim.ClaimValue);
    }

    /// <summary>
    ///     Checks whether a claim reaches the create data transfer object with its type and its topics.
    /// </summary>
    [TestMethod]
    public void ToDtoCreateUpdateUserClaimMapsTypeAndValues()
    {
        var claim = new UserClaim
        {
            UserId = 42,
            ClaimType = ClaimType.SubscriptionBlacklist.ToString(),
            ClaimValue = """["g","h/+"]"""
        };

        var dto = claim.ToDtoCreateUpdateUserClaim();

        Assert.AreEqual(42, dto.UserId);
        Assert.AreEqual(ClaimType.SubscriptionBlacklist, dto.ClaimType);
        CollectionAssert.AreEqual(new[] { "g", "h/+" }, dto.ClaimValues);
    }

    /// <summary>
    ///     Checks whether a list of claims is mapped completely.
    /// </summary>
    [TestMethod]
    public void ToDtoReadUserClaimsMapsEveryClaim()
    {
        var claims = new[]
        {
            new UserClaim { ClaimType = ClaimType.SubscriptionBlacklist.ToString(), ClaimValue = """["a"]""" },
            new UserClaim { ClaimType = ClaimType.PublishWhitelist.ToString(), ClaimValue = """["d"]""" }
        };

        var dtos = claims.ToDtoReadUserClaims();

        Assert.AreEqual(2, dtos.Count);
        Assert.AreEqual(ClaimType.SubscriptionBlacklist, dtos[0].ClaimType);
        Assert.AreEqual(ClaimType.PublishWhitelist, dtos[1].ClaimType);
    }

    /// <summary>
    ///     Checks the names of the claim type members. The broker builds its database query from
    ///     <c>ClaimType.SubscriptionBlacklist.ToString()</c> and friends, so renaming a member silently stops matching
    ///     every claim that is already stored.
    /// </summary>
    [TestMethod]
    public void TheClaimTypeNamesAreTheOnesTheBrokerQueriesFor()
    {
        CollectionAssert.AreEqual(
            new[] { "SubscriptionBlacklist", "SubscriptionWhitelist", "PublishBlacklist", "PublishWhitelist" },
            Enum.GetNames<ClaimType>());
    }
}
