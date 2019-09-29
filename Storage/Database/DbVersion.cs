﻿using System;

namespace Storage.Database
{
    /// <summary>
    ///     The database version.
    /// </summary>
    public class DbVersion
    {
        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        public virtual long Id { get; set; }

        /// <summary>
        ///     Gets or sets the version.
        /// </summary>
        public virtual string Version { get; set; }

        /// <summary>
        ///     Gets or sets the version name.
        /// </summary>
        public virtual string VersionName { get; set; }

        /// <summary>
        ///     Gets or sets the created at timestamp.
        /// </summary>
        public virtual DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        ///     Returns a <seealso cref="string" /> which represents the object instance.
        /// </summary>
        /// <returns>A <seealso cref="string" /> representation of the instance.</returns>
        public override string ToString()
        {
            return
                $"{nameof(Id)}: {Id}, {nameof(Version)}: {Version}, {nameof(VersionName)}: {VersionName}, {nameof(CreatedAt)}: {CreatedAt}";
        }
    }
}