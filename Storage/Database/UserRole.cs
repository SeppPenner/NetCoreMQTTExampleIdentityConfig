namespace Storage.Database
{
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// This class contains the customized user claim data if neccessary.
    /// </summary>
    /// <seealso cref="IdentityUserRole{long}" />
    public class UserRole : IdentityUserRole<long>
    {
    }
}