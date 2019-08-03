
namespace Storage.Database
{
    using Microsoft.AspNetCore.Identity;
    using System;

    /// <summary>
    /// This class contains the customized user token data if neccessary.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Identity.IdentityUserToken{long}" />
    public class UserToken: IdentityUserToken<long>
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