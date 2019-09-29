using System;
using Microsoft.AspNetCore.Identity;

namespace Storage.Database
{
    /// <summary>
    ///     This class contains the customized user claim data if necessary.
    /// </summary>
    /// <seealso cref="IdentityUserClaim{TKey}" />
    public class UserClaim : IdentityUserClaim<long>
    {
        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        public new virtual long Id { get; set; }

        /// <summary>
        ///     Gets or sets the created at timestamp.
        /// </summary>
        public virtual DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        ///     Gets or sets the updated at timestamp.
        /// </summary>
        public virtual DateTimeOffset? UpdatedAt { get; set; } = null;

        /// <summary>
        ///     Returns a <seealso cref="string" /> which represents the object instance.
        /// </summary>
        /// <returns>A <seealso cref="string" /> representation of the instance.</returns>
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(CreatedAt)}: {CreatedAt}, {nameof(UpdatedAt)}: {UpdatedAt}";
        }
    }
}