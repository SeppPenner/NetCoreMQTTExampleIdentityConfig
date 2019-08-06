
using Microsoft.AspNetCore.Identity;

namespace NetCoreMQTTExampleIdentityConfig.Controllers.Extensions
{
    /// <summary>
    /// Some extension methods for the <see cref="IdentityError"></see> class.
    /// </summary>
    public class IdentityErrorExt: IdentityError
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
        /// Converts to string.
        /// </summary>
        /// <param name="error">The error.</param>
        public override string ToString()
        {
            return $"{this.Code}:{this.Description}";
        }
    }
}