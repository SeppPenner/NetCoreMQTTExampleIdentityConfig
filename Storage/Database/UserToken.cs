
namespace Storage.Database
{
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// This class contains the customized user token data if neccessary.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Identity.IdentityUserToken{long}" />
    public class UserToken: IdentityUserToken<long>
    {
    }
}