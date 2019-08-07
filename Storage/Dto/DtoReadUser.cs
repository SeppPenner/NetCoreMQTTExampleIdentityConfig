namespace Storage.Dto
{
    using System;

    /// <summary>
    /// The user class returned from the controller.
    /// </summary>
    public class DtoReadUser
    {
        /// <summary>
        /// Gets or sets the lockout end date.
        /// </summary>
        public DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether two factor authentication is enabled or not.
        /// </summary>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the phone number is confirmed or not.
        /// </summary>
        public bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the email is confirmed or not.
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the lockout is enabled or not.
        /// </summary>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// Gets or sets the access failed count.
        /// </summary>
        public int AccessFailedCount { get; set; }

        /// <summary>
        /// Gets or sets the created at timestamp.
        /// </summary>
        public virtual DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the updated at timestamp.
        /// </summary>
        public virtual DateTimeOffset? UpdatedAt { get; set; } = null;

        /// <summary>
        /// Gets or sets the client identifier prefix.
        /// </summary>
        public string ClientIdPrefix { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Returns a <seealso cref="string"/> which represents the object instance.
        /// </summary>
        /// <returns>A <seealso cref="string"/> representation of the instance.</returns>
        public override string ToString()
        {
            return $"{nameof(this.LockoutEnd)}: {this.LockoutEnd}, {nameof(this.TwoFactorEnabled)}: {this.TwoFactorEnabled}, {nameof(this.PhoneNumberConfirmed)}: {this.PhoneNumberConfirmed}, {nameof(this.PhoneNumber)}: {this.PhoneNumber}, {nameof(this.EmailConfirmed)}: {this.EmailConfirmed}, {nameof(this.Email)}: {this.Email}, {nameof(this.UserName)}: {this.UserName}, {nameof(this.Id)}: {this.Id}, {nameof(this.LockoutEnabled)}: {this.LockoutEnabled}, {nameof(this.AccessFailedCount)}: {this.AccessFailedCount}, {nameof(this.CreatedAt)}: {this.CreatedAt}, {nameof(this.UpdatedAt)}: {this.UpdatedAt}, {nameof(this.ClientIdPrefix)}: {this.ClientIdPrefix}, {nameof(this.ClientId)}: {this.ClientId}";
        }
    }
}