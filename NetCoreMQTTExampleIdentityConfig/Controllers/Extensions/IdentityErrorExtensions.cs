
namespace NetCoreMQTTExampleIdentityConfig.Controllers.Extensions
{
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// Some extension methods for the <see cref="IdentityError"></see> class.
    /// </summary>
    public class IdentityErrorExt : IdentityError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityErrorExt"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        public IdentityErrorExt(IdentityError error)
        {
            this.Code = error.Code;
            this.Description = error.Description;
        }

        /// <summary>
        /// Converts the <seealso cref="IdentityError"/> to a <seealso cref="string"/> value.
        /// </summary>
        /// <returns>A <seealso cref="string"/> value of the <seealso cref="IdentityError"/>.</returns>
        public override string ToString()
        {
            return $"{this.Code}:{this.Description}";
        }
    }
}