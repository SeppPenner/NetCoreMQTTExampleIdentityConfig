
namespace Storage.Database
{
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// This class contains the customized role data if neccessary.
    /// </summary>
    /// <seealso cref="IdentityRole{long}" />
    public class Role: IdentityRole<long>
    {
    }
}