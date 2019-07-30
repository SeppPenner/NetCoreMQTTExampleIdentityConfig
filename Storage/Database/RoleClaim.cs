namespace Storage.Database
{
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// This class contains the customized role claim data if neccessary.
    /// </summary>
    /// <seealso cref="IdentityRoleClaim{long}" />
    public class RoleClaim: IdentityRoleClaim<long>
    {
    }
}