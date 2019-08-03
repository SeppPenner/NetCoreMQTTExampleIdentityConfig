
namespace Storage.Database
{
    using System;

    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// This class contains the customized role data if neccessary.
    /// </summary>
    /// <seealso cref="IdentityRole{long}" />
    public class Role: IdentityRole<long>
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