namespace Storage.Database
{
    using System;

    /// <summary>
    /// The database version.
    /// </summary>
    public class DbVersion
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public virtual long Id { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public virtual string Version { get; set; }

        /// <summary>
        /// Gets or sets the version name.
        /// </summary>
        public virtual string VersionName { get; set; }

        /// <summary>
        /// Gets or sets the created at timestamp.
        /// </summary>
        public virtual DateTimeOffset CreatedAt { get; set; }
    }
}