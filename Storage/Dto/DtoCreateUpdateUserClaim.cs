
namespace Storage.Dto
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Storage.Enumerations;
    using System.Collections.Generic;

    /// <summary>
    /// The user class to create or update a user claim.
    /// </summary>
    public class DtoCreateUpdateUserClaim
    {
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
    }
}