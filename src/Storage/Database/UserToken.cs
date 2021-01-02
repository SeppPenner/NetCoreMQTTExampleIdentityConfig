// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserToken.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   This class contains the customized user token data if necessary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Database
{
    using System;

    using Microsoft.AspNetCore.Identity;

    using Newtonsoft.Json;

    /// <summary>
    ///     This class contains the customized user token data if necessary.
    /// </summary>
    /// <seealso cref="IdentityUserToken{TKey}" />
    public class UserToken : IdentityUserToken<long>
    {
        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        public virtual long Id { get; set; }

        /// <summary>
        ///     Gets or sets the created at timestamp.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public virtual DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        ///     Gets or sets the updated at timestamp.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public virtual DateTimeOffset? UpdatedAt { get; set; } = null;

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