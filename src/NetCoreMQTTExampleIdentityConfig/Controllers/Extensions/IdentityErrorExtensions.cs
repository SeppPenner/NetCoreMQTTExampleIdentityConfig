// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityErrorExtensions.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   Some extension methods for the <see cref="IdentityError"></see> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NetCoreMQTTExampleIdentityConfig.Controllers.Extensions;

/// <summary>
///     Some extension methods for the <see cref="IdentityError"></see> class.
/// </summary>
public class IdentityErrorExt : IdentityError
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IdentityErrorExt" /> class.
    /// </summary>
    /// <param name="error">The error.</param>
    public IdentityErrorExt(IdentityError error)
    {
        this.Code = error.Code;
        this.Description = error.Description;
    }

    /// <summary>
    ///     Converts the <seealso cref="IdentityError" /> to a <seealso cref="string" /> value.
    /// </summary>
    /// <returns>A <seealso cref="string" /> value of the <seealso cref="IdentityError" />.</returns>
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
