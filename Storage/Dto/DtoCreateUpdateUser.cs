﻿using System;

namespace Storage.Dto
{
    /// <summary>
    ///     The user class to create or update a user.
    /// </summary>
    public class DtoCreateUpdateUser
    {
        /// <summary>
        ///     Gets or sets the lockout end date.
        /// </summary>
        public DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether two factor authentication is enabled or not.
        /// </summary>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the phone number is confirmed or not.
        /// </summary>
        public bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        ///     Gets or sets the phone number.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the email is confirmed or not.
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        ///     Gets or sets the email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     Gets or sets the name of the user.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the lockout is enabled or not.
        /// </summary>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        ///     Gets or sets the access failed count.
        /// </summary>
        public int AccessFailedCount { get; set; }

        /// <summary>
        ///     Gets or sets the client identifier prefix.
        /// </summary>
        public string ClientIdPrefix { get; set; }

        /// <summary>
        ///     Gets or sets the client identifier.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        ///     Returns a <seealso cref="string" /> which represents the object instance.
        /// </summary>
        /// <returns>A <seealso cref="string" /> representation of the instance.</returns>
        public override string ToString()
        {
            return
                $"{nameof(LockoutEnd)}: {LockoutEnd}, {nameof(TwoFactorEnabled)}: {TwoFactorEnabled}, {nameof(PhoneNumberConfirmed)}: {PhoneNumberConfirmed}, {nameof(PhoneNumber)}: {PhoneNumber}, {nameof(EmailConfirmed)}: {EmailConfirmed}, {nameof(Email)}: {Email}, {nameof(UserName)}: {UserName}, {nameof(Password)}: {Password}, {nameof(LockoutEnabled)}: {LockoutEnabled}, {nameof(AccessFailedCount)}: {AccessFailedCount}, {nameof(ClientIdPrefix)}: {ClientIdPrefix}, {nameof(ClientId)}: {ClientId}";
        }
    }
}