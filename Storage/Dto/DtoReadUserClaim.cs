using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Storage.Enumerations;

namespace Storage.Dto
{
    /// <summary>
    ///     The user claim class returned from the controller.
    /// </summary>
    public class DtoReadUserClaim
    {
        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     Gets or sets the user identifier.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        ///     Gets or sets the type of the claim.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ClaimType ClaimType { get; set; }

        /// <summary>
        ///     Gets or sets the claim value.
        /// </summary>
        public List<string> ClaimValues { get; set; }

        /// <summary>
        ///     Gets or sets the created at timestamp.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        ///     Gets or sets the updated at timestamp.
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; set; }

        /// <summary>
        ///     Returns a <seealso cref="string" /> which represents the object instance.
        /// </summary>
        /// <returns>A <seealso cref="string" /> representation of the instance.</returns>
        public override string ToString()
        {
            return
                $"{nameof(Id)}: {Id}, {nameof(UserId)}: {UserId}, {nameof(ClaimType)}: {ClaimType}, {nameof(ClaimValues)}: {ClaimValues}, {nameof(CreatedAt)}: {CreatedAt}, {nameof(UpdatedAt)}: {UpdatedAt}";
        }
    }
}