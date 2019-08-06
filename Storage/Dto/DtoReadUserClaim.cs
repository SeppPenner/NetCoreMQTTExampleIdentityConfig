namespace Storage.Dto
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Storage.Enumerations;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The user claim class returned from the controller.
    /// </summary>
    public class DtoReadUserClaim
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public long UserId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        /// <summary>
        /// Gets or sets the type of the claim.
        /// </summary>
        public ClaimType ClaimType { get; set; }

        /// <summary>
        /// Gets or sets the claim value.
        /// </summary>
        public List<string> ClaimValues { get; set; }

        /// <summary>
        /// Gets or sets the created at timestamp.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the updated at timestamp.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}