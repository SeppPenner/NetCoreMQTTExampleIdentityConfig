﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoleClaim.cs" company="Haemmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   This class contains the customized role claim data if necessary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Database
{
    using System;

    using Microsoft.AspNetCore.Identity;

    using Newtonsoft.Json;

    /// <summary>
    ///     This class contains the customized role claim data if necessary.
    /// </summary>
    /// <seealso cref="IdentityRoleClaim{TKey}" />
    public class RoleClaim : IdentityRoleClaim<long>
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
            return JsonConvert.SerializeObject(this);
        }
    }
}