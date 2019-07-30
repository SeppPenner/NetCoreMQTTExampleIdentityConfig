namespace Storage.Dto
{
    /// <summary>
    /// The user model class to send to the controller.
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }
    }
}