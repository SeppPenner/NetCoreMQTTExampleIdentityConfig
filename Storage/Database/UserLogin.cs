namespace Storage.Database
{
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// This class contains the customized user login data if neccessary.
    /// </summary>
    /// <seealso cref="IdentityUserLogin{long}" />
    public class UserLogin: IdentityUserLogin<long>
    {
    }
}