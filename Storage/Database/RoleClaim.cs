namespace Storage.Database
{
    using System;

    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// This class contains the customized role claim data if necessary.
    /// </summary>
    /// <seealso cref="IdentityRoleClaim{TKey}" />
    public class RoleClaim : IdentityRoleClaim<long>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public virtual new long Id { get; set; }

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