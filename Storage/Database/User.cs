namespace Storage.Database
{
    using Microsoft.AspNetCore.Identity;
    using System;

    /// <summary>
    /// This class contains the customized user data if neccessary.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Identity.IdentityUser{long}" />
    public class User: IdentityUser<long>
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