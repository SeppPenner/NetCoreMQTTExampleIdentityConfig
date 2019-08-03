namespace Storage.Database
{
    using Microsoft.AspNetCore.Identity;
    using System;

    /// <summary>
    /// This class contains the customized role claim data if neccessary.
    /// </summary>
    /// <seealso cref="IdentityRoleClaim{long}" />
    public class RoleClaim: IdentityRoleClaim<long>
    {
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