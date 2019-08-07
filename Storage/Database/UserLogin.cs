namespace Storage.Database
{
    using System;

    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// This class contains the customized user login data if necessary.
    /// </summary>
    /// <seealso cref="IdentityUserLogin{TKey}" />
    public class UserLogin : IdentityUserLogin<long>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public virtual long Id { get; set; }

        /// <summary>
        /// Gets or sets the created at timestamp.
        /// </summary>
        public virtual DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the updated at timestamp.
        /// </summary>
        public virtual DateTimeOffset? UpdatedAt { get; set; } = null;

        /// <summary>
        /// Returns a <seealso cref="string"/> which represents the object instance.
        /// </summary>
        /// <returns>A <seealso cref="string"/> representation of the instance.</returns>
        public override string ToString()
        {
            return $"{nameof(this.Id)}: {this.Id}, {nameof(this.CreatedAt)}: {this.CreatedAt}, {nameof(this.UpdatedAt)}: {this.UpdatedAt}";
        }
    }
}