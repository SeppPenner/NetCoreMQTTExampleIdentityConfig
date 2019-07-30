namespace Storage.Dto
{
    /// <summary>
    /// The claim model class to send to the controller.
    /// </summary>
    public class ClaimModel
    {
        /// <summary>
        /// Gets or sets the type of the claim.
        /// </summary>
        public string ClaimType { get; set; }

        /// <summary>
        /// Gets or sets the claim value.
        /// </summary>
        public string ClaimValue { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public long UserId { get; set; }
    }
}