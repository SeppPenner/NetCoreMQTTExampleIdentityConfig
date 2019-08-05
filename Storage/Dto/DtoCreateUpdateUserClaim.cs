using System;

namespace Storage.Dto
{
    /// <summary>
    /// The user class to create or update a user claim.
    /// </summary>
    public class DtoCreateUpdateUserClaim
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the type of the claim.
        /// </summary>
        public string ClaimType { get; set; }

        /// <summary>
        /// Gets or sets the claim value.
        /// </summary>
        public string ClaimValue { get; set; }
    }
}