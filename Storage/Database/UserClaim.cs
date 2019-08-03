
namespace Storage.Database
{
    using Microsoft.AspNetCore.Identity;
    using System;

    /// <summary>
    /// This class contains the customized user claim data if neccessary.
    /// </summary>
    /// <seealso cref="IdentityUserClaim{long}" />
    public class UserClaim: IdentityUserClaim<long>
    {
#pragma warning disable CS0114 // Element blendet vererbtes Element aus; fehlendes Überschreibungsschlüsselwort
        /// <summary>
        /// Gets or sets the identifier for this user claim.
        /// </summary>
        public long Id { get; set; }
#pragma warning restore CS0114 // Element blendet vererbtes Element aus; fehlendes Überschreibungsschlüsselwort

        /// <summary>
        /// Gets or sets the created at timestamp.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the updated at timestamp.
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; }
    }
}