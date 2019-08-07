
namespace NetCoreMQTTExampleIdentityConfig.Controllers.Extensions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Some extension methods for the <see cref="ControllerBase"></see> class.
    /// </summary>
    public static class ControllerBaseExtensions
    {
        /// <summary>
        /// Returns a 500 internal server error.
        /// </summary>
        /// <param name="controllerBase">The controller base.</param>
        /// <param name="ex">The exception.</param>
        /// <returns>A <seealso cref="ObjectResult"/>.</returns>
        public static ObjectResult InternalServerError(this ControllerBase controllerBase, Exception ex)
        {
            return controllerBase.StatusCode(500, $"{ex.Message}{ex.StackTrace}");
        }

        /// <summary>
        /// Returns a 500 internal server error.
        /// </summary>
        /// <param name="controllerBase">The controller base.</param>
        /// <param name="identityErrors">The identity errors.</param>
        /// <returns>A <seealso cref="ObjectResult"/>.</returns>
        public static ObjectResult InternalServerError(this ControllerBase controllerBase, IEnumerable<IdentityErrorExt> identityErrors)
        {
            return controllerBase.StatusCode(500, string.Join(";", identityErrors));
        }
    }
}