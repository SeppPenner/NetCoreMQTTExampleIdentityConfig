// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DtoCreateUpdateUser.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   The user class to create or update a user.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Dto
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    ///     The user class to create or update a user.
    /// </summary>
    public class DtoCreateUpdateUser
    {
        /// <summary>
        ///     Gets or sets the lockout end date.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether two factor authentication is enabled or not.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the phone number is confirmed or not.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        ///     Gets or sets the phone number.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public string PhoneNumber { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the email is confirmed or not.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public bool EmailConfirmed { get; set; }

        /// <summary>
        ///     Gets or sets the email.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public string Email { get; set; }

        /// <summary>
        ///     Gets or sets the name of the user.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public string UserName { get; set; }

        /// <summary>
        ///     Gets or sets the password.
        /// </summary>
        [JsonIgnore]
        // ReSharper disable once UnusedMember.Global
        public string Password { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the lockout is enabled or not.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public bool LockoutEnabled { get; set; }

        /// <summary>
        ///     Gets or sets the access failed count.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public int AccessFailedCount { get; set; }

        /// <summary>
        ///     Gets or sets the client identifier prefix.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public string ClientIdPrefix { get; set; }

        /// <summary>
        ///     Gets or sets the client identifier.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public string ClientId { get; set; }

        /// <summary>
        ///     Returns a <seealso cref="string" /> which represents the object instance.
        /// </summary>
        /// <returns>A <seealso cref="string" /> representation of the instance.</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}