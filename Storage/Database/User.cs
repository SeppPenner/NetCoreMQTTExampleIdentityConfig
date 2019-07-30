namespace Storage.Database
{
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// This class contains the customized user data if neccessary.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Identity.IdentityUser{long}" />
    public class User: IdentityUser<long>
    {
    }
}