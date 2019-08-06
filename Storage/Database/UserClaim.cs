
namespace Storage.Database
{
    using System;

    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// This class contains the customized user claim data if necessary.
    /// </summary>
    /// <seealso cref="IdentityUserClaim{TKey}" />
    public class UserClaim : IdentityUserClaim<long>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public new virtual long Id { get; set; }

        /// <summary>
        /// Gets or sets the created at timestamp.
        /// </summary>
        public virtual DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the updated at timestamp.
        /// </summary>
        public virtual DateTimeOffset? UpdatedAt { get; set; } = null;
    }
}